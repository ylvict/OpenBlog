using Microsoft.AspNetCore.Mvc;

namespace OpenBlog.Web.WebFramework.Install
{
    public class SystemInitializedCheckAttribute : TypeFilterAttribute
    {
        public SystemInitializedCheckAttribute()
            : base(typeof(SystemInitializedCheckAsyncActionFilter))
        {
         
        }
    }
}