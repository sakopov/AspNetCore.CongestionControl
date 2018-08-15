using System;

namespace AspNetCore.CongestionControl.Configuration
{
    public class ConcurrentRequestLimiterConfigurationException : Exception
    {
        public ConcurrentRequestLimiterConfigurationException(string message) : base(message)
        {
        }

        public ConcurrentRequestLimiterConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}