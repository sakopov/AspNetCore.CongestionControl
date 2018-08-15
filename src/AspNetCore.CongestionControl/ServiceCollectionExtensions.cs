using System;
using System.Runtime.CompilerServices;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

[assembly: InternalsVisibleTo("AspNetCore.CongestionControl.UnitTests")]

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements extension methods for <see cref="IServiceCollection"/>
    /// interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds congestion control components to the service collection.
        /// </summary>
        /// <param name="collection">
        /// The service collection.
        /// </param>
        /// <param name="configure">
        /// The congestion control configuration options.
        /// </param>
        /// <returns>
        /// The service collection.
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
            
            if (options.ClientIdentifierProvider == null)
            {
                options.AddHeaderBasedClientIdentifierProvider();
            }

            collection.AddSingleton(provider => options);
            collection.AddSingleton(provider => options.ClientIdentifierProvider);

            if (options.RequestRateLimiterConfiguration != null)
            {
                collection.AddSingleton(provider => options.RequestRateLimiterConfiguration);
            }

            if (options.ConcurrentRequestLimiterConfiguration != null)
            {
                collection.AddSingleton(provider => options.ConcurrentRequestLimiterConfiguration);
            }

            if (options.RedisServerConfiguration != null)
            {
                collection.AddSingleton(provider => options.RedisServerConfiguration);
                collection.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(options.RedisServerConfiguration.Options));
                collection.AddTransient<IConcurrentRequestsTracker, RedisConcurrentRequestsTracker>();
                collection.AddTransient<ITokenBucketConsumer, RedisTokenBucketConsumer>();
            }
            else
            {
                collection.AddTransient<IConcurrentRequestsTracker, InMemoryConcurrentRequestsTracker>();
                collection.AddTransient<ITokenBucketConsumer, InMemoryTokenBucketConsumer>();
            }

            return collection;
        }
    }
}