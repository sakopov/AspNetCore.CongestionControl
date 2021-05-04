// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs">
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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AspNetCore.CongestionControl.UnitTests")]

namespace AspNetCore.CongestionControl
{
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The extension methods for <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default congestion control components.
        /// </summary>
        /// <param name="services">
        /// The services collection.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddCongestionControl(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddCongestionControl(services, options =>
            {
                options.AddRequestRateLimiter();
            });

            return services;
        }

        /// <summary>
        /// Adds congestion control components.
        /// </summary>
        /// <param name="services">
        /// The services collection.
        /// </param>
        /// <param name="configure">
        /// The congestion control configuration options.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddCongestionControl(this IServiceCollection services, Action<CongestionControlConfiguration> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new CongestionControlConfiguration();

            configure(options);

            // Wire-up client identifier providers
            if (!options.ClientIdentifierProviders.Any())
            {
                options.ClientIdentifierProviders.Add(new HeaderBasedClientIdentifierProvider());

                services.AddSingleton<IClientIdentifierProvider, HeaderBasedClientIdentifierProvider>();
            }
            else
            {
                foreach (var provider in options.ClientIdentifierProviders)
                {
                    services.AddSingleton<IClientIdentifierProvider>(provider);
                }
            }

            // Wire-up rate limit response formatter
            if (options.HttpResponseFormatter == null)
            {
                options.HttpResponseFormatter = new DefaultHttpResponseFormatter();

                services.AddSingleton<IHttpResponseFormatter, DefaultHttpResponseFormatter>();
            }
            else
            {
                services.AddSingleton(provider => options.HttpResponseFormatter);
            }

            // Wire-up configurations and rate limiters
            services.AddSingleton(provider => options);

            if (options.RequestRateLimiterConfiguration != null)
            {
                services.AddSingleton(provider => options.RequestRateLimiterConfiguration);
                services.AddTransient<ITokenBucketConsumer, InMemoryTokenBucketConsumer>();
            }

            if (options.ConcurrentRequestLimiterConfiguration != null)
            {
                services.AddSingleton(provider => options.ConcurrentRequestLimiterConfiguration);
                services.AddTransient<IConcurrentRequestsManager, InMemoryConcurrentRequestsManager>();
            }

            if (options.RedisConfiguration != null)
            {
                services.AddSingleton(provider => options.RedisConfiguration);
                services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(options.RedisConfiguration.Options));

                if (options.RequestRateLimiterConfiguration != null)
                {
                    services.AddTransient<ITokenBucketConsumer, RedisTokenBucketConsumer>();
                }

                if (options.ConcurrentRequestLimiterConfiguration != null)
                {
                    services.AddTransient<IConcurrentRequestsManager, RedisConcurrentRequestsManager>();
                }
            }

            return services;
        }
    }
}
