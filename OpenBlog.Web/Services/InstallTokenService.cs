using Niusys.Security;

namespace OpenBlog.Web.Services
{
    public class InstallTokenService
    {
        public string Token { get; private set; }
        public bool IsSystemInited { get; set; }

        public InstallTokenService(IEncryptionService encryptionService)
        {
            this.Token = encryptionService.CreateSaltKey(32);
        }
    }
}