namespace AspNetCore.CongestionControl.UnitTests
{
    using Microsoft.Extensions.Logging;
    using Configuration;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;

    class InMemoryTokenBucketConsumerTests
    {
        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Positive Test")]
        public class When_client_consumes_a_token_for_the_first_time
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();
                _consumer = new InMemoryTokenBucketConsumer(_configuration, _loggerMock.Object);

                _capacity = _configuration.AverageRate * _configuration.Bursting;
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
                _response.Limit.ShouldEqual(_capacity);
                _response.Remaining.ShouldEqual(_capacity - 1);
            };

            const string ClientId = "tester";
            const int Requested = 1;

            static int _capacity;
            static ConsumeResult _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
            static Mock<ILogger<InMemoryTokenBucketConsumer>> _loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
        }

        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Positive Test")]
        public class When_client_consumes_tokens_repeatedly
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();
                _consumer = new InMemoryTokenBucketConsumer(_configuration, _loggerMock.Object);
                _consumer.ConsumeAsync(ClientId, PreviousConsumptionCount).Await();

                _capacity = _configuration.AverageRate * _configuration.Bursting;
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
                _response.Limit.ShouldEqual(_capacity);
                _response.Remaining.ShouldEqual(_capacity - PreviousConsumptionCount - Requested);
            };

            const string ClientId = "tester";
            const int Requested = 1;
            const int PreviousConsumptionCount = 5;

            static int _capacity;
            static ConsumeResult _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
            static Mock<ILogger<InMemoryTokenBucketConsumer>> _loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
        }

        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Negative Test")]
        public class When_client_consumes_all_available_tokens
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();

                _consumer = new InMemoryTokenBucketConsumer(_configuration, _loggerMock.Object);

                _capacity = _configuration.AverageRate * _configuration.Bursting;

                _consumer.ConsumeAsync(ClientId, _capacity).Await();
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_not_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeFalse();
                _response.Limit.ShouldEqual(_capacity);
                _response.Remaining.ShouldEqual(0);
            };

            const string ClientId = "tester";
            const int Requested = 1;

            static int _capacity;
            static ConsumeResult _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
            static Mock<ILogger<InMemoryTokenBucketConsumer>> _loggerMock = new Mock<ILogger<InMemoryTokenBucketConsumer>>();
        }
    }
}