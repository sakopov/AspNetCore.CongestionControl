namespace AspNetCore.CongestionControl.Configuration
{
    /// <summary>
    /// This class implements configuration options for request rate limiter.
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

        /// <inheritdoc />
        internal override void Validate()
        {
            if (Interval <= 0)
            {
                throw new RequestRateLimiterConfigurationException(
                    "Interval must be greater than 0 for request rate limiter.");
            }

            if (AverageRate <= 0)
            {
                throw new RequestRateLimiterConfigurationException(
                    "Average rate must be greater than 0 for request rate limiter.");
            }

            if (Bursting <= 0)
            {
                throw new RequestRateLimiterConfigurationException(
                    "Bursting must be greater than 0 for request rate limiter.");
            }
        }
    }
}