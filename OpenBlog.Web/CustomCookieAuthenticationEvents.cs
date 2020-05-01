using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenBlog.DomainModels;
using System.Linq;
using System.Threading.Tasks;
using OpenBlog.WebFramework;

namespace OpenBlog.Web
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IUserRepository _userRepository;

        public CustomCookieAuthenticationEvents(IUserRepository userRepository)
        {
            // Get the database from registered DI services.
            _userRepository = userRepository;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                where c.Type == "LastChanged"
                select c.Value).FirstOrDefault();
            
            var userId = (from c in userPrincipal.Claims
                where c.Type == ClaimNames.UserId
                select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lastChanged) || ! await _userRepository.ValidateLastChanged(userId, lastChanged))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}