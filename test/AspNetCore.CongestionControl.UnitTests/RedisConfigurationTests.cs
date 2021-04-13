namespace AspNetCore.CongestionControl.UnitTests
{
    using System;
    using Configuration;
    using Machine.Specifications;

    class RedisConfigurationTests
    {
        [Subject(typeof(RedisConfiguration), "Redis Configuration"), Tags("Positive Test")]
        public class When_connection_is_provided
        {
            Because of = () =>
            {
                _configuration = new RedisConfiguration("127.0.0.1:6379");
            };

            It should_set_connection_configuration_options = () =>
            {
                _configuration.Options.ShouldNotBeNull();
            };

            static RedisConfiguration _configuration;
        }

        [Subject(typeof(RedisConfiguration), "Redis Configuration"), Tags("Negative Test")]
        public class When_connection_is_null
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => new RedisConfiguration(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
            };

            static Exception _exception;
        }
    }
}