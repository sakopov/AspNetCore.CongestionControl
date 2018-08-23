// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs">
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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AspNetCore.CongestionControl.UnitTests")]

namespace AspNetCore.CongestionControl
{
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;
    using System;

    /// <summary>
    /// The extension methods for <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds congestion control components to <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IServiceCollection"/>.
        /// </param>
        /// <param name="configure">
        /// The congestion control configuration options.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddCongestionControl(this IServiceCollection collection, Action<CongestionControlConfiguration> configure)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new CongestionControlConfiguration();

            configure(options);
            
            // Wire-up client identifier provider
            if (options.ClientIdentifierProvider == null)
            {
                options.ClientIdentifierProvider = new HeaderBasedClientIdentifierProvider();

                collection.AddSingleton<IClientIdentifierProvider, HeaderBasedClientIdentifierProvider>();
            }
            else
            {
                collection.AddSingleton(provider => options.ClientIdentifierProvider);
            }

            // Wire-up rate limit response formatter
            if (options.HttpResponseFormatter == null)
            {
                options.HttpResponseFormatter = new DefaultHttpResponseFormatter();

                collection.AddSingleton<IHttpResponseFormatter, DefaultHttpResponseFormatter>();
            }
            else
            {
                collection.AddSingleton(provider => options.HttpResponseFormatter);
            }

            // Wire-up configurations and rate limiters
            collection.AddSingleton(provider => options);

            if (options.RequestRateLimiterConfiguration != null)
            {
                collection.AddSingleton(provider => options.RequestRateLimiterConfiguration);
            }

            if (options.ConcurrentRequestLimiterConfiguration != null)
            {
                collection.AddSingleton(provider => options.ConcurrentRequestLimiterConfiguration);
            }

            if (options.RedisConfiguration != null)
            {
                collection.AddSingleton(provider => options.RedisConfiguration);
                collection.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(options.RedisConfiguration.Options));
                collection.AddTransient<IConcurrentRequestsManager, RedisConcurrentRequestsManager>();
                collection.AddTransient<ITokenBucketConsumer, RedisTokenBucketConsumer>();
            }
            else
            {
                collection.AddTransient<IConcurrentRequestsManager, InMemoryConcurrentRequestsManager>();
                collection.AddTransient<ITokenBucketConsumer, InMemoryTokenBucketConsumer>();
            }

            return collection;
        }
    }
}