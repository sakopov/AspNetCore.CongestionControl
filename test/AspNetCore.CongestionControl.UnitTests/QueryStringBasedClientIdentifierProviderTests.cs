namespace AspNetCore.CongestionControl.UnitTests
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Machine.Specifications;

    class QueryStringBasedClientIdentifierProviderTests
    {
        [Subject(typeof(QueryBasedClientIdentifierProvider), "Query-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_client_set_client_identifier_in_query_string
        {
            Establish context = () =>
            {
                _context.Request.QueryString = new QueryString($"?{QueryStringParameter}={ClientId}");
            };

            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_the_expected_client_identifier = () =>
            {
                _result.ShouldEqual(ClientId);
            };

            const string QueryStringParameter = "api_key";
            const string ClientId = "tester";

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static QueryBasedClientIdentifierProvider _provider = new QueryBasedClientIdentifierProvider();
        }

        [Subject(typeof(QueryBasedClientIdentifierProvider), "Query-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_client_is_using_custom_query_string_parameter
        {
            Establish context = () =>
            {
                _context.Request.QueryString = new QueryString($"?{QueryStringParameter}={ClientId}");
                _provider = new QueryBasedClientIdentifierProvider(QueryStringParameter);
            };

            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_the_expected_client_identifier = () =>
            {
                _result.ShouldEqual(ClientId);
            };

            const string QueryStringParameter = "my-custom-parameter";
            const string ClientId = "tester";

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static QueryBasedClientIdentifierProvider _provider;
        }

        [Subject(typeof(QueryBasedClientIdentifierProvider), "Query-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_client_does_not_set_client_identifier_in_query_string
        {
            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_null_client_identifer = () =>
            {
                _result.ShouldBeNull();
            };

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static QueryBasedClientIdentifierProvider _provider = new QueryBasedClientIdentifierProvider();
        }

        [Subject(typeof(QueryBasedClientIdentifierProvider), "Query-Based Client Identifier Provider"), Tags("Negative Test")]
        public class When_query_string_parameter_is_not_provided
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => new QueryBasedClientIdentifierProvider(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
            };

            static Exception _exception;
            static DefaultHttpContext _context = new DefaultHttpContext();
        }
    }
}