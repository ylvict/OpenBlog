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
                .PersistKeysToFileSystem(new DirectoryInfo(@$"{AppContext.BaseDirectory}\DataProtection-Keys"));
            services.AddHttpClient(NamedHttpClients.ProxiedClient);

            // 注册HttpContextAccessor 建议默认就注册进来
            services.AddHttpContextAccessor();

            // 添加Cookie认证
            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "aspnetcoredemo.auth";
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                });

            services.AddControllersWithViews()
                .AddFluentValidation();

            #region 框架服务注册
            services.Configure<TypeFinderOptions>(options =>
            {
                options.AssemblyMatchRegex = "^OpenBlog";
            });
            services.AddScoped<ITypeFinder, DefaultTypeFinder>();
            services.AddDependencyRegister("^OpenBlog");

            var autoScanAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OpenBlog"));
            services.AddAutoMapper(autoScanAssemblies);
            #endregion

            #region 注册存储服务
            services.RegisterMongoStorage(Configuration);
            #endregion

            #region 基础服务注册
            // 注册UserSession
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            #endregion

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
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
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            // 认证
            app.UseAuthentication();

            // 授权
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "HomePage", pattern: "", new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
