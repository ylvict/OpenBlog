using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenBlog.DomainModels;
using OpenBlog.Web.Services;

namespace OpenBlog.Web.HostedServices
{
    public class InstallTokenHostService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly InstallTokenService _installTokenService;

        public InstallTokenHostService(IServiceProvider serviceProvider, InstallTokenService installTokenService)
        {
            _serviceProvider = serviceProvider;
            _installTokenService = installTokenService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var userRepository = serviceScope.ServiceProvider.GetRequiredService<IUserRepository>();
            if (!await userRepository.IsSystemAdminInited())
            {
                var tokenTipMessage = new StringBuilder();
                tokenTipMessage.AppendLine("==============================================================");
                tokenTipMessage.AppendLine("Init Token");
                tokenTipMessage.AppendLine($"{_installTokenService.Token}");
                tokenTipMessage.AppendLine("==============================================================");
                Console.WriteLine(tokenTipMessage);
            }
        }
    }


}