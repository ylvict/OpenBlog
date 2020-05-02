using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using MimeKit;
using Niusys;
using OpenBlog.Web.Models;
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
        public async Task<IActionResult> FormSubmit(
            [FromServices] EmailService emailService,
            [FromServices] IOptions<EmailSetting> emailSettingOptions,
            [FromServices] IOptions<EmailReceivers> emailReceiversOptions)
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
                if (Request.Form.ContainsKey("form_type") && Request.Form["form_type"].ToString().Equals("contact"))
                {
                    await ProcessContactMessage(emailService, emailReceiversOptions.Value);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Content(ex.FullMessage());
            }
        }

        #region Message Handler

        private async Task ProcessContactMessage(EmailService emailService, EmailReceivers emailReceiversOptions)
        {
            var model = new ContactSubmitViewModel();

            var formValueProvider = new FormValueProvider(BindingSource.Form, Request.Form, CultureInfo.CurrentCulture);
            await TryUpdateModelAsync(model, prefix: "", valueProvider: formValueProvider);

            var subject = $"OpenBlog New Connect";
            var content = $"Name: {model.Name} <br/> Email:{model.Email}  <br/> Message:{model.Message}";
            await emailService.SendEmailAsync(emailReceiversOptions.Contacts, subject, content);
        }

        #endregion
    }
}