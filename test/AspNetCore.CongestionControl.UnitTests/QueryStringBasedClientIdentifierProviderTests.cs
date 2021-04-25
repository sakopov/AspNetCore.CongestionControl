// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryStringBasedClientIdentifierProviderTests.cs">
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
    using Microsoft.AspNetCore.Http;
    using FluentAssertions;
    using Xunit;

    public class QueryStringBasedClientIdentifierProviderTests
    {
        [Fact(DisplayName = "Client Identifier Set Using Default Query String Parameter")]
        public async void ClientIdentifierSetUsingDefaultQueryStringParameter()
        {
            // Given
            const string QueryStringParameter = "api_key";
            const string ClientId = "tester";

            var context = new DefaultHttpContext();
            var provider = new QueryBasedClientIdentifierProvider();

            context.Request.QueryString = new QueryString($"?{QueryStringParameter}={ClientId}");

            // When the query-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // It should return the expected client identifier
            result.Should().Be(ClientId);
        }

        [Fact(DisplayName = "Client Identifier Set Using Custom Query String Parameter")]
        public async void ClientIdentifierSetUsingCustomQueryStringParameter()
        {
            // Given
            const string QueryStringParameter = "my-custom-parameter";
            const string ClientId = "tester";

            var provider = new QueryBasedClientIdentifierProvider(QueryStringParameter);
            var context = new DefaultHttpContext();

            context.Request.QueryString = new QueryString($"?{QueryStringParameter}={ClientId}");

            // When the query-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // Then it should return the expected client identifier
            result.Should().Be(ClientId);
        }

        [Fact(DisplayName = "Client Identifier Not Set in Query String")]
        public async void ClientIdentifierNotSetInQueryString()
        {
            // Given
            var context = new DefaultHttpContext();
            var provider = new QueryBasedClientIdentifierProvider();

            // When query-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // Then it should return null for client identifier
            result.Should().BeNull();
        }

        [Fact(DisplayName = "Custom Query String Parameter is Not Provided")]
        public void CustomQueryStringParameterIsNotProvided()
        {
            // Given
            var context = new DefaultHttpContext();

            // When a new instance of the query-based client identifier provider is created
            var exception = Record.Exception(() => new QueryBasedClientIdentifierProvider(null));

            // It should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
