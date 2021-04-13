namespace AspNetCore.CongestionControl.UnitTests
{
    using System.Linq;
    using System;
    using Configuration;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;

    class CongestionControlConfigurationTests
    {
        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_request_rate_limiter_with_default_options
        {
            Because of = () =>
            {
                _configuration.AddRequestRateLimiter();
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.RequestRateLimiterConfiguration.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_request_rate_limiter_with_custom_options
        {
            Because of = () =>
            {
                _configuration.AddRequestRateLimiter(config =>
                {
                    config.AverageRate = 1;
                    config.Interval = 10;
                    config.Bursting = 2;
                });
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.RequestRateLimiterConfiguration.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_concurrent_request_limiter_with_default_options
        {
            Because of = () =>
            {
                _configuration.AddConcurrentRequestLimiter();
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.ConcurrentRequestLimiterConfiguration.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_concurrent_request_limiter_with_custom_options
        {
            Because of = () =>
            {
                _configuration.AddConcurrentRequestLimiter(config =>
                {
                    config.Capacity = 10;
                    config.RequestTimeToLive = 60;
                });
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.ConcurrentRequestLimiterConfiguration.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_redis_storage
        {
            Because of = () =>
            {
                _configuration.AddRedisStorage("127.0.0.1:6379");
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.RedisConfiguration.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_header_based_client_identifier_provider
        {
            Because of = () =>
            {
                _configuration.AddHeaderBasedClientIdentifierProvider();
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.ClientIdentifierProviders.ShouldNotBeEmpty();
                _configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(HeaderBasedClientIdentifierProvider)).ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_query_based_client_identifier_provider
        {
            Because of = () =>
            {
                _configuration.AddQueryBasedClientIdentifierProvider();
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.ClientIdentifierProviders.ShouldNotBeEmpty();
                _configuration.ClientIdentifierProviders.Any(provider => provider.GetType() == typeof(QueryBasedClientIdentifierProvider)).ShouldBeTrue();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_custom_client_identifier_provider
        {
            Because of = () =>
            {
                _configuration.AddClientIdentifierProvider(new Mock<IClientIdentifierProvider>().Object);
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.ClientIdentifierProviders.ShouldNotBeEmpty();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Negative Test")]
        public class When_adding_custom_client_identifier_provider_and_passing_null
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.AddClientIdentifierProvider(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
            };

            static Exception _exception;
            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Positive Test")]
        public class When_adding_custom_http_response_formatter
        {
            Because of = () =>
            {
                _configuration.AddHttpResponseFormatter(new Mock<IHttpResponseFormatter>().Object);
            };

            It should_set_the_configuration_internally = () =>
            {
                _configuration.HttpResponseFormatter.ShouldNotBeNull();
            };

            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }

        [Subject(typeof(CongestionControlConfiguration), "Congestion Control Configuration"), Tags("Negative Test")]
        public class When_adding_custom_http_response_formatter_and_passing_null
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.AddHttpResponseFormatter(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
            };

            static Exception _exception;
            static CongestionControlConfiguration _configuration = new CongestionControlConfiguration();
        }
    }
}