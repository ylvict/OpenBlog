// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;

namespace OpenBlog.BlazorWasmService
{
    internal class DefaultSpaBuilder : IMulitSpaBuilder
    {
        public IApplicationBuilder ApplicationBuilder { get; }

        public MulitSpaOptions Options { get; }

        public DefaultSpaBuilder(IApplicationBuilder applicationBuilder, MulitSpaOptions options)
        {
            ApplicationBuilder = applicationBuilder 
                ?? throw new ArgumentNullException(nameof(applicationBuilder));

            Options = options
                ?? throw new ArgumentNullException(nameof(options));
        }
    }
}
