using OpenBlog.WebFramework.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace OpenBlog.WebFramework
{
    public abstract class BaseRazorPage<T> : RazorPage<T>
    {
        private IServiceProvider ServiceProvider => Context.RequestServices;

        public IUserSession UserSession => ServiceProvider.GetService<IUserSession>();
    }
}
