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

    class RedisConcurrentRequestsManagerTests
    {
        // Not a lot of test coverage yet because it doesn't seem to be
        // possible to mock RedisResult in StackExchange.Redis library.
        // However, this is going to be available in version 2.0 which
        // is currently in alpha.
        // https://github.com/StackExchange/StackExchange.Redis/issues/856

        [Subject(typeof(RedisConcurrentRequestsManager), "Redis Concurrent Requests Manager"), Tags("Negative Test")]
        public class When_an_unexpected_exception_is_thrown_by_redis_during_add
        {
            Establish context = () =>
            {
                var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

                connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<object>()))
                    .Throws<Exception>();

                _manager = new RedisConcurrentRequestsManager(
                    connectionMultplexerMock.Object,
                    _configuration,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _manager.AddAsync(
                    Guid.NewGuid().ToString(),
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
                _result.IsAllowed.ShouldBeTrue();
                _result.Limit.ShouldEqual(_configuration.Capacity);
                _result.Remaining.ShouldEqual(_configuration.Capacity - 1);
            };

            static AddConcurrentRequestResult _result;
            static RedisConcurrentRequestsManager _manager;
            static Mock<ILogger<RedisConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(RedisConcurrentRequestsManager), "Redis Concurrent Requests Manager"), Tags("Negative Test")]
        public class When_an_unexpected_exception_is_thrown_by_redis_during_remove
        {
            Establish context = () =>
            {
                var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

                connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<object>()))
                    .Throws<Exception>();

                _manager = new RedisConcurrentRequestsManager(
                    connectionMultplexerMock.Object,
                    _configuration,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _manager.RemoveAsync(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()).Await();
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

            It should_return_false = () =>
            {
                _result.ShouldBeFalse();
            };

            static bool _result;
            static RedisConcurrentRequestsManager _manager;
            static Mock<ILogger<RedisConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(RedisConcurrentRequestsManager), "Redis Concurrent Requests Manager"), Tags("Positive Test")]
        public class When_a_request_is_successfully_removed
        {
            Establish context = () =>
            {
                var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
                var databaseMock = new Mock<IDatabase>();
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

                databaseMock.Setup(mock => mock.SortedSetRemoveAsync(
                        Moq.It.IsAny<RedisKey>(),
                        Moq.It.IsAny<RedisValue>(),
                        Moq.It.IsAny<CommandFlags>()))
                    .ReturnsAsync(true);

                connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<object>()))
                    .Returns(databaseMock.Object);

                _manager = new RedisConcurrentRequestsManager(
                    connectionMultplexerMock.Object,
                    _configuration,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _manager.RemoveAsync(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()).Await();
            };

            It should_log_a_debug_message = () =>
            {
                _loggerMock.Verify(mock => mock.Log(
                    LogLevel.Debug,
                    Moq.It.IsAny<EventId>(),
                    Moq.It.IsAny<FormattedLogValues>(),
                    Moq.It.IsAny<Exception>(),
                    Moq.It.IsAny<Func<object, Exception, string>>()));
            };

            It should_return_false = () =>
            {
                _result.ShouldBeTrue();
            };

            static bool _result;
            static RedisConcurrentRequestsManager _manager;
            static Mock<ILogger<RedisConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }

        [Subject(typeof(RedisConcurrentRequestsManager), "Redis Concurrent Requests Manager"), Tags("Negative Test")]
        public class When_a_request_is_not_removed
        {
            Establish context = () =>
            {
                var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
                var databaseMock = new Mock<IDatabase>();
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

                databaseMock.Setup(mock => mock.SortedSetRemoveAsync(
                        Moq.It.IsAny<RedisKey>(),
                        Moq.It.IsAny<RedisValue>(),
                        Moq.It.IsAny<CommandFlags>()))
                    .ReturnsAsync(false);

                connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                        Moq.It.IsAny<int>(),
                        Moq.It.IsAny<object>()))
                    .Returns(databaseMock.Object);

                _manager = new RedisConcurrentRequestsManager(
                    connectionMultplexerMock.Object,
                    _configuration,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _manager.RemoveAsync(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()).Await();
            };

            It should_log_a_debug_message = () =>
            {
                _loggerMock.Verify(mock => mock.Log(
                    LogLevel.Debug,
                    Moq.It.IsAny<EventId>(),
                    Moq.It.IsAny<FormattedLogValues>(),
                    Moq.It.IsAny<Exception>(),
                    Moq.It.IsAny<Func<object, Exception, string>>()));
            };

            It should_return_false = () =>
            {
                _result.ShouldBeFalse();
            };

            static bool _result;
            static RedisConcurrentRequestsManager _manager;
            static Mock<ILogger<RedisConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
        }
    }
}