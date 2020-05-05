using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenBlog.BlazorWasmService.Extensions.Proxying
{
    public static class BlazorWasmProxyingExtensions
    {
        public static void UseProxyToSpaDevelopmentServer(
            this IMulitSpaBuilder spaBuilder,
            string baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                new Uri(baseUri));
        }
        
        public static void UseProxyToSpaDevelopmentServer(
            this IMulitSpaBuilder spaBuilder,
            Uri baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                () => Task.FromResult(baseUri));
        }
        
        public static void UseProxyToSpaDevelopmentServer(
            this IMulitSpaBuilder spaBuilder,
            Func<Task<Uri>> baseUriTaskFactory)
        {
            var applicationBuilder = spaBuilder.ApplicationBuilder;
            var applicationStoppingToken = GetStoppingToken(applicationBuilder);

            // Since we might want to proxy WebSockets requests (e.g., by default, AngularCliMiddleware
            // requires it), enable it for the app
            //applicationBuilder.UseWebSockets();

            // It's important not to time out the requests, as some of them might be to
            // server-sent event endpoints or similar, where it's expected that the response
            // takes an unlimited time and never actually completes
            var neverTimeOutHttpClient =
                SpaProxy.CreateHttpClientForProxy(Timeout.InfiniteTimeSpan);

            // Proxy all requests to the SPA development server
            applicationBuilder.Use(async (context, next) =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("BlazorWasmProxy");
                logger.LogInformation($"context.Request.Path: {context.Request.Path} PublicPath:{spaBuilder.Options.PublicPath}");
                if (context.Request.Path.StartsWithSegments(PathString.FromUriComponent(spaBuilder.Options.PublicPath)))
                {
                    logger.LogInformation($"Use BlazorWasmProxy to process the request");
                    var didProxyRequest = await SpaProxy.PerformProxyRequest(
                        context, neverTimeOutHttpClient, baseUriTaskFactory(), applicationStoppingToken,
                        proxy404s: true);
                }
                else
                {
                    await next();
                }
            });
        }

        private static CancellationToken GetStoppingToken(IApplicationBuilder appBuilder)
        {
            var applicationLifetime = appBuilder
                .ApplicationServices
                .GetService(typeof(IHostApplicationLifetime));
            return ((IHostApplicationLifetime)applicationLifetime).ApplicationStopping;
        }
    }
}