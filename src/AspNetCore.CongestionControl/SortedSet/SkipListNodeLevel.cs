namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements a skip list node level.
    /// </summary>
    public class SkipListNodeLevel
    {
        /// <summary>
        /// Gets or sets the forward node.
        /// </summary>
        public SkipListNode Forward { get; set; }

        /// <summary>
        /// Gets or sets the number of nodes spanned at current level.
        /// </summary>
        public long Span { get; set; }
    }
}