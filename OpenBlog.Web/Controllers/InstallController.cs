using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Niusys.Security;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models;
using OpenBlog.Web.Services;
using OpenBlog.Web.WebFramework.Install;

namespace OpenBlog.Web.Controllers
{
  [SystemInitializedCheck]
  public class InstallController : Controller
    {
        private readonly InstallTokenService _installTokenService;

        public InstallController(InstallTokenService installTokenService)
        {
            _installTokenService = installTokenService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Index")]
        public IActionResult TokenVerify(string token)
        {
            if (!_installTokenService.Token.Equals(token))
            {
                ModelState.AddModelError("", "Token不正确");
                return View();
            }

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
            if (!_installTokenService.Token.Equals(model.Token))
            {
                ModelState.AddModelError("", "Token不正确");
                return View();
            }

            var passwordSalt = encryptionService.CreateSaltKey(20);
            var passwordHash = encryptionService.CreatePasswordHash(model.Password, passwordSalt, "SHA256");
            await userRepository.InitSystemAdminUser(model.Email, passwordSalt, passwordHash);
            return RedirectToRoute("HomePage");
        }
    }
}