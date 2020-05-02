using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenBlog.Web.Services.EmailServices;

namespace OpenBlog.Web
{
    public static class ServiceExtensions
    {
        public static void RegisterEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<EmailService>();
            services.Configure<EmailSetting>(options =>
            {
                var optionsType = options.GetType();
                configuration.GetSection(optionsType.Name).Bind(options);
                foreach (var item in optionsType.GetProperties())
                {
                    var propertyEnvValue = Environment.GetEnvironmentVariable($"{ResolvePropertyName(optionsType.Name)}.{ResolvePropertyName(item.Name)}");
                    if (!string.IsNullOrWhiteSpace(propertyEnvValue))
                    {
                        item.SetValue(options, propertyEnvValue);
                    }
                }
            });

            services.Configure<EmailReceivers>(options =>
            {
                var optionsType = options.GetType();
                configuration.GetSection(optionsType.Name).Bind(options);
                foreach (var item in optionsType.GetProperties())
                {
                    var propertyEnvValue = Environment.GetEnvironmentVariable($"{ResolvePropertyName(optionsType.Name)}.{ResolvePropertyName(item.Name)}");
                    if (!string.IsNullOrWhiteSpace(propertyEnvValue))
                    {
                        item.SetValue(options, propertyEnvValue);
                    }
                }
            });
        }
        private static readonly Regex Regex = new Regex("(?!(^[A-Z]))([A-Z])");

        private static string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, "_$2").ToUpper();
        }

    }
}