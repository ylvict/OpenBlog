using AutoMapper;
using Niusys;
using OpenBlog.DomainModels;

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
}