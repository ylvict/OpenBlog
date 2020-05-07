using Microsoft.Extensions.Logging;
using OpenBlog.Web.WebFramework.Sessions;
using System;
using System.Threading.Tasks;
using WilderMinds.MetaWeblog;

namespace OpenBlog.Web.Services
{
    public class MetaWeblogProvider : IMetaWeblogProvider
    {
        private readonly ILogger<MetaWeblogProvider> _logger;
        private readonly IRequestSession _requestSession;

        public MetaWeblogProvider(ILogger<MetaWeblogProvider> logger, IRequestSession requestSession)
        {
            _logger = logger;
            _requestSession = requestSession;
        }
        public Task<int> AddCategoryAsync(string key, string username, string password, NewCategory category)
        {
            _logger.LogInformation($"Call {nameof(AddCategoryAsync)}");
            throw new NotImplementedException();
        }

        public Task<string> AddPageAsync(string blogid, string username, string password, Page page, bool publish)
        {
            _logger.LogInformation($"Call {nameof(AddPageAsync)}");
            throw new NotImplementedException();
        }

        public Task<string> AddPostAsync(string blogid, string username, string password, Post post, bool publish)
        {
            _logger.LogInformation($"Call {nameof(AddPostAsync)}");
            throw new NotImplementedException();
        }

        public Task<bool> DeletePageAsync(string blogid, string username, string password, string pageid)
        {
            _logger.LogInformation($"Call {nameof(DeletePageAsync)}");
            throw new NotImplementedException();
        }

        public Task<bool> DeletePostAsync(string key, string postid, string username, string password, bool publish)
        {
            _logger.LogInformation($"Call {nameof(DeletePostAsync)}");
            throw new NotImplementedException();
        }

        public Task<bool> EditPageAsync(string blogid, string pageid, string username, string password, Page page, bool publish)
        {
            _logger.LogInformation($"Call {nameof(EditPageAsync)}");
            throw new NotImplementedException();
        }

        public Task<bool> EditPostAsync(string postid, string username, string password, Post post, bool publish)
        {
            _logger.LogInformation($"Call {nameof(EditPostAsync)}");
            throw new NotImplementedException();
        }

        public Task<Author[]> GetAuthorsAsync(string blogid, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetAuthorsAsync)}");
            throw new NotImplementedException();
        }

        public Task<CategoryInfo[]> GetCategoriesAsync(string blogid, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetCategoriesAsync)}");
            throw new NotImplementedException();
        }

        public Task<Page> GetPageAsync(string blogid, string pageid, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetPageAsync)}");
            throw new NotImplementedException();
        }

        public Task<Page[]> GetPagesAsync(string blogid, string username, string password, int numPages)
        {
            _logger.LogInformation($"Call {nameof(GetPagesAsync)}");
            throw new NotImplementedException();
        }

        public Task<Post> GetPostAsync(string postid, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetPostAsync)}");
            throw new NotImplementedException();
        }

        public Task<Post[]> GetRecentPostsAsync(string blogid, string username, string password, int numberOfPosts)
        {
            _logger.LogInformation($"Call {nameof(GetRecentPostsAsync)}");
            throw new NotImplementedException();
        }

        public Task<UserInfo> GetUserInfoAsync(string key, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetUserInfoAsync)}");
            throw new NotImplementedException();
        }

        public async Task<BlogInfo[]> GetUsersBlogsAsync(string key, string username, string password)
        {
            _logger.LogInformation($"Call {nameof(GetUsersBlogsAsync)}");

            await Task.CompletedTask;
            return new BlogInfo[] {
                new BlogInfo() { blogid = "1", blogName = "Blog A", url = $"{_requestSession.Host}/blog_a" }, 
                new BlogInfo(){ blogid = "2", blogName = "Blog B", url = $"{_requestSession.Host}/blog_b" } 
            };
        }

        public Task<MediaObjectInfo> NewMediaObjectAsync(string blogid, string username, string password, MediaObject mediaObject)
        {
            _logger.LogInformation($"Call {nameof(NewMediaObjectAsync)}");
            throw new NotImplementedException();
        }
    }
}
