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
        public async Task FileUploadTest_Stream()
        {
            // Arrange
            var client = _factory.CreateClient();

            var sendContent = new MultipartFormDataContent($"{new string('-', 10)}{DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture)}");

            var uploadFiles = new List<FileUpload>
            {
                new FileUpload("logo", new FileStream("logo.png", FileMode.Open), "logo.png"),
                new FileUpload("meitu", new FileStream("meitu.jpg", FileMode.Open), "meitu.jpg")
            };

            foreach (var item in uploadFiles)
            {
                StreamContent streamContent = new StreamContent(item.Stream);
                var originalFileName = item.OriginalFileName;
                sendContent.Add(streamContent, item.Name, item.OriginalFileName);
            }

            // Act
            var response = await client.PostAsync("api/file/upload", sendContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task FileUploadTest_Buffring()
        {
            // Arrange
            var client = _factory.CreateClient();

            var fs = new FileStream("logo.png", FileMode.Open);

            var streamContent = new StreamContent(fs);

            // Act
            var response = await client.PostAsync("api/file/stream_upload", streamContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        public class FileUpload
        {
            public FileUpload(string name, Stream stream, string fileName)
            {
                Name = name;
                Stream = stream;
                OriginalFileName = fileName;
            }
            public string Name { get; set; }
            public string NewFileName { get; set; }
            public string OriginalFileName { get; set; }
            public Stream Stream { get; set; }
            public int FileType { get; set; }
        }
    }
}
