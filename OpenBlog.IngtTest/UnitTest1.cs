using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenBlog.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace OpenBlog.IngtTest
{
    public class FileUploadTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public FileUploadTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task FileUploadTestStream()
        {
            // Arrange
            var client = _factory.CreateClient();

            var sendContent = new MultipartFormDataContent($"{new string('-', 10)}{DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture)}");

            //if (uploadFiles != null)
            //{
            //    foreach (var item in uploadFiles)
            //    {
            //        StreamContent streamContent = new StreamContent(item.Stream);
            //        streamContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
            //        var originalFileName = $"\"{item.OriginalFileName}{(item.OriginalFileName.IndexOf(".", StringComparison.OrdinalIgnoreCase) < 0 ? ".jpg" : string.Empty)}\"";
            //        sendContent.Add(streamContent, item.NewFileName ?? originalFileName, originalFileName);
            //    }
            //}

            // Act
            var response = await client.PostAsync("api/file/upload", sendContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task FileUploadTestBuffring()
        {
            // Arrange
            var client = _factory.CreateClient();

            FileStream fs = new FileStream("logo.png", FileMode.Open);

            var streamContent = new StreamContent(fs);

            // Act
            var response = await client.PostAsync("api/file/upload", streamContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        public class FileUpload
        {
            public FileUpload(string originalFileName, string contentType, Stream stream)
            {
                OriginalFileName = originalFileName;
                ContentType = contentType;
                Stream = stream;
            }

            public string NewFileName { get; set; }
            public string OriginalFileName { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
            public int FileType { get; set; }

            //public static IEnumerable<FileUpload> GetUploadFiles(HttpRequest request)
            //{
            //    foreach (string uploadFile in request.Files)
            //    {
            //        var file = request.Files[uploadFile];

            //        if (!(file != null && file.ContentLength > 0)) continue;

            //        yield return new FileUpload(file.FileName, file.ContentType, file.InputStream); ;
            //    }
            //}
        }

    }
}
