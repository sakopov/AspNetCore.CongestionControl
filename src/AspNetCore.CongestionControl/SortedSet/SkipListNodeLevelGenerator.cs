// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkipListNodeLevelGenerator.cs">
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

namespace AspNetCore.CongestionControl.SortedSet
{
    using System;

    /// <summary>
    /// The random skip node level generator using William Pugh's original
    /// RandomLevel function. More information available @
    /// https://eugene-eeo.github.io/blog/skip-lists.html
    /// </summary>
    public class SkipListNodeLevelGenerator : ISkipListNodeLevelGenerator
    {
        /// <summary>
        /// The probability that any given node will have an l-level
        /// pointer with the lowest level being level 1. Currently,
        /// P = 1/4. 
        /// </summary>
        private const double SkipListProbability = 0.25;

        /// <summary>
        /// The randomizer.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Generates random skip list node level number using
        /// William Pugh's original RandomLevel function.
        /// </summary>
        /// <param name="maxLevel">
        /// The maximum number of levels allowed.
        /// </param>
        /// <returns>
        /// The random skip list node level number.
        /// </returns>
        public int Generate(int maxLevel)
        {
            var level = 1;

            while ((Random.Next() & 0xFFFF) < SkipListProbability * 0xFFFF)
            {
                level += 1;
            }

            return level < maxLevel ? level : maxLevel;
        }
    }
}