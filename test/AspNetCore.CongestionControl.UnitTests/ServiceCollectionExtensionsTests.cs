namespace AspNetCore.CongestionControl.UnitTests
{
    using Configuration;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using It = Machine.Specifications.It;

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
        public class When_client_identifier_provider_is_not_confiured
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
        public class When_http_response_formatter_is_not_configured
        {
            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    _configuration = options;
                });
            };

            It should_set_http_response_formatter_to_default_in_congestion_control_configuration = () =>
            {
                _configuration.HttpResponseFormatter.ShouldNotBeNull();
                _configuration.HttpResponseFormatter.ShouldBeOfExactType<DefaultHttpResponseFormatter>();
            };

            It should_add_client_identifier_provider_to_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IHttpResponseFormatter))
                    .ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration;

            static IServiceCollection _services = new ServiceCollection();
        }

        [Subject(typeof(ServiceCollectionExtensions), "Service Collection Extensions"), Tags("Positive Test")]
        public class When_http_response_formatter_is_custom
        {
            class CustomHttpResponseFormatter : IHttpResponseFormatter
            {
                public Task FormatAsync(HttpContext httpContext, RateLimitContext rateLimitContext)
                {
                    throw new NotImplementedException();
                }
            }

            Because of = () =>
            {
                _services.AddCongestionControl(options =>
                {
                    options.AddHttpResponseFormatter(new CustomHttpResponseFormatter());
                    _configuration = options;
                });
            };

            It should_set_client_identifier_provider_in_congestion_control_configuration = () =>
            {
                _configuration.HttpResponseFormatter.ShouldNotBeNull();
                _configuration.HttpResponseFormatter.ShouldBeOfExactType<CustomHttpResponseFormatter>();
            };

            It should_add_client_identifier_provider_to_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IHttpResponseFormatter))
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
                _configuration.RedisConfiguration.ShouldNotBeNull();
            };

            It should_add_redis_connection_multiplexer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                    .ShouldBeTrue();
            };

            It should_add_redis_concurrent_request_tracker_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConcurrentRequestsManager) &&
                                         service.ImplementationType == typeof(RedisConcurrentRequestsManager))
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
                _configuration.RedisConfiguration.ShouldBeNull();
            };

            It should_not_add_redis_connection_multiplexer_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConnectionMultiplexer))
                    .ShouldBeFalse();
            };

            It should_add_in_memory_concurrent_request_tracker_to_the_service_collection = () =>
            {
                _services.Any(service => service.ServiceType == typeof(IConcurrentRequestsManager) &&
                                         service.ImplementationType == typeof(InMemoryConcurrentRequestsManager))
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