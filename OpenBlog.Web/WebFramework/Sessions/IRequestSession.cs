namespace OpenBlog.Web.WebFramework.Sessions
{
    public interface IRequestSession
    {
        string ClientIp { get; }
        string Host { get; }
        string Schema { get; }
    }
}