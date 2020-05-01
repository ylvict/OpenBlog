using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo;
using OpenBlog.WebFramework.Sessions;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using OpenBlog.Infrastructure;
using Niusys.Extensions.TypeFinders;
using Niusys.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Niusys.Security;
using OpenBlog.Web.HostedServices;
using OpenBlog.Web.Services;
using OpenBlog.Web.WebFramework.Middlewares;
using OpenBlog.Web.WebFramework.RouteTransformers;

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
            services.AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo(
                        @$"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}DataProtection-Keys"));
            services.AddHttpClient(NamedHttpClients.ProxiedClient);

            // ע��HttpContextAccessor ����Ĭ�Ͼ�ע�����
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            // ����Cookie��֤
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "aspnetcoredemo.auth";
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                });

            services.AddControllersWithViews()
                .AddFluentValidation();

            #region ��ܷ���ע��

            services.Configure<TypeFinderOptions>(options => { options.AssemblyMatchRegex = "^OpenBlog"; });
            services.AddScoped<ITypeFinder, DefaultTypeFinder>();
            services.AddDependencyRegister("^OpenBlog");

            var autoScanAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OpenBlog"));
            services.AddAutoMapper(autoScanAssemblies);

            #endregion

            #region ע��洢����

            services.RegisterMongoStorage(Configuration);

            #endregion

            #region ��������ע��

            // ע��UserSession
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddSingleton<InstallTokenService>();
            services.AddSingleton<IEncryptionService, EncryptionService>();

            #endregion

            #region Register Route Transformer

            services.AddScoped<BloggerTransformer>();
            services.AddScoped<GenericPageTransformer>();

            #endregion

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"}); });
            services.AddHostedService<InstallTokenHostService>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseMiddleware<InstallCheckMiddleware>();
            app.UseRouting();

            // ��֤
            app.UseAuthentication();

            // ��Ȩ
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "HomePage", pattern: "",
                    new {controller = "Home", action = "Index"});
                endpoints.MapDynamicControllerRoute<BloggerTransformer>("blog/{category}/{*slug}");
                endpoints.MapDynamicControllerRoute<BloggerTransformer>("blog/{year}/{month}/{*slug}");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDynamicControllerRoute<GenericPageTransformer>("{slug}");
            });
        }
    }
}