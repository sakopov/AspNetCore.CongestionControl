// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestRateLimiterConfiguration.cs">
//   Copyright (c) 2018 Sergey Akopov
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

namespace AspNetCore.CongestionControl.Configuration
{
    using System;

    /// <summary>
    /// The request rate limiter configuration.
    /// </summary>
    public class RequestRateLimiterConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Gets or sets the length of the time unit in seconds. For example, if a client is allowed
        /// 200 requests per hour, the <see cref="Interval"/> would be set to 3600 seconds (1 hour).
        /// The default value is 1 second.
        /// </summary>
        public int Interval { get; set; } = 1;

        /// <summary>
        /// Gets or sets how many requests per <see cref="Interval"/> a client is allowed to perform.
        /// The default value is 100 requests.
        /// </summary>
        public int AverageRate { get; set; } = 100;

        /// <summary>
        /// Gets or sets the amount of bursting to allow. The default value is 5.
        /// </summary>
        public int Bursting { get; set; } = 5;

        /// <summary>
        /// Gets or sets the prefix that will be used for keys in underlying storage
        /// data structures. The default value is "request_rate_limiter".
        /// </summary>
        public string KeysPrefix { get; set; } = "request_rate_limiter";

        /// <summary>
        /// Validates configuration options.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal override void Validate()
        {
            if (Interval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Interval), 
                    "Interval must be greater than 0.");
            }

            if (AverageRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(AverageRate),
                    "Average rate must be greater than 0.");
            }

            if (Bursting <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Bursting),
                    "Bursting must be greater than 0.");
            }

            if (string.IsNullOrEmpty(KeysPrefix))
            {
                throw new ArgumentNullException(nameof(KeysPrefix),
                    "Keys prefix must be provided.");
            }
        }
    }
}