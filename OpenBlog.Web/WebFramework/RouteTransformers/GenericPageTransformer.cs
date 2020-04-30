using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenBlog.Web.WebFramework.RouteTransformers
{
    public enum GenericPages
    {
        Abount,
        Contact
    }

    public class GenericPageTransformer : DynamicRouteValueTransformer
    {
        public static Dictionary<string, string> SlugViewMapping { get; private set; }

        static GenericPageTransformer()
        {
            var genericPageUrls = typeof(GenericPages).GetFields();
            SlugViewMapping = new Dictionary<string, string>();
            foreach (var item in genericPageUrls)
            {
                SlugViewMapping.TryAdd(item.GetValue(null).ToString(), item.Name);
            }
        }

        public async override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
            RouteValueDictionary values)
        {
            if (values == null)
                values = new RouteValueDictionary();

            var slug = values["slug"]?.ToString().TrimEnd('/').ToLower();
            if (string.IsNullOrWhiteSpace(slug))
                return values;

            //if (Regex.IsMatch(slug, ReservedActionSlug))
            if (SlugViewMapping.ContainsKey(slug))
            {
                values.TryAdd("controller", "GenericPage");
                values.TryAdd("action", "ViewPage");
                values.TryAdd("viewName", SlugViewMapping[slug]);
                return values;
            }

            await Task.CompletedTask;
            return values;
        }
    }
}