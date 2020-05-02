using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niusys;
using OpenBlog.Web.WebFramework;
using Serilog;

namespace OpenBlog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    if (hostingContext.HostingEnvironment.ApplicationName.IsNullOrWhitespace())
                    {
                        hostingContext.HostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
                    }

                    var localConfigFile = HostBuilderExtensions.CloneLocalConfiguration(hostingContext.HostingEnvironment.ApplicationName);

                    configurationBuilder
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", true, true);
                    configurationBuilder.AddJsonFile(
                        hostingContext.HostingEnvironment.IsDevelopment()
                            ? $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json"
                            : localConfigFile, true, true);
                    configurationBuilder.AddEnvironmentVariables();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.ColoredConsole(outputTemplate: "[{Timestamp:HH:mm:ss:fff} {Level:u3}] {SourceContext:l} {Message:lj}{NewLine}{Exception}");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
