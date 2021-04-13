namespace AspNetCore.CongestionControl.UnitTests
{
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;

    class RequestRateLimiterMiddlewareTests
    {
        [Subject(typeof(RequestRateLimiterMiddleware), "Request Rate Limiter Middleware"), Tags("Positive Test")]
        public class When_request_is_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
                _loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

                _tokenBucketConsumerMock
                    .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                    .ReturnsAsync(new ConsumeResult(true, 1, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _context.Items.AddClientId(Guid.NewGuid().ToString());

                _middleware = new RequestRateLimiterMiddleware(Next,
                    _configuration,
                    _tokenBucketConsumerMock.Object,
                    new DefaultHttpResponseFormatter(),
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeTrue();
            };

            static bool _isNextCalled;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
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
                _tokenBucketConsumerMock = new Mock<ITokenBucketConsumer>();
                _httpResponseFormatterMock = new Mock<IHttpResponseFormatter>();
                _loggerMock = new Mock<ILogger<RequestRateLimiterMiddleware>>();

                _tokenBucketConsumerMock
                    .Setup(mock => mock.ConsumeAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<int>()))
                    .ReturnsAsync(new ConsumeResult(false, 0, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _context.Items.AddClientId(Guid.NewGuid().ToString());

                _middleware = new RequestRateLimiterMiddleware(Next, 
                    _configuration,
                    _tokenBucketConsumerMock.Object,
                    _httpResponseFormatterMock.Object,
                    _loggerMock.Object);
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
            static Mock<ITokenBucketConsumer> _tokenBucketConsumerMock;
            static Mock<IHttpResponseFormatter> _httpResponseFormatterMock;
            static RequestRateLimiterMiddleware _middleware;
        }
    }
}