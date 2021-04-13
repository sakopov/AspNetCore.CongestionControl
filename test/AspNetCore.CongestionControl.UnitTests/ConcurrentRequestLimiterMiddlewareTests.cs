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

    class ConcurrentRequestLimiterMiddlewareTests
    {
        [Subject(typeof(ConcurrentRequestLimiterMiddleware), "Concurrent Request Limiter Middleware"), Tags("Positive Test")]
        public class When_request_is_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _concurrentRequestTrackerMock = new Mock<IConcurrentRequestsManager>();
                _loggerMock = new Mock<ILogger<ConcurrentRequestLimiterMiddleware>>();

                _concurrentRequestTrackerMock
                    .Setup(mock => mock.AddAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long>()))
                    .ReturnsAsync(new AddConcurrentRequestResult(true, 1, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _context.Items.AddClientId(Guid.NewGuid().ToString());

                _middleware = new ConcurrentRequestLimiterMiddleware(Next, 
                    _configuration,
                    _concurrentRequestTrackerMock.Object,
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

            It should_remove_request_from_request_tracker = () =>
            {
                _concurrentRequestTrackerMock.Verify(mock => mock.RemoveAsync(
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>()), Times.Once);
            };

            static bool _isNextCalled;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IConcurrentRequestsManager> _concurrentRequestTrackerMock;
            static Mock<ILogger<ConcurrentRequestLimiterMiddleware>> _loggerMock;
            static ConcurrentRequestLimiterMiddleware _middleware;
        }

        [Subject(typeof(ConcurrentRequestLimiterMiddleware), "Concurrent Request Limiter Middleware"), Tags("Negative Test")]
        public class When_request_is_not_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _concurrentRequestTrackerMock = new Mock<IConcurrentRequestsManager>();
                _httpResponseFormatterMock = new Mock<IHttpResponseFormatter>();
                _loggerMock = new Mock<ILogger<ConcurrentRequestLimiterMiddleware>>();

                _concurrentRequestTrackerMock
                    .Setup(mock => mock.AddAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long>()))
                    .ReturnsAsync(new AddConcurrentRequestResult(false, 1, 1));

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ConcurrentRequestLimiterMiddleware(Next,
                    _configuration,
                    _concurrentRequestTrackerMock.Object,
                    _httpResponseFormatterMock.Object,
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _context.Items.AddClientId(Guid.NewGuid().ToString());

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

            It should_not_call_remove_on_request_tracker = () =>
            {
                _concurrentRequestTrackerMock.Verify(mock => mock.RemoveAsync(
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>()), Times.Never);
            };

            static bool _isNextCalled;
            static HttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IConcurrentRequestsManager> _concurrentRequestTrackerMock;
            static Mock<IHttpResponseFormatter> _httpResponseFormatterMock;
            static Mock<ILogger<ConcurrentRequestLimiterMiddleware>> _loggerMock;
            static ConcurrentRequestLimiterMiddleware _middleware;
        }
    }
}