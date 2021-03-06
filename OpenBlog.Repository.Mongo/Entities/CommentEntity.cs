using System;
using MongoDB.Bson.Serialization.Attributes;
using OpenBlog.Repository.Mongo.Abstracts;

namespace OpenBlog.Repository.Mongo.Entities
{
    public class CommentEntity : MongoEntityBase
    {
        public string PostId { get; set; }
        public string CommentParentId { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
    }
}