using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models.AdminModels;

namespace OpenBlog.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public AdminController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(PostList));
        }

        [HttpGet]
        public IActionResult NewPost()
        {
            var viewModel = new PostCreateViewModel();
            return View(viewModel);
        }

        [HttpPost, ActionName("NewPost")]
        public async Task<IActionResult> NewPostSubmit(PostCreateViewModel postCreateViewModel)
        {
            var postEntity = _mapper.Map<Post>(postCreateViewModel);
            await _postRepository.CreatePostAsync(postEntity);
            return RedirectToAction(nameof(PostList));
        }

        [HttpGet]
        public async Task<IActionResult> PostList()
        {
            var postSearchResult = await _postRepository.SearchPost(1, 20);
            var postList = _mapper.Map<List<PostListItem>>(postSearchResult.Records);
            return View(postList);
        }
    }
}
