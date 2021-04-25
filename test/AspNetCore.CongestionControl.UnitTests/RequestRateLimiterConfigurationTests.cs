// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestRateLimiterConfigurationTests.cs">
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

    public class RequestRateLimiterConfigurationTests
    {
        [Fact(DisplayName = "Configuration Validated While Using Default Options")]
        public void ConfigurationValidatedWhileUsingDefaultOptions()
        {
            // Given
            var configuration = new RequestRateLimiterConfiguration();

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should successfully validate
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "Configuration Validated With Average Rate Set To Zero")]
        public void ConfigurationValidatedWithAverageRateSetToZero()
        {
            // Given
            var configuration = new RequestRateLimiterConfiguration
            {
                AverageRate = 0
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentOutOfRangeException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Contain("Average rate must be greater than 0.");
            ((ArgumentOutOfRangeException)exception).ParamName.Should().Be("AverageRate");
        }

        [Fact(DisplayName = "Configuration Validated With Interval Set To Zero")]
        public void ConfigurationValidatedWithInternvalSetToZero()
        {
            // Given
            var configuration = new RequestRateLimiterConfiguration
            {
                Interval = 0
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentOutOfRangeException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Contain("Interval must be greater than 0.");
            ((ArgumentOutOfRangeException)exception).ParamName.Should().Be("Interval");
        }

        [Fact(DisplayName = "Configuration Validated With Bursting Set To Zero")]
        public void ConfigurationValidatedWithBurstingSetToZero()
        {
            // Given
            var configuration = new RequestRateLimiterConfiguration
            {
                Bursting = 0
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentOutOfRangeException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Contain("Bursting must be greater than 0.");
            ((ArgumentOutOfRangeException)exception).ParamName.Should().Be("Bursting");
        }

        [Fact(DisplayName = "Configuration Validated With Keys Prefix Set To Null")]
        public void ConfigurationValidatedWithKeysPrefixSetToNull()
        {
            // Given
            var configuration = new RequestRateLimiterConfiguration
            {
                KeysPrefix = null
            };

            // When configuration is validated
            var exception = Record.Exception(() => configuration.Validate());

            // Then it should throw ArgumentNullException
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Contain("Keys prefix must be provided.");
            ((ArgumentNullException)exception).ParamName.Should().Be("KeysPrefix");
        }
    }
}
