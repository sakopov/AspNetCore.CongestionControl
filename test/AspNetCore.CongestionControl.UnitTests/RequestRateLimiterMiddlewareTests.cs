namespace AspNetCore.CongestionControl.UnitTests
{
    using Configuration;
    using Machine.Specifications;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using It = Machine.Specifications.It;

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
                    .ReturnsAsync(new ConsumeResult(true, 1, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new RequestRateLimiterMiddleware(Next,
                    _configuration,
                    _clientIdentifierProviderMock.Object,
                    _tokenBucketConsumerMock.Object,
                    new DefaultHttpResponseFormatter());
            };

            Because of = () =>
            {
                _middleware.Invoke(new DefaultHttpContext()).Await();
            };

            It should_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeTrue();
            };

            static bool _isNextCalled;
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
                _httpResponseFormatterMock = new Mock<IHttpResponseFormatter>();
                _loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(Guid.NewGuid().ToString);

                _tokenBucketConsumerMock
                    .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                    .ReturnsAsync(new ConsumeResult(false, 0, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new RequestRateLimiterMiddleware(Next, 
                    _configuration,
                    _clientIdentifierProviderMock.Object,
                    _tokenBucketConsumerMock.Object,
                    _httpResponseFormatterMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_execute_http_response_formatter = () =>
            {
                _httpResponseFormatterMock.Verify(mock => mock.FormatAsync(
                    Moq.It.IsAny<HttpContext>(),
                    Moq.It.IsAny<RateLimitContext>()), Times.Once);
            };

            It should_not_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeFalse();
            };

            static bool _isNextCalled;
            static HttpContext _context = new DefaultHttpContext();
            static Mock<ILogger<RequestRateLimiterMiddleware>> _loggerMock;
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ITokenBucketConsumer> _tokenBucketConsumerMock;
            static Mock<IHttpResponseFormatter> _httpResponseFormatterMock;
            static RequestRateLimiterMiddleware _middleware;
        }
    }
}