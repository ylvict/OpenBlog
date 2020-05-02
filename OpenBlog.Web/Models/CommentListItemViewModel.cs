using System;
using System.Collections.Generic;

namespace OpenBlog.Web.Models
{
    public class CommentListItemViewModel
    {
        public string CommentId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        
        public List<CommentListItemViewModel> ReplyComments { get; set; }
    }
}