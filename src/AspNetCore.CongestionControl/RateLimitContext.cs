// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RateLimitContext.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
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
    using System.Net;

    /// <summary>
    /// The contextual information about rate-limited request.
    /// </summary>
    public class RateLimitContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RateLimitContext"/>
        /// class.
        /// </summary>
        /// <param name="remaining">
        /// The number of remaining requests.
        /// </param>
        /// <param name="limit">
        /// The number of allowed requests.
        /// </param>
        /// <param name="httpStatusCode">
        /// The HTTP status code.
        /// </param>
        /// <param name="source">
        /// Gets the source responsible for rate-limiting a request.
        /// </param>
        public RateLimitContext(
            int remaining,
            int limit,
            HttpStatusCode httpStatusCode,
            string source)
        {
            Remaining = remaining;
            Limit = limit;
            HttpStatusCode = httpStatusCode;
            Source = source;
        }

        /// <summary>
        /// Gets the number of remaining requests.
        /// </summary>
        public int Remaining { get; }

        /// <summary>
        /// Gets the number of allowed requests.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets the HTTP status code to return in the response.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// Gets the source responsible for rate-limiting a request.
        /// </summary>
        public string Source { get; }
    }
}
