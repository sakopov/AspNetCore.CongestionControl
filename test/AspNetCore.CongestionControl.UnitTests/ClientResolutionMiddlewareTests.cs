// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientResolutionMiddlewareTests.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Configuration;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class ClientResolutionMiddlewareTests
    {
        [Fact(DisplayName = "Allow Anonymous Clients")]
        public async void AllowAnonymousClients()
        {
            // Given
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            var configuration = new CongestionControlConfiguration();
            var loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

            var clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
            clientIdentifierProviderMock
                .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                .ReturnsAsync((string)null);

            async Task Next(HttpContext httpContext)
            {
                await Task.CompletedTask;

                isNextCalled = true;
            }

            var middleware = new ClientResolutionMiddleware(Next,
                configuration,
                new List<IClientIdentifierProvider> { clientIdentifierProviderMock.Object },
                loggerMock.Object);

            // When the middleware is invoked
            await middleware.Invoke(context);

            // Then it should execute next delegate in pipeline
            isNextCalled.Should().BeTrue();

            // And it should add client ID to http context items
            context.Items.GetClientId().Should().NotBeNull();
        }

        [Fact(DisplayName = "Disallow Anonymous Clients")]
        public async void DisallowAnonymousClients()
        {
            // Given
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            var configuration = new CongestionControlConfiguration { AllowAnonymousClients = false };
            var loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

            var clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
            clientIdentifierProviderMock
                .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                .ReturnsAsync((string)null);

            async Task Next(HttpContext httpContext)
            {
                await Task.CompletedTask;

                isNextCalled = true;
            }

            var middleware = new ClientResolutionMiddleware(Next,
                configuration,
                new List<IClientIdentifierProvider> { clientIdentifierProviderMock.Object },
                loggerMock.Object);

            // When the middleware is invoked
            await middleware.Invoke(context);

            // Then it should not execute next delegate in pipeline
            isNextCalled.Should().BeFalse();

            // And it should return unauthorized response
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "Client Identity Resolved")]
        public async void ClientIdentityResolved()
        {
            // Given
            var clientId = Guid.NewGuid().ToString();
            var isNextCalled = false;
            var context = new DefaultHttpContext();
            var configuration = new CongestionControlConfiguration();
            var loggerMock = new Mock<ILogger<ClientResolutionMiddleware>>();

            var clientIdentifierProviderMock = new Mock<IClientIdentifierProvider>();
            clientIdentifierProviderMock
                .Setup(mock => mock.ExecuteAsync(Moq.It.IsAny<HttpContext>()))
                .ReturnsAsync(clientId);

            async Task Next(HttpContext httpContext)
            {
                await Task.CompletedTask;

                isNextCalled = true;
            }

            var middleware = new ClientResolutionMiddleware(Next,
                configuration,
                new List<IClientIdentifierProvider> { clientIdentifierProviderMock.Object },
                loggerMock.Object);

            // When the middleware is invoked
            await middleware.Invoke(context);

            // Then it should execute next delegate in pipeline
            isNextCalled.Should().BeTrue();

            // And it should add client ID to http context items
            context.Items.GetClientId().Should().Be(clientId);
        }
    }
}
