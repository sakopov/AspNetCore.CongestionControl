// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs">
//   Copyright (c) 2018 Sergey Akopov
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl
{
    using Microsoft.AspNetCore.Builder;
    using AspNetCore.CongestionControl.Configuration;

    /// <summary>
    /// The extensions for <see cref="IApplicationBuilder"/> interface.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds congestion control middleware.
        /// </summary>
        /// <param name="builder">
        /// The application builder.
        /// </param>
        /// <returns>
        /// The application builder.
        /// </returns>
        public static IApplicationBuilder UseCongestionControl(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ClientResolutionMiddleware>();

            if (builder.ApplicationServices.GetService(typeof(RequestRateLimiterConfiguration)) != null)
            {
                builder.UseMiddleware<RequestRateLimiterMiddleware>();
            }

            if (builder.ApplicationServices.GetService(typeof(ConcurrentRequestLimiterConfiguration)) != null)
            {
                builder.UseMiddleware<ConcurrentRequestLimiterMiddleware>();
            }

            return builder;
        }
    }
}
