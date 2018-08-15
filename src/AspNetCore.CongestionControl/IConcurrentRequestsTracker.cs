using System.Threading.Tasks;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This interface defines the contract for concurrent requests tracker, which
    /// is responsible for tracking the number of simultaneously-executing requests
    /// per client and identifying if a client is exceeding the capacity.
    /// </summary>
    public interface IConcurrentRequestsTracker
    {
        /// <summary>
        /// Adds a request identifier at the specified timestamp to the
        /// tracker.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client who initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp of the request.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request was added. Otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddAsync(string clientId, string requestId, long timestamp);

        /// <summary>
        /// Removes tracked request identifier.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client who initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request was removed. Otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveAsync(string clientId, string requestId);
    }
}