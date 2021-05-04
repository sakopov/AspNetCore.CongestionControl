// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConcurrentRequestsManager.cs">
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
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The concurrent requests monitor which uses Redis for storage.
    /// </summary>
    public class RedisConcurrentRequestsManager : IConcurrentRequestsManager
    {
        /// <summary>
        /// The Redis API client.
        /// </summary>
        private readonly IConnectionMultiplexer _redisClient;

        /// <summary>
        /// The concurrent request limiter configuration.
        /// </summary>
        private readonly ConcurrentRequestLimiterConfiguration _configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The name of the Lua script.
        /// </summary>
        private const string ScriptName = "concurrent_requests_limiter.lua";

        /// <summary>
        /// The prepared Lua script.
        /// </summary>
        private LuaScript _preparedLuaScript;

        /// <summary>
        /// Initializes a new instance of <see cref="RedisConcurrentRequestsManager"/>
        /// class.
        /// </summary>
        /// <param name="redisClient">
        /// The Redis API client.
        /// </param>
        /// <param name="configuration">
        /// The concurrent request limiter configuration options.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RedisConcurrentRequestsManager(
            IConnectionMultiplexer redisClient,
            ConcurrentRequestLimiterConfiguration configuration,
            ILogger<RedisConcurrentRequestsManager> logger)
        {
            _redisClient = redisClient;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new client request to the manager, if it isn't exceeding
        /// the capacity.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client which initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp of the request.
        /// </param>
        /// <returns>
        /// The <see cref="AddConcurrentRequestResult"/> instance containing the response.
        /// </returns>
        public async Task<AddConcurrentRequestResult> AddAsync(string clientId, string requestId, long timestamp)
        {
            var ttl = _configuration.RequestTimeToLive;
            var capacity = _configuration.Capacity;
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            if (_preparedLuaScript == null)
            {
                var rawLuaScript = await ResourceLoader.GetResourceAsync(ScriptName);

                _preparedLuaScript = LuaScript.Prepare(rawLuaScript);
            }

            try
            {
                _logger.LogDebug("Received request {0} from client {1} with timestamp {2}.",
                    requestId, clientId, timestamp);

                var database = _redisClient.GetDatabase();

                await database.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, timestamp - ttl);

                var parameters = new
                {
                    capacity = capacity,
                    timestamp = timestamp,
                    requestId = requestId,
                    key = (RedisKey)key
                };

                var result = (RedisResult[])await _preparedLuaScript.EvaluateAsync(database, parameters);

                var isAllowed = (bool) result[0];
                var remaining = _configuration.Capacity - (int) result[1];

                _logger.LogInformation(
                    isAllowed
                        ? "Allowed request {0} from client {1} with timestamp {2}. The client has {3} request(s) remaining."
                        : "Rejected request {0} from client {1} with timestamp {2}. The client has {3} request(s) remaining.",
                    requestId, clientId, timestamp, remaining);

                return new AddConcurrentRequestResult(
                    isAllowed: isAllowed,
                    remaining: remaining,
                    limit: capacity
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled Redis exception: {0}", ex.Message);

                // Fail open so Redis outage doesn't take down everything
                // with it.
                return new AddConcurrentRequestResult(
                    isAllowed: true,
                    remaining: capacity - 1,
                    limit: capacity
                );
            }
        }

        /// <summary>
        /// Removes an existing request from the manager.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client which initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request was removed. Otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> RemoveAsync(string clientId, string requestId)
        {
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            try
            {
                var database = _redisClient.GetDatabase();

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var isRemoved = await database.SortedSetRemoveAsync(key, requestId);

                _logger.LogDebug(isRemoved
                        ? "Removed request {0} for client {1} at {2}."
                        : "Couldn't remove request {0} for client {1} at {2}.",
                    requestId, clientId, timestamp);

                return isRemoved;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled Redis exception: {0}", ex.Message);

                return false;
            }
        }
    }
}
