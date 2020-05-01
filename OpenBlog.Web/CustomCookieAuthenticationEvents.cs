using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenBlog.DomainModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenBlog.Web.Services;
using OpenBlog.WebFramework;

namespace OpenBlog.Web
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CustomCookieAuthenticationEvents> _logger;
        private readonly InstallTokenService _installTokenService;

        public CustomCookieAuthenticationEvents(IUserRepository userRepository,
            ILogger<CustomCookieAuthenticationEvents> logger,
            InstallTokenService installTokenService)
        {
            // Get the database from registered DI services.
            _userRepository = userRepository;
            _logger = logger;
            _installTokenService = installTokenService;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (!_installTokenService.IsSystemInited)
            {
                await RejectAndSignOut(context);
                return;
            }

            var userPrincipal = context.Principal;

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                where c.Type == "LastChanged"
                select c.Value).FirstOrDefault();

            var userId = (from c in userPrincipal.Claims
                where c.Type == ClaimNames.UserId
                select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lastChanged) || !await _userRepository.ValidateLastChanged(userId, lastChanged))
            {
                _logger.LogWarning($"User Session Rejected");
                await RejectAndSignOut(context);
            }
        }

        private static async Task RejectAndSignOut(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}