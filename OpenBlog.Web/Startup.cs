using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Niusys.Extensions.DependencyInjection;
using Niusys.Extensions.TypeFinders;
using Niusys.Security;
using OpenBlog.BlazorWasmService;
using OpenBlog.BlazorWasmService.BlazorWasmCli;
using OpenBlog.BlazorWasmService.Extensions.StaticFiles;
using OpenBlog.DomainModels;
using OpenBlog.Infrastructure;
using OpenBlog.Repository.Mongo;
using OpenBlog.Web.HostedServices;
using OpenBlog.Web.Services;
using OpenBlog.Web.WebFramework.Middlewares;
using OpenBlog.Web.WebFramework.Notifications;
using OpenBlog.Web.WebFramework.RouteTransformers;
using OpenBlog.Web.WebFramework.Sessions;

namespace OpenBlog.Web
{
    public static class NamedHttpClients
    {
        public const string ProxiedClient = "ProxiedClient";
    }

    public class Startup
    {
        private readonly IHostEnvironment _hostEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Application Assembly

            services.AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo(
                        @$"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}DataProtection-Keys"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<TypeFinderOptions>(options => { options.AssemblyMatchRegex = "^OpenBlog"; });
            services.AddScoped<ITypeFinder, DefaultTypeFinder>();
            services.AddDependencyRegister("^OpenBlog");

            var autoScanAssemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName != null && x.FullName.StartsWith("OpenBlog"));
            services.AddAutoMapper(autoScanAssemblies);

            services.AddHttpClient(NamedHttpClients.ProxiedClient);

            #endregion

            #region Mvc Configuration

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllersWithViews()
                .AddFluentValidation(config =>
                    {
                        var scanAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(x => x.FullName != null && x.FullName.StartsWith("OpenBlog"));
                        config.RegisterValidatorsFromAssemblies(scanAssemblies);
                    }
                );

            #endregion

            #region Infrastructure Service

            services.RegisterMongoStorage(Configuration);
            services.RegisterEmailService(Configuration);

            #endregion

            #region Security Part (UserIdentity & Authentication & Authorization)

            services.AddScoped<IRequestSession, DefaultRequestSession>();
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.ConsentCookie.Expiration = TimeSpan.FromDays(30);
            });

            // Cookie Config
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "openblog.auth";
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                });

            #endregion

            #region Installation

            services.AddSingleton<InstallTokenService>();
            services.AddHostedService<InstallTokenHostService>();

            #endregion

            #region Application Service & Repository

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            #endregion

            #region Register Route Transformer

            services.AddScoped<BloggerTransformer>();
            services.AddScoped<GenericPageTransformer>();

            #endregion

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"}); });

            services.AddMultipleSpaStaticFiles(configuration =>
            {
                configuration.RootPath = Path.Combine("wwwroot", "blazorapp");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/RouteNoMatch", "?httpStatusCode={0}");
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenBlog API V1"); });

            app.UseMiddleware<InstallCheckMiddleware>();
            app.UseRouting();

            // Authentication
            app.UseAuthentication();

            // Authorization
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "HomePage", pattern: "",
                    new {controller = "Home", action = "Index"});
                endpoints.MapControllerRoute(name: "FormSubmitRoute", pattern: "form-submit",
                    new {controller = "GenericPage", action = "FormSubmit"});
                endpoints.MapDynamicControllerRoute<BloggerTransformer>("blog/{category?}/{slug?}");
                endpoints.MapDynamicControllerRoute<BloggerTransformer>("blog/{year}/{month}/{*slug}");
                endpoints.MapControllerRoute(name: "MyArea", pattern: "{area:exists}/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDynamicControllerRoute<GenericPageTransformer>("{slug}");
            });

            app.UseMulitSpa(spaBuilder =>
            {
                spaBuilder.Options.PublicPath = "/profile";
                spaBuilder.Options.DefaultPageStaticFileOptions =
                    new BlazorStaticFileOptions(spaBuilder.Options.PublicPath);

                if (env.IsDevelopment())
                {
                    spaBuilder.Options.SourcePath = "../OpenBlog.UserCenterWeb";
                    spaBuilder.Options.StartupTimeout = TimeSpan.FromSeconds(2);
                    // 开发环境的public path通过vue中的地址获取
                    spaBuilder.UseBlazorWasmCliServer(
                        startScript: $"watch run --pathbase={spaBuilder.Options.PublicPath}");
                }
                else
                {
                    // 非开发环境需要指定public path跟default page
                    spaBuilder.Options.DistPath = $"UserCenterWebApp{Path.DirectorySeparatorChar}wwwroot";
                    spaBuilder.Options.DefaultPage = $"/profile/index.html";
                }
            });

            app.UseMulitSpa(spaBuilder =>
            {
                spaBuilder.Options.PublicPath = "/newadmin";
                spaBuilder.Options.DefaultPageStaticFileOptions =
                    new BlazorStaticFileOptions(spaBuilder.Options.PublicPath);

                if (env.IsDevelopment())
                {
                    spaBuilder.Options.SourcePath = "../OpenBlog.AdminWeb";
                    spaBuilder.Options.StartupTimeout = TimeSpan.FromSeconds(2);
                    // 开发环境的public path通过vue中的地址获取
                    spaBuilder.UseBlazorWasmCliServer(
                        startScript: $"watch run --pathbase={spaBuilder.Options.PublicPath}");
                }
                else
                {
                    // 非开发环境需要指定public path跟default page
                    spaBuilder.Options.DistPath = $"AdminWebApp{Path.DirectorySeparatorChar}wwwroot";
                    spaBuilder.Options.DefaultPage = $"{spaBuilder.Options.PublicPath}/index.html";
                }
            });
        }
    }

    public class BlazorStaticFileOptions : StaticFileOptions
    {
        public BlazorStaticFileOptions(string publicPath)
        {
            base.RequestPath = publicPath;
            base.HttpsCompression = HttpsCompressionMode.Compress;
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.TryAdd(".dll", "application/octet-stream");
            provider.Mappings.TryAdd(".dat", "application/octet-stream");
            provider.Mappings.TryAdd(".json", "application/json");
            provider.Mappings.TryAdd(".wasm", "application/wasm");
            provider.Mappings.TryAdd(".woff", "application/font-woff");
            provider.Mappings.TryAdd(".woff2", "application/font-woff");
            ContentTypeProvider = provider;
        }
    }
}