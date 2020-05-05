// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBlog.BlazorWasmService.Extensions.StaticFiles;
using OpenBlog.BlazorWasmService.Extensions.Util;

namespace OpenBlog.BlazorWasmService
{
    internal class SpaDefaultPageMiddleware
    {
        private const string LogCategoryName = "OpenBlog.SpaDefaultPageMiddleware";

        public static void Attach(IMulitSpaBuilder spaBuilder)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var app = spaBuilder.ApplicationBuilder;
            var options = spaBuilder.Options;

            var appBuilder = spaBuilder.ApplicationBuilder;
            var logger = LoggerFinder.GetOrCreateLogger(appBuilder, LogCategoryName);
            logger.LogInformation($"Public path:{options.PublicPath}");
            
            // 先处理静态资源文件, 未找到的重置为index.html来处理
            app.UseSpaStaticFilesInternal(options.PublicPath, spaBuilder.Options.DistPath,
                options.DefaultPageStaticFileOptions ?? new StaticFileOptions(),
                allowFallbackOnServingWebRootFiles: true);

            // 剩余的请求全部转发到默认页面
            app.Use((context, next) =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var localLogger = loggerFactory.CreateLogger($"{LogCategoryName}.Request");
                localLogger.LogInformation($"Request {context.Request.Path}");
                if (!context.Request.Path.StartsWithSegments(PathString.FromUriComponent(options.PublicPath)))
                {
                    return next();
                }

                // If we have an Endpoint, then this is a deferred match - just noop.
                if (context.GetEndpoint() != null)
                {
                    return next();
                }

                context.Request.Path = options.DefaultPage;
                localLogger.LogInformation($"Reset request path to {options.DefaultPage}");
                return next();
            });

            // Serve it as a static file
            // Developers who need to host more than one SPA with distinct default pages can
            // override the file provider
            app.UseSpaStaticFilesInternal(options.PublicPath, spaBuilder.Options.DistPath,
                options.DefaultPageStaticFileOptions ?? new StaticFileOptions(),
                allowFallbackOnServingWebRootFiles: true);

            // If the default file didn't get served as a static file (usually because it was not
            // present on disk), the SPA is definitely not going to work.
            app.Use((context, next) =>
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var localLogger = loggerFactory.CreateLogger($"{LogCategoryName}.2ndRequest");

                if (!context.Request.Path.StartsWithSegments(PathString.FromUriComponent(options.PublicPath)))
                {
                    localLogger.LogInformation("Not public path, Move to next");
                    return next();
                }

                // If we have an Endpoint, then this is a deferred match - just noop.
                if (context.GetEndpoint() != null)
                {
                    localLogger.LogInformation("context.GetEndpoint() != null, Move to next");
                    return next();
                }

                var message = "The SPA default page middleware could not return the default page " +
                              $"'{options.DefaultPage}' because it was not found, and no other middleware " +
                              "handled the request.\n";

                // Try to clarify the common scenario where someone runs an application in
                // Production environment without first publishing the whole application
                // or at least building the SPA.
                var hostEnvironment =
                    (IWebHostEnvironment) context.RequestServices.GetService(typeof(IWebHostEnvironment));
                if (hostEnvironment != null && hostEnvironment.IsProduction())
                {
                    message += "Your application is running in Production mode, so make sure it has " +
                               "been published, or that you have built your SPA manually. Alternatively you " +
                               "may wish to switch to the Development environment.\n";
                }

                throw new InvalidOperationException(message);
            });
        }
    }
}