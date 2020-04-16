﻿using AutoMapper;
using MongoDB.Driver;
using Niusys.Extensions.ComponentModels;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Abstracts;
using OpenBlog.Repository.Mongo.Entities;
using System;
using System.Threading.Tasks;

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

        public async Task<string> CreatePostAsync(Post post)
        {
            var postEntity = _mapper.Map<PostEntity>(post);
            await _postRep.AddAsync(postEntity);
            return postEntity.Sysid.ToString();
        }

        public async Task<Page<Post>> SearchPost(int pageIndex, int pageSize)
        {
            var filterBuilder = Builders<PostEntity>.Filter;
            var sortBuilder = Builders<PostEntity>.Sort;
            var sort = sortBuilder.Descending(x => x.CreateTime);
            var searchResult = await _postRep.PaginationSearchAsync(filterBuilder.Empty, sort, pageIndex: pageIndex, pageSize: pageSize);
            return _mapper.Map<Page<Post>>(searchResult);
        }
    }
}
