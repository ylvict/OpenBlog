using System.Threading.Tasks;

namespace OpenBlog.DomainModels
{
    public interface IUserRepository
    {
        Task<string> CreateUserAsync(User user);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserAsync(string userId);
        Task<bool> IsSystemAdminInited();
        Task InitSystemAdminUser(string email, string passwordSalt, string passwordHash);
        Task<bool> ValidateLastChanged(string userId, string lastChangeToken);
        
    }
}
