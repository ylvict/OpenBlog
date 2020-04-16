using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenBlog.MongoRepositoryTest.Testing
{
    public static class TestingServiceExtensions
    {
        public static IServiceCollection AddXunitLogging(this IServiceCollection services)
        {
            services.AddTransient(typeof(ILogger<>), typeof(XunitTestLogger<>));
            services.AddSingleton<ILoggerFactory, XunitLoggerFactory>();
            return services;
        }
    }
}
