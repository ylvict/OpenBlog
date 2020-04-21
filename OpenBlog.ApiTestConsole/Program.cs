using System.Threading.Tasks;

namespace OpenBlog.ApiTestConsole
{
    public static class MainClass
    {
        static async Task Main(string[] args)
        {
            DataProtectorTest.TestApp.Start(args);
        }
    }
}