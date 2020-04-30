using System.Threading.Tasks;

namespace OpenBlog.DomainModels
{
    public interface IUserRepository
    {
        Task<string> CreateUserAsync(User user);
        Task<User> GetUserByEmial(string email);
        bool ValidateLastChanged(string lastChanged);
    }
}
