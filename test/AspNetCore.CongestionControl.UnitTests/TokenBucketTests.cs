// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenBucketTests.cs">
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

namespace AspNetCore.CongestionControl.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    public class TokenBucketTests
    {
        [Fact(DisplayName = "The Requested Number of Tokens Does Not Exceed Available Tokens")]
        public void TheRequestedNumberOfTokensDoesNotExceedAvailableTokens()
        {
            // Given
            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 3;
            const int Capacity = AverageRate * Bursting;

            var tokenBucket = new TokenBucket.TokenBucket(
                interval: Interval,
                averageRate: AverageRate,
                bursting: Bursting);

            // When tokens are requested
            var response = tokenBucket.Consume(Requested);

            // Then it should allow token consumption
            response.IsAllowed.Should().BeTrue();
            response.Limit.Should().Be(Capacity);
            response.Remaining.Should().Be(Capacity - Requested);

            // And it should return the expected number of available tokens
            response.Remaining.Should().Be(AverageRate * Bursting - Requested);
        }

        [Fact(DisplayName = "The Requested Number of Tokens Depletes the Token Bucket")]
        public void TheRequestedNumberOfTokensDepletesTheTokenBucket()
        {
            // Given
            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 4;
            const int Capacity = AverageRate * Bursting;

            var tokenBucket = new TokenBucket.TokenBucket(
                interval: Interval,
                averageRate: AverageRate,
                bursting: Bursting);

            // When the number of tokens requested depletes the token bucket
            var response = tokenBucket.Consume(Requested);

            // Then it should allow token consumption
            response.IsAllowed.Should().BeTrue();
            response.Limit.Should().Be(Capacity);
            response.Remaining.Should().Be(Capacity - Requested);

            // And it should return the expected number of available tokens
            response.Remaining.Should().Be(AverageRate * Bursting - Requested);
        }

        [Fact(DisplayName = "The Token Consumption Rate Does Not Burst Above Capacity in an Interval")]
        public void TheTokenConsumptionRateDoesNotBurstAboveCapacityInAnInterval()
        {
            // Given
            const int Interval = 1;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 1;

            var response = new List<ConsumeResult>();

            var tokenBucket = new TokenBucket.TokenBucket(
                interval: Interval,
                averageRate: AverageRate,
                bursting: Bursting);

            // Do not burst above average rate
            response.Add(tokenBucket.Consume(Requested));
            response.Add(tokenBucket.Consume(Requested));

            var lastConsumedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Wait for interval to lapse
            Thread.Sleep(Interval * 1000);

            // When another token is consumed
            response.Add(tokenBucket.Consume(Requested));

            var availableTokensAfterBurst = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastConsumedAt) / Interval * AverageRate;

            // Then it should return the expected sequence of responses
            var capacityAtBurst = AverageRate * Bursting;

            response[0].IsAllowed.Should().BeTrue();
            response[0].Limit.Should().Be(capacityAtBurst);
            response[0].Remaining.Should().Be(capacityAtBurst - 1);

            response[1].IsAllowed.Should().BeTrue();
            response[1].Limit.Should().Be(capacityAtBurst);
            response[1].Remaining.Should().Be(capacityAtBurst - 2);

            response[2].IsAllowed.Should().BeTrue();
            response[2].Limit.Should().Be(capacityAtBurst);
            response[2].Remaining.Should().Be(capacityAtBurst - 1);
        }

        [Fact(DisplayName = "The Token Consumption Rate Bursts Above Capacity in an Interval")]
        public void TheTokenConsumptionRateBurstsAboveCapacityInAnInterval()
        {
            // Given
            const int Interval = 1;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 1;

            var response = new List<ConsumeResult>();

            var tokenBucket = new TokenBucket.TokenBucket(
                interval: Interval,
                averageRate: AverageRate,
                bursting: Bursting);

            // Burst above average rate by 3 requests
            response.Add(tokenBucket.Consume(Requested));
            response.Add(tokenBucket.Consume(Requested));
            response.Add(tokenBucket.Consume(Requested));
            response.Add(tokenBucket.Consume(Requested));
            response.Add(tokenBucket.Consume(Requested));

            var lastConsumedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Wait for interval to lapse
            Thread.Sleep(Interval * 1000);

            // When 1 more token is consumed in new interval
            response.Add(tokenBucket.Consume(Requested));

            var availableTokensAfterBurst = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastConsumedAt) / Interval * AverageRate;

            // Then it should return the expected sequence of responses
            var capacityAtBurst = AverageRate * Bursting;

            response[0].IsAllowed.Should().BeTrue();
            response[0].Limit.Should().Be(capacityAtBurst);
            response[0].Remaining.Should().Be(capacityAtBurst - 1);

            response[1].IsAllowed.Should().BeTrue();
            response[1].Limit.Should().Be(capacityAtBurst);
            response[1].Remaining.Should().Be(capacityAtBurst - 2);

            response[2].IsAllowed.Should().BeTrue();
            response[2].Limit.Should().Be(capacityAtBurst);
            response[2].Remaining.Should().Be(capacityAtBurst - 3);

            response[3].IsAllowed.Should().BeTrue();
            response[3].Limit.Should().Be(capacityAtBurst);
            response[3].Remaining.Should().Be(capacityAtBurst - 4);

            response[4].IsAllowed.Should().BeFalse();
            response[4].Limit.Should().Be(capacityAtBurst);
            response[4].Remaining.Should().Be(0);

            response[5].IsAllowed.Should().BeTrue();
            response[5].Limit.Should().Be(capacityAtBurst);
            response[5].Remaining.Should().Be((int)availableTokensAfterBurst - 1);
        }

        [Fact(DisplayName = "The Requested Number of Tokens Exceeds Available Tokens")]
        public void TheRequestedNumberOfTokensExceedsAvailableTokens()
        {
            // Given
            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 10;
            const int Capacity = AverageRate * Bursting;

            var tokenBucket = new TokenBucket.TokenBucket(
                interval: Interval,
                averageRate: AverageRate,
                bursting: Bursting);

            // When a token is consumed exceeding available capacity
            var response = tokenBucket.Consume(Requested);

            // Then it should not allow token consumption
            response.IsAllowed.Should().BeFalse();
            response.Limit.Should().Be(Capacity);
            response.Remaining.Should().Be(Capacity);
        }
    }
}
