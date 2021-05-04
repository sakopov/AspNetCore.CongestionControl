// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestRateLimiterMiddleware.cs">
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
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Configuration;

    /// <summary>
    /// The middleware responsible for enforcing rate limiting of client
    /// requests over a time interval.
    /// </summary>
    public class RequestRateLimiterMiddleware
    {
        /// <summary>
        /// The next item in the middleware pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The congestion control configuration.
        /// </summary>
        private readonly CongestionControlConfiguration _configuration;

        /// <summary>
        /// The token bucket consumer.
        /// </summary>
        private readonly ITokenBucketConsumer _tokenBucketConsumer;

        /// <summary>
        /// The HTTP response formatter.
        /// </summary>
        private readonly IHttpResponseFormatter _httpResponseFormatter;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The number of tokens to consume per request.
        /// </summary>
        private const int RequestedTokens = 1;

        /// <summary>
        /// The name of the rate limiter.
        /// </summary>
        private const string RateLimiterName = "RequestRateLimiter";

        /// <summary>
        /// Initializes new instances of <see cref="RequestRateLimiterMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next item in the middleware pipeline.
        /// </param>
        /// <param name="configuration">
        /// The congestion control configuration.
        /// </param>
        /// <param name="tokenBucketConsumer">
        /// The token bucket consumer.
        /// </param>
        /// <param name="httpResponseFormatter">
        /// The rate limit response formatter.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RequestRateLimiterMiddleware(
            RequestDelegate next,
            CongestionControlConfiguration configuration,
            ITokenBucketConsumer tokenBucketConsumer,
            IHttpResponseFormatter httpResponseFormatter,
            ILogger<RequestRateLimiterMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _tokenBucketConsumer = tokenBucketConsumer;
            _httpResponseFormatter = httpResponseFormatter;
            _logger = logger;
        }

        /// <summary>
        /// Begins middleware execution.
        /// </summary>
        /// <param name="httpContext">
        /// The context for the active HTTP request.
        /// </param>
        /// <returns>
        /// The next task in the middleware pipeline if the request is allowed; Otherwise,
        /// terminates the middleware pipeline execution and returns unsuccessful response.
        /// </returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var clientId = httpContext.Items.GetClientId();

            var response = await _tokenBucketConsumer.ConsumeAsync(clientId, RequestedTokens);

            if (!response.IsAllowed)
            {
                _logger.LogInformation("Request is not allowed by Congestion Control Token Bucket.");

                await _httpResponseFormatter.FormatAsync(httpContext, new RateLimitContext(
                    remaining: response.Remaining,
                    limit: response.Limit,
                    httpStatusCode: (HttpStatusCode)_configuration.HttpStatusCode,
                    source: RateLimiterName
                ));

                return;
            }

            await _next(httpContext);
        }
    }
}
