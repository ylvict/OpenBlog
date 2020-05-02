using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace OpenBlog.Web.WebFramework
{
    public static class HttpContextExtensions
    {
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