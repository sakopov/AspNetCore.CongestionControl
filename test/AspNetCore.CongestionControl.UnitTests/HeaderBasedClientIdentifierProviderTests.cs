// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderBasedClientIdentifierProviderTests.cs">
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

    public class HeaderBasedClientIdentifierProviderTests
    {
        [Fact(DisplayName = "Client Identifier Set Using Default Header")]
        public async void ClientIdentifierSetUsingDefaultHeader()
        {
            // Given
            const string HeaderName = "x-api-key";
            const string ClientId = "tester";

            var provider = new HeaderBasedClientIdentifierProvider();
            var context = new DefaultHttpContext();

            context.Request.Headers.Add(HeaderName, ClientId);

            // When header-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // Then it should return the expected client identifier
            result.Should().Be(ClientId);
        }

        [Fact(DisplayName = "Client Identifier Set Using Custom Header")]
        public async void ClientIdentifierSetUsingCustomHeader()
        {
            // Given
            const string HeaderName = "my-custom-header";
            const string ClientId = "tester";

            var provider = new HeaderBasedClientIdentifierProvider(HeaderName);
            var context = new DefaultHttpContext();

            context.Request.Headers.Add(HeaderName, ClientId);

            // When header-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // Then it should return the expected client identifier
            result.Should().Be(ClientId);
        }

        [Fact(DisplayName = "No Client Identifier in Headers")]
        public async void NoClientIdentifierInHeaders()
        {
            // Given
            var context = new DefaultHttpContext();
            var provider = new HeaderBasedClientIdentifierProvider();

            // When header-based client identifier provider is executed
            var result = await provider.ExecuteAsync(context);

            // Then it should return null identifier
            result.Should().BeNull();
        }

        [Fact(DisplayName = "Header Name Not Provided")]
        public void HeaderNameNotProvided()
        {
            // Given
            var context = new DefaultHttpContext();

            // When header-based client identifier provider is executed
            var exception = Record.Exception(() => new HeaderBasedClientIdentifierProvider(null));

            // It should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
