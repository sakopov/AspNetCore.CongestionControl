// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTokenBucketConsumerTests.cs">
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
    using Microsoft.Extensions.Logging;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class InMemoryTokenBucketConsumerTests
    {
        [Fact(DisplayName = "A Token is Consumed For the First Time")]
        public async void ATokenIsConsumedForTheFirstTime()
        {
            // Given
            const string ClientId = "tester";
            const int Requested = 1;

            var configuration = new RequestRateLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
            var consumer = new InMemoryTokenBucketConsumer(configuration, loggerMock.Object);

            var capacity = configuration.AverageRate * configuration.Bursting;

            // When a token is consumed
            var response = await consumer.ConsumeAsync(ClientId, Requested);

            // Then it should allow token consumption
            response.IsAllowed.Should().BeTrue();
            response.Limit.Should().Be(capacity);
            response.Remaining.Should().Be(capacity - 1);
        }

        [Fact(DisplayName = "A Token is Consumed Repeatedly")]
        public async void ATokenIsConsumedRepeatedly()
        {
            // Given
            const string ClientId = "tester";
            const int Requested = 1;
            const int PreviousConsumptionCount = 5;

            var configuration = new RequestRateLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
            var consumer = new InMemoryTokenBucketConsumer(configuration, loggerMock.Object);

            await consumer.ConsumeAsync(ClientId, PreviousConsumptionCount);

            var capacity = configuration.AverageRate * configuration.Bursting;

            // When a token is consumed
            var response = await consumer.ConsumeAsync(ClientId, Requested);

            // Then it should allow token consumption
            response.IsAllowed.Should().BeTrue();
            response.Limit.Should().Be(capacity);
            response.Remaining.Should().Be(capacity - PreviousConsumptionCount - Requested);
        }

        [Fact(DisplayName = "All Available Tokens are Consumed")]
        public async void AllAvailableTokensAreConsumed()
        {
            // Given
            const string ClientId = "tester";
            const int Requested = 1;

            var configuration = new RequestRateLimiterConfiguration();
            var loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
            var consumer = new InMemoryTokenBucketConsumer(configuration, loggerMock.Object);
            var capacity = configuration.AverageRate * configuration.Bursting;

            await consumer.ConsumeAsync(ClientId, capacity);

            // When a token is consumed
            var response = await consumer.ConsumeAsync(ClientId, Requested);

            // Then it should not allow consumption
            response.IsAllowed.Should().BeFalse();
            response.Limit.Should().Be(capacity);
            response.Remaining.Should().Be(0);
        }
    }
}
