namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;
    using System.Net;

    class RateLimitContextTests
    {
        [Subject(typeof(RateLimitContext), "Rate Limit Context"), Tags("Positive Test")]
        public class When_creating_new_rate_limit_context
        {
            Because of = () =>
            {
                _context = new RateLimitContext(1, 10, HttpStatusCode.ServiceUnavailable, "source");
            };

            It should_create_the_expected_rate_limit_context = () =>
            {
                _context.Remaining.ShouldEqual(1);
                _context.Limit.ShouldEqual(10);
                _context.HttpStatusCode.ShouldEqual(HttpStatusCode.ServiceUnavailable);
                _context.Source.ShouldEqual("source");
            };

            static RateLimitContext _context;
        }
    }
}