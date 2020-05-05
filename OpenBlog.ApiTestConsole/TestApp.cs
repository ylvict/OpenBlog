using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace OpenBlog.ApiTestConsole
{
    public partial class TestApp
    {
        public static void Start(string[] args)
        {
            // Get the path to %LOCALAPPDATA%\myapp-keys
            var destFolder = Path.Combine(Environment.CurrentDirectory, "myapp-keys");

            // Instantiate the data protection system at this folder
            var dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(destFolder), setup =>
            {
                setup.SetApplicationName("my app name");
            });

            var protector = dataProtectionProvider.CreateProtector("Program.No-DI");
            Console.Write("Enter input: ");
            var input = Console.ReadLine();

            // Protect the payload
            var protectedPayload = protector.Protect(input);
            Console.WriteLine($"Protect returned: {protectedPayload}");

            // Unprotect the payload
            var unprotectedPayload = protector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
