using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenBlog.Web.Services;

namespace OpenBlog.Web.WebFramework.Install
{
    public class SystemInitializedCheckAsyncActionFilter : IAsyncActionFilter
    {
        private readonly InstallTokenService _installTokenService;

        public SystemInitializedCheckAsyncActionFilter(InstallTokenService installTokenService)
        {
            _installTokenService = installTokenService;
        }
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Do something before the action executes.
            if (_installTokenService.IsSystemInited)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                // next() calls the action method.
                var resultContext = await next();
                // resultContext.Result is set.
                // Do something after the action executes.
            }
        }
    }
}