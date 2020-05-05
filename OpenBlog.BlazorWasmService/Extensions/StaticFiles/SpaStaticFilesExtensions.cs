// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenBlog.BlazorWasmService.Extensions.Util;

namespace OpenBlog.BlazorWasmService.Extensions.StaticFiles
{
    /// <summary>
    /// Extension methods for configuring an application to serve static files for a
    /// Single Page Application (SPA).
    /// </summary>
    public static class SpaStaticFilesExtensions
    {
        /// <summary>
        /// Registers an <see>
        ///     <cref>ISpaStaticFileProvider</cref>
        /// </see>
        /// service that can provide static
        /// files to be served for a Single Page Application (SPA).
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">If specified, this callback will be invoked to set additional configuration options.</param>
        public static void AddMultipleSpaStaticFiles(
            this IServiceCollection services,
            Action<SpaStaticFilesOptions> configuration = null)
        {
            services.AddSingleton<IMulitSpaStaticFileProvider>(serviceProvider =>
            {
                // Use the options configured in DI (or blank if none was configured)
                var optionsProvider = serviceProvider.GetService<IOptions<SpaStaticFilesOptions>>();
                var options = optionsProvider.Value;

                // Allow the developer to perform further configuration
                configuration?.Invoke(options);

                if (string.IsNullOrEmpty(options.RootPath))
                {
                    throw new InvalidOperationException($"No {nameof(SpaStaticFilesOptions.RootPath)} " +
                        $"was set on the {nameof(SpaStaticFilesOptions)}.");
                }

                return new DefaultSpaStaticFileProvider(serviceProvider, options);
            });
        }

        // /// <summary>
        // /// Configures the application to serve static files for a Single Page Application (SPA).
        // /// The files will be located using the registered <see cref="ISpaStaticFileProvider"/> service.
        // /// </summary>
        // /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        // public static void UseSpaStaticFiles(this IApplicationBuilder applicationBuilder, string clientAppDistPath)
        // {
        //     UseMulitSpaStaticFiles(applicationBuilder, clientAppDistPath, new StaticFileOptions());
        // }

        // /// <summary>
        // /// Configures the application to serve static files for a Single Page Application (SPA).
        // /// The files will be located using the registered <see cref="ISpaStaticFileProvider"/> service.
        // /// </summary>
        // /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        // /// <param name="options">Specifies options for serving the static files.</param>
        // public static void UseMultipleSpaStaticFiles(this IApplicationBuilder applicationBuilder, string publicPath, StaticFileOptions options)
        // {
        //     if (applicationBuilder == null)
        //     {
        //         throw new ArgumentNullException(nameof(applicationBuilder));
        //     }
        //
        //     if (options == null)
        //     {
        //         throw new ArgumentNullException(nameof(options));
        //     }
        //
        //     UseSpaStaticFilesInternal(applicationBuilder, publicPath, staticFileOptions: options, allowFallbackOnServingWebRootFiles: false);
        // }

        internal static void UseSpaStaticFilesInternal(
            this IApplicationBuilder app,
            string publicPath,
            string clientAppDistPath,
            StaticFileOptions staticFileOptions,
            bool allowFallbackOnServingWebRootFiles)
        {
            var logger = LoggerFinder.GetOrCreateLogger(app, "OpenBlog.SpaStaticFilesExtensions");

            if (staticFileOptions == null)
            {
                logger.LogWarning($"{nameof(staticFileOptions)} argument is null");
                throw new ArgumentNullException(nameof(staticFileOptions));
            }

            // If the file provider was explicitly supplied, that takes precedence over any other
            // configured file provider. This is most useful if the application hosts multiple SPAs
            // (via multiple calls to UseSpa()), so each needs to serve its own separate static files
            // instead of using AddSpaStaticFiles/UseSpaStaticFiles.
            // But if no file provider was specified, try to get one from the DI config.
            if (staticFileOptions.FileProvider == null)
            {
                var shouldServeStaticFiles = ShouldServeStaticFiles(
                    app,publicPath, clientAppDistPath,
                    allowFallbackOnServingWebRootFiles,
                    out var fileProviderOrDefault);
                if (shouldServeStaticFiles)
                {
                    logger.LogInformation($"use static files serve");
                    staticFileOptions.FileProvider = fileProviderOrDefault;
                }
                else
                {
                    logger.LogWarning($"don't serve static files");
                    // The registered ISpaStaticFileProvider says we shouldn't
                    // serve static files
                    return;
                }
            }

            app.UseStaticFiles(staticFileOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="publicPath">对外的请求子目录</param>
        /// <param name="clientAppDistPath">ClientApp存储物理目录</param>
        /// <param name="allowFallbackOnServingWebRootFiles"></param>
        /// <param name="fileProviderOrDefault"></param>
        /// <returns></returns> 
        /// <exception cref="InvalidOperationException"></exception>
        private static bool ShouldServeStaticFiles(
            IApplicationBuilder app, string publicPath, string clientAppDistPath,
            bool allowFallbackOnServingWebRootFiles,
            out IFileProvider fileProviderOrDefault)
        {
            var spaStaticFilesService = app.ApplicationServices.GetService<IMulitSpaStaticFileProvider>();
            if (!string.IsNullOrWhiteSpace(clientAppDistPath) && spaStaticFilesService != null)
            {
                // If an ISpaStaticFileProvider was configured but it says no IFileProvider is available
                // (i.e., it supplies 'null'), this implies we should not serve any static files. This
                // is typically the case in development when SPA static files are being served from a
                // SPA development server (e.g., Angular CLI or create-react-app), in which case no
                // directory of prebuilt files will exist on disk.
                fileProviderOrDefault = spaStaticFilesService.GetAppFileProvider(publicPath, clientAppDistPath);
                return fileProviderOrDefault != null;
            }
            else if (!allowFallbackOnServingWebRootFiles)
            {
                throw new InvalidOperationException($"To use {nameof(UseSpaStaticFilesInternal)}, you must " +
                    $"first register an {nameof(IMulitSpaStaticFileProvider)} in the service provider, typically " +
                    $"by calling services.{nameof(AddMultipleSpaStaticFiles)}.");
            }
            else
            {
                // Fall back on serving wwwroot
                fileProviderOrDefault = null;
                return true;
            }
        }
    }
}
