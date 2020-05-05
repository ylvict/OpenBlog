using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Niusys;

namespace OpenBlog.Web.WebFramework.RouteTransformers
{
    public class BloggerTransformer : DynamicRouteValueTransformer
    {
        private readonly ILogger<BloggerTransformer> _logger;

        public BloggerTransformer(ILogger<BloggerTransformer> logger)
        {
            _logger = logger;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
            RouteValueDictionary values)
        {
            _logger.LogInformation(
                $"Request Path:{httpContext.Request.Path} RouteValues whether null:{values == null},{JsonConvert.SerializeObject(values)}");
            if (values == null)
                return null;
            
            if (!values.Any())
            {
                // means blog root
                values.TryAdd("controller", "Post");
                values.TryAdd("action", "Index");
            }
            else
            {
                var slug = values["slug"]?.ToString()?.TrimEnd('/').ToLower();
                var urlCategoryPart = values["category"]?.ToString();
                if (!string.IsNullOrEmpty(urlCategoryPart))
                {
                    ProcessCategoryRoute(values, urlCategoryPart, slug);
                }
                else if(int.TryParse(values["year"]?.ToString(),out var year) && int.TryParse(values["month"]?.ToString(),out var month))
                {
                    ProcessYearMonthRoute(values, year, month, slug);
                }
                else
                {
                    return null;
                }
            }

            await Task.CompletedTask;
            return values;
        }

        private static void ProcessYearMonthRoute(RouteValueDictionary values, int year, int month, string slug)
        {
            values.TryAdd("year", year);
            values.TryAdd("month", month);
            if (slug.IsNullOrWhitespace())
            {
                values.TryAdd("controller", "Post");
                values.TryAdd("action", "PostListByMonth");
            }
            else
            {
                values.TryAdd("controller", "Post");
                values.TryAdd("action", "ViewPostBySlug");
                values.TryAdd("slug", slug);
            }
        }

        private static void ProcessCategoryRoute(RouteValueDictionary values, string urlCategoryPart, string slug)
        {
            switch (urlCategoryPart.ToLower())
            {
                case "category":
                    values.TryAdd("controller", "Post");
                    values.TryAdd("action", "PostListByCategory");
                    values.TryAdd("categoryName", slug);
                    break;
                case "tag":
                    values.TryAdd("controller", "Post");
                    values.TryAdd("action", "PostListByTag");
                    values.TryAdd("tagName", slug);
                    break;
                default:
                    break;
            }
        }
    }
}