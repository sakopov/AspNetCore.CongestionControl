// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentRequestLimiterConfigurationTests.cs">
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

    public class ConcurrentRequestLimiterConfigurationTests
    {
        [Fact(DisplayName = "Validate with Default Configuration")]
        public void ValidateWithDefaultConfiguration()
        {
            // Given
            var configuration = new ConcurrentRequestLimiterConfiguration();

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should successfully validate
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "Validate with Zero Capacity")]
        public void ValidateWithZeroCapacity()
        {
            // Given
            var configuration = new ConcurrentRequestLimiterConfiguration
            {
                Capacity = 0
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentOutOfRangeException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Contain("Capacity must be greater than 0.");
            ((ArgumentOutOfRangeException)exception).ParamName.Should().Be("Capacity");
        }

        [Fact(DisplayName = "Validate with Zero TTL")]
        public void ValidateWithZeroTimeToLive()
        {
            // Given
            var configuration = new ConcurrentRequestLimiterConfiguration
            {
                RequestTimeToLive = 0
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentOutOfRangeException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Contain("Request time-to-live must be greater than 0.");
            ((ArgumentOutOfRangeException)exception).ParamName.Should().Be("RequestTimeToLive");
        }

        [Fact(DisplayName = "Validate with Null Key Prefix")]
        public void ValidateWithNullKeyPrefix()
        {
            // Given
            var configuration = new ConcurrentRequestLimiterConfiguration
            {
                KeysPrefix = null
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // It should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Contain("Keys prefix must be provided.");
            ((ArgumentNullException)exception).ParamName.Should().Be("KeysPrefix");
        }
    }
}
