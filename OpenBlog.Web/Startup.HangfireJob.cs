using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenBlog.Web
{
    public static partial class ServiceCollectionExtensions
    {
        public static bool IsHangfireEnable(this IHostEnvironment hostingEnvironment)
        {
            return !hostingEnvironment.IsDevelopment();
        }

        public static void RegisterHangire(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment, string connectionStringKey = "Mongo:Job:ConnectionString")
        {
            if (!hostEnvironment.IsHangfireEnable())
            {
                return;
            }

            services.AddSingleton<HangfireJobActivator>();
            services.AddSingleton<JobActivator>(serficeProvider => serficeProvider.GetRequiredService<HangfireJobActivator>());

            var connection = configuration.GetSection(connectionStringKey).Value;

            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMongoStorage(connection,
                        new MongoStorageOptions()
                        {
                            MigrationOptions = new MongoMigrationOptions()
                            {
                                Strategy = MongoMigrationStrategy.Migrate,
                                BackupStrategy = MongoBackupStrategy.Collections
                            },
                            Prefix = "hgjob"
                        }));

            // Init JobStore.Current
            var serviceBuilder = services.BuildServiceProvider();
            _ = serviceBuilder.GetRequiredService<JobStorage>();
            JobActivator.Current = serviceBuilder.GetRequiredService<JobActivator>();
        }

        public static void ConfigureHangfirePipline(this IApplicationBuilder app, IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsHangfireEnable())
            {
                return;
            }

            app.UseHangfireServer();

            app.UseHangfireDashboard("/hgjobs", new DashboardOptions()
            {
                DashboardTitle = "OpenBlog Jobs",
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
        }

        private class HangfireJobActivator : JobActivator
        {
            private readonly IServiceProvider _serviceProvider;

            public HangfireJobActivator(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            }

            public override object ActivateJob(Type jobType)
            {
                // Ensure jobs run in their own "request" scope. Please.
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                var scope = scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetService(jobType);
                return job ?? throw new Exception($"Could not activate a job of type '{jobType.FullName}'.");
            }
        }

        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return httpContext.User.Identity.IsAuthenticated;
            }
        }
    }
}