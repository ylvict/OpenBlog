using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBlog.DomainModels
{
    /// <summary>
    /// 文章
    /// </summary>
    public class Post
    {
        public string PostId { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keywords { get; set; }
        
        /// <summary>
        /// 描述概要
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// 博客URL
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 是否已发布
        /// </summary>
        public bool IsPublished { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        
        /// <summary>
        /// 文章分类
        /// </summary>
        public  List<Category> Categories { get; set; }
        
        /// <summary>
        /// 文章标签
        /// </summary>
        public List<Tag> Tags { get; set; }
    }
}
