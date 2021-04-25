// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConcurrentRequestsManager.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
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
    using System.Threading.Tasks;

    /// <summary>
    /// The contract for concurrent requests manager responsible for tracking
    /// the number of simultaneously-executing requests per client and identifying
    /// if a client exceeds the capacity.
    /// </summary>
    public interface IConcurrentRequestsManager
    {
        /// <summary>
        /// Adds a new client request to the manager, if it isn't exceeding
        /// the capacity.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client which initiated the request.
        /// </param>
        /// <param name="requestId">
        /// The request identifier.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp of the request.
        /// </param>
        /// <returns>
        /// The <see cref="AddConcurrentRequestResult"/> instance containing the response.
        /// </returns>
        Task<AddConcurrentRequestResult> AddAsync(string clientId, string requestId, long timestamp);

        /// <summary>
        /// Removes an existing request from the manager.
        /// </summary>
        /// <param name="clientId">
        /// The identifier of the client which initiated the request.
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
