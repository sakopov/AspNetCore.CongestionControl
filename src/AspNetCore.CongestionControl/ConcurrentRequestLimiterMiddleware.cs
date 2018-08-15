using System;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements middleware responsible for enforcing rate
    /// limiting for the number of simultaneously-executing requests for
    /// each client.
    /// </summary>
    public class ConcurrentRequestLimiterMiddleware
    {
        /// <summary>
        /// The next item in the middleware pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The congestion control configuration options.
        /// </summary>
        private readonly CongestionControlConfiguration _configuration;

        /// <summary>
        /// The client identifier provider implementation.
        /// </summary>
        private readonly IClientIdentifierProvider _clientIdentifierProvider;

        /// <summary>
        /// The concurrent requests tracker implementation.
        /// </summary>
        private readonly IConcurrentRequestsTracker _concurrentRequestsTracker;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<ConcurrentRequestLimiterMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentRequestLimiterMiddleware"/>
        /// class.
        /// </summary>
        /// <param name="next">
        /// The next item in the middleware pipeline.
        /// </param>
        /// <param name="configuration">
        /// The congestion control configuration options.
        /// </param>
        /// <param name="clientIdentifierProvider">
        /// The client identifier provider.
        /// </param>
        /// <param name="concurrentRequestsTracker">
        /// The concurrent request tracker.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ConcurrentRequestLimiterMiddleware(
            RequestDelegate next,
            CongestionControlConfiguration configuration,
            IClientIdentifierProvider clientIdentifierProvider,
            IConcurrentRequestsTracker concurrentRequestsTracker,
            ILogger<ConcurrentRequestLimiterMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _clientIdentifierProvider = clientIdentifierProvider;
            _concurrentRequestsTracker = concurrentRequestsTracker;
            _logger = logger;
        }

        /// <summary>
        /// Begins middleware execution.
        /// </summary>
        /// <param name="context">
        /// The context for the active HTTP request.
        /// </param>
        /// <returns>
        /// The next task in the middleware pipeline if the request is allowed; Otherwise,
        /// terminates the middleware pipeline execution and returns unsuccessful response.
        /// </returns>
        public async Task Invoke(HttpContext context)
        {
            var clientId = await _clientIdentifierProvider.ExecuteAsync(context);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var requestId = Guid.NewGuid().ToString("N");

            var isAllowed = await _concurrentRequestsTracker.AddAsync(clientId, requestId, timestamp);

            if (!isAllowed)
            {
                _logger.LogInformation("The request \"{0}\" for client \"{1}\" was not allowed because the client has exceeded concurrent request limiter capacity.", requestId, clientId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = _configuration.HttpStatusCode;

                return;
            }

            await _next(context);

            if (string.IsNullOrEmpty(requestId))
            {
                return;
            }

            await _concurrentRequestsTracker.RemoveAsync(clientId, requestId);
        }
    }
}