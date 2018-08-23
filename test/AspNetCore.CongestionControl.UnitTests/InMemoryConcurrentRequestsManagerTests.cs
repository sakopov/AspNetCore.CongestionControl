namespace AspNetCore.CongestionControl.UnitTests
{
    using Configuration;
    using Machine.Specifications;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using It = Machine.Specifications.It;

    class InMemoryConcurrentRequestsManagerTests
    {
        [Subject(typeof(InMemoryConcurrentRequestsManager), "In-Memory Concurrent Requests Manager"), Tags("Positive Test")]
        public class When_client_adds_multiple_simultaneous_requests_under_capacity
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();

                _manager = new InMemoryConcurrentRequestsManager(_configuration, _loggerMock.Object);
            };

            Because of = () =>
            {
                _result1 = _manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
                _result2 = _manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            It should_allow_first_request = () =>
            {
                _result1.IsAllowed.ShouldBeTrue();
                _result1.Limit.ShouldEqual(_configuration.Capacity);
                _result1.Remaining.ShouldEqual(_configuration.Capacity - 1);
            };

            It should_allow_second_request = () =>
            {
                _result2.IsAllowed.ShouldBeTrue();
                _result2.Limit.ShouldEqual(_configuration.Capacity);
                _result2.Remaining.ShouldEqual(_configuration.Capacity - 2);
            };

            const string ClientId = "tester";

            static AddConcurrentRequestResult _result1, _result2;

            static Mock<ILogger<InMemoryConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsManager _manager;
        }

        [Subject(typeof(InMemoryConcurrentRequestsManager), "In-Memory Concurrent Requests Manager"), Tags("Positive Test")]
        public class When_client_removes_a_request_which_was_previously_added
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();

                _manager = new InMemoryConcurrentRequestsManager(_configuration, _loggerMock.Object);

                _manager.AddAsync(ClientId, RequestId, DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            Because of = () =>
            {
                _result = _manager.RemoveAsync(ClientId, RequestId).Await();
            };

            It should_remove_the_request = () =>
            {
                _result.ShouldBeTrue();
            };

            const string ClientId = "tester";
            const string RequestId = "123";

            static bool _result;

            static Mock<ILogger<InMemoryConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsManager _manager;
        }

        [Subject(typeof(InMemoryConcurrentRequestsManager), "In-Memory Concurrent Requests Manager"), Tags("Negative Test")]
        public class When_client_adds_a_request_which_puts_it_over_capacity
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();

                _manager = new InMemoryConcurrentRequestsManager(_configuration, _loggerMock.Object);

                for (var i = 0; i < _configuration.Capacity; i++)
                {
                    _manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
                }
            };

            Because of = () =>
            {
                _result = _manager.AddAsync(ClientId, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Await();
            };

            It should_not_allow_the_request = () =>
            {
                _result.IsAllowed.ShouldBeFalse();
                _result.Limit.ShouldEqual(_configuration.Capacity);
                _result.Remaining.ShouldEqual(0);
            };

            const string ClientId = "tester";

            static AddConcurrentRequestResult _result;

            static Mock<ILogger<InMemoryConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsManager _manager;
        }

        [Subject(typeof(InMemoryConcurrentRequestsManager), "In-Memory Concurrent Requests Manager"), Tags("Negative Test")]
        public class When_client_removes_a_request_which_was_not_previously_added
        {
            Establish context = () =>
            {
                _configuration = new ConcurrentRequestLimiterConfiguration();
                _loggerMock = new Mock<ILogger<InMemoryConcurrentRequestsManager>>();

                _manager = new InMemoryConcurrentRequestsManager(_configuration, _loggerMock.Object);
            };

            Because of = () =>
            {
                _result = _manager.RemoveAsync(ClientId, Guid.NewGuid().ToString()).Await();
            };

            It should_not_remove_the_request = () =>
            {
                _result.ShouldBeFalse();
            };

            const string ClientId = "tester";

            static bool _result;

            static Mock<ILogger<InMemoryConcurrentRequestsManager>> _loggerMock;
            static ConcurrentRequestLimiterConfiguration _configuration;
            static InMemoryConcurrentRequestsManager _manager;
        }
    }
}