using AspNetCore.CongestionControl.Configuration;
using Machine.Specifications;

namespace AspNetCore.CongestionControl.UnitTests
{
    class InMemoryTokenBucketConsumerTests
    {
        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Positive Test")]
        public class When_client_consumes_a_token_for_the_first_time
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();
                _consumer = new InMemoryTokenBucketConsumer(_configuration);
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
            };

            It should_return_the_remaining_tokens_left = () =>
            {
                var capacity = _configuration.AverageRate * _configuration.Bursting;

                _response.TokensLeft.Equals(capacity - 1);
            };

            const string ClientId = "tester";
            const int Requested = 1;

            static TokenConsumeResponse _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
        }

        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Positive Test")]
        public class When_client_consumes_tokens_repeatedly
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();
                _consumer = new InMemoryTokenBucketConsumer(_configuration);
                _consumer.ConsumeAsync(ClientId, PreviousConsumptionCount).Await();
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
            };

            It should_return_the_remaining_tokens_left = () =>
            {
                _response.TokensLeft.Equals(_configuration.AverageRate * _configuration.Bursting - PreviousConsumptionCount);
            };

            const string ClientId = "tester";
            const int Requested = 1;
            const int PreviousConsumptionCount = 5;

            static TokenConsumeResponse _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
        }

        [Subject(typeof(InMemoryTokenBucketConsumer), "In-Memory Token Bucket Consumer"), Tags("Negative Test")]
        public class When_client_consumes_all_available_tokens
        {
            Establish context = () =>
            {
                _configuration = new RequestRateLimiterConfiguration();

                _consumer = new InMemoryTokenBucketConsumer(_configuration);

                var capacity = _configuration.AverageRate * _configuration.Bursting;

                _consumer.ConsumeAsync(ClientId, capacity).Await();
            };

            Because of = () =>
            {
                _response = _consumer.ConsumeAsync(ClientId, Requested).Await();
            };

            It should_not_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeFalse();
            };

            It should_return_0_available_tokens = () =>
            {
                _response.TokensLeft.ShouldEqual(0);
            };

            const string ClientId = "tester";
            const int Requested = 1;

            static TokenConsumeResponse _response;
            static RequestRateLimiterConfiguration _configuration;
            static InMemoryTokenBucketConsumer _consumer;
        }
    }
}