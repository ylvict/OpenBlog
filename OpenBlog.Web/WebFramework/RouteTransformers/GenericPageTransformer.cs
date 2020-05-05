using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OpenBlog.Web.WebFramework.RouteTransformers
{
    public enum GenericPages
    {
        About,
        Contact,
        History
    }

    public class GenericPageTransformer : DynamicRouteValueTransformer
    {
        private readonly ILogger<GenericPageTransformer> _logger;

        public GenericPageTransformer(ILogger<GenericPageTransformer> logger)
        {
            _logger = logger;
        }
        private static Dictionary<string, string> SlugViewMapping { get; set; }

        static GenericPageTransformer()
        {
            var genericPageUrls = typeof(GenericPages).GetFields();
            SlugViewMapping = new Dictionary<string, string>();
            foreach (var item in genericPageUrls)
            {
                SlugViewMapping.TryAdd(item.Name.ToLower(), item.Name);
            }
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
            RouteValueDictionary values)
        {
            values ??= new RouteValueDictionary();

            var slug = values["slug"]?.ToString()?.TrimEnd('/').ToLower();
            _logger.LogInformation($"Slug:{slug}");
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            //if (Regex.IsMatch(slug, ReservedActionSlug))
            if (SlugViewMapping.ContainsKey(slug))
            {
                values.TryAdd("controller", "GenericPage");
                values.TryAdd("action", "ViewPage");
                values.TryAdd("viewName", SlugViewMapping[slug]);
                return values;
            }

            await Task.CompletedTask;
            return null;
        }
    }
}