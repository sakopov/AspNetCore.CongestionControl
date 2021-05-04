// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryStringBasedClientIdentifierProvider.cs">
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
    using System;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The client identifier provider which looks at the query
    /// string in the incoming request to get client api key/identifier.
    /// </summary>
    public class QueryBasedClientIdentifierProvider : IClientIdentifierProvider
    {
        /// <summary>
        /// The default name of the query string parameter that contains client api key/identifier.
        /// </summary>
        private const string DefaultQueryStringParameter = "api_key";

        /// <summary>
        /// The name of the query string parameter containing client api key/identifier.
        /// </summary>
        private readonly string _queryStringParameter;

        /// <summary>
        /// Initializes a new instance of <see cref="QueryBasedClientIdentifierProvider"/> class.
        /// </summary>
        /// <param name="queryStringParameter">
        /// The name of the query string parameter containing client api key/identifier.
        /// </param>
        public QueryBasedClientIdentifierProvider(string queryStringParameter = DefaultQueryStringParameter)
        {
            if (string.IsNullOrEmpty(queryStringParameter))
            {
                throw new ArgumentNullException(nameof(queryStringParameter));
            }

            _queryStringParameter = queryStringParameter;
        }

        /// <summary>
        /// Retrieves client api key/identifier from query string.
        /// </summary>
        /// <param name="httpContext">
        /// The context for current HTTP request.
        /// </param>
        /// <returns>
        /// The client api key/identifier.
        /// </returns>
        public Task<string> ExecuteAsync(HttpContext httpContext)
        {
            string clientId = null;

            if (httpContext?.Request?.Query?.TryGetValue(_queryStringParameter, out var value) ?? false)
            {
                clientId = value.ToString();
            }

            return Task.FromResult(clientId);
        }
    }
}
