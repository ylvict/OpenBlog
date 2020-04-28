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
            var postList = _mapper.Map<List<PostListItem>>(postSearchResult.Records);
            return View(postList);
        }
    }
}
