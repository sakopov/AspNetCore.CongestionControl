// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs">
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
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Configuration;
    using FluentAssertions;
    using StackExchange.Redis;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        [Fact(DisplayName = "Service Collection Argument Is Null")]
        public void ServiceCollectionArgumentIsNull()
        {
            // When congestion control is added without service collection argument
            var exception = Record.Exception(() => ServiceCollectionExtensions.AddCongestionControl(null, null));

            // Then it should throw ArgumentNullException
            exception.Should().BeOfType<ArgumentNullException>();
            ((ArgumentNullException) exception).ParamName.Should().Be("services");
        }

        [Fact(DisplayName = "Configure Argument Is Null")]
        public void ConfigureArgumentIsNull()
        {
            // When congestion control is added without configure argument
            var exception = Record.Exception(() => new ServiceCollection().AddCongestionControl(null));

            // Then it should throw ArgumentNullException
            exception.Should().BeOfType<ArgumentNullException>();
            ((ArgumentNullException)exception).ParamName.Should().Be("configure");
        }

        [Fact(DisplayName = "Client Identifier Provider Is Not Configured")]
        public void ClientIdentifierProviderIsNotConfigured()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When client identifier provider is not explicitly configured
            services.AddCongestionControl(options =>
            {
                configuration = options;
            });

            // Then it should be defaulted to header-based client identifier provider
            configuration.ClientIdentifierProviders.Should().NotBeEmpty();
            configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(HeaderBasedClientIdentifierProvider)).Should().BeTrue();

            // And it should add header-based client identifier provider to the service collection
            services.Any(service =>
                service.ServiceType == typeof(IClientIdentifierProvider) &&
                service.ImplementationType == typeof(HeaderBasedClientIdentifierProvider)).Should().BeTrue();
        }

        [Fact(DisplayName = "Client Identifier Provider Is Custom")]
        public void ClientIdentifierProviderIsCustom()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When a custom client identifier provider is added
            services.AddCongestionControl(options =>
            {
                options.AddClientIdentifierProvider(new CustomClientIdentifierProvider());
                configuration = options;
            });

            // Then it should set client identifier provider in congestion control configuration
            configuration.ClientIdentifierProviders.Should().NotBeEmpty();
            configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(CustomClientIdentifierProvider)).Should().BeTrue();

            // And it should add client identifier provider to service collection
            services.Any(service =>
                service.ServiceType == typeof(IClientIdentifierProvider) &&
                service.ImplementationInstance.GetType() == typeof(CustomClientIdentifierProvider)).Should().BeTrue();
        }

        [Fact(DisplayName = "HTTP Response Formatter Is Not Configured")]
        public void HttpResponseFormatterIsNotConfigured()
        {
            // Given
            CongestionControlConfiguration configuration = null;
            var services = new ServiceCollection();

            // When HTTP response formatter is not explicitly configured
            services.AddCongestionControl(options =>
            {
                configuration = options;
            });

            // Then it should be defaulted to default HTTP response formatter
            configuration.HttpResponseFormatter.Should().NotBeNull();
            configuration.HttpResponseFormatter.Should().BeOfType<DefaultHttpResponseFormatter>();

            // And it should add default HTTP response formatter to the service collection
            services.Any(service =>
                service.ServiceType == typeof(IHttpResponseFormatter) &&
                service.ImplementationType == typeof(DefaultHttpResponseFormatter)).Should().BeTrue();
        }

        [Fact(DisplayName = "HTTP Response Formatter Is Custom")]
        public void HttpResponseFormatterIsCustom()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When a custom HTTP response formatter is added
            services.AddCongestionControl(options =>
            {
                options.AddHttpResponseFormatter(new CustomHttpResponseFormatter());
                configuration = options;
            });

            // Then it should set HTTP response formatter in congestion control configuration
            configuration.HttpResponseFormatter.Should().NotBeNull();
            configuration.HttpResponseFormatter.Should().BeOfType<CustomHttpResponseFormatter>();

            // And it should add HTTP response formatter to service collection
            services.Any(service => service.ServiceType == typeof(IHttpResponseFormatter)).Should().BeTrue();
        }

        [Fact(DisplayName = "Request Rate Limiter Is Configured")]
        public void RequestRateLimiterIsConfigured()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When request rate limiter is added explicitly
            services.AddCongestionControl(options =>
            {
                options.AddRequestRateLimiter();
                configuration = options;
            });

            // Then it should add request rate limiter configuration to the service collection
            services.Any(service => service.ServiceType == typeof(RequestRateLimiterConfiguration))
                .Should().BeTrue();

            // And it should add in-memory token bucket consumer to the service collection
            services.Any(service =>
                service.ServiceType == typeof(ITokenBucketConsumer) &&
                service.ImplementationType == typeof(InMemoryTokenBucketConsumer)).Should().BeTrue();
        }

        [Fact(DisplayName = "Request Rate Limiter Is Configured With Redis Server")]
        public void RequestRateLimiterIsConfiguredWithRedisServer()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When adding request rate limiter with redis
            services.AddCongestionControl(options =>
            {
                options.AddRedisStorage("127.0.0.1:6379");
                options.AddRequestRateLimiter();
                configuration = options;
            });

            // Then it should set Redis server configuration in congestion control configuration
            configuration.RedisConfiguration.Should().NotBeNull();

            // And it should add Redis connection multiplexer to the services collection
            services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                .Should().BeTrue();

            // And it should add Redis token bucket consumer to the services collection
            services.Any(service =>
                service.ServiceType == typeof(ITokenBucketConsumer) &&
                service.ImplementationType == typeof(RedisTokenBucketConsumer)).Should().BeTrue();
        }

        [Fact(DisplayName = "Concurrent Request Limiter Is Configured")]
        public void ConcurrentRequestLimiterIsConfigured()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When concurrent request limiter is added explicitly
            services.AddCongestionControl(options =>
            {
                options.AddConcurrentRequestLimiter();
                configuration = options;
            });

            // Then it should add concurrent request limiter configuration to the service collection
            services.Any(service => service.ServiceType == typeof(ConcurrentRequestLimiterConfiguration))
                .Should().BeTrue();

            // And it should add in-memory concurrent request manager to the service collection
            services.Any(service =>
                service.ServiceType == typeof(IConcurrentRequestsManager) &&
                service.ImplementationType == typeof(InMemoryConcurrentRequestsManager)).Should().BeTrue();
        }

        [Fact(DisplayName = "Concurrent Request Limiter Is Configured With Redis Server")]
        public void ConcurrentRequestLimiterIsConfiguredWithRedisServer()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When adding concurrent request limiter with Redis server
            services.AddCongestionControl(options =>
            {
                options.AddRedisStorage("127.0.0.1:6379");
                options.AddConcurrentRequestLimiter();
                configuration = options;
            });

            // Then it should set Redis server configuration in congestion control configuration
            configuration.RedisConfiguration.Should().NotBeNull();

            // And it should add Redis connection multiplexer to the services collection
            services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                .Should().BeTrue();

            // And it should add Redis concurrent request manager to the services collection
            services.Any(service =>
                service.ServiceType == typeof(IConcurrentRequestsManager) &&
                service.ImplementationType == typeof(RedisConcurrentRequestsManager)).Should().BeTrue();
        }

        [Fact(DisplayName = "Redis Server Configuration Is Not Provided")]
        public void RedisServerConfigurationIsNotProvided()
        {
            // Given
            var services = new ServiceCollection();
            CongestionControlConfiguration configuration = null;

            // When Redis server configuration is not provided
            services.AddCongestionControl(options =>
            {
                configuration = options;
            });

            // Then it should not set Redis server configuration in congestion control configuration
            configuration.RedisConfiguration.Should().BeNull();

            // And it should not add Redis connection multiplexer to the services collection
            services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                .Should().BeFalse();

            // And it should not add Redis concurrent request tracker to the services collection
            services.Any(service =>
                service.ServiceType == typeof(IConcurrentRequestsManager) &&
                service.ImplementationType == typeof(RedisConcurrentRequestsManager)).Should().BeFalse();

            // And it should not add Redis token bucket consumer to the services collection
            services.Any(service =>
                service.ServiceType == typeof(ITokenBucketConsumer) &&
                service.ImplementationType == typeof(RedisTokenBucketConsumer)).Should().BeFalse();
        }
    }

    public class CustomClientIdentifierProvider : IClientIdentifierProvider
    {
        public Task<string> ExecuteAsync(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomHttpResponseFormatter : IHttpResponseFormatter
    {
        public Task FormatAsync(HttpContext httpContext, RateLimitContext rateLimitContext)
        {
            throw new NotImplementedException();
        }
    }
}
