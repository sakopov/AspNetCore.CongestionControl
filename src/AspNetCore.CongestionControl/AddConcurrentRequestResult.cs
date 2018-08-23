﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddConcurrentRequestResult.cs">
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
    /// The response object used to communicate whether the addition
    /// of a request to <see cref="IConcurrentRequestsManager"/> was
    /// successful.
    /// </summary>
    public class AddConcurrentRequestResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AddConcurrentRequestResult"/>
        /// class.
        /// </summary>
        /// <param name="isAllowed">
        /// The value indicating whether the request was allowed to be added
        /// </param>
        /// <param name="remaining">
        /// The remaining number of requests left.
        /// </param>
        /// <param name="limit">
        /// The total number of requests available.
        /// </param>
        public AddConcurrentRequestResult(bool isAllowed, int remaining, int limit)
        {
            IsAllowed = isAllowed;
            Remaining = remaining;
            Limit = limit;
        }

        /// <summary>
        /// Gets the value indicating whether the request wasadded.
        /// </summary>
        public bool IsAllowed { get; }

        /// <summary>
        /// Gets the remaining number of requests left.
        /// </summary>
        public int Remaining { get; }

        /// <summary>
        /// Gets the total number of requests available.
        /// </summary>
        public int Limit { get; }
    }
}