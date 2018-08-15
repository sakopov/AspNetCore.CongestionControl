using Microsoft.AspNetCore.Builder;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements extensions for <see cref="IApplicationBuilder"/> interface.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds request rate limiter middleware.
        /// </summary>
        /// <param name="builder">
        /// The application builder.
        /// </param>
        /// <returns>
        /// The application builder.
        /// </returns>
        public static IApplicationBuilder UseRequestRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestRateLimiterMiddleware>();
        }

        /// <summary>
        /// Adds concurrent requests limiter middleware.
        /// </summary>
        /// <param name="builder">
        /// The application builder.
        /// </param>
        /// <returns>
        /// The application builder.
        /// </returns>
        public static IApplicationBuilder UseConcurrentRequestsLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ConcurrentRequestLimiterMiddleware>();
        }
    }
}