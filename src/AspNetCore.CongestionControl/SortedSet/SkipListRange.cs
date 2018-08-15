namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This class implements score range for skip list.
    /// </summary>
    public class SkipListRange
    {
        /// <summary>
        /// Gets or sets the minimum score.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum score.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the minimum
        /// score is exclusive in the range
        /// </summary>
        public bool IsMinExclusive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the maximum
        /// score is exclusive in the range.
        /// </summary>
        public bool IsMaxExclusive { get; set; }
    }
}