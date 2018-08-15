using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements header-based client identifier provider.
    /// </summary>
    public class HeaderBasedClientIdentifierProvider : IClientIdentifierProvider
    {
        /// <summary>
        /// The name of the header containing client identifier.
        /// </summary>
        private readonly string _headerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderBasedClientIdentifierProvider"/> class.
        /// </summary>
        /// <param name="headerName">
        /// The name of the header containing client identifier.
        /// </param>
        public HeaderBasedClientIdentifierProvider(string headerName = "x-client-id")
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException(nameof(headerName));
            }

            _headerName = headerName;
        }

        /// <summary>
        /// Gets client identifier from headers.
        /// </summary>
        /// <param name="httpContext">
        /// The context for current HTTP request.
        /// </param>
        /// <returns>
        /// The client identifier.
        /// </returns>
        public Task<string> ExecuteAsync(HttpContext httpContext)
        {
            var clientId = "anonymous";

            if (httpContext?.Request?.Headers?.TryGetValue(_headerName, out var value) ?? false)
            {
                clientId = value.ToString();
            }

            return Task.FromResult(clientId);
        }
    }
}