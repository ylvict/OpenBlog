using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OpenBlog.Web.Controllers
{
    public class ProfileController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}