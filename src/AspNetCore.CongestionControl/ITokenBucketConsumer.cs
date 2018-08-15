using System.Threading.Tasks;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This interface defines the contract for token bucket consumer responsible
    /// for requesting and draining tokens from the token bucket.
    /// </summary>
    public interface ITokenBucketConsumer
    {
        /// <summary>
        /// Consumes tokens by the requested number for the specified client.
        /// </summary>
        /// <param name="clientId">
        /// The client identifier to consume tokens for.
        /// </param>
        /// <param name="requested">
        /// The requested number of tokens to consume.
        /// </param>
        /// <returns>
        /// The token consumption response.
        /// </returns>
        Task<TokenConsumeResponse> ConsumeAsync(string clientId, int requested);
    }
}