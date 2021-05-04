// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultHttpResponseFormatterTests.cs">
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
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using FluentAssertions;
    using Xunit;

    public class DefaultHttpResponseFormatterTests
    {
        [Fact(DisplayName = "Happy Path")]
        public async void HappyPath()
        {
            // Given
            var formatter = new DefaultHttpResponseFormatter();
            var rateLimitContext = new RateLimitContext(10, 20, HttpStatusCode.ServiceUnavailable, "source");

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/json";

            // When formatter is executed
            await formatter.FormatAsync(httpContext, rateLimitContext);

            // Then it should set the response content type to request content type
            httpContext.Response.ContentType.Should().Be(httpContext.Request.ContentType);

            // And it should set the response status code to configured HTTP status code
            httpContext.Response.StatusCode.Should().Be((int)rateLimitContext.HttpStatusCode);
        }
    }
}
