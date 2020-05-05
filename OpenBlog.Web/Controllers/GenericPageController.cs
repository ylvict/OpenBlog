using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using Niusys;
using OpenBlog.DomainModels;
using OpenBlog.Repository.Mongo;
using OpenBlog.Web.Models;
using OpenBlog.Web.Services;
using OpenBlog.Web.Services.EmailServices;

namespace OpenBlog.Web.Controllers
{
    public class GenericPageController : Controller
    {
        public IActionResult ViewPage([FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ICompositeViewEngine compositeViewEngine, string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return RedirectToRoutePermanent("HomePage");
            }

            var viewResult = compositeViewEngine.FindView(actionContextAccessor.ActionContext, viewName, false);
            if (viewResult.Success)
            {
                ViewBag.PageName = viewName;
                return View(viewName);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FormSubmit([FromServices] IOptions<EmailSetting> emailSettingOptions)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            if (Request.HasFormContentType)
            {
                await HttpContext.Request.ReadFormAsync();
            }

            var emailSetting = emailSettingOptions.Value;

            // 非法字符过滤
            var keywords = emailSetting.IllegalChars;
            foreach (var (key, value) in Request.Form)
            {
                switch (key)
                {
                    case "__RequestVerificationToken":
                    case "form_type":
                    case "submit":
                        continue;
                    default:
                        if (Regex.IsMatch(value, keywords))
                        {
                            return NotFound();
                        }

                        break;
                }
            }

            try
            {
                if (!Request.Form.ContainsKey("form_type"))
                {
                    return Ok();
                }

                switch (Request.Form["form_type"].ToString())
                {
                    case "contact":
                        // Contact Form Submit
                        await ProcessContactFormSubmit();
                        return Ok();
                    case "postComment":
                        // Blog Post Comment Form Submit
                        await ProcessPostCommentFormSubmit();
                        return Ok();
                    case "globalSearch":
                        var model = await GetBindModel<GlobalSearchForm>();
                        return Redirect($"/search?q={model.SearchText}");
                    default:
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.FullMessage());
            }
        }

        #region Global Search Form

        class GlobalSearchForm
        {
            public string SearchText { get; set; }
        }

        #endregion

        #region Contact Form
        private async Task ProcessContactFormSubmit()
        {
            var model = await GetBindModel<ContactService.ContactSubmitViewModel>();
            var contactService = HttpContext.RequestServices.GetRequiredService<ContactService>();
            await contactService.ProcessContactMessage(model);
        }
        #endregion

        #region Post Comment Form
        private async Task ProcessPostCommentFormSubmit()
        {
            var model = await GetBindModel<PostCommentModel>();

            var commentRep = HttpContext.RequestServices.GetRequiredService<ICommentRepository>();
            var commentModel = new Comment()
            {
                PostId = model.CommentPostId,
                CommentParentId = model.CommentParentId,
                Author = model.Author,
                Email = model.Email,
                Url = model.Url,
                Content = model.Content
            };
            await commentRep.CreateCommentForPost(commentModel);
        }

        class PostCommentModel
        {
            public string CommentPostId { get; set; }
            public string CommentParentId { get; set; }
            public string Author { get; set; }
            public string Email { get; set; }
            public string Url { get; set; }
            public string Content { get; set; }
        }
        #endregion

        #region Utils
        private async Task<T> GetBindModel<T>() where T : class, new()
        {
            var model = new T();
            var formValueProvider = new FormValueProvider(BindingSource.Form, Request.Form, CultureInfo.CurrentCulture);
            await TryUpdateModelAsync(model, prefix: "", valueProvider: formValueProvider);
            return model;
        }
        #endregion
    }
}