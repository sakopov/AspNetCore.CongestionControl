using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Machine.Specifications;

namespace AspNetCore.CongestionControl.UnitTests
{
    class TokenBucketTests
    {
        [Subject(typeof(TokenBucket.TokenBucket), "Token Bucket"), Tags("Positive Test")]
        public class When_the_requested_number_of_tokens_does_not_exceed_available_tokens
        {
            Establish context = () =>
            {
                _tokenBucket = new TokenBucket.TokenBucket(
                    interval: Interval,
                    averageRate: AverageRate,
                    bursting: Bursting);
            };

            Because of = () =>
            {
                _response = _tokenBucket.Consume(Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
            };

            It should_return_the_expected_number_of_available_tokens = () =>
            {
                _response.TokensLeft.ShouldEqual(AverageRate * Bursting - Requested);
            };

            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 3;

            static TokenConsumeResponse _response;
            static TokenBucket.TokenBucket _tokenBucket;
        }

        [Subject(typeof(TokenBucket.TokenBucket), "Token Bucket"), Tags("Positive Test")]
        public class When_the_requested_number_of_tokens_depletes_the_token_bucket
        {
            Establish context = () =>
            {
                _tokenBucket = new TokenBucket.TokenBucket(
                    interval: Interval,
                    averageRate: AverageRate,
                    bursting: Bursting);
            };

            Because of = () =>
            {
                _response = _tokenBucket.Consume(Requested).Await();
            };

            It should_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
            };

            It should_return_the_expected_number_of_available_tokens = () =>
            {
                _response.TokensLeft.ShouldEqual(AverageRate * Bursting - Requested);
            };

            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 4;

            static TokenConsumeResponse _response;
            static TokenBucket.TokenBucket _tokenBucket;
        }

        [Subject(typeof(TokenBucket.TokenBucket), "Token Bucket"), Tags("Positive Test")]
        public class When_the_token_consumption_rate_does_not_burst_above_capacity_in_an_interval
        {
            Establish context = () =>
            {
                _tokenBucket = new TokenBucket.TokenBucket(
                    interval: Interval,
                    averageRate: AverageRate,
                    bursting: Bursting);
            };

            Because of = () =>
            {
                // Do not burst above average rate
                _response.Add(_tokenBucket.Consume(Requested).Await());
                _response.Add(_tokenBucket.Consume(Requested).Await());

                var lastConsumedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Wait for interval to lapse
                Thread.Sleep(Interval * 1000);

                // Consume another token
                _response.Add(_tokenBucket.Consume(Requested).Await());

                _availableTokensAfterBurst = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastConsumedAt) / Interval * AverageRate;
            };

            It should_return_the_expected_sequence_of_responses = () =>
            {
                var capacityAtBurst = AverageRate * Bursting;

                _response[0].IsAllowed.ShouldBeTrue();
                _response[0].TokensLeft.ShouldEqual(capacityAtBurst - 1);

                _response[1].IsAllowed.ShouldBeTrue();
                _response[1].TokensLeft.ShouldEqual(capacityAtBurst - 2);

                _response[2].IsAllowed.ShouldBeTrue();
                _response[2].TokensLeft.ShouldEqual(capacityAtBurst - 1);
            };

            const int Interval = 1;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 1;

            static long _availableTokensAfterBurst;
            static List<TokenConsumeResponse> _response = new List<TokenConsumeResponse>();
            static TokenBucket.TokenBucket _tokenBucket;
        }

        [Subject(typeof(TokenBucket.TokenBucket), "Token Bucket"), Tags("Positive Test")]
        public class When_the_token_consumption_rate_bursts_above_capacity_in_an_interval
        {
            Establish context = () =>
            {
                _tokenBucket = new TokenBucket.TokenBucket(
                    interval: Interval,
                    averageRate: AverageRate,
                    bursting: Bursting);
            };

            Because of = () =>
            {
                // Burst above average rate by 3 requests
                _response.Add(_tokenBucket.Consume(Requested).Await());
                _response.Add(_tokenBucket.Consume(Requested).Await());
                _response.Add(_tokenBucket.Consume(Requested).Await());
                _response.Add(_tokenBucket.Consume(Requested).Await());
                _response.Add(_tokenBucket.Consume(Requested).Await());

                var lastConsumedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Wait for interval to lapse
                Thread.Sleep(Interval * 1000);

                // Consume 1 more in new interval
                _response.Add(_tokenBucket.Consume(Requested).Await());

                _availableTokensAfterBurst = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastConsumedAt) / Interval * AverageRate;
            };

            It should_return_the_expected_sequence_of_responses = () =>
            {
                var capacityAtBurst = AverageRate * Bursting;

                _response[0].IsAllowed.ShouldBeTrue();
                _response[0].TokensLeft.ShouldEqual(capacityAtBurst - 1);

                _response[1].IsAllowed.ShouldBeTrue();
                _response[1].TokensLeft.ShouldEqual(capacityAtBurst - 2);

                _response[2].IsAllowed.ShouldBeTrue();
                _response[2].TokensLeft.ShouldEqual(capacityAtBurst - 3);

                _response[3].IsAllowed.ShouldBeTrue();
                _response[3].TokensLeft.ShouldEqual(capacityAtBurst - 4);

                _response[4].IsAllowed.ShouldBeFalse();
                _response[4].TokensLeft.ShouldEqual(0);

                _response[5].IsAllowed.ShouldBeTrue();
                _response[5].TokensLeft.ShouldEqual((int)_availableTokensAfterBurst - 1);
            };

            const int Interval = 1;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 1;

            static long _availableTokensAfterBurst;
            static List<TokenConsumeResponse> _response = new List<TokenConsumeResponse>();
            static TokenBucket.TokenBucket _tokenBucket;
        }

        [Subject(typeof(TokenBucket.TokenBucket), "Token Bucket"), Tags("Negative Test")]
        public class When_the_requested_number_of_tokens_exceeds_available_tokens
        {
            Establish context = () =>
            {
                _tokenBucket = new TokenBucket.TokenBucket(
                    interval: Interval,
                    averageRate: AverageRate,
                    bursting: Bursting);
            };

            Because of = () =>
            {
                _response = _tokenBucket.Consume(Requested).Await();
            };

            It should_not_allow_token_consumption = () =>
            {
                _response.IsAllowed.ShouldBeFalse();
            };

            It should_return_the_expected_number_of_available_tokens = () =>
            {
                _response.TokensLeft.ShouldEqual(AverageRate * Bursting);
            };

            const int Interval = 10;
            const int AverageRate = 2;
            const int Bursting = 2;
            const int Requested = 10;

            static TokenConsumeResponse _response;
            static TokenBucket.TokenBucket _tokenBucket;
        }
    }
}