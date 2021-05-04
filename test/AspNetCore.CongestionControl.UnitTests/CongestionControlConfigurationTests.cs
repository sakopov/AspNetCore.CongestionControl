// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CongestionControlConfigurationTests.cs">
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
    using System.Linq;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class CongestionControlConfigurationTests
    {
        [Fact(DisplayName = "Adding Request Rate Limiter with Default Options")]
        public void AddingRequestRateLimiterWithDefaultOptions()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When request rate limiter is added
            configuration.AddRequestRateLimiter();

            // Then the request rate limiter instance should be set internally
            configuration.RequestRateLimiterConfiguration.Should().NotBeNull();
        }

        [Fact(DisplayName = "Adding Request Rate Limiter with Custom Options")]
        public void AddingRequestRateLimiterWithCustomOptions()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When request rate limiter is added with custom options
            configuration.AddRequestRateLimiter(config =>
            {
                config.AverageRate = 1;
                config.Interval = 10;
                config.Bursting = 2;
            });

            // Then the request rate limiter instance should be set internally
            configuration.RequestRateLimiterConfiguration.Should().NotBeNull();
            configuration.RequestRateLimiterConfiguration.AverageRate.Should().Be(1);
            configuration.RequestRateLimiterConfiguration.Interval.Should().Be(10);
            configuration.RequestRateLimiterConfiguration.Bursting.Should().Be(2);
        }

        [Fact(DisplayName = "Adding Concurrent Request Limiter with Default Options")]
        public void AddingConcurrentRequestLimiterWithDefaultOptions()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When the concurrent request limiter is added
            configuration.AddConcurrentRequestLimiter();

            // Then the concurrent request limiter instance should be set internally
            configuration.ConcurrentRequestLimiterConfiguration.Should().NotBeNull();
        }

        [Fact(DisplayName = "Adding Concurrent Request Limiter with Custom Options")]
        public void AddingConcurrentRequestLimiterWithCustomOptions()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When the concurrent request limiter is added
            configuration.AddConcurrentRequestLimiter(config =>
            {
                config.Capacity = 10;
                config.RequestTimeToLive = 60;
            });

            // Then the concurrent request limiter instance should be set internally
            configuration.ConcurrentRequestLimiterConfiguration.Should().NotBeNull();
            configuration.ConcurrentRequestLimiterConfiguration.Capacity.Should().Be(10);
            configuration.ConcurrentRequestLimiterConfiguration.RequestTimeToLive.Should().Be(60);
        }

        [Fact(DisplayName = "Adding Redis Storage")]
        public void AddingRedisStorage()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When Redis storage is added
            configuration.AddRedisStorage("127.0.0.1:6379");

            // Then redis storage configuration should be set internally
            configuration.RedisConfiguration.Should().NotBeNull();
        }

        [Fact(DisplayName = "Adding Header-Based Client Identifier Provider")]
        public void AddingHeaderBasedClientIdentifierProvider()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When header-based client identifier provider is added
            configuration.AddHeaderBasedClientIdentifierProvider();

            // Then header-based client identifier provider should be set internally
            configuration.ClientIdentifierProviders.Should().NotBeEmpty();
            configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(HeaderBasedClientIdentifierProvider)).Should().BeTrue();
        }

        [Fact(DisplayName = "Adding Query-Based Client Identifier Provider")]
        public void AddingQueryBasedClientIdentifierProvider()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When query-based client identifier provider is added
            configuration.AddQueryBasedClientIdentifierProvider();

            // Then query-based client identifier provider should be set internally
            configuration.ClientIdentifierProviders.Should().NotBeEmpty();
            configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(QueryBasedClientIdentifierProvider)).Should().BeTrue();
        }

        [Fact(DisplayName = "Adding Custom Client Identifier Provider")]
        public void AddingCustomClientIdentifierProvider()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When custom client identifier provider is added
            configuration.AddClientIdentifierProvider(new Mock<IClientIdentifierProvider>().Object);

            // Then custom client identifier provider should be set internally
            configuration.ClientIdentifierProviders.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "Adding Null Client Identifier Provider")]
        public void AddingNullClientIdentifierProvider()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When adding null client identifier provider
            var exception = Record.Exception(() => configuration.AddClientIdentifierProvider(null));

            // Then it should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "Adding Custom HTTP Response Formatter")]
        public void AddingCustomHttpResponseFormatter()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When adding custom HTTP response formatter
            configuration.AddHttpResponseFormatter(new Mock<IHttpResponseFormatter>().Object);

            // Then it should set custom HTTP response formatter internally
            configuration.HttpResponseFormatter.Should().NotBeNull();
        }

        [Fact(DisplayName = "Adding Null HTTP Response Formatter")]
        public void AddingNullHttpResponseFormatter()
        {
            // Given
            var configuration = new CongestionControlConfiguration();

            // When adding null HTTP response formatter
            var exception = Record.Exception(() => configuration.AddHttpResponseFormatter(null));

            // Then it should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
