using Machine.Specifications;

namespace AspNetCore.CongestionControl.UnitTests
{
    class TokenConsumeResponseTests
    {
        [Subject(typeof(TokenConsumeResponse), "Token Consume Response"), Tags("Positive Test")]
        public class When_building_non_conforming_response
        {
            Because of = () =>
            {
                _response = TokenConsumeResponse.NonConforming(1);
            };

            It should_return_the_expected_response = () =>
            {
                _response.IsAllowed.ShouldBeFalse();
                _response.TokensLeft.ShouldEqual(1);
            };

            static TokenConsumeResponse _response;
        }

        [Subject(typeof(TokenConsumeResponse), "Token Consume Response"), Tags("Positive Test")]
        public class When_building_conforming_response
        {
            Because of = () =>
            {
                _response = TokenConsumeResponse.Conforming(1);
            };

            It should_return_the_expected_response = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
                _response.TokensLeft.ShouldEqual(1);
            };

            static TokenConsumeResponse _response;
        }

        [Subject(typeof(TokenConsumeResponse), "Token Consume Response"), Tags("Positive Test")]
        public class When_creating_new_instance
        {
            Because of = () =>
            {
                _response = new TokenConsumeResponse(true, 1);
            };

            It should_return_the_expected_response = () =>
            {
                _response.IsAllowed.ShouldBeTrue();
                _response.TokensLeft.ShouldEqual(1);
            };

            static TokenConsumeResponse _response;
        }
    }
}