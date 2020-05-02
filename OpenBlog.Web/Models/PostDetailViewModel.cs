using System;
using System.Collections.Generic;
using Niusys.Extensions.ComponentModels;

namespace OpenBlog.Web.Models
{
    public class PostDetailViewModel
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// 博客URL
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        
        public Page<CommentListItemViewModel> CommentList { get; set; }
    }
}