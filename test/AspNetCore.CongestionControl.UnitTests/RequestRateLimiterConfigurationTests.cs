namespace AspNetCore.CongestionControl.UnitTests
{
    using Configuration;
    using Machine.Specifications;
    using System;

    class RequestRateLimiterConfigurationTests
    {
        [Subject(typeof(RequestRateLimiterConfiguration), "Request Rate Limiter Configuration"), Tags("Positive Test")]
        public class When_validate_is_called_while_using_default_configuration
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();
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
            static RequestRateLimiterConfiguration _configuration;
        }

        [Subject(typeof(RequestRateLimiterConfiguration), "Request Rate Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_average_rate_is_zero
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration
                {
                    AverageRate = 0
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
                _exception.Message.ShouldContain("Average rate must be greater than 0.");
                ((ArgumentOutOfRangeException)_exception).ParamName.ShouldEqual("AverageRate");
            };

            static Exception _exception;
            static RequestRateLimiterConfiguration _configuration;
        }

        [Subject(typeof(RequestRateLimiterConfiguration), "Request Rate Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_interval_is_zero
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration
                {
                    Interval = 0
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
                _exception.Message.ShouldContain("Interval must be greater than 0.");
                ((ArgumentOutOfRangeException)_exception).ParamName.ShouldEqual("Interval");
            };

            static Exception _exception;
            static RequestRateLimiterConfiguration _configuration;
        }

        [Subject(typeof(RequestRateLimiterConfiguration), "Request Rate Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_bursting_is_zero
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration
                {
                    Bursting = 0
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
                _exception.Message.ShouldContain("Bursting must be greater than 0.");
                ((ArgumentOutOfRangeException)_exception).ParamName.ShouldEqual("Bursting");
            };

            static Exception _exception;
            static RequestRateLimiterConfiguration _configuration;
        }

        [Subject(typeof(RequestRateLimiterConfiguration), "Request Rate Limiter Configuration"), Tags("Negative Test")]
        public class When_validate_is_called_and_keys_prefix_is_null
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration
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
            static RequestRateLimiterConfiguration _configuration;
        }
    }
}