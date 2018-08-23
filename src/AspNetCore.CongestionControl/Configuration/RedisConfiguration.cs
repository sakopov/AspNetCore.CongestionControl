// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisConfiguration.cs">
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
    using StackExchange.Redis;

    /// <summary>
    /// The Redis configuration.
    /// </summary>
    public class RedisConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RedisConfiguration"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection string to the Redis server.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedisConfiguration(string connection)
        {
            if (string.IsNullOrEmpty(connection))
            {
                throw new ArgumentNullException(nameof(connection), "Redis connection string is required.");
            }

            Options = ConfigurationOptions.Parse(connection);
        }

        /// <summary>
        /// Gets the Redis connection configuration options parsed from the connection string.
        /// </summary>
        internal ConfigurationOptions Options { get; }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        internal override void Validate()
        {
        }
    }
}