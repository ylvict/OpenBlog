using System;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Niusys.Extensions.TypeFinders;
using OpenBlog.UnitTest.Testing;

namespace OpenBlog.UnitTest.Abstracts
{
    public class UnitTestFixture
    {
        private readonly IServiceCollection _services;

        public UnitTestFixture()
        {
            var configurationBuilder = new ConfigurationBuilder();

            Configuration = configurationBuilder.Build();

            _services = new ServiceCollection();
            _services.AddSingleton(_services);
            _services.AddSingleton(Configuration);

            _services.AddXunitLogging();
            _services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OpenBlog")).ToList());
            _services.Configure<TypeFinderOptions>(options => { options.AssemblyMatchRegex = "^OpenBlog"; });
            _services.AddSingleton<ITypeFinder, DefaultTypeFinder>();
            _services.AddDependencyRegister("^OpenBlog");
            _services.AddSingleton(x => x);
        }

        public ServiceProvider RootServiceProvider { get; private set; }
        public IConfiguration Configuration { get; }

        public void ConfigureServicee(Action<IServiceCollection> configure)
        {
            configure(_services);
            Init();
        }

        public IServiceProvider CreateScopedServiceProvider()
        {
            return RootServiceProvider.CreateScope().ServiceProvider;
        }

        public void Init()
        {
            RootServiceProvider = _services.BuildServiceProvider();
        }
    }
}
