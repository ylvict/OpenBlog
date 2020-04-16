using OpenBlog.Repository.Mongo.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBlog.Repository.Mongo.Entities
{
    public class PostEntity : MongoEntityBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
