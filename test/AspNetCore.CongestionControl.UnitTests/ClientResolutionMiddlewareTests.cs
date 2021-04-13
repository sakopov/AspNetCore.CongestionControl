namespace AspNetCore.CongestionControl.UnitTests
{
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using Machine.Specifications;
    using Moq;
    using It = Machine.Specifications.It;
    using System.Collections.Generic;
    using System.Net;

    class ClientResolutionMiddlewareTests
    {
        [Subject(typeof(ClientResolutionMiddleware), "Client Resolution Middleware"), Tags("Positive Test")]
        public class When_anonymous_clients_are_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync((string)null);

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ClientResolutionMiddleware(Next,
                    _configuration,
                    new List<IClientIdentifierProvider> { _clientIdentifierProviderMock.Object },
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeTrue();
            };

            It should_add_client_id_to_http_context_items = () =>
            {
                _context.Items.GetClientId().ShouldNotBeNull();
            };

            static bool _isNextCalled;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ILogger<ClientResolutionMiddleware>> _loggerMock;
            static ClientResolutionMiddleware _middleware;
        }

        [Subject(typeof(ClientResolutionMiddleware), "Client Resolution Middleware"), Tags("Positive Test")]
        public class When_client_identity_is_resolved
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration();
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync(_clientId);

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ClientResolutionMiddleware(Next,
                    _configuration,
                    new List<IClientIdentifierProvider> { _clientIdentifierProviderMock.Object },
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeTrue();
            };

            It should_add_client_id_to_http_context_items = () =>
            {
                _context.Items.GetClientId().ShouldEqual(_clientId);
            };

            static string _clientId = Guid.NewGuid().ToString();
            static bool _isNextCalled;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ILogger<ClientResolutionMiddleware>> _loggerMock;
            static ClientResolutionMiddleware _middleware;
        }

        [Subject(typeof(ClientResolutionMiddleware), "Client Resolution Middleware"), Tags("Negative Test")]
        public class When_anonymous_clients_are_not_allowed
        {
            Establish context = () =>
            {
                _configuration = new CongestionControlConfiguration { AllowAnonymousClients = false };
                _clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
                _loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

                _clientIdentifierProviderMock
                    .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                    .ReturnsAsync((string)null);

                async Task Next(HttpContext httpContext)
                {
                    await Task.CompletedTask;

                    _isNextCalled = true;
                }

                _middleware = new ClientResolutionMiddleware(Next,
                    _configuration,
                    new List<IClientIdentifierProvider> { _clientIdentifierProviderMock.Object },
                    _loggerMock.Object);
            };

            Because of = () =>
            {
                _middleware.Invoke(_context).Await();
            };

            It should_not_execute_next_delegate_in_pipeline = () =>
            {
                _isNextCalled.ShouldBeFalse();
            };

            It should_return_unauthorized_response = () =>
            {
                _context.Response.StatusCode.ShouldEqual((int)HttpStatusCode.Unauthorized);
            };

            static bool _isNextCalled;
            static DefaultHttpContext _context = new DefaultHttpContext();
            static CongestionControlConfiguration _configuration;
            static Mock<IClientIdentifierProvider> _clientIdentifierProviderMock;
            static Mock<ILogger<ClientResolutionMiddleware>> _loggerMock;
            static ClientResolutionMiddleware _middleware;
        }
    }
}