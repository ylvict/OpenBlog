using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using OpenBlog.DomainModels;
using OpenBlog.Web.Services;

namespace OpenBlog.Web.WebFramework.Middlewares
{
    public class InstallCheckMiddleware
    {
        private readonly InstallTokenService _installTokenService;
        private RequestDelegate Next { get; }

        public InstallCheckMiddleware(RequestDelegate next, InstallTokenService installTokenService)
        {
            _installTokenService = installTokenService;
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            if (!_installTokenService.IsSystemInited  && !context.Request.Path.StartsWithSegments(new PathString("/install")))
            {
                var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
                if (await userRepository.IsSystemAdminInited())
                {
                    _installTokenService.IsSystemInited = true;
                }
                else
                {
                    context.Request.Path = new PathString("/install");
                }
            }

            await Next(context).ConfigureAwait(false);
        }
    }
}