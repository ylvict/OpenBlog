using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenBlog.AdminWeb.Services
{
    public interface IAuthService
    {
        Task Logout();
    }
}