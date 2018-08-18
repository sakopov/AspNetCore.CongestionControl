using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements middleware responsible for enforcing rate limiting
    /// for each client request.
    /// </summary>
    public class RequestRateLimiterMiddleware
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
        /// The token bucket consumer implementation.
        /// </summary>
        private readonly ITokenBucketConsumer _tokenBucketConsumer;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<RequestRateLimiterMiddleware> _logger;

        /// <summary>
        /// Initializes new instances of the <see cref="RequestRateLimiterMiddleware"/> class.
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
        /// <param name="tokenBucketConsumer">
        /// The token bucket consumer.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RequestRateLimiterMiddleware(
            RequestDelegate next,
            CongestionControlConfiguration configuration,
            IClientIdentifierProvider clientIdentifierProvider,
            ITokenBucketConsumer tokenBucketConsumer,
            ILogger<RequestRateLimiterMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _clientIdentifierProvider = clientIdentifierProvider;
            _tokenBucketConsumer = tokenBucketConsumer;
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

            var response = await _tokenBucketConsumer.ConsumeAsync(clientId, 1);

            if (!response.IsAllowed)
            {
                _logger.LogInformation("The request for client {0} was not allowed because the client has exceeded request rate limiter capacity.", clientId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = _configuration.HttpStatusCode;

                return;
            }

            await _next(context);
        }
    }
}