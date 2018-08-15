namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements token consumption response.
    /// </summary>
    public class TokenConsumeResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConsumeResponse"/> class.
        /// </summary>
        /// <param name="isAllowed">
        /// The the value indicating whether token reduction was allowed.
        /// </param>
        /// <param name="tokensLeft">
        /// The remaining number of tokens.
        /// </param>
        public TokenConsumeResponse(bool isAllowed, int tokensLeft)
        {
            IsAllowed = isAllowed;
            TokensLeft = tokensLeft;
        }

        /// <summary>
        /// Gets the value indicating whether token reduction was allowed.
        /// </summary>
        public bool IsAllowed { get; }

        /// <summary>
        /// Gets the remaining number of tokens.
        /// </summary>
        public int TokensLeft { get; }

        /// <summary>
        /// Returns non-conforming response.
        /// </summary>
        /// <param name="tokensLeft">
        /// The remaining number of tokens.
        /// </param>
        /// <returns>
        /// The non-conforming token reduction response.
        /// </returns>
        public static TokenConsumeResponse NonConforming(int tokensLeft)
        {
            return new TokenConsumeResponse(false, tokensLeft);
        }

        /// <summary>
        /// Returns confirming response.
        /// </summary>
        /// <param name="tokensLeft">
        /// The remaining number of tokens.
        /// </param>
        /// <returns>
        /// The conforming token reduction response.
        /// </returns>
        public static TokenConsumeResponse Conforming(int tokensLeft)
        {
            return new TokenConsumeResponse(true, tokensLeft);
        }
    }
}