using System.Text.RegularExpressions;
using AutoMapper;
using Niusys;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.Models
{
    public class PostSummaryResolver : IValueResolver<Post, PostPublicListItem, string>
    {
        private const string HtmlTagFilterRegex = @"<[^>]*>";
        public string Resolve(Post source, PostPublicListItem destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Summary) && !string.IsNullOrEmpty(source.Content))
            {
                var pureContent = Regex.Replace(source.Content, HtmlTagFilterRegex, string.Empty);
                return pureContent?.SafeLimitString(200);
            }
            else
            {
                return source.Summary;
            }
        }
    }
}