using System;

namespace AspNetCore.CongestionControl.Configuration
{
    /// <summary>
    /// This class implements configuration options for all congestion control
    /// middlewares.
    /// </summary>
    public class CongestionControlConfiguration
    {
        /// <summary>
        /// Gets the Redis configuration.
        /// </summary>
        internal RedisServerConfiguration RedisServerConfiguration { get; private set; }

        /// <summary>
        /// Gets the request rate limiter configuration.
        /// </summary>
        internal RequestRateLimiterConfiguration RequestRateLimiterConfiguration { get; private set; }

        /// <summary>
        /// Gets the concurrent request limiter configuration.
        /// </summary>
        internal ConcurrentRequestLimiterConfiguration ConcurrentRequestLimiterConfiguration { get; private set; }
        
        /// <summary>
        /// Gets the client identifier provider.
        /// </summary>
        internal IClientIdentifierProvider ClientIdentifierProvider { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP status code to return to the client when their
        /// request is rate limited. The default value is HTTP 429/Too Many Requests.
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        /// <summary>
        /// Adds request rate limiter using default configuration options.
        /// </summary>
        public void AddRequestRateLimiter()
        {
            var config = new RequestRateLimiterConfiguration();

            config.Validate();

            RequestRateLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds request rate limiter using the specified configuration.
        /// </summary>
        /// <param name="configure">
        /// The delegate to configure request rate limiter.
        /// </param>
        public void AddRequestRateLimiter(Action<RequestRateLimiterConfiguration> configure)
        {
            var config = new RequestRateLimiterConfiguration();

            configure(config);

            config.Validate();

            RequestRateLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds concurrent request limiter using default configuration options.
        /// </summary>
        public void AddConcurrentRequestLimiter()
        {
            var config = new ConcurrentRequestLimiterConfiguration();

            config.Validate();

            ConcurrentRequestLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds concurrent request limiter using the specified configuration.
        /// </summary>
        /// <param name="configure">
        /// The delegate to configure concurrent request limiter.
        /// </param>
        public void AddConcurrentRequestLimiter(Action<ConcurrentRequestLimiterConfiguration> configure)
        {
            var config = new ConcurrentRequestLimiterConfiguration();

            configure(config);

            config.Validate();

            ConcurrentRequestLimiterConfiguration = config;
        }

        /// <summary>
        /// Adds Redis as distributed storage.
        /// </summary>
        /// <param name="connection">
        /// The connection string for the Redis server.
        /// </param>
        public void AddRedisStorage(string connection)
        {
            var config = new RedisServerConfiguration(connection);

            config.Validate();

            RedisServerConfiguration = config;
        }

        /// <summary>
        /// Adds header-based client identifier provider using the specified
        /// header name to retrieve unique client identifier.
        /// </summary>
        /// <param name="headerName">
        /// The name of the header containing client identifier.
        /// </param>
        public void AddHeaderBasedClientIdentifierProvider(string headerName)
        {
            ClientIdentifierProvider = new HeaderBasedClientIdentifierProvider(headerName);
        }

        /// <summary>
        /// Adds header-based client identifier provider using "x-client-id" header
        /// name to retrieve unique client identifier.
        /// </summary>
        public void AddHeaderBasedClientIdentifierProvider()
        {
            ClientIdentifierProvider = new HeaderBasedClientIdentifierProvider();
        }

        /// <summary>
        /// Adds custom client identifier provider.
        /// </summary>
        /// <param name="provider">
        /// The implementation of <see cref="IClientIdentifierProvider"/> interface.
        /// </param>
        public void AddClientIdentifierProvider(IClientIdentifierProvider provider)
        {
            ClientIdentifierProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
    }
}