using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBlog.ApiTestConsole
{
    partial class TestApp
    {
        public static class NamedHttpClients
        {
            public const string ProxiedClient = "ProxiedClient";
        }

        static async Task Run(string[] args)
        {
            var services = new ServiceCollection();
            services.AddHttpClient(NamedHttpClients.ProxiedClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    Proxy = new WebProxy("http://localhost:8888")
                });

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Arrange
            var client = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
            client.BaseAddress = new Uri("http://localhost:5000");

            var sendContent = new MultipartFormDataContent($"{new string('-', 10)}{DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture)}");
            var uploadFiles = new List<FileUpload>();
            uploadFiles.Add(new FileUpload("logo", new FileStream("logo.png", FileMode.Open), "logo.png"));
            uploadFiles.Add(new FileUpload("meitu", new FileStream("meitu.jpg", FileMode.Open), "meitu.jpg"));

            if (uploadFiles != null)
            {
                foreach (var item in uploadFiles)
                {
                    StreamContent streamContent = new StreamContent(item.Stream);
                    var originalFileName = item.OriginalFileName;
                    sendContent.Add(streamContent, item.Name, item.OriginalFileName);
                }
            }
            // Act
            var response = await client.PostAsync("api/file/upload", sendContent);

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
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