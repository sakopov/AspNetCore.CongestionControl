using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements a skip list where the node comparison
    /// occurs by score and data element.
    /// </summary>
    public class SkipList
    {
        /// <summary>
        /// The maximum number of levels in the list.
        /// </summary>
        internal const int SkipListMaxLevel = 64;

        /// <summary>
        /// The node level generator implementation.
        /// </summary>
        private readonly ISkipListNodeLevelGenerator _nodeLevelGenerator;

        /// <summary>
        /// Gets the head node of the skip list.
        /// </summary>
        public SkipListNode Head { get; private set; }

        /// <summary>
        /// Gets the tail node of the skip list.
        /// </summary>
        public SkipListNode Tail { get; private set; }

        /// <summary>
        /// Gets the total number of items in the skip list.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets the total number of levels in the skip list.
        /// </summary>
        public int Levels { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkipList"/> class
        /// using <see cref="SkipListNodeLevelGenerator"/> as the node
        /// level generator.
        /// </summary>
        public SkipList() : this(new SkipListNodeLevelGenerator())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkipList"/> class.
        /// </summary>
        public SkipList(ISkipListNodeLevelGenerator nodeLevelGenerator)
        {
            _nodeLevelGenerator = nodeLevelGenerator;

            Levels = 1;
            Length = 0;
            Head = CreateNode(0, null);

            for (var i = 0; i < SkipListMaxLevel; i++)
            {
                Head.Levels.Add(new SkipListNodeLevel
                {
                    Forward = null,
                    Span = 0
                });
            }

            Head.Backward = null;
            Tail = null;
        }

        /// <summary>
        /// Inserts a node in the skip list. Note, this method assumes
        /// that the data element doesn't already exist. The caller
        /// of this method should keep track of inserted elements.
        /// </summary>
        /// <param name="score">
        /// The score associated with the node.
        /// </param>
        /// <param name="element">
        /// The data element associated with the node.
        /// </param>
        /// <returns>
        /// The inserted node.
        /// </returns>
        public SkipListNode Insert(double score, string element)
        {
            var update = new SkipListNode[SkipListMaxLevel];
            var rank = new long[SkipListMaxLevel];

            var currentNode = Head;

            for (var currentLevel = Levels - 1; currentLevel >= 0; currentLevel--)
            {
                rank[currentLevel] = currentLevel == (Levels - 1) ? 0 : rank[currentLevel + 1];

                while (currentNode.Levels[currentLevel].Forward != null &&
                       (currentNode.Levels[currentLevel].Forward.Score < score ||
                        (currentNode.Levels[currentLevel].Forward.Score.NearlyEquals(score) &&
                         string.CompareOrdinal(currentNode.Levels[currentLevel].Forward.Element, element) < 0)))
                {
                    rank[currentLevel] += currentNode.Levels[currentLevel].Span;

                    currentNode = currentNode.Levels[currentLevel].Forward;
                }

                update[currentLevel] = currentNode;
            }

            var insertLevel = GetRandomLevel();

            // Provision new level if it's greater than current number of levels
            if (insertLevel > Levels)
            {
                for (var currentLevel = Levels; currentLevel < insertLevel; currentLevel++)
                {
                    rank[currentLevel] = 0;
                    update[currentLevel] = Head;
                    update[currentLevel].Levels[currentLevel].Span = Length;
                }

                Levels = insertLevel;
            }

            currentNode = CreateNode(score, element);

            for (var currentLevel = 0; currentLevel < insertLevel; currentLevel++)
            {
                var newLevel = new SkipListNodeLevel
                {
                    Forward = update[currentLevel].Levels[currentLevel].Forward
                };

                currentNode.Levels.Add(newLevel);

                update[currentLevel].Levels[currentLevel].Forward = currentNode;

                currentNode.Levels[currentLevel].Span = update[currentLevel].Levels[currentLevel].Span - (rank[0] - rank[currentLevel]);

                update[currentLevel].Levels[currentLevel].Span = (rank[0] - rank[currentLevel]) + 1;
            }

            // Increment span for untouched levels
            for (var currentLevel = insertLevel; currentLevel < Levels; currentLevel++)
            {
                update[currentLevel].Levels[currentLevel].Span++;
            }

            currentNode.Backward = (update[0] == Head) ? null : update[0];

            if (currentNode.Levels[0].Forward != null)
            {
                currentNode.Levels[0].Forward.Backward = currentNode;
            }
            else
            {
                Tail = currentNode;
            }

            Length++;

            return currentNode;
        }

        /// <summary>
        /// Deletes a node with matching score and element from the skip list.
        /// </summary>
        /// <param name="score">
        /// The score to match on.
        /// </param>
        /// <param name="element">
        /// The element to match on.
        /// </param>
        /// <returns>
        /// <c>true</c> if a node was deleted; Otherwise, <c>false</c>.
        /// </returns>
        public bool Delete(double score, string element)
        {
            var update = new SkipListNode[SkipListMaxLevel];

            var currentNode = Head;

            for (var currentLevel = Levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.Levels[currentLevel].Forward != null &&
                       (currentNode.Levels[currentLevel].Forward.Score < score ||
                        (currentNode.Levels[currentLevel].Forward.Score.NearlyEquals(score) &&
                         string.CompareOrdinal(currentNode.Levels[currentLevel].Forward.Element, element) < 0)))
                {
                    currentNode = currentNode.Levels[currentLevel].Forward;
                }

                update[currentLevel] = currentNode;
            }

            // We may have multiple elements with the same score, what we need
            // is to find the element with both the right score and object.
            currentNode = currentNode.Levels[0].Forward;

            if (currentNode != null &&
                currentNode.Score.NearlyEquals(score) &&
                string.CompareOrdinal(currentNode.Element, element) == 0)
            {
                DeleteNode(currentNode, update);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the score on a node with matching score and element.
        /// </summary>
        /// <param name="currentScore">
        /// The current score of the node.
        /// </param>
        /// <param name="element">
        /// The element of the node.
        /// </param>
        /// <param name="newScore">
        /// The new score.
        /// </param>
        /// <returns>
        /// The updated node.
        /// </returns>
        public SkipListNode Update(double currentScore, string element, double newScore)
        {
            var update = new SkipListNode[SkipListMaxLevel];

            var currentNode = Head;

            for (var currentLevel = Levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.Levels[currentLevel].Forward != null &&
                       (currentNode.Levels[currentLevel].Forward.Score < currentScore ||
                        (currentNode.Levels[currentLevel].Forward.Score.NearlyEquals(currentScore) &&
                         string.CompareOrdinal(currentNode.Levels[currentLevel].Forward.Element, element) < 0)))
                {
                    currentNode = currentNode.Levels[currentLevel].Forward;
                }

                update[currentLevel] = currentNode;
            }

            // Jump to element
            currentNode = currentNode.Levels[0].Forward;

            Debug.Assert(currentNode != null && 
                         currentScore.NearlyEquals(currentNode.Score) && 
                         string.CompareOrdinal(currentNode.Element, element) == 0);

            // If the node, after the score update, would be still exactly
            // at the same position, we can just update the score without
            // actually removing and re-inserting the element in the skiplist.
            if ((currentNode.Backward == null || currentNode.Backward.Score < newScore) &&
                (currentNode.Levels[0].Forward == null || currentNode.Levels[0].Forward.Score > newScore))
            {
                currentNode.Score = newScore;

                return currentNode;
            }

            // Remove old and and insert a new node at different place.
            DeleteNode(currentNode, update);

            var newNode = Insert(newScore, currentNode.Element);

            currentNode.Element = null;

            return newNode;
        }

        /// <summary>
        /// Deletes nodes found within the provided range.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <returns>
        /// The number of deleted nodes.
        /// </returns>
        public long DeleteRangeByScore(SkipListRange range)
        {
            return DeleteRangeByScore(range, null);
        }

        /// <summary>
        /// Deletes the nodes found within the provided range.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary containing the map of element to score. Any deleted
        /// elements will be deleted from the dictionary.
        /// </param>
        /// <returns>
        /// The number of deleted nodes.
        /// </returns>
        public long DeleteRangeByScore(SkipListRange range, IDictionary<string, double> dictionary)
        {
            var update = new SkipListNode[SkipListMaxLevel];

            long removed = 0;

            var currentNode = Head;

            for (var currentLevel = Levels - 1; currentLevel >= 0; currentLevel--)
            {
                while (currentNode.Levels[currentLevel].Forward != null && 
                       (range.IsMinExclusive
                           ? currentNode.Levels[currentLevel].Forward.Score <= range.Min
                           : currentNode.Levels[currentLevel].Forward.Score < range.Min))
                {
                    currentNode = currentNode.Levels[currentLevel].Forward;
                }

                update[currentLevel] = currentNode;
            }

            /* Current node is the last with score < or <= min. */
            currentNode = currentNode.Levels[0].Forward;

            /* Delete nodes while in range. */
            while (currentNode != null &&
                   (range.IsMaxExclusive 
                       ? currentNode.Score < range.Max 
                       : currentNode.Score <= range.Max))
            {
                var next = currentNode.Levels[0].Forward;

                DeleteNode(currentNode, update);

                dictionary?.Remove(currentNode.Element);

                removed++;

                currentNode = next;
            }

            return removed;
        }

        /// <summary>
        /// Deletes a node from the specified skip list.
        /// </summary>
        /// <param name="node">
        /// The node to delete.
        /// </param>
        /// <param name="update">
        /// The graph of nodes in the path to the node to be deleted.
        /// </param>
        private void DeleteNode(SkipListNode node, SkipListNode[] update)
        {
            for (var currentLevel = 0; currentLevel < Levels; currentLevel++)
            {
                if (update[currentLevel].Levels[currentLevel].Forward == node) 
                {
                    if (node.Levels[currentLevel].Forward != null)
                    {
                        update[currentLevel].Levels[currentLevel].Span += node.Levels[currentLevel].Span - 1;
                    }
                    else
                    {
                        update[currentLevel].Levels[currentLevel].Span = 0;
                    }

                    update[currentLevel].Levels[currentLevel].Forward = node.Levels[currentLevel].Forward;
                }
                else
                {
                    update[currentLevel].Levels[currentLevel].Span -= 1;
                }
            }

            if (node.Levels[0].Forward != null)
            {
                node.Levels[0].Forward.Backward = node.Backward;
            }
            else
            {
                Tail = node.Backward;
            }

            while (Levels > 1 && Head.Levels[Levels - 1].Forward == null)
            {
                Levels--;
            }

            Length--;
        }

        /// <summary>
        /// Creates a node with the specified score and element.
        /// </summary>
        /// <param name="score">
        /// The score.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The new node.
        /// </returns>
        private static SkipListNode CreateNode(double score, string element)
        {
            var node = new SkipListNode
            {
                Score = score,
                Element = element
            };

            return node;
        }

        /// <summary>
        /// Generates a random level for skip list inserts.
        /// </summary>
        /// <returns>
        /// The randomly-generated level number.
        /// </returns>
        private int GetRandomLevel()
        {
            return _nodeLevelGenerator.Generate(SkipListMaxLevel);
        }
    }
}