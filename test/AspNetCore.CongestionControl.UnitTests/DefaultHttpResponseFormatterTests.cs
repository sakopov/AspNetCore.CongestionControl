namespace AspNetCore.CongestionControl.UnitTests
{
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Machine.Specifications;

    class DefaultHttpResponseFormatterTests
    {
        [Subject(typeof(DefaultHttpResponseFormatter), "Default HTTP Response Formatter"), Tags("Positive Test")]
        public class When_formatting_rate_limit_response
        {
            Establish context = () =>
            {
                _httpContext = new DefaultHttpContext();
                _httpContext.Request.ContentType = "application/json";

                _rateLimitContext = new RateLimitContext(10, 20, HttpStatusCode.ServiceUnavailable, "source");
            };

            Because of = () =>
            {
                _formatter.FormatAsync(_httpContext, _rateLimitContext).Await();
            };

            It should_set_the_response_content_type_to_request_content_type = () =>
            {
                _httpContext.Response.ContentType.ShouldEqual(_httpContext.Request.ContentType);
            };

            It should_set_the_response_status_code_to_configured_http_status_code = () =>
            {
                _httpContext.Response.StatusCode.ShouldEqual((int)_rateLimitContext.HttpStatusCode);
            };

            static RateLimitContext _rateLimitContext;
            static HttpContext _httpContext = new DefaultHttpContext();
            static DefaultHttpResponseFormatter _formatter = new DefaultHttpResponseFormatter();
        }
    }
}