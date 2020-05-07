using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenBlog.ClientShared;
using OpenBlog.DomainModels;
using OpenBlog.Web.WebFramework.Sessions;

namespace OpenBlog.Web.ApiControllers
{
    [Route("api/account")]
    [ApiController, Authorize(AuthenticationSchemes = AuthenticateSchemas.JwtBearer)]
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet, Route("current_user")]
        public async Task<UserDetailInfo> GetCurrentUserDetail([FromServices] IUserSession userSession,
            [FromServices] IUserRepository userRepository)
        {
            _logger.LogInformation($"Try get user #{userSession.Email}");
            var userId = userSession.UserId;
            var userInfo = await userRepository.GetUserByEmail(userSession.Email);
            return new UserDetailInfo()
            {
                UserId = userInfo.Sysid, 
                DisplayName = userInfo.DisplayName,
                Email = userInfo.Email
            };
        }
    }
}