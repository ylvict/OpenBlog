using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenBlog.Web.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FormOptions _defaultFormOptions;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".ex2", ".png", ".jpg" };

        public FileController(IOptions<FormOptions> formOptions)
        {
            _fileSizeLimit = 1 * 1024 * 1024 * 1024;
            _defaultFormOptions = formOptions?.Value ?? throw new ArgumentNullException(nameof(formOptions));
        }


        [HttpPost, Route("upload")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> OnPostUploadAsync()
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

            //_context.File.Add(file);
            //await _context.SaveChangesAsync();

            //return Created(nameof(StreamingController), null);
            return Ok();
        }
        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
    public class FormData
    {
        public string Note { get; set; }
    }
    public class AppFile
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "File Name")]
        public string UntrustedName { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTime UploadDT { get; set; }
    }
}