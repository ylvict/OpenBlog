namespace OpenBlog.DomainModels
{
    public interface IUserRepository
    {
        bool ValidateLastChanged(string lastChanged);
    }
}
