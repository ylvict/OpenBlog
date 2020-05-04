using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OpenBlog.Web.Controllers
{
    public class SearchController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}