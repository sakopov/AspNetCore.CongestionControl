namespace AspNetCore.CongestionControl.UnitTests
{
    using Machine.Specifications;
    using Microsoft.AspNetCore.Http;
    using System;

    class HeaderBasedClientIdentifierProviderTests
    {
        [Subject(typeof(HeaderBasedClientIdentifierProvider), "Header-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_client_set_client_identifier_in_headers
        {
            Establish context = () =>
            {
                _context.Request.Headers.Add(HeaderName, ClientId);
            };

            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_the_expected_client_identifier = () =>
            {
                _result.ShouldEqual(ClientId);
            };

            const string HeaderName = "x-client-id";
            const string ClientId = "tester";

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static HeaderBasedClientIdentifierProvider _provider = new HeaderBasedClientIdentifierProvider();
        }

        [Subject(typeof(HeaderBasedClientIdentifierProvider), "Header-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_client_is_using_custom_header_name
        {
            Establish context = () =>
            {
                _context.Request.Headers.Add(HeaderName, ClientId);
                _provider = new HeaderBasedClientIdentifierProvider(HeaderName);
            };

            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_the_expected_client_identifier = () =>
            {
                _result.ShouldEqual(ClientId);
            };

            const string HeaderName = "my-custom-header";
            const string ClientId = "tester";

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static HeaderBasedClientIdentifierProvider _provider;
        }

        [Subject(typeof(HeaderBasedClientIdentifierProvider), "Header-Based Client Identifier Provider"), Tags("Positive Test")]
        public class When_header_name_is_not_provided
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => new HeaderBasedClientIdentifierProvider(null));
            };

            It should_throw_argument_null_exception = () =>
            {
                _exception.ShouldNotBeNull();
                _exception.ShouldBeOfExactType<ArgumentNullException>();
            };

            static Exception _exception;
            static DefaultHttpContext _context = new DefaultHttpContext();
        }

        [Subject(typeof(HeaderBasedClientIdentifierProvider), "Header-Based Client Identifier Provider"), Tags("Negative Test")]
        public class When_client_does_not_set_client_identifier_in_headers
        {
            Because of = () =>
            {
                _result = _provider.ExecuteAsync(_context).Await();
            };

            It should_return_anonymous_client_identifer = () =>
            {
                _result.ShouldEqual("anonymous");
            };

            static string _result;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static HeaderBasedClientIdentifierProvider _provider = new HeaderBasedClientIdentifierProvider();
        }
    }
}