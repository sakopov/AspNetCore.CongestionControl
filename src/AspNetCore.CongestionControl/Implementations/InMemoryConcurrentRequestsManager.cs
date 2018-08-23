// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryConcurrentRequestsManager.cs">
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
    using Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The concurrent requests manager which uses memory for storage.
    /// </summary>
    public class InMemoryConcurrentRequestsManager : IConcurrentRequestsManager
    {
        /// <summary>
        /// The concurrent request limiter configuration options.
        /// </summary>
        private readonly ConcurrentRequestLimiterConfiguration _configuration;

        /// <summary>
        /// The map which associates client identifiers with sorted sets.
        /// </summary>
        private readonly IDictionary<string, SortedSet.SortedSet> _map = new Dictionary<string, SortedSet.SortedSet>();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<InMemoryConcurrentRequestsManager> _logger;

        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// Initializes a new instance of <see cref="InMemoryConcurrentRequestsManager"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The concurrent request limiter configuration.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public InMemoryConcurrentRequestsManager(
            ConcurrentRequestLimiterConfiguration configuration,
            ILogger<InMemoryConcurrentRequestsManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

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
        public Task<AddConcurrentRequestResult> AddAsync(string clientId, string requestId, long timestamp)
        {
            var ttl = _configuration.RequestTimeToLive;
            var capacity = _configuration.Capacity;
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            lock (_syncObject)
            {
                _logger.LogDebug("Received request {0} from client {1} with timestamp {2}.", 
                    requestId, clientId, timestamp);

                if (!_map.TryGetValue(key, out var sortedSet))
                {
                    sortedSet = new SortedSet.SortedSet();

                    _map.Add(key, sortedSet);
                }

                sortedSet.DeleteRangeByScore(double.NegativeInfinity, timestamp - ttl);

                var isAllowed = sortedSet.Length < capacity;

                if (isAllowed)
                {
                    _logger.LogInformation("Allowed request {0} from client {1} with timestamp {2}. The client has {3} request(s) remaining.", 
                        requestId, clientId, timestamp, capacity - sortedSet.Length);

                    sortedSet.Insert(timestamp, requestId);
                }
                else
                {
                    _logger.LogInformation("Rejected request {0} from client {1} with timestamp {2}. The client has {3} request(s) remaining.", 
                        requestId, clientId, timestamp, capacity - sortedSet.Length);
                }

                return Task.FromResult(new AddConcurrentRequestResult(
                    isAllowed: isAllowed, 
                    remaining: capacity - sortedSet.Length, 
                    limit: capacity
                ));
            }
        }

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
        public Task<bool> RemoveAsync(string clientId, string requestId)
        {
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            lock (_syncObject)
            {
                if (!_map.TryGetValue(key, out var sortedSet))
                {
                    return Task.FromResult(false);
                }

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var isRemoved = sortedSet.Delete(requestId);

                _logger.LogDebug(isRemoved
                    ? "Removed request {0} for client {1} at {2}."
                    : "Couldn't remove request {0} for client {1} at {2}.", 
                    requestId, clientId, timestamp);

                return Task.FromResult(isRemoved);
            }
        }
    }
}