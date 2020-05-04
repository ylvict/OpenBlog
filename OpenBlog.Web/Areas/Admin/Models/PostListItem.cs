using System;

namespace OpenBlog.Web.Areas.Admin.Models
{
    /// <summary>
    /// 公共页面展示列表项模型
    /// </summary>
    public class PostListItem
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        
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
    }
}