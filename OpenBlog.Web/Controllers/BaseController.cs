using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using OpenBlog.Web.WebFramework.Notifications;
using System.Text;

namespace OpenBlog.Web.Controllers
{
    public class BaseController : Controller
    {
        #region Notification(通知提示)

        private INotificationService NotificationService => HttpContext.RequestServices.GetService<INotificationService>();
        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        protected void Notification(NotifyType type, string message, bool encode = true)
        {
            NotificationService.Notification(type, message, encode);
        }

        /// <summary>
        /// 显示操作成功提示
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        protected void SuccessNotification(string message, bool encode = true)
        {
            NotificationService.SuccessNotification(message, encode);
        }

        /// <summary>
        /// 显示警告提示
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        protected void WarningNotification(string message, bool encode = true)
        {
            NotificationService.WarningNotification(message, encode);
        }

        /// <summary>
        /// 显示错误提示
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        protected void ErrorNotification(string message, bool encode = true)
        {
            NotificationService.ErrorNotification(message, encode);
        }
        #endregion

        public IActionResult RedirectToRefererPage()
        {
            return Redirect(Request.Headers["Referer"].ToString());
        }

        protected Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
    }
}