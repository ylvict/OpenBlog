using AutoMapper;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostPublicListItem>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId));
            CreateMap<Post, PostDetailViewModel>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId));
            CreateMap<Comment, CommentListItemViewModel>();
        }
    }
}