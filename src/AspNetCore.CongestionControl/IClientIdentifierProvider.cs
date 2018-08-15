using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This interface defines the contract for client identifier providers
    /// responsible for extracting unique client identifier from client requests.
    /// </summary>
    public interface IClientIdentifierProvider
    {
        /// <summary>
        /// Executes the client identifier provider.
        /// </summary>
        /// <param name="httpContext">
        /// The context for current HTTP request.
        /// </param>
        /// <returns>
        /// The client identifier.
        /// </returns>
        Task<string> ExecuteAsync(HttpContext httpContext);
    }
}