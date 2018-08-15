namespace AspNetCore.CongestionControl.Configuration
{
    /// <summary>
    /// This class implements configuration options for concurrent requests limiter.
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

        /// <inheritdoc />
        internal override void Validate()
        {
            if (Capacity <= 0)
            {
                throw new ConcurrentRequestLimiterConfigurationException(
                    "Capacity must be greater than 0.");
            }

            if (RequestTimeToLive <= 0)
            {
                throw new ConcurrentRequestLimiterConfigurationException(
                    "Request time-to-live must be greater than 0.");
            }
        }
    }
}