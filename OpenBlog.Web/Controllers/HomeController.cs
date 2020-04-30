using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenBlog.DomainModels;
using OpenBlog.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenBlog.Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository, IMapper mapper)
        {
            _logger = logger;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var postSearchResult = await _postRepository.SearchPost(1, 20);
            var postList = _mapper.Map<List<PostPublicListItem>>(postSearchResult.Records);
            return View(postList);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPost(string postId)
        {
            var postModel = await _postRepository.GetPost(postId);
            var postDetailViewModel = _mapper.Map<PostDetailViewModel>(postModel);
            return View(postDetailViewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewPostBySlug(string slug)
        {
            var postModel = await _postRepository.GetPostBySlug(slug);
            if (postModel == null)
            {
                return NotFound();
            }
            var postDetailViewModel = _mapper.Map<PostDetailViewModel>(postModel);
            return View(nameof(ViewPost), postDetailViewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> PostListByCategory(string categoryName)
        {
            var postSearchResult = await _postRepository.SearchPost(1, 20);
            var postList = _mapper.Map<List<PostPublicListItem>>(postSearchResult.Records);
            return View(postList);
        }
        
        [HttpGet]
        public async Task<IActionResult> PostListByTag(string tagName)
        {
            var postSearchResult = await _postRepository.SearchPost(1, 20);
            var postList = _mapper.Map<List<PostPublicListItem>>(postSearchResult.Records);
            return View(postList);
        }
    }
}
