using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.WebFramework.Middlewares
{
    public class InstallCheckMiddleware
    {
        private bool IsSystemInited = false;
        private RequestDelegate Next { get; }

        public InstallCheckMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            if (!IsSystemInited)
            {
                var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
                if(await userRepository.IsSystemAdminInited())
                {
                    IsSystemInited = true;
                }
                else
                {
                    context.Request.Path=new PathString("/install");
                }
            }
            await Next(context).ConfigureAwait(false);
        }
    }
}