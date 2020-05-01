using System;

namespace OpenBlog.DomainModels
{
    /// <summary>
    /// 博客系统用户
    /// </summary>
    public class User
    {
        public string Sysid { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
