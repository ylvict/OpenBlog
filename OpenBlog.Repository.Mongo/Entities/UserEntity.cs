using System;
using MongoDB.Bson.Serialization.Attributes;
using OpenBlog.Repository.Mongo.Abstracts;

namespace OpenBlog.Repository.Mongo.Entities
{
    public class UserEntity : MongoEntityBase
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public UserType UserType { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastLoginTime { get; set; }
    }

    public enum UserType
    {
        /// <summary>
        /// 系统级管理员
        /// </summary>
        SystemAdmin = 1,

        /// <summary>
        /// 内容贡献者
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// 读者
        /// </summary>
        Reader = 3
    }
}