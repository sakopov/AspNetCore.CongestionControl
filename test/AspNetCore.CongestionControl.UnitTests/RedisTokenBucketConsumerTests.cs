namespace AspNetCore.CongestionControl.UnitTests
{
    using System;
    using Microsoft.Extensions.Logging.Internal;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using Machine.Specifications;
    using Moq;
    using StackExchange.Redis;
    using It = Machine.Specifications.It;

    class RedisTokenBucketConsumerTests
    {
        // Not a lot of test coverage yet because it doesn't seem to be
        // possible to mock RedisResult in StackExchange.Redis library.
        // However, this is going to be available in version 2.0 which
        // is currently in alpha.
        // https://github.com/StackExchange/StackExchange.Redis/issues/856

        [Subject(typeof(RedisTokenBucketConsumer), "Redis Token Bucket Consumer"), Tags("Negative Test")]
        public class When_an_unexpected_exception_is_thrown_by_redis
        {
            Establish context = () =>
            {
                var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
                _configuration = new RequestRateLimiterConfiguration();
                _loggerMock = new Mock<ILogger<RedisTokenBucketConsumer>>();

                connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<object>()))
                    .Throws<Exception>();

                _consumer = new RedisTokenBucketConsumer(
                    connectionMultplexerMock.Object,
                    _configuration,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _consumer.ConsumeAsync(
                    Guid.NewGuid().ToString(),
                    1).Await();
            };

            It should_log_a_warning = () =>
            {
                _loggerMock.Verify(mock => mock.Log(
                    LogLevel.Warning,
                    Moq.It.IsAny<EventId>(),
                    Moq.It.IsAny<FormattedLogValues>(),
                    Moq.It.IsAny<Exception>(),
                    Moq.It.IsAny<Func<object, Exception, string>>()));
            };

            It should_allow_the_request = () =>
            {
                var capacity = _configuration.AverageRate * _configuration.Bursting;

                _result.IsAllowed.ShouldBeTrue();
                _result.Limit.ShouldEqual(capacity);
                _result.Remaining.ShouldEqual(capacity - 1);
            };

            static ConsumeResult _result;
            static RedisTokenBucketConsumer _consumer;
            static Mock<ILogger<RedisTokenBucketConsumer>> _loggerMock;
            static RequestRateLimiterConfiguration _configuration;
        }
    }
}