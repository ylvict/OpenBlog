using System;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using OpenBlog.Web.WebFramework.Sessions;

namespace OpenBlog.Web.WebFramework
{
    public abstract class BaseRazorPage<T> : RazorPage<T>
    {
        private IServiceProvider ServiceProvider => Context.RequestServices;

        public IUserSession UserSession => ServiceProvider.GetService<IUserSession>();
    }
}
