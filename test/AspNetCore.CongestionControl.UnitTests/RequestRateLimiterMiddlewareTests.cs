// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestRateLimiterMiddlewareTests.cs">
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
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class RequestRateLimiterMiddlewareTests
    {
        [Fact(DisplayName = "Request Is Allowed")]
        public async void RequestIsAllowed()
        {
            // Given
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            var configuration = new CongestionControlConfiguration();
            var tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
            var loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

            tokenBucketConsumerMock
                .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                .ReturnsAsync(new ConsumeResult(true, 1, 1));

            async Task Next(HttpContext httpContext)
            {
                await Task.CompletedTask;

                isNextCalled = true;
            }

            context.Items.AddClientId(Guid.NewGuid().ToString());

            var middleware = new RequestRateLimiterMiddleware(Next,
                configuration,
                tokenBucketConsumerMock.Object,
                new DefaultHttpResponseFormatter(),
                loggerMock.Object);

            // When the middleware is invoked
            await middleware.Invoke(context);

            // Then it should execute next delegate in the pipeline
            isNextCalled.Should().BeTrue();
        }

        [Fact(DisplayName = "Request Is Not Allowed")]
        public async void RequestIsNotAllowed()
        {
            // Given
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            var configuration = new CongestionControlConfiguration();
            var tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
            var httpResponseFormatterMock = new Mock<IHttpResponseFormatter>();
            var loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

            tokenBucketConsumerMock
                .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                .ReturnsAsync(new ConsumeResult(false, 0, 1));

            async Task Next(HttpContext httpContext)
            {
                await Task.CompletedTask;

                isNextCalled = true;
            }

            context.Items.AddClientId(Guid.NewGuid().ToString());

            var middleware = new RequestRateLimiterMiddleware(Next,
                configuration,
                tokenBucketConsumerMock.Object,
                httpResponseFormatterMock.Object,
                loggerMock.Object);

            // When the middleware is invoked
            await middleware.Invoke(context);

            // Then it should execute HTTP response formatter
            httpResponseFormatterMock.Verify(mock => mock.FormatAsync(
                Moq.It.IsAny<HttpContext>(),
                Moq.It.IsAny<RateLimitContext>()), Times.Once);

            // And it should not execute next delegate in pipeline
            isNextCalled.Should().BeFalse();
        }
    }
}
