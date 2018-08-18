using System;
using System.Threading.Tasks;

namespace AspNetCore.CongestionControl.TokenBucket
{
    /// <summary>
    /// This class implements a simple in-memory token bucket.
    /// </summary>
    public class TokenBucket
    {
        /// <summary>
        /// How many requests per <see cref="_interval"/> a client is allowed to perform.
        /// </summary>
        private readonly int _averageRate;

        /// <summary>
        /// The total capacity of the bucket.
        /// </summary>
        private readonly int _capacity;

        /// <summary>
        /// The length of the time unit in seconds.
        /// </summary>
        private readonly int _interval;

        /// <summary>
        /// Keeps track of the last time the bucket was updated.
        /// </summary>
        private long _lastUpdateTimestamp;

        /// <summary>
        /// Keeps track of available tokens in the bucket.
        /// </summary>
        private int _availableTokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucket"/> class.
        /// </summary>
        /// <param name="interval">
        /// The length of the time unit in seconds.
        /// </param>
        /// <param name="averageRate">
        /// How many requests per <see cref="interval"/> a client is allowed to perform.
        /// </param>
        /// <param name="bursting">
        /// The amount of bursting to allow.
        /// </param>
        public TokenBucket(int interval, int averageRate, int bursting)
        {
            _averageRate = averageRate;
            _capacity = averageRate * bursting;
            _interval = interval;
            _lastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _availableTokens = _capacity;
        }

        /// <summary>
        /// Reduces available tokens in the bucket by the requested amount.
        /// </summary>
        /// <param name="requestedTokens">
        /// The number of tokens requested.
        /// </param>
        /// <returns>
        /// The response containing information about whether the reduction
        /// of tokens was allowed.
        /// </returns>
        public Task<TokenConsumeResponse> Consume(int requestedTokens)
        {
            var intervalsSinceLastsUpdate = GetIntervalsSinceLastsUpdate();

            // Get the number of tokens that became available between now and the last
            // bucket update. Add to available tokens count.
            _availableTokens += intervalsSinceLastsUpdate * _averageRate;

            // Get the total elapsed time (in seconds) between now and the last bucket
            // update. Add to the last known timestamp.
            _lastUpdateTimestamp += intervalsSinceLastsUpdate * _interval;

            // Reset everything if the bucket is over its capacity.
            if (_availableTokens >= _capacity)
            {
                _availableTokens = _capacity;
                _lastUpdateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            // If there are no more tokens available, return a non-conforming
            // response.
            if (requestedTokens > _availableTokens)
            {
                return Task.FromResult(TokenConsumeResponse.NonConforming(_availableTokens));
            }

            // Proceed and reduce available tokens.
            _availableTokens -= requestedTokens;

            return Task.FromResult(TokenConsumeResponse.Conforming(_availableTokens));
        }

        /// <summary>
        /// Gets the number of elapsed intervals between now and last bucket
        /// update.
        /// </summary>
        /// <returns>
        /// The number of intervals.
        /// </returns>
        private int GetIntervalsSinceLastsUpdate()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return (int)(now - _lastUpdateTimestamp) / _interval;
        }
    }
}