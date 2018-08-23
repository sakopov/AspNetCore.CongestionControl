// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderBasedClientIdentifierProvider.cs">
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
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The client identifier provider which looks at the configured
    /// header in the incoming request to get client identifier.
    /// </summary>
    public class HeaderBasedClientIdentifierProvider : IClientIdentifierProvider
    {
        /// <summary>
        /// The name of the header containing client identifier.
        /// </summary>
        private readonly string _headerName;

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderBasedClientIdentifierProvider"/> class.
        /// </summary>
        /// <param name="headerName">
        /// The name of the header containing client identifier.
        /// </param>
        public HeaderBasedClientIdentifierProvider(string headerName = "x-client-id")
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException(nameof(headerName));
            }

            _headerName = headerName;
        }

        /// <summary>
        /// Gets client identifier from headers.
        /// </summary>
        /// <param name="httpContext">
        /// The context for current HTTP request.
        /// </param>
        /// <returns>
        /// The client identifier.
        /// </returns>
        public Task<string> ExecuteAsync(HttpContext httpContext)
        {
            var clientId = "anonymous";

            if (httpContext?.Request?.Headers?.TryGetValue(_headerName, out var value) ?? false)
            {
                clientId = value.ToString();
            }

            return Task.FromResult(clientId);
        }
    }
}