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
        public string Sysid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
