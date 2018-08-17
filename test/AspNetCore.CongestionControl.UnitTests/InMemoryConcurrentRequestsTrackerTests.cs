using System;
using AspNetCore.CongestionControl.Configuration;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using It = Machine.Specifications.It;

namespace AspNetCore.CongestionControl.UnitTests
{
    class InMemoryConcurrentRequestsTrackerTests
    {
        [Subject(typeof(InMemoryConcurrentRequestsTracker), "In-Memory Concurrent Requests Tracker"), Tags("Positive Test")]
        public class When_client_adds_multiple_simultaneous_requests_under_capacity
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsTracker>>();

                _tracker = new InMemoryConcurrentRequestsTracker(_configuration, _loggerMock.Object);
            };

            Because of = () =>
            {
                _result1 = _tracker.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
                _result2 = _tracker.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            It should_allow_all_requests = () =>
            {
                _result1.ShouldBeTrue();
                _result2.ShouldBeTrue();
            };

            const string ClientId = "tester";

            static bool _result1, _result2;

            static Mock<ILogger<InMemoryConcurrentRequestsTracker>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsTracker _tracker;
        }

        [Subject(typeof(InMemoryConcurrentRequestsTracker), "In-Memory Concurrent Requests Tracker"), Tags("Positive Test")]
        public class When_client_removes_a_request_which_was_previously_added
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsTracker>>();

                _tracker = new InMemoryConcurrentRequestsTracker(_configuration, _loggerMock.Object);

                _tracker.AddAsync(ClientId, RequestId, DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            Because of = () =>
            {
                _result = _tracker.RemoveAsync(ClientId, RequestId).Await();
            };

            It should_remove_the_request = () =>
            {
                _result.ShouldBeTrue();
            };

            const string ClientId = "tester";
            const string RequestId = "123";

            static bool _result;

            static Mock<ILogger<InMemoryConcurrentRequestsTracker>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsTracker _tracker;
        }

        [Subject(typeof(InMemoryConcurrentRequestsTracker), "In-Memory Concurrent Requests Tracker"), Tags("Negative Test")]
        public class When_client_adds_a_request_which_puts_it_over_capacity
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsTracker>>();

                _tracker = new InMemoryConcurrentRequestsTracker(_configuration, _loggerMock.Object);

                for (var i = 0; i < _configuration.Capacity; i++)
                {
                    _tracker.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
                }
            };

            Because of = () =>
            {
                _result = _tracker.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            It should_not_allow_the_request = () =>
            {
                _result.ShouldBeFalse();
            };

            const string ClientId = "tester";

            static bool _result;

            static Mock<ILogger<InMemoryConcurrentRequestsTracker>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsTracker _tracker;
        }

        [Subject(typeof(InMemoryConcurrentRequestsTracker), "In-Memory Concurrent Requests Tracker"), Tags("Negative Test")]
        public class When_client_removes_a_request_which_was_not_previously_added
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsTracker>>();

                _tracker = new InMemoryConcurrentRequestsTracker(_configuration, _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _tracker.RemoveAsync(ClientId, Guid.NewGuid().ToString()).Await();
            };

            It should_not_remove_the_request = () =>
            {
                _result.ShouldBeFalse();
            };

            const string ClientId = "tester";

            static bool _result;

            static Mock<ILogger<InMemoryConcurrentRequestsTracker>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsTracker _tracker;
        }
    }
}