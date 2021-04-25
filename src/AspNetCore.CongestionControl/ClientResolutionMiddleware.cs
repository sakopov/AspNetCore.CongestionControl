// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientResolutionMiddleware.cs">
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
    using Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// The middleware responsible for resolving and verifying client
    /// identity.
    /// </summary>
    public class ClientResolutionMiddleware
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
        /// The client identifier providers.
        /// </summary>
        private readonly IEnumerable<IClientIdentifierProvider> _clientIdentifierProviders;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes new instances of <see cref="RequestRateLimiterMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next item in the middleware pipeline.
        /// </param>
        /// <param name="configuration">
        /// The congestion control configuration.
        /// </param>
        /// <param name="clientIdentifierProviders">
        /// The client identifier providers.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ClientResolutionMiddleware(
            RequestDelegate next,
            CongestionControlConfiguration configuration,
            IEnumerable<IClientIdentifierProvider> clientIdentifierProviders,
            ILogger<ClientResolutionMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _clientIdentifierProviders = clientIdentifierProviders;
            _logger = logger;
        }

        /// <summary>
        /// Begins middleware execution.
        /// </summary>
        /// <param name="httpContext">
        /// The context for the active HTTP request.
        /// </param>
        /// <returns>
        /// The next task in the middleware pipeline if the request is successful; Otherwise,
        /// terminates the middleware pipeline execution and returns unsuccessful response.
        /// </returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string resolvedClientId = null;

            foreach (var provider in _clientIdentifierProviders)
            {
                resolvedClientId = await provider.ExecuteAsync(httpContext);

                if (resolvedClientId != null)
                {
                    _logger.LogInformation("Successfully resolved client {Client} using {Strategy}.", resolvedClientId, provider.GetType().Name);

                    break;
                }
            }

            if (!_configuration.AllowAnonymousClients && resolvedClientId == null)
            {
                _logger.LogInformation("No client identifier present in the request and anonymous clients are not allowed.");

                httpContext.Response.ContentType = httpContext.Request.ContentType;
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return;
            }

            var clientId = resolvedClientId ?? Guid.NewGuid().ToString();

            httpContext.Items.AddClientId(clientId);

            await _next(httpContext);
        }
    }
}
