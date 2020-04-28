using AutoMapper;
using OpenBlog.DomainModels;

namespace OpenBlog.Web.Models
{
    public class NewPostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class PostListItem
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewPostViewModel, Post>();
            CreateMap<Post, PostListItem>()
                .ForMember(d => d.PostId, mo => mo.MapFrom(s => s.Sysid));
        }
    }
}
