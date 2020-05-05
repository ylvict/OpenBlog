using AutoMapper;
using MongoDB.Driver;
using Niusys.Extensions.ComponentModels;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Abstracts;
using OpenBlog.Repository.Mongo.Entities;
using System;
using System.Threading.Tasks;
using MongoDB.Libmongocrypt;
using Niusys.Extensions.Storage.Mongo;

namespace OpenBlog.Repository.Mongo
{
    public class PostRepository : IPostRepository
    {
        private readonly IMapper _mapper;
        private readonly IAppNoSqlBaseRepository<PostEntity> _postRep;

        public PostRepository(IMapper mapper, IAppNoSqlBaseRepository<PostEntity> postRep)
        {
            _mapper = mapper;
            _postRep = postRep;
        }

        public async Task<string> CreatePostAsync(Post postModel)
        {
            var postEntity = _mapper.Map<PostEntity>(postModel);
            postEntity.CreateTime = DateTime.Now;
            postEntity.UpdateTime = DateTime.Now;
            await _postRep.AddAsync(postEntity);
            return postEntity.Sysid.ToString();
        }        
        
        public async Task ModifyPostAsync(Post postModel)
        {
           var postEntity = _mapper.Map<PostEntity>(postModel);
           postEntity.UpdateTime = DateTime.Now;
           await _postRep.UpdateAsync(postEntity);
        }

        public async Task<Page<Post>> SearchPost(int pageIndex, int pageSize)
        {
            var filterBuilder = Builders<PostEntity>.Filter;
            var sortBuilder = Builders<PostEntity>.Sort;
            var sort = sortBuilder.Descending(x => x.CreateTime);
            var searchResult = await _postRep.PaginationSearchAsync(filterBuilder.Empty, sort, pageIndex: pageIndex,
                pageSize: pageSize);
            return _mapper.Map<Page<Post>>(searchResult);
        }

        public async Task<Post> GetPost(string postId)
        {
            var postEntity = await _postRep.GetByPropertyAsync(x => x.Sysid, postId.SafeToObjectId());
            return _mapper.Map<Post>(postEntity);
        }

        public async Task<Post> GetPostBySlug(string slug)
        {
            var postEntity = await _postRep.GetByPropertyAsync(x => x.Slug, slug);
            return _mapper.Map<Post>(postEntity);
        }
    }
}