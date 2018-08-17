using System;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements concurrent requests tracker which uses Redis
    /// for storage.
    /// </summary>
    public class RedisConcurrentRequestsTracker : IConcurrentRequestsTracker
    {
        /// <summary>
        /// The Redis API client.
        /// </summary>
        private readonly IConnectionMultiplexer _redisClient;

        /// <summary>
        /// The concurrent request limiter configuration options.
        /// </summary>
        private readonly ConcurrentRequestLimiterConfiguration _configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<RedisConcurrentRequestsTracker> _logger;

        /// <summary>
        /// The name of the Lua script.
        /// </summary>
        private const string ScriptName = "concurrent_requests_limiter.lua";

        /// <summary>
        /// The prepared Lua script.
        /// </summary>
        private LuaScript _preparedLuaScript;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConcurrentRequestsTracker"/>
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
        public RedisConcurrentRequestsTracker(
            IConnectionMultiplexer redisClient,
            ConcurrentRequestLimiterConfiguration configuration,
            ILogger<RedisConcurrentRequestsTracker> logger)
        {
            _redisClient = redisClient;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Adds a request identifier at the specified timestamp to the
        /// tracker.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client who initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request was added. Otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> AddAsync(string clientId, string requestId, long timestamp)
        {
            var ttl = _configuration.RequestTimeToLive;
            var capacity = _configuration.Capacity;
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            if (_preparedLuaScript == null)
            {
                var rawLuaScript = await ScriptLoader.GetScriptAsync(ScriptName);

                _preparedLuaScript = LuaScript.Prepare(rawLuaScript);
            }

            try
            {
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

                var isAllowed = (bool)result[0];

                return isAllowed;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled Redis exception: {0}", ex.Message);

                // Fail open so Redis outage doesn't take down everything
                // with it.
                return true;
            }
        }

        /// <summary>
        /// Removes tracked request identifier.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client who initiated the request.
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

                var isRemoved = await database.SortedSetRemoveAsync(key, requestId);

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