// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTokenBucketConsumer.cs">
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
    using Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The token bucket consumer which uses memory for token storage.
    /// </summary>
    public class InMemoryTokenBucketConsumer : ITokenBucketConsumer
    {
        /// <summary>
        /// The request rate limiter configuration.
        /// </summary>
        private readonly RequestRateLimiterConfiguration _configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<InMemoryTokenBucketConsumer> _logger;

        /// <summary>
        /// The lock sync object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// The map which associates clients with token buckets.
        /// </summary>
        private readonly IDictionary<string, TokenBucket.TokenBucket> _map = new Dictionary<string, TokenBucket.TokenBucket>();

        /// <summary>
        /// Initializes a new instance of <see cref="InMemoryTokenBucketConsumer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The request rate limiter configuration.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public InMemoryTokenBucketConsumer(
            RequestRateLimiterConfiguration configuration,
            ILogger<InMemoryTokenBucketConsumer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Consumes requested number of tokens for the specified client.
        /// </summary>
        /// <param name="clientId">
        /// The client identifier.
        /// </param>
        /// <param name="requested">
        /// The number of tokens to consume.
        /// </param>
        /// <returns>
        /// The consumption result.
        /// </returns>
        public Task<ConsumeResult> ConsumeAsync(string clientId, int requested)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            lock (_syncObject)
            {
                _logger.LogDebug("Received request from client {0} with timestamp {1}.", clientId, timestamp);

                if (!_map.ContainsKey(clientId))
                {
                    var clientBucket = new TokenBucket.TokenBucket(
                        _configuration.Interval,
                        _configuration.AverageRate,
                        _configuration.Bursting);

                    _map.Add(clientId, clientBucket);
                }

                var response = _map[clientId].Consume(requested);

                _logger.LogInformation(
                    response.IsAllowed
                        ? "Allowed request from client {0} with timestamp {1}. The client has {2} token(s) remaining."
                        : "Rejected request from client {0} with timestamp {1}. The client has {2} token(s) remaining.",
                    clientId, timestamp, response.Remaining);

                return Task.FromResult(response);
            }
        }
    }
}