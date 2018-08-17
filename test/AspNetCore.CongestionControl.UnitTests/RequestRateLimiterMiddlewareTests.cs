using System;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using It = Machine.Specifications.It;

namespace AspNetCore.CongestionControl.UnitTests
{
    class RequestRateLimiterMiddlewareTests
    {
        [Subject(typeof(RequestRateLimiterMiddleware), "Request Rate Limiter Middleware"), Tags("Positive Test")]
        public class When_request_is_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
                _loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(Guid.NewGuid().ToString);

                _tokenBucketConsumerMock
                    .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                    .ReturnsAsync(new TokenConsumeResponse(true, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new RequestRateLimiterMiddleware(Next, 
                    _configuration, 
                    _clientIdentifierProviderMock.Object, 
                    _tokenBucketConsumerMock.Object, 
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(new DefaultHttpContext()).Await();
            };

            It should_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeTrue();
            };

            static bool _isNextCalled = false;
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ITokenBucketConsumer> _tokenBucketConsumerMock;
            static Mock<ILogger<RequestRateLimiterMiddleware>> _loggerMock;
            static RequestRateLimiterMiddleware _middleware;
        }

        [Subject(typeof(RequestRateLimiterMiddleware), "Request Rate Limiter Middleware"), Tags("Negative Test")]
        public class When_request_is_not_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
                _loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(Guid.NewGuid().ToString);

                _tokenBucketConsumerMock
                    .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                    .ReturnsAsync(new TokenConsumeResponse(false, 0));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new RequestRateLimiterMiddleware(Next, 
                    _configuration,
                    _clientIdentifierProviderMock.Object,
                    _tokenBucketConsumerMock.Object,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_log_information_message = () =>
            {
                _loggerMock.Verify(mock => mock.Log(
                    LogLevel.Information,
                    Moq.It.IsAny<EventId>(),
                    Moq.It.IsAny<FormattedLogValues>(),
                    Moq.It.IsAny<Exception>(),
                    Moq.It.IsAny<Func<object, Exception, string>>()
                ), Times.Once);
            };

            It should_set_the_expected_http_response = () =>
            {
                _context.Response.ShouldNotBeNull();
                _context.Response.ContentType.ShouldEqual("application/json");
                _context.Response.StatusCode.ShouldEqual(_configuration.HttpStatusCode);
            };

            It should_not_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeFalse();
            };

            static bool _isNextCalled = false;
            static HttpContext _context = new DefaultHttpContext();
            static Mock<ILogger<RequestRateLimiterMiddleware>> _loggerMock;
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ITokenBucketConsumer> _tokenBucketConsumerMock;
            static RequestRateLimiterMiddleware _middleware;
        }
    }
}