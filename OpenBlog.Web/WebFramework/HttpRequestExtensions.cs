using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Niusys;

namespace OpenBlog.Web.WebFramework
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }

        public static T GetHeaderValueAs<T>(this HttpRequest httpRequest, string headerName)
        {
            if (httpRequest?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                string rawValues = values.ToString(); // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrEmpty())
                {
                    return (T) Convert.ChangeType(values.ToString(), typeof(T));
                }
            }

            return default(T);
        }

        public static T GetContextItemValueAs<T>(this HttpContext httpContext, string itemName)
        {
            object values = null;
            if (httpContext?.Items?.TryGetValue(itemName, out values) ?? false)
            {
                if (values != null)
                {
                    return (T) Convert.ChangeType(values, typeof(T));
                }
            }

            return default(T);
        }

        #region IsStaticResource

        private const string StaticFileNameSuffix = @"\.jpg$|\.js$|\.png$|\.woff2$|\.css$|\.ico$|\.map$";

        public static bool IsStaticResource(this HttpRequest httpRequest)
        {
            if (httpRequest is null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            return Regex.IsMatch(httpRequest.Path.Value, StaticFileNameSuffix);
        }

        public static bool IsStaticFileName(this string fileName)
        {
            return Regex.IsMatch(fileName, StaticFileNameSuffix);
        }

        #endregion
    }
}