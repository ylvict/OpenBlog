using AutoMapper;
using Niusys;
using OpenBlog.DomainModels;
using System.Text.RegularExpressions;

namespace OpenBlog.Web.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostPublicListItem>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId))
                .ForMember(d => d.Summary, mo => mo.MapFrom<PostSummaryResolver>())
                .ForMember(d => d.Summary,
                    mo => mo.MapFrom(s => s.Summary.IsNullOrEmpty() ? s.Content.SafeLimitString(200) : s.Summary));
            CreateMap<Post, PostDetailViewModel>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId));
            CreateMap<Comment, CommentListItemViewModel>();
        }
    }

    public class PostSummaryResolver : IValueResolver<Post, PostPublicListItem, string>
    {
        private const string _htmlTagFilterRegex = @"<[^>]*>";
        public string Resolve(Post source, PostPublicListItem destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Summary) && !string.IsNullOrEmpty(source.Content))
            {
                var pureContent = Regex.Replace(source.Content, _htmlTagFilterRegex, string.Empty);
                return pureContent?.SafeLimitString(200);
            }
            else
            {
                return source.Summary;
            }
        }
    }
}