using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Niusys.Security;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models;

namespace OpenBlog.Web.Controllers
{
    public class InstallController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Index")]
        public IActionResult TokenVerify(string token)
        {
            return RedirectToAction(nameof(InitAdmin), new {token = token});
        }

        [HttpGet]
        public IActionResult InitAdmin(string token)
        {
            var viewModel = new SystemAdminInitViewModel {Token = token};
            return View(viewModel);
        }

        [HttpPost, ActionName("InitAdmin")]
        public async Task<IActionResult> InitAdminPost(SystemAdminInitViewModel model,
            [FromServices] IUserRepository userRepository,
            [FromServices] IEncryptionService encryptionService)
        {
            //todo: verify token
            var passwordSalt = encryptionService.CreateSaltKey(20);
            var passwordHash = encryptionService.CreatePasswordHash(model.Password, passwordSalt, "SHA1");
            await userRepository.InitSystemAdminUser(model.Email, passwordSalt, passwordHash);
            return RedirectToRoute("HomePage");
        }
    }
}