using System;

namespace OpenBlog.DomainModels
{
    public class Comment
    {
        public string CommentId { get; set; }
        public string PostId { get; set; }
        public string CommentParentId { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}