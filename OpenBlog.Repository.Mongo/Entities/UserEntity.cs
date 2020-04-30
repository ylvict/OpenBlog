using OpenBlog.Repository.Mongo.Abstracts;

namespace OpenBlog.Repository.Mongo.Entities
{
    public class UserEntity : MongoEntityBase
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
