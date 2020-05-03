using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using OpenBlog.Web.WebFramework;

namespace OpenBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult RouteNoMatch([FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ICompositeViewEngine compositeViewEngine, string httpStatusCode)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // 对于静态资源,返回原本的404
            if (feature?.OriginalPath?.IsStaticFileName() ?? false)
            {
                return NotFound();
            }

            var viewResult =
                compositeViewEngine.FindView(actionContextAccessor.ActionContext, viewName: httpStatusCode, false);
            if (!viewResult.Success) return RedirectToRoute("HomePage");
            ViewBag.StatusCode = httpStatusCode;
            ViewBag.OriginalPath = feature?.OriginalPath;
            ViewBag.OriginalQueryString = feature?.OriginalQueryString;
            return View(httpStatusCode);
        }
    }
}