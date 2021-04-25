// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs">
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
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Implements extension methods for the items dictionary in the <see cref="HttpContext" /> class.
    /// </summary>
    internal static class HttpContextItemsDictionaryExtensions
    {
        /// <summary>
        /// The name of the key in items dictionary used to store client identifier.
        /// </summary>
        private const string CongestionControlClientIdKey = "CongestionControlClientId";

        /// <summary>
        /// Adds client identifier to the items dictionary in the <see cref="HttpContext" /> class.
        /// </summary>
        /// <param name="items">
        /// The items dictionary.
        /// </param>
        /// <param name="clientId">
        /// The client identifier to add.
        /// </param>
        public static void AddClientId(this IDictionary<object, object> items, string clientId)
        {
            items.Add(CongestionControlClientIdKey, clientId);
        }

        /// <summary>
        /// Gets client identifier from the items dictionary in the <see cref="HttpContext" /> class.
        /// </summary>
        /// <param name="items">
        /// The items dictionary.
        /// </param>
        /// <returns>
        /// The client identifier.
        /// </returns>
        public static string GetClientId(this IDictionary<object, object> items)
        {
            if (items.TryGetValue(CongestionControlClientIdKey, out object value))
            {
                return (string)value;
            }

            return null;
        }
    }
}
