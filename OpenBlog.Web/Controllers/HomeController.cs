using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Niusys.Extensions.ComponentModels;
using Niusys.Extensions.Storage.Mongo;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo;
using OpenBlog.Web.Models;
using OpenBlog.Web.WebFramework;

namespace OpenBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository,
            ICommentRepository commentRepository, IMapper mapper)
        {
            _logger = logger;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
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
            return postModel == null ? NotFound() : await ViewPostInternal(postModel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPostBySlug(string slug)
        {
            ObjectId postId = slug.SafeToObjectId();
            if (!postId.Equals(ObjectId.Empty))
            {
                return await ViewPost(postId.ToString());
            }

            var postModel = await _postRepository.GetPostBySlug(slug);
            return postModel == null ? NotFound() : await ViewPostInternal(postModel);
        }

        private async Task<IActionResult> ViewPostInternal(Post postModel)
        {
            var postDetailViewModel = _mapper.Map<PostDetailViewModel>(postModel);
            var rootCommentList = await _commentRepository.GetRootCommentListForPost(postModel.PostId, 1, int.MaxValue);
            var rootCommentIdList = rootCommentList.Records.Select(x => x.CommentId).ToList();
            postDetailViewModel.CommentList = _mapper.Map<Page<CommentListItemViewModel>>(rootCommentList);
            
            var childCommentList =
                await _commentRepository.GetCommentListForPostAndComment(postModel.PostId, rootCommentIdList);
            
            foreach (var currentComment in postDetailViewModel.CommentList.Records)
            {
                var currentCommentReplyList =
                    childCommentList.Where(x => x.CommentParentId == currentComment.CommentId).ToList();
                currentComment.ReplyComments = _mapper.Map<List<CommentListItemViewModel>>(currentCommentReplyList);
            }
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

        public IActionResult RouteNoMatch([FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ICompositeViewEngine compositeViewEngine, string httpStatusCode)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // 对于静态资源,返回原本的404
            if (feature?.OriginalPath?.IsStaticFileName() ?? false)
            {
                return NotFound();
            }

            var viewResult =
                compositeViewEngine.FindView(actionContextAccessor.ActionContext, viewName: httpStatusCode, false);
            if (!viewResult.Success) return RedirectToRoute("HomePage");
            ViewBag.StatusCode = httpStatusCode;
            ViewBag.OriginalPath = feature?.OriginalPath;
            ViewBag.OriginalQueryString = feature?.OriginalQueryString;
            return View(httpStatusCode);
        }
    }
}