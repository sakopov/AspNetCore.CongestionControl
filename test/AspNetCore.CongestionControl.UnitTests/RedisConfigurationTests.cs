// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConfigurationTests.cs">
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
    using Configuration;
    using FluentAssertions;
    using Xunit;

    public class RedisConfigurationTests
    {
        [Fact(DisplayName = "Redis Connection String Is Provided")]
        public void RedisConnectionStringIsProvided()
        {
            // When Redis configuration is instantiated with a connection string
            var configuration = new RedisConfiguration("127.0.0.1:6379");

            // Then it should set connection configuration options internally
            configuration.Options.Should().NotBeNull();
        }

        [Fact(DisplayName = "Redis Connection String Is Null")]
        public void RedisConnectionStringIsNull()
        {
            // When Redis configuration is instantiated without a connection string
            var exception = Record.Exception(() => new RedisConfiguration(null));

            // Then it should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
