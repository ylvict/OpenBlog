using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenBlog.MongoRepositoryTest.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using OpenBlog.Infrastructure;
namespace OpenBlog.MongoRepositoryTest.Abstracts
{
    public class TestClassFixture
    {
        private readonly IServiceCollection _services;

        public ServiceProvider RootServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; }
        public IServiceProvider ServiceProvider => RootServiceProvider.CreateScope().ServiceProvider;

        public TestClassFixture()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                          .SetBasePath(Environment.CurrentDirectory)
                          .AddJsonFile("appsettings.json", true, true);

            Configuration = configurationBuilder.Build();
            _services = new ServiceCollection();
            _services.AddSingleton<IConfiguration>(Configuration);
            _services.AddXunitLogging();
            _services.AddOptions();
            _services.AddSingleton(factory => _services);
            _services.AddSingleton(factory => Configuration);
            _services.RegisterMongoStorage(Configuration);
        }

        public void ConfigureServicee(Action<IServiceCollection> configure)
        {
            configure(_services);
            Init();
        }

        public void Init()
        {
            RootServiceProvider = _services.BuildServiceProvider();
        }
    }
}
