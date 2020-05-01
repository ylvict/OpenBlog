﻿using System;
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
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime LastLoginTime { get; set; }
    }

    public enum UserType
    {
        /// <summary>
        /// 系统级管理员
        /// </summary>
        SystemAdmin,
        
        /// <summary>
        /// 内容贡献者
        /// </summary>
        Contributor,
        
        /// <summary>
        /// 读者
        /// </summary>
        Reader
    }
         
}
