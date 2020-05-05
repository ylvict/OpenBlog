using AutoMapper;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.Areas.Admin.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostCreateViewModel, Post>()
                .ForMember(d => d.Slug, mo => mo.MapFrom(s => s.Slug.Replace(' ', '-')));
            CreateMap<PostEditViewModel, Post>()
                .ForMember(d => d.Slug, mo => mo.MapFrom(s => s.Slug.Replace(' ', '-')));
            CreateMap<Post, PostEditViewModel>();
            CreateMap<Post, PostListItem>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.PostId));
        }
    }
}