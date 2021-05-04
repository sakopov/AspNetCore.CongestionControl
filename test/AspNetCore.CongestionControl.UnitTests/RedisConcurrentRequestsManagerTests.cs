// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConcurrentRequestsManagerTests.cs">
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
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Internal;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using StackExchange.Redis;
    using Xunit;

    // TODO: Add more test coverage!
    // Not a lot of test coverage yet because it doesn't seem to be
    // possible to mock RedisResult in StackExchange.Redis library.
    // However, this is going to be available in version 2.0 which
    // is currently in alpha.
    // https://github.com/StackExchange/StackExchange.Redis/issues/856
    public class RedisConcurrentRequestsManagerTests
    {
        [Fact(DisplayName = "Unexpected Exception Thrown By Redis During Add")]
        public async void UnexpectedExceptionThrownByRedisDuringAdd()
        {
            // Given
            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

            connectionMultiplexerMock.Setup(mock => mock.GetDatabase(
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<object>()))
                .Throws<Exception>();

            var manager = new RedisConcurrentRequestsManager(
                connectionMultiplexerMock.Object,
                configuration,
                loggerMock.Object);

            // When a request is added
            var result = await manager.AddAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                1);

            // Then it should log a warning
            loggerMock.Verify(mock => mock.Log(
                LogLevel.Warning,
                Moq.It.IsAny<EventId>(),
                Moq.It.IsAny<FormattedLogValues>(),
                Moq.It.IsAny<Exception>(),
                Moq.It.IsAny<Func<object, Exception, string>>()));

            // And it should allow the request
            result.IsAllowed.Should().BeTrue();
            result.Limit.Should().Be(configuration.Capacity);
            result.Remaining.Should().Be(configuration.Capacity - 1);
        }

        [Fact(DisplayName = "Unexpected Exception Thrown By Redis During Remove")]
        public async void UnexpectedExceptionThrownByRedisDuringRemove()
        {
            // Given
            var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

            connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<object>()))
                .Throws<Exception>();

            var manager = new RedisConcurrentRequestsManager(
                connectionMultplexerMock.Object,
                configuration,
                loggerMock.Object);

            // When a request is removed
            var result = await manager.RemoveAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Then it should log a warning
            loggerMock.Verify(mock => mock.Log(
                LogLevel.Warning,
                Moq.It.IsAny<EventId>(),
                Moq.It.IsAny<FormattedLogValues>(),
                Moq.It.IsAny<Exception>(),
                Moq.It.IsAny<Func<object, Exception, string>>()));

            // And it should return false
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Request Is Successfully Removed")]
        public async void RequestIsSuccessfullyRemoved()
        {
            // Given
            var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
            var databaseMock = new Mock<IDatabase>();
            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

            databaseMock.Setup(mock => mock.SortedSetRemoveAsync(
                    Moq.It.IsAny<RedisKey>(),
                    Moq.It.IsAny<RedisValue>(),
                    Moq.It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<object>()))
                .Returns(databaseMock.Object);

            var manager = new RedisConcurrentRequestsManager(
                connectionMultplexerMock.Object,
                configuration,
                loggerMock.Object);

            // When a request is removed
            var result = await manager.RemoveAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Then it should log a debug message
            loggerMock.Verify(mock => mock.Log(
                LogLevel.Debug,
                Moq.It.IsAny<EventId>(),
                Moq.It.IsAny<FormattedLogValues>(),
                Moq.It.IsAny<Exception>(),
                Moq.It.IsAny<Func<object, Exception, string>>()));

            // And it should return true
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "Request Is Not Removed")]
        public async void RequestIsNotRemoved()
        {
            // Given
            var connectionMultplexerMock = new Mock<IConnectionMultiplexer>();
            var databaseMock = new Mock<IDatabase>();
            var configuration = new ConcurrentRequestLimiterConfiguration();
            var loggerMock = new Mock<ILogger<RedisConcurrentRequestsManager>>();

            databaseMock.Setup(mock => mock.SortedSetRemoveAsync(
                    Moq.It.IsAny<RedisKey>(),
                    Moq.It.IsAny<RedisValue>(),
                    Moq.It.IsAny<CommandFlags>()))
                .ReturnsAsync(false);

            connectionMultplexerMock.Setup(mock => mock.GetDatabase(
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<object>()))
                .Returns(databaseMock.Object);

            var manager = new RedisConcurrentRequestsManager(
                connectionMultplexerMock.Object,
                configuration,
                loggerMock.Object);

            // When a request is removed
            var result = await manager.RemoveAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Then it should log a debug message
            loggerMock.Verify(mock => mock.Log(
                LogLevel.Debug,
                Moq.It.IsAny<EventId>(),
                Moq.It.IsAny<FormattedLogValues>(),
                Moq.It.IsAny<Exception>(),
                Moq.It.IsAny<Func<object, Exception, string>>()));

            // And it should return false
            result.Should().BeFalse();
        }
    }
}
