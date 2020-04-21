using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenBlog.WebFramework.Sessions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenBlog.Web
{
    public static class NamedHttpClients
    {
        public const string ProxiedClient = "ProxiedClient";
    }
    public class Startup
    {
        private readonly IHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

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

            services.AddControllersWithViews();


            #region 基础服务注册
            // 注册UserSession
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IUserRepository, UserRepository>();
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

    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IUserRepository _userRepository;

        public CustomCookieAuthenticationEvents(IUserRepository userRepository)
        {
            // Get the database from registered DI services.
            _userRepository = userRepository;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                               where c.Type == "LastChanged"
                               select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lastChanged) || !_userRepository.ValidateLastChanged(lastChanged))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }

    public interface IUserRepository
    {
        bool ValidateLastChanged(string lastChanged);
    }

    public class UserRepository : IUserRepository
    {
        public bool ValidateLastChanged(string lastChanged)
        {
            if (!DateTime.TryParse(lastChanged, out var lastChangedTime))
            {
                return false;
            }

            return lastChangedTime.AddSeconds(30) > DateTime.Now;
        }
    }
}
