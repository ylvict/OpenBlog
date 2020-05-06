using System.Threading.Tasks;

namespace OpenBlog.ApiTestConsole
{
    public static class MainClass
    {
        static Task Main(string[] args)
        {
            TestApp.Start(args);
            return Task.CompletedTask;
        }
    }
}