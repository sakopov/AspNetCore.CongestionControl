using System;
using AspNetCore.CongestionControl.Configuration;
using Machine.Specifications;

namespace AspNetCore.CongestionControl.UnitTests
{
    class ConcurrentRequestLimiterConfigurationTests
    {
        [Subject(typeof(ConcurrentRequestLimiterConfiguration), "Concurrent Request Limiter Configuration"), Tags("Positive Test")]
        public class When_validate_is_called_while_using_default_configuration
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.Validate());
            };

            It should_successfully_validate = () =>
            {
                _exception.ShouldBeNull();
            };

            static Exception _exception;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(ConcurrentRequestLimiterConfiguration), "Concurrent Request Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_capacity_is_zero
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration
                {
                    Capacity = 0
                };
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.Validate());
            };

            It should_throw_argument_out_of_range_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentOutOfRangeException>();
                _exception.Message.ShouldContain("Capacity must be greater than 0.");
                ((ArgumentOutOfRangeException)_exception).ParamName.ShouldEqual("Capacity");
            };

            static Exception _exception;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(ConcurrentRequestLimiterConfiguration), "Concurrent Request Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_ttl_is_zero
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration
                {
                    RequestTimeToLive = 0
                };
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.Validate());
            };

            It should_throw_argument_out_of_range_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentOutOfRangeException>();
                _exception.Message.ShouldContain("Request time-to-live must be greater than 0.");
                ((ArgumentOutOfRangeException)_exception).ParamName.ShouldEqual("RequestTimeToLive");
            };

            static Exception _exception;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(ConcurrentRequestLimiterConfiguration), "Concurrent Request Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_key_prefix_is_null
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration
                {
                    KeysPrefix = null
                };
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _configuration.Validate());
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
                _exception.Message.ShouldContain("Keys prefix must be provided.");
                ((ArgumentNullException)_exception).ParamName.ShouldEqual("KeysPrefix");
            };

            static Exception _exception;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }
    }
}