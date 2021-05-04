// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpContextItemsDictionaryExtensionsTests.cs">
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
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class HttpContextItemsDictionaryExtensionsTests
    {
        [Fact(DisplayName = "Adding Client ID")]
        public void AddingClientId()
        {
            // Given
            var items = new Dictionary<object, object>();

            // When client ID is added
            items.AddClientId("client-id");

            // Then it can be retrieved
            items.GetClientId().Should().Be("client-id");
        }

        [Fact(DisplayName = "Client ID Does Not Exist")]
        public void ClientIdDoesNotExist()
        {
            // Given
            var items = new Dictionary<object, object>();

            // When client ID is retrieved
            var clientId = items.GetClientId();

            // Then it should return null
            clientId.Should().BeNull();
        }
    }
}
