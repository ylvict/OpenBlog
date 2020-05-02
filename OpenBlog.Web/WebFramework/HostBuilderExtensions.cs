using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Hosting;
using Niusys;

namespace OpenBlog.Web.WebFramework
{
    public static class HostBuilderExtensions
    {
        public static string CloneLocalConfiguration(string applicationName)
        {
            var localConfigPath = Environment.OSVersion.Platform == PlatformID.Unix && InDocker
                ? $"{Path.DirectorySeparatorChar}appdata" 
                : @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}dotnet_app{Path.DirectorySeparatorChar}{applicationName}";
            
            var localConfigFile = Path.Combine(localConfigPath, $"appsettings.local.json");
            
            try
            {
                if (!Directory.Exists(localConfigPath))
                {
                    Directory.CreateDirectory(localConfigPath);
                }

                if (File.Exists(localConfigFile))
                {
                    Console.WriteLine($"Skip Default Configuration File {localConfigFile} Clone");
                    return localConfigFile;
                }
                
                using var fs = new FileStream(localConfigFile, FileMode.OpenOrCreate);
                var defaultConfig = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
                fs.Write(Encoding.UTF8.GetBytes(defaultConfig));
                fs.Flush();
                Console.WriteLine($"Default Configuration File {localConfigFile} Clone Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Default Configuration File Clone Error, FilePath:{localConfigFile}, Error Message:{ex.FullMessage()}");
            }
            return localConfigFile;
        }
        
        private static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    }
}