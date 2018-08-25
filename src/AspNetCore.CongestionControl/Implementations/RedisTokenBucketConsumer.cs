// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisTokenBucketConsumer.cs">
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
    using StackExchange.Redis;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The token bucket consumer which uses Redis for token storage.
    /// </summary>
    public class RedisTokenBucketConsumer : ITokenBucketConsumer
    {
        /// <summary>
        /// The name of the Lua script which performs the consumption of tokens from Redis.
        /// </summary>
        private const string ScriptName = "request_rate_limiter.lua";

        /// <summary>
        /// The redis API client.
        /// </summary>
        private readonly IConnectionMultiplexer _redisClient;

        /// <summary>
        /// The request rate limiter configuration.
        /// </summary>
        private readonly RequestRateLimiterConfiguration _configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<RedisTokenBucketConsumer> _logger;

        /// <summary>
        /// The prepared Lua script.
        /// </summary>
        private LuaScript _preparedLuaScript;

        /// <summary>
        /// Initializes a new instance of <see cref="RedisTokenBucketConsumer"/> class.
        /// </summary>
        /// <param name="redisClient">
        /// The Redis API client.
        /// </param>
        /// <param name="configuration">
        /// The request rate limiter configuration.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RedisTokenBucketConsumer(
            IConnectionMultiplexer redisClient,
            RequestRateLimiterConfiguration configuration,
            ILogger<RedisTokenBucketConsumer> logger)
        {
            _redisClient = redisClient;
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
        public async Task<ConsumeResult> ConsumeAsync(string clientId, int requested)
        {
            if (_preparedLuaScript == null)
            {
                var rawLuaScript = await ResourceLoader.GetResourceAsync(ScriptName);

                _preparedLuaScript = LuaScript.Prepare(rawLuaScript);
            }

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var capacity = _configuration.AverageRate * _configuration.Bursting;

            var parameters = new
            {
                refill_rate = _configuration.AverageRate,
                refill_time = _configuration.Interval,
                capacity = capacity,
                timestamp = timestamp,
                requested = 1,
                tokens_key = (RedisKey) $"{_configuration.KeysPrefix}.{clientId}.tokens",
                timestamp_key = (RedisKey) $"{_configuration.KeysPrefix}.{clientId}.timestamp"
            };

            try
            {
                _logger.LogDebug("Received request from client {0} with timestamp {1}.", clientId, timestamp);

                var database = _redisClient.GetDatabase();

                // TODO: Send hash instead of full script?
                var result = (RedisResult[]) await _preparedLuaScript.EvaluateAsync(database, parameters);

                var response = new ConsumeResult
                (
                    isAllowed: !result[0].IsNull && (bool) result[0],
                    remaining: !result[1].IsNull ? (int) result[1] : 0,
                    limit: capacity
                );

                _logger.LogInformation(
                    response.IsAllowed
                        ? "Allowed request from client {0} with timestamp {1}. The client has {2} token(s) remaining."
                        : "Rejected request from client {0} with timestamp {1}. The client has {2} token(s) remaining.",
                    clientId, timestamp, response.Remaining);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled Redis exception: {0}", ex.Message);

                // Fail open if there is an exception.
                return new ConsumeResult(
                    isAllowed: true,
                    remaining: capacity - 1,
                    limit: capacity
                );
            }
        }
    }
}