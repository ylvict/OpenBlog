using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Niusys.Extensions.DependencyInjection;
using Niusys.Extensions.TypeFinders;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace OpenBlog.MongoRepositoryTest.Abstracts
{
    public abstract class RepositoryUnitTest : IClassFixture<TestClassFixture>
    {
        private readonly TestClassFixture _testClassFixture;
        private readonly ITestOutputHelper _testOutputHelper;

        protected IServiceProvider ServiceProvider => _testClassFixture.ServiceProvider.CreateScope().ServiceProvider;

        public RepositoryUnitTest(TestClassFixture testClassFixture, ITestOutputHelper testOutputHelper)
        {
            testClassFixture.ConfigureServicee(services =>
            {
                services.AddSingleton(testOutputHelper);

                services.Configure<TypeFinderOptions>(options =>
                {
                    options.AssemblyMatchRegex = "^OpenBlog";
                });
                services.AddScoped<ITypeFinder, DefaultTypeFinder>();
                services.AddDependencyRegister("^OpenBlog");

                // 配置
                //services.Configure<DatabaseOptions>(testClassFixture.Configuration.GetSection("DbSetting"));
                //services.RegisterRdsConnectionManager();

                var autoScanAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OpenBlog"));
                services.AddAutoMapper(autoScanAssemblies);
            });

            _testClassFixture = testClassFixture;
            _testOutputHelper = testOutputHelper;
        }

        public void ConfigureServicee(Action<IServiceCollection> servicesConfigure)
        {
            _testClassFixture.ConfigureServicee(servicesConfigure);
        }

        public IServiceProvider CreateScopedServiceProvider()
        {
            return _testClassFixture.ServiceProvider;
        }
        protected T GetService<T>()
        {
            return CreateScopedServiceProvider().GetRequiredService<T>();
        }

        protected T TryGetService<T>()
        {
            return CreateScopedServiceProvider().TryGetService<T>();
        }

        protected void WriteLine(string message)
        {
            _testOutputHelper.WriteLine(message);
        }
    }
}
