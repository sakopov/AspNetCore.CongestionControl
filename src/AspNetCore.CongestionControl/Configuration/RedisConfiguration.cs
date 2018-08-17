using System;
using StackExchange.Redis;

namespace AspNetCore.CongestionControl.Configuration
{
    /// <summary>
    /// This class implements configuration options for Redis.
    /// </summary>
    public class RedisConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConfiguration"/> class.
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
        /// Gets Redis connection configuration options parsed from the connection string.
        /// </summary>
        internal ConfigurationOptions Options { get; }

        /// <inheritdoc />
        internal override void Validate()
        {
        }
    }
}