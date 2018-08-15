using System.Collections.Generic;

namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements a sorted set.
    /// </summary>
    public class SortedSet
    {
        /// <summary>
        /// The dictionary which maps data elements to scores.
        /// </summary>
        private readonly IDictionary<string, double> _dictionary = new Dictionary<string, double>();

        /// <summary>
        /// The skip list.
        /// </summary>
        private readonly SkipList _skipList = new SkipList();

        /// <summary>
        /// The lock sync object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// Gets the total number of items in the set.
        /// </summary>
        public long Length
        {
            get
            {
                lock (_syncObject)
                {
                    return _skipList.Length;
                }
            }
        }

        /// <summary>
        /// Inserts new element with the corresponding score in the set.
        /// </summary>
        /// <param name="score">
        /// The score.
        /// </param>
        /// <param name="element">
        /// The data element.
        /// </param>
        /// <returns>
        /// <c>true</c> if the insert succeeded; Otherwise, <c>false</c>.
        /// </returns>
        public bool Insert(double score, string element)
        {
            lock (_syncObject)
            {
                if (_dictionary.ContainsKey(element))
                {
                    return false;
                }

                var isInserted = _skipList.Insert(score, element) != null;

                if (isInserted)
                {
                    _dictionary.Add(element, score);
                }

                return isInserted;
            }
        }

        /// <summary>
        /// Deletes the specified element with the corresponding score from the set.
        /// </summary>
        /// <param name="score">
        /// The score.
        /// </param>
        /// <param name="element">
        /// The data element.
        /// </param>
        /// <returns>
        /// <c>true</c> if the delete succeeded; Otherwise, <c>false</c>.
        /// </returns>
        public bool Delete(double score, string element)
        {
            lock (_syncObject)
            {
                if (!_dictionary.ContainsKey(element))
                {
                    return false;
                }

                var isDeleted = _skipList.Delete(score, element);

                if (isDeleted)
                {
                    _dictionary.Remove(element);
                }

                return isDeleted;
            }
        }

        /// <summary>
        /// Deletes the specified element from the set.
        /// </summary>
        /// <param name="element">
        /// The data element.
        /// </param>
        /// <returns>
        /// <c>true</c> if the delete succeeded; Otherwise, <c>false</c>.
        /// </returns>
        public bool Delete(string element)
        {
            lock (_syncObject)
            {
                if (!_dictionary.TryGetValue(element, out var score))
                {
                    return false;
                }

                var isDeleted = _skipList.Delete(score, element);

                if (isDeleted)
                {
                    _dictionary.Remove(element);
                }

                return isDeleted;
            }
        }

        /// <summary>
        /// Updates the score on the specified element in the set.
        /// </summary>
        /// <param name="currentScore">
        /// The current score.
        /// </param>
        /// <param name="element">
        /// The data element.
        /// </param>
        /// <param name="newScore">
        /// The new score.
        /// </param>
        /// <returns>
        /// <c>true</c> if the update succeeded; Otherwise, <c>false</c>.
        /// </returns>
        public bool Update(double currentScore, string element, double newScore)
        {
            lock (_syncObject)
            {
                if (!_dictionary.TryGetValue(element, out var value))
                {
                    return false;
                }

                if (!value.NearlyEquals(currentScore))
                {
                    return false;
                }

                var isUpdated = _skipList.Update(currentScore, element, newScore) != null;

                if (isUpdated)
                {
                    _dictionary[element] = newScore;
                }

                return isUpdated;
            }
        }

        /// <summary>
        /// Deletes all items found within the specified score range (inclusive).
        /// </summary>
        /// <param name="start">
        /// The beginning of the range.
        /// </param>
        /// <param name="stop">
        /// The end of the range.
        /// </param>
        public void DeleteRangeByScore(double start, double stop)
        {
            lock (_syncObject)
            {
                _skipList.DeleteRangeByScore(new SkipListRange
                {
                    Min = start,
                    Max = stop
                }, _dictionary);
            }
        }
    }
}