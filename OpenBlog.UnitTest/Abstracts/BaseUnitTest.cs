using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace OpenBlog.MongoRepositoryTest.Abstracts
{
    /// <summary>
    /// 提供测试过程中的IOC支持
    /// 不需要IOC支持可以不用
    /// </summary>
    public abstract class BaseUnitTest : IClassFixture<UnitTestFixture>
    {
        protected UnitTestFixture UnitTestFixture { get; }
        protected ITestOutputHelper TestOutputHelper { get; }
        protected IServiceProvider ServiceProvider => CreateScopedServiceProvider();


        public BaseUnitTest(UnitTestFixture unitTestFixture, ITestOutputHelper testOutputHelper)
        {
            UnitTestFixture = unitTestFixture;
            TestOutputHelper = testOutputHelper;

            unitTestFixture.ConfigureServicee(services =>
            {
                services.AddSingleton(testOutputHelper);
            });

            unitTestFixture.Init();
        }

        public void ConfigureServicee(Action<IServiceCollection> servicesConfigure)
        {
            UnitTestFixture.ConfigureServicee(servicesConfigure);
        }

        public IServiceProvider CreateScopedServiceProvider()
        {
            return UnitTestFixture.CreateScopedServiceProvider();
        }

        protected T GetService<T>()
        {
            return CreateScopedServiceProvider().GetRequiredService<T>();
        }

        protected T TryGetService<T>()
        {
            return CreateScopedServiceProvider().TryGetService<T>();
        }

        public IConfiguration Configuration => UnitTestFixture.Configuration;
    }
}
