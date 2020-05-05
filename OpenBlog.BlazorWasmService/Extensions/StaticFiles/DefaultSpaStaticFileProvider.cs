// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace OpenBlog.BlazorWasmService.Extensions.StaticFiles
{
    /// <summary>
    /// Provides an implementation of <see cref="ISpaStaticFileProvider"/> that supplies
    /// physical files at a location configured using <see cref="SpaStaticFilesOptions"/>.
    /// </summary>
    internal class DefaultSpaStaticFileProvider : IMulitSpaStaticFileProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SpaStaticFilesOptions _options;
        private readonly ILogger<DefaultSpaStaticFileProvider> _logger;

        private readonly Dictionary<string, IFileProvider> _fileProviderStore;

        public DefaultSpaStaticFileProvider(
            IServiceProvider serviceProvider,
            SpaStaticFilesOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.RootPath))
            {
                throw new ArgumentException($"The {nameof(options.RootPath)} property " +
                                            $"of {nameof(options)} cannot be null or empty.");
            }

            _serviceProvider = serviceProvider;
            _options = options;
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DefaultSpaStaticFileProvider>();
            _fileProviderStore = new Dictionary<string, IFileProvider>();
        }

        public IFileProvider GetAppFileProvider(string publicPath, string clientAppDistPath)
        {
            if (!_fileProviderStore.ContainsKey(publicPath))
            {
                _logger.LogInformation($"Fill Provider Store not contain app {publicPath}");
                var env = _serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var absoluteRootPath = Path.Combine(
                    env.ContentRootPath, //application path
                    _options.RootPath, // spa root path
                    clientAppDistPath //client app path
                );
                _logger.LogInformation(
                    $"Content Root: {env.ContentRootPath} Spa Root Path: {_options.RootPath} ClientAppDistPath: {clientAppDistPath} Public Path: {publicPath}");
                // PhysicalFileProvider will throw if you pass a non-existent path,
                // but we don't want that scenario to be an error because for SPA
                // scenarios, it's better if non-existing directory just means we
                // don't serve any static files.
                if (Directory.Exists(absoluteRootPath))
                {
                    _logger.LogInformation($"{absoluteRootPath} path exist");
                    _fileProviderStore.TryAdd(publicPath, new PhysicalFileProvider(absoluteRootPath));
                }
                else
                {
                    _logger.LogWarning($"{absoluteRootPath} path not exist");
                }
            }

            return _fileProviderStore.ContainsKey(publicPath) ? _fileProviderStore[publicPath] : null;
        }
    }
}