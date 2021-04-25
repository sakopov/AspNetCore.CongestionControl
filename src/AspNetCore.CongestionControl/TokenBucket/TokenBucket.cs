// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenBucket.cs">
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

namespace AspNetCore.CongestionControl.TokenBucket
{
    using System;

    /// <summary>
    /// The simple in-memory token bucket.
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
        /// Initializes a new instance of <see cref="TokenBucket"/> class.
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
        public ConsumeResult Consume(int requestedTokens)
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
                return new ConsumeResult(
                    isAllowed: false,
                    remaining: _availableTokens,
                    limit: _capacity
                );
            }

            // Proceed and reduce available tokens.
            _availableTokens -= requestedTokens;

            return new ConsumeResult(
                isAllowed: true,
                remaining: _availableTokens,
                limit: _capacity
            );
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
