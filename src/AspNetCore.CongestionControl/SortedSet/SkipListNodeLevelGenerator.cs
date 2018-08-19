using System;

namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements random skip node level generator using
    /// William Pugh's original RandomLevel function. More information
    /// available @ https://eugene-eeo.github.io/blog/skip-lists.html
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