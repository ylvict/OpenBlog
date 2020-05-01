using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace OpenBlog.Web.Controllers
{
    public class GenericPageController : Controller
    {
        public IActionResult ViewPage([FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ICompositeViewEngine compositeViewEngine, string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return RedirectToRoutePermanent("HomePage");
            }

            var viewResult = compositeViewEngine.FindView(actionContextAccessor.ActionContext, viewName, false);
            if (viewResult.Success)
            {
                ViewBag.PageName = viewName;
                return View(viewName);
            }

            return NotFound();
        }
    }
}