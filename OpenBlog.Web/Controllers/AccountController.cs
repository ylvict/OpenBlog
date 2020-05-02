using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niusys.Security;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models;
using OpenBlog.Web.WebFramework;

namespace OpenBlog.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginViewModel loginViewModel,[FromServices]IEncryptionService encryptionService)
        {
            var user = await _userRepository.GetUserByEmail(loginViewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "用户不存在");
                return View();
            }

            var newPasswordHash =
                encryptionService.CreatePasswordHash(loginViewModel.Password, user.PasswordSalt, "SHA256");

            if (!newPasswordHash.Equals(user.PasswordHash))
            {
                ModelState.AddModelError("", "密码错误");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimNames.UserId, user.Sysid),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimNames.FullName, user.DisplayName),
                new Claim("LastChanged", user.UpdateTime.ToString(CultureInfo.InvariantCulture))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                // 记住cookie默认三天
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays((int)loginViewModel.RemberDays),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = loginViewModel.IsRemberme,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                // IssuedUtc = new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromDays(3)),
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToRoute("HomePage");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToRoute("HomePage");
        }
    }
}
