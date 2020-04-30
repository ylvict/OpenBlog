using AutoMapper;
using Niusys.Extensions.ComponentModels;
using Niusys.Extensions.Storage.Mongo;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Entities;
using System.Collections.Generic;

namespace OpenBlog.Repository.Mongo
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap(typeof(Page<>), typeof(Page<>)).ConvertUsing(typeof(TrackedPropertyToPropertyVersionConverter<,>));

            CreateMap<Post, PostEntity>()
                .ForMember(d => d.Sysid, mo => mo.MapFrom(s => s.Sysid.SafeToObjectId()));
            CreateMap<PostEntity, Post>()
                .ForMember(d => d.Sysid, mo => mo.MapFrom(s => s.Sysid.ToString()))
                .ForMember(d=>d.Categories,mo=>mo.MapFrom(s=>s.Categories))
                .ForMember(d=>d.Tags,mo=>mo.MapFrom(s=>s.Tags))
                ;

            CreateMap<string, Category>()
                .ForMember(x => x.Name, mo => mo.MapFrom(s => s));
            CreateMap<string, Tag>()
                .ForMember(x => x.Name, mo => mo.MapFrom(s => s));
        }
    }

    public class TrackedPropertyToPropertyVersionConverter<TSource, TDestination> : ITypeConverter<Page<TSource>, Page<TDestination>>
    {
        public Page<TDestination> Convert(Page<TSource> source, Page<TDestination> destination, ResolutionContext context)
        {
            if (destination == null)
                destination = new Page<TDestination>();
            destination.Paging = source.Paging;
            destination.Records = context.Mapper.Map<List<TDestination>>(source.Records);
            return destination;
        }
    }
}
