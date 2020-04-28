using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var viewModel = new NewPostViewModel();
            return View(viewModel);
        }

        [HttpPost, ActionName("NewPost")]
        public async Task<IActionResult> NewPostSubmit(NewPostViewModel postViewModel)
        {
            var postEntity = _mapper.Map<Post>(postViewModel);
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
