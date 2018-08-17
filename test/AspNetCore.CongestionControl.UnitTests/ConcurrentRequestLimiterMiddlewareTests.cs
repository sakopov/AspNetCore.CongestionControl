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
    class ConcurrentRequestLimiterMiddlewareTests
    {
        [Subject(typeof(ConcurrentRequestLimiterMiddleware), "Concurrent Request Limiter Middleware"), Tags("Positive Test")]
        public class When_request_is_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _concurrentRequestTrackerMock = new Mock<IConcurrentRequestsTracker>();
                _loggerMock = new Mock<ILogger<ConcurrentRequestLimiterMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(Guid.NewGuid().ToString);

                _concurrentRequestTrackerMock
                    .Setup(mock => mock.AddAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long>()))
                    .ReturnsAsync(true);

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ConcurrentRequestLimiterMiddleware(Next, 
                    _configuration,
                    _clientIdentifierProviderMock.Object,
                    _concurrentRequestTrackerMock.Object,
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

            It should_remove_request_from_request_tracker = () =>
            {
                _concurrentRequestTrackerMock.Verify(mock => mock.RemoveAsync(
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>()), Times.Once);
            };

            static bool _isNextCalled = false;
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<IConcurrentRequestsTracker> _concurrentRequestTrackerMock;
            static Mock<ILogger<ConcurrentRequestLimiterMiddleware>> _loggerMock;
            static ConcurrentRequestLimiterMiddleware _middleware;
        }

        [Subject(typeof(ConcurrentRequestLimiterMiddleware), "Concurrent Request Limiter Middleware"), Tags("Negative Test")]
        public class When_request_is_not_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _concurrentRequestTrackerMock = new Mock<IConcurrentRequestsTracker>();
                _loggerMock = new Mock<ILogger<ConcurrentRequestLimiterMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(Guid.NewGuid().ToString);

                _concurrentRequestTrackerMock
                    .Setup(mock => mock.AddAsync(Moq.It.IsAny<string>(), Moq.It.IsAny<string>(), Moq.It.IsAny<long>()))
                    .ReturnsAsync(false);

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ConcurrentRequestLimiterMiddleware(Next,
                    _configuration,
                    _clientIdentifierProviderMock.Object,
                    _concurrentRequestTrackerMock.Object,
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

            It should_not_call_remove_on_request_tracker = () =>
            {
                _concurrentRequestTrackerMock.Verify(mock => mock.RemoveAsync(
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>()), Times.Never);
            };

            static bool _isNextCalled = false;
            static HttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<IConcurrentRequestsTracker> _concurrentRequestTrackerMock;
            static Mock<ILogger<ConcurrentRequestLimiterMiddleware>> _loggerMock;
            static ConcurrentRequestLimiterMiddleware _middleware;
        }
    }
}