// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkipListNode.cs">
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

namespace AspNetCore.CongestionControl.SortedSet
{
    using System.Collections.Generic;

    /// <summary>
    /// The skip list node.
    /// </summary>
    /// <remarks>
    /// This probably should use some IEquatable or IComparable and perhaps some operator
    /// overrides to make node comparisons easier in the SkipList class.
    /// </remarks>
    public class SkipListNode
    {
        /// <summary>
        /// Gets or sets the data element.
        /// </summary>
        public string Element { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the node linked at the back.
        /// </summary>
        public SkipListNode Backward { get; set; }

        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        public List<SkipListNodeLevel> Levels { get; set; } = new List<SkipListNodeLevel>();
    }
}
