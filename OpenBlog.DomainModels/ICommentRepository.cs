using System.Collections.Generic;
using System.Threading.Tasks;
using Niusys.Extensions.ComponentModels;

namespace OpenBlog.DomainModels
{
    public interface ICommentRepository
    {
        /// <summary>
        /// 创建Comment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> CreateCommentForPost(Comment entity);

        /// <summary>
        /// 获取一级Comment
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<Page<Comment>> GetRootCommentListForPost(string postId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取二级comment
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="parentCommentIdList"></param>
        /// <returns></returns>
        Task<IList<Comment>> GetCommentListForPostAndComment(string postId, List<string> parentCommentIdList);

        /// <summary>
        /// 获取最新的x条comment
        /// </summary>
        /// <returns></returns>
        Task<IList<Comment>> GetLatestCommentList(int topLimit);
    }
}