using System;
using OpenBlog.BlazorWasmService.VueCli;

namespace OpenBlog.BlazorWasmService.BlazorWasmCli
{
    public static class BlazorWasmCliMiddlewareExtensions
    {
        public static void UseBlazorWasmCliServer(
            this IMulitSpaBuilder spaBuilder,
            string startScript = "watch run")
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var spaOptions = spaBuilder.Options;

            if (string.IsNullOrEmpty(spaOptions.SourcePath))
            {
                throw new InvalidOperationException(
                    $"To use {nameof(UseBlazorWasmCliServer)}, you must supply a non-empty value for the {nameof(MulitSpaOptions.SourcePath)} property of {nameof(MulitSpaOptions)} when calling {nameof(MulitSpaApplicationBuilderExtensions.UseMulitSpa)}.");
            }

            BalzorWasmCliMiddleware.Attach(spaBuilder, startScript);
        }
    }
}