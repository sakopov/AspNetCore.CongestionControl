using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.CongestionControl.Configuration;
using Microsoft.Extensions.Logging;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements concurrent requests tracker which uses memory for
    /// storage.
    /// </summary>
    public class InMemoryConcurrentRequestsTracker : IConcurrentRequestsTracker
    {
        /// <summary>
        /// The concurrent request limiter configuration options.
        /// </summary>
        private readonly ConcurrentRequestLimiterConfiguration _configuration;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<InMemoryConcurrentRequestsTracker> _logger;

        /// <summary>
        /// The map which associates client identifiers with sorted sets.
        /// </summary>
        private readonly IDictionary<string, SortedSet.SortedSet> _map = new Dictionary<string, SortedSet.SortedSet>();

        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryConcurrentRequestsTracker"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The concurrent request limiter configuration options.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public InMemoryConcurrentRequestsTracker(
            ConcurrentRequestLimiterConfiguration configuration,
            ILogger<InMemoryConcurrentRequestsTracker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

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
        public Task<bool> AddAsync(string clientId, string requestId, long timestamp)
        {
            var ttl = _configuration.RequestTimeToLive;
            var capacity = _configuration.Capacity;
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            lock (_syncObject)
            {
                if (!_map.TryGetValue(key, out var sortedSet))
                {
                    sortedSet = new SortedSet.SortedSet();

                    _map.Add(key, sortedSet);
                }

                sortedSet.DeleteRangeByScore(double.NegativeInfinity, timestamp - ttl);

                var isAllowed = sortedSet.Length < capacity;

                if (isAllowed)
                {
                    sortedSet.Insert(timestamp, requestId);
                }

                return Task.FromResult(isAllowed);
            }
        }

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
        public Task<bool> RemoveAsync(string clientId, string requestId)
        {
            var key = $"{_configuration.KeysPrefix}.{clientId}";

            lock (_syncObject)
            {
                if (!_map.TryGetValue(key, out var sortedSet))
                {
                    return Task.FromResult(false);
                }

                var isDeleted = sortedSet.Delete(requestId);

                return Task.FromResult(isDeleted);
            }
        }
    }
}