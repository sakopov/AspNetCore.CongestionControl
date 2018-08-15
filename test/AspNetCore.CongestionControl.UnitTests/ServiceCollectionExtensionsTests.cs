using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;
using It = Machine.Specifications.It;

namespace AspNetCore.CongestionControl.UnitTests
{
    class ServiceCollectionExtensionsTests
    {
        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Negative Test")]
        public class When_collection_argument_is_null
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => ServiceCollectionExtensions.AddCongestionControl(null, null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldBeOfExactType<ArgumentNullException>();
                ((ArgumentNullException) _exception).ParamName.ShouldEqual("collection");
            };

            static Exception _exception;
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Negative Test")]
        public class When_configure_argument_is_null
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => new ServiceCollection().AddCongestionControl(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldBeOfExactType<ArgumentNullException>();
                ((ArgumentNullException)_exception).ParamName.ShouldEqual("configure");
            };

            static Exception _exception;
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_client_identifier_provider_is_not_set
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    _configuration = options;
                });
            };

            It should_set_client_identifier_provider_to_header_based_in_congestion_control_configuration = () =>
            {
                _configuration.ClientIdentifierProvider.ShouldNotBeNull();
                _configuration.ClientIdentifierProvider.ShouldBeOfExactType<HeaderBasedClientIdentifierProvider>();
            };

            It should_add_client_identifier_provider_to_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IClientIdentifierProvider))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_client_identifier_provider_is_custom
        {
            class CustomClientIdentifierProvider : IClientIdentifierProvider
            {
                public Task<string> ExecuteAsync(HttpContext httpContext)
                {
                    throw new NotImplementedException();
                }
            }

            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    options.AddClientIdentifierProvider(new CustomClientIdentifierProvider());
                    _configuration = options;
                });
            };

            It should_set_client_identifier_provider_in_congestion_control_configuration = () =>
            {
                _configuration.ClientIdentifierProvider.ShouldNotBeNull();
                _configuration.ClientIdentifierProvider.ShouldBeOfExactType<CustomClientIdentifierProvider>();
            };

            It should_add_client_identifier_provider_to_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IClientIdentifierProvider))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_request_rate_limiter_is_configured
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    options.AddRequestRateLimiter();
                    _configuration = options;
                });
            };

            It should_add_request_rate_limiter_configuration_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(RequestRateLimiterConfiguration))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_concurrent_request_rate_limiter_is_configured
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    options.AddConcurrentRequestLimiter();
                    _configuration = options;
                });
            };

            It should_add_concurrent_request_rate_limiter_configuration_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(ConcurrentRequestLimiterConfiguration))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_redis_server_configuration_is_configured
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    options.AddRedisStorage("127.0.0.1:6379");
                    _configuration = options;
                });
            };

            It should_set_redis_server_configuration_in_congestion_control_configuration = () =>
            {
                _configuration.RedisServerConfiguration.ShouldNotBeNull();
            };

            It should_add_redis_connection_multiplexer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                    .ShouldBeTrue();
            };

            It should_add_redis_concurrent_request_tracker_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConcurrentRequestsTracker) &&
                                         service.ImplementationType == typeof(RedisConcurrentRequestsTracker))
                    .ShouldBeTrue();
            };

            It should_add_redis_token_bucket_consumer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(ITokenBucketConsumer) &&
                                         service.ImplementationType == typeof(RedisTokenBucketConsumer))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_redis_server_configuration_is_not_configured
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    _configuration = options;
                });
            };

            It should_not_set_redis_server_configuration_in_congestion_control_configuration = () =>
            {
                _configuration.RedisServerConfiguration.ShouldBeNull();
            };

            It should_not_add_redis_connection_multiplexer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                    .ShouldBeFalse();
            };

            It should_add_in_memory_concurrent_request_tracker_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConcurrentRequestsTracker) &&
                                         service.ImplementationType == typeof(InMemoryConcurrentRequestsTracker))
                    .ShouldBeTrue();
            };

            It should_add_in_memory_token_bucket_consumer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(ITokenBucketConsumer) &&
                                         service.ImplementationType == typeof(InMemoryTokenBucketConsumer))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }
    }
}