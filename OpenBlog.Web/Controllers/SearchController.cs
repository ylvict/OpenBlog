using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OpenBlog.Web.Controllers
{
    public class SearchController : BaseMvcController
    {
        public async Task<IActionResult> Index(string q)
        {
            ViewBag.SearchText = q;
            await Task.CompletedTask;
            return View();
        }
    }
}