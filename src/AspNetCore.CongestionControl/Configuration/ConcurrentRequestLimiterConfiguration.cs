// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentRequestLimiterConfiguration.cs">
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
    /// The concurrent requests limiter configuration.
    /// </summary>
    public class ConcurrentRequestLimiterConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Gets or sets the number of requests a client can execute at the same time.
        /// The default value is 100.
        /// </summary>
        public int Capacity { get; set; } = 100;

        /// <summary>
        /// Get or sets the amount of time in seconds a client request can take.
        /// The default value is 60.
        /// </summary>
        public int RequestTimeToLive { get; set; } = 60;

        /// <summary>
        /// Gets or sets the prefix that will be used for keys in underlying storage
        /// data structures. The default value is "concurrent_request_limiter".
        /// </summary>
        public string KeysPrefix { get; set; } = "concurrent_request_limiter";

        /// <summary>
        /// Validates configuration options.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal override void Validate()
        {
            if (Capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Capacity), 
                    "Capacity must be greater than 0.");
            }

            if (RequestTimeToLive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(RequestTimeToLive), 
                    "Request time-to-live must be greater than 0.");
            }

            if (string.IsNullOrEmpty(KeysPrefix))
            {
                throw new ArgumentNullException(nameof(KeysPrefix), 
                    "Keys prefix must be provided.");
            }
        }
    }
}