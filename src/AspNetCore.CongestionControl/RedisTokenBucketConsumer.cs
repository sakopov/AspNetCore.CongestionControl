using System;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements token bucket consumer which uses Redis for storage.
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
        /// The logger.
        /// </summary>
        private readonly ILogger<RedisTokenBucketConsumer> _logger;

        /// <summary>
        /// The request rate limiter configuration.
        /// </summary>
        private readonly RequestRateLimiterConfiguration _configuration;

        /// <summary>
        /// The prepared Lua script.
        /// </summary>
        private LuaScript _preparedLuaScript;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTokenBucketConsumer"/> class.
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
        /// Consumes tokens by the requested number for the specified client.
        /// </summary>
        /// <param name="clientId">
        /// The client identifier to consume tokens for.
        /// </param>
        /// <param name="requested">
        /// The requested number of tokens to consume.
        /// </param>
        /// <returns>
        /// The token consumption response.
        /// </returns>
        public async Task<TokenConsumeResponse> ConsumeAsync(string clientId, int requested)
        {
            if (_preparedLuaScript == null)
            {
                var rawLuaScript = await ScriptLoader.GetScriptAsync(ScriptName);

                _preparedLuaScript = LuaScript.Prepare(rawLuaScript);
            }

            var parameters = new
            {
                refill_rate = _configuration.AverageRate,
                refill_time = _configuration.Interval,
                capacity = _configuration.AverageRate * _configuration.Bursting,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                requested = 1,
                tokens_key = (RedisKey)$"{_configuration.KeysPrefix}.{clientId}.tokens",
                timestamp_key = (RedisKey)$"{_configuration.KeysPrefix}.{clientId}.timestamp"
            };

            var database = _redisClient.GetDatabase();

            try
            {
                // TODO: Update to send hash instead of the entire script body
                var result = (RedisResult[])await _preparedLuaScript.EvaluateAsync(database, parameters);
                
                return new TokenConsumeResponse
                (
                    isAllowed: !result[0].IsNull && (bool)result[0],
                    tokensLeft: !result[1].IsNull ? (int)result[1] : 0
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unhandled Redis exception: {0}", ex.Message);

                // Fail open if there is an exception.
                return TokenConsumeResponse.Conforming(_configuration.AverageRate * _configuration.Bursting);
            }
        }
    }
}