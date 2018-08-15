using System;

namespace AspNetCore.CongestionControl.Configuration
{
    public class RequestRateLimiterConfigurationException : Exception
    {
        public RequestRateLimiterConfigurationException(string message) : base(message)
        {
        }

        public RequestRateLimiterConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
