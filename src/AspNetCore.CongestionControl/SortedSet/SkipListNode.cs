using System.Collections.Generic;

namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements a skip list node.
    /// </summary>
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