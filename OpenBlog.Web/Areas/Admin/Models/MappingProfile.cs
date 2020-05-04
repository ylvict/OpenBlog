using AutoMapper;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.Areas.Admin.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostCreateViewModel, Post>();
            CreateMap<Post, PostListItem>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId));
        }
    }
}