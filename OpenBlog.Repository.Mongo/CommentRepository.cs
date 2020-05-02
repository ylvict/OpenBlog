using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using Niusys.Extensions.ComponentModels;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo.Abstracts;
using OpenBlog.Repository.Mongo.Entities;

namespace OpenBlog.Repository.Mongo
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMapper _mapper;
        private readonly IAppNoSqlBaseRepository<CommentEntity> _commentRep;

        public CommentRepository(IMapper mapper, IAppNoSqlBaseRepository<CommentEntity> commentRep)
        {
            _mapper = mapper;
            _commentRep = commentRep;
        }

        public async Task<string> CreateCommentForPost(Comment model)
        {
            var entity = _mapper.Map<CommentEntity>(model);
            entity.CreateTime = DateTime.Now;
            await _commentRep.AddAsync(entity);
            return entity.Sysid.ToString();
        }

        public async Task<Page<Comment>> GetRootCommentListForPost(string postId, int pageIndex, int pageSize)
        {
            var filterBuilder = Builders<CommentEntity>.Filter;
            var filter = filterBuilder.Eq(x => x.PostId, postId)
                         & filterBuilder.Eq(x => x.CommentParentId, null);
            var sort = Builders<CommentEntity>.Sort.Descending(x => x.CreateTime);
            var pageList = await _commentRep.PaginationSearchAsync(filter, sort, pageIndex, pageSize);
            return _mapper.Map<Page<Comment>>(pageList);
        }

        public async Task<IList<Comment>> GetCommentListForPostAndComment(string postId,
            List<string> parentCommentIdList)
        {
            var filterBuilder = Builders<CommentEntity>.Filter;
            var filter = filterBuilder.Eq(x => x.PostId, postId)
                         & filterBuilder.In(x => x.CommentParentId, parentCommentIdList);
            var sort = Builders<CommentEntity>.Sort.Ascending(x => x.CreateTime);
            var searchList= await _commentRep.SearchAsync(filter, sort);
            return _mapper.Map<IList<Comment>>(searchList);
        }

        public async Task<IList<Comment>> GetLatestCommentList(int topLimit)
        {
            var filter = Builders<CommentEntity>.Filter.Empty;
            var sort = Builders<CommentEntity>.Sort.Descending(x => x.CreateTime);
            var searchList= await _commentRep.SearchAsync(filter, sort,topLimit);
            return _mapper.Map<IList<Comment>>(searchList);
        }
    }
}