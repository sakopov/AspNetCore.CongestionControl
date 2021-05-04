// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CongestionControlConfiguration.cs">
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

namespace AspNetCore.CongestionControl.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The top-level configuration class responsible for configuring
    /// all components of the library.
    /// </summary>
    public class CongestionControlConfiguration
    {
        /// <summary>
        /// Gets the Redis configuration.
        /// </summary>
        internal RedisConfiguration RedisConfiguration { get; private set; }

        /// <summary>
        /// Gets the request rate limiter configuration.
        /// </summary>
        internal RequestRateLimiterConfiguration RequestRateLimiterConfiguration { get; private set; }

        /// <summary>
        /// Gets the concurrent request limiter configuration.
        /// </summary>
        internal ConcurrentRequestLimiterConfiguration ConcurrentRequestLimiterConfiguration { get; private set; }

        /// <summary>
        /// Gets or sets the client identifier providers.
        /// </summary>
        internal IList<IClientIdentifierProvider> ClientIdentifierProviders { get; set; } = new List<IClientIdentifierProvider>();

        /// <summary>
        /// Gets or sets the HTTP response formatter.
        /// </summary>
        internal IHttpResponseFormatter HttpResponseFormatter { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code to return to the client when their
        /// request is rate limited. The default value is HTTP 429/Too Many Requests.
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        /// <summary>
        /// Gets or sets a value indicating whether anonymous clients are allowed. If
        /// set to false, anonymous clients will result in HTTP 401/Unauthorized response.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool AllowAnonymousClients { get; set; } = true;

        /// <summary>
        /// Adds request rate limiter using default configuration options.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddRequestRateLimiter()
        {
            var config = new RequestRateLimiterConfiguration();

            config.Validate();

            RequestRateLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds request rate limiter using the specified configuration.
        /// </summary>
        /// <param name="configure">
        /// The delegate to configure request rate limiter.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddRequestRateLimiter(Action<RequestRateLimiterConfiguration> configure)
        {
            var config = new RequestRateLimiterConfiguration();

            configure(config);

            config.Validate();

            RequestRateLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds concurrent request limiter using default configuration options.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddConcurrentRequestLimiter()
        {
            var config = new ConcurrentRequestLimiterConfiguration();

            config.Validate();

            ConcurrentRequestLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds concurrent request limiter using the specified configuration.
        /// </summary>
        /// <param name="configure">
        /// The delegate to configure concurrent request limiter.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddConcurrentRequestLimiter(Action<ConcurrentRequestLimiterConfiguration> configure)
        {
            var config = new ConcurrentRequestLimiterConfiguration();

            configure(config);

            config.Validate();

            ConcurrentRequestLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds Redis as distributed storage.
        /// </summary>
        /// <param name="connection">
        /// The connection string for the Redis server.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRedisStorage(string connection)
        {
            var config = new RedisConfiguration(connection);

            config.Validate();

            RedisConfiguration = config;
        }

        /// <summary>
        /// Adds header-based client identifier provider using the specified
        /// header name to retrieve unique client identifier.
        /// </summary>
        /// <param name="headerName">
        /// The name of the header containing client identifier.
        /// </param>
        public void AddHeaderBasedClientIdentifierProvider(string headerName)
        {
            ClientIdentifierProviders.Add(new HeaderBasedClientIdentifierProvider(headerName));
        }

        /// <summary>
        /// Adds header-based client identifier provider using the default `x-api-key`
        /// header name to retrieve unique client identifier.
        /// </summary>
        public void AddHeaderBasedClientIdentifierProvider()
        {
            ClientIdentifierProviders.Add(new HeaderBasedClientIdentifierProvider());
        }

        /// <summary>
        /// Adds query-based client identifier provider using the specified
        /// query string parameter name to retrieve unique client identifier.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the query string parameter containing client identifier.
        /// </param>
        public void AddQueryBasedClientIdentifierProvider(string parameterName)
        {
            ClientIdentifierProviders.Add(new QueryBasedClientIdentifierProvider(parameterName));
        }

        /// <summary>
        /// Adds query-based client identifier provider using the default `api_key`
        /// query string parameter to retrieve unique client identifier.
        /// </summary>
        public void AddQueryBasedClientIdentifierProvider()
        {
            ClientIdentifierProviders.Add(new QueryBasedClientIdentifierProvider());
        }

        /// <summary>
        /// Adds custom client identifier provider.
        /// </summary>
        /// <param name="clientIdentifierProvider">
        /// The implementation of <see cref="IClientIdentifierProvider"/> interface.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddClientIdentifierProvider(IClientIdentifierProvider clientIdentifierProvider)
        {
            ClientIdentifierProviders.Add(clientIdentifierProvider ?? throw new ArgumentNullException(nameof(clientIdentifierProvider)));
        }

        /// <summary>
        /// Adds custom HTTP response formatter.
        /// </summary>
        /// <param name="httpResponseFormatter">
        /// The implementation of <see cref="IHttpResponseFormatter"/> interface.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddHttpResponseFormatter(IHttpResponseFormatter httpResponseFormatter)
        {
            HttpResponseFormatter = httpResponseFormatter ?? throw new ArgumentNullException(nameof(httpResponseFormatter));
        }
    }
}
