using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OpenBlog.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenBlog.Web.WebFramework;

namespace OpenBlog.Web.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : BaseController
    {
        private readonly FormOptions _defaultFormOptions;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".ex2", ".png", ".jpg" };

        public FileController(IOptions<FormOptions> formOptions)
        {
            _fileSizeLimit = 1 * 1024 * 1024 * 1024;
            _defaultFormOptions = formOptions?.Value ?? throw new ArgumentNullException(nameof(formOptions));
        }
        [HttpPost, Route("stream_upload")]
        [DisableFormValueModelBinding]
        [Consumes("application/octet-stream")]
        public async Task<IActionResult> OnStreamPostUploadAsync(IFormFile file)
        {
            using (var fs = new FileStream($"{Guid.NewGuid()}", FileMode.OpenOrCreate | FileMode.Append))
            {
                await Request.Body.CopyToAsync(fs);
            }
            return Ok();
        }

        [HttpPost, Route("upload")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> OnMulipartPostUploadAsync()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();
            var fileStreamStore = new List<KeyValuePair<string, byte[]>>();
            var trustedFileNameForDisplay = string.Empty;
            var untrustedFileNameForStorage = string.Empty;
            //var streamedFileContent = new byte[0];

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        untrustedFileNameForStorage = contentDisposition.FileName.Value;
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        trustedFileNameForDisplay = WebUtility.HtmlEncode(
                                contentDisposition.FileName.Value);

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition,
                                 ModelState, _permittedExtensions, _fileSizeLimit);
                        fileStreamStore.Add(new KeyValuePair<string, byte[]>(trustedFileNameForDisplay, streamedFileContent));

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Don't limit the key name length because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities
                            .RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);

                        if (encoding == null)
                        {
                            ModelState.AddModelError("File",
                                $"The request couldn't be processed (Error 2).");
                            // Log error

                            return BadRequest(ModelState);
                        }

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by 
                            // MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();

                            if (string.Equals(value, "undefined",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount >
                                _defaultFormOptions.ValueCountLimit)
                            {
                                // Form key count limit of 
                                // _defaultFormOptions.ValueCountLimit 
                                // is exceeded.
                                ModelState.AddModelError("File", $"The request couldn't be processed (Error 3).");
                                // Log error

                                return BadRequest(ModelState);
                            }
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to the model
            var formData = new FormData();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);
            var bindingSuccessful = await TryUpdateModelAsync(formData, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                ModelState.AddModelError("File", "The request couldn't be processed (Error 5).");
                // Log error

                return BadRequest(ModelState);
            }

            // **WARNING!**
            // In the following example, the file is saved without
            // scanning the file's contents. In most production
            // scenarios, an anti-virus/anti-malware scanner API
            // is used on the file before making the file available
            // for download or for use by other systems. 
            // For more information, see the topic that accompanies 
            // this sample app.

            //var file = new AppFile()
            //{
            //    Content = streamedFileContent,
            //    UntrustedName = untrustedFileNameForStorage,
            //    Note = formData.Note,
            //    Size = streamedFileContent.Length,
            //    UploadDT = DateTime.UtcNow
            //};

            foreach (var item in fileStreamStore)
            {
                using (var fs = new FileStream($"{Guid.NewGuid()}_{item.Key}", FileMode.OpenOrCreate | FileMode.Append))
                {
                    fs.Write(item.Value, 0, item.Value.Length);
                }
            }
            return Ok();
        }
    }
}