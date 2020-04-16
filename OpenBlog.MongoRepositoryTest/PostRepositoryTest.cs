using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenBlog.DomainModels;
using OpenBlog.MongoRepositoryTest.Abstracts;
using OpenBlog.MongoRepositoryTest.Abstracts;
using OpenBlog.Repository.Mongo;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OpenBlog.MongoRepositoryTest
{
    public class PostRepositoryTest : RepositoryUnitTest
    {
        public PostRepositoryTest(TestClassFixture factory, ITestOutputHelper testOutputHelper)
            : base(factory, testOutputHelper)
        {
            factory.ConfigureServicee(services =>
            {
                services.AddSingleton<IPostRepository, PostRepository>();
            });
        }

        [Fact(DisplayName = "博客文章创建测试")]
        public async Task PostCreateTest()
        {
            var postRep = ServiceProvider.GetService<IPostRepository>();
            var result = await postRep.CreatePostAsync(new Post());
            WriteLine(result);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "博客文章分页查询测试")]
        public async Task PostPageListSearchTest()
        {
            var postRep = ServiceProvider.GetService<IPostRepository>();
            var result = await postRep.SearchPost(1, 10);
            WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.NotNull(result);
            Assert.NotNull(result.Paging);
            Assert.NotNull(result.Records);
        }
    }
}
