// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsumeResult.cs">
//   Copyright (c) 2018 Sergey Akopov
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// The response object used by <see cref="ITokenBucketConsumer"/> to communicate
    /// whether the consumption of a token from token bucket was successful.
    /// </summary>
    public class ConsumeResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConsumeResult"/> class.
        /// </summary>
        /// <param name="isAllowed">
        /// The value indicating whether token consumption was allowed.
        /// </param>
        /// <param name="remaining">
        /// The remaining number of tokens left.
        /// </param>
        /// <param name="limit">
        /// The total number of tokens available.
        /// </param>
        public ConsumeResult(bool isAllowed, int remaining, int limit)
        {
            IsAllowed = isAllowed;
            Remaining = remaining;
            Limit = limit;
        }

        /// <summary>
        /// Gets the value indicating whether token consumption was allowed.
        /// </summary>
        public bool IsAllowed { get; }

        /// <summary>
        /// Gets the remaining number of tokens left.
        /// </summary>
        public int Remaining { get; }

        /// <summary>
        /// Gets the total number of tokens available.
        /// </summary>
        public int Limit { get; }
    }
}