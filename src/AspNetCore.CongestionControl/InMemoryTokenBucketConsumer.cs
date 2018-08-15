using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements token bucket consumer which uses in-memory
    /// token bucket for storage.
    /// </summary>
    public class InMemoryTokenBucketConsumer : ITokenBucketConsumer
    {
        /// <summary>
        /// The request rate limiter configuration.
        /// </summary>
        private readonly RequestRateLimiterConfiguration _configuration;

        /// <summary>
        /// The lock sync object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// The map which associates clients with token buckets.
        /// </summary>
        private readonly IDictionary<string, TokenBucket.TokenBucket> _map = new Dictionary<string, TokenBucket.TokenBucket>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTokenBucketConsumer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The request rate limiter configuration.
        /// </param>
        public InMemoryTokenBucketConsumer(RequestRateLimiterConfiguration configuration)
        {
            _configuration = configuration;
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
        public Task<TokenConsumeResponse> ConsumeAsync(string clientId, int requested)
        {
            lock (_syncObject)
            {
                if (!_map.ContainsKey(clientId))
                {
                    var clientBucket = new TokenBucket.TokenBucket(
                        _configuration.Interval,
                        _configuration.AverageRate,
                        _configuration.Bursting);

                    _map.Add(clientId, clientBucket);
                }
                
                return _map[clientId].Consume(requested);
            }
        }
    }
}