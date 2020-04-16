using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Niusys.Extensions.Storage.Mongo;
using OpenBlog.Repository.Mongo.Abstracts;

namespace OpenBlog.MongoRepositoryTest.Abstracts
{
    public static partial class ServiceCollectionExtensions
    {
        internal static void RegisterMongoStorage(this IServiceCollection services, IConfiguration configuration)
        {
            //services.ConfigurePOCO<MongoDefaultSetting>(configuration.GetSection("Mongo:Default"));
            services.Configure<MongoDefaultSetting>(configuration.GetSection("Mongo:Default"));
            services.AddSingleton<MongodbContext<MongoDefaultSetting>>();
            services.AddScoped(typeof(IAppNoSqlBaseRepository<>), typeof(AppNoSqlBaseRepository<>));
        }
    }
}
