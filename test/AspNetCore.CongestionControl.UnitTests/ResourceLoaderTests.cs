// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceLoaderTests.cs">
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
    using FluentAssertions;
    using Xunit;

    public class ResourceLoaderTests
    {
        [Fact(DisplayName = "Loading Script From Assembly")]
        public async void LoadingScriptFromAssembly()
        {
            // Given
            const string ResourceName = "request_rate_limiter.lua";

            // When loading script
            var resourceBody = await ResourceLoader.GetResourceAsync(ResourceName);

            // Then it should load the resource
            resourceBody.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "Loading Non-Existing Resource From Assembly")]
        public async void LoadingNonExistingResourceFromAssembly()
        {
            // Given
            const string ResourceName = "does_not_exist.lua";

            // When loading non-existing resource
            var exception = await Record.ExceptionAsync(() => ResourceLoader.GetResourceAsync(ResourceName));

            // Then it should throw exception
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be($"The resource \"{ResourceName}\" does not exist.");
        }
    }
}
