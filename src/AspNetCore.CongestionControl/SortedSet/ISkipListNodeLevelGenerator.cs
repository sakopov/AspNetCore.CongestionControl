namespace AspNetCore.CongestionControl.SortedSet
{
    /// <summary>
    /// This interface defines the contract for skip list node level
    /// generator.
    /// </summary>
    public interface ISkipListNodeLevelGenerator
    {
        /// <summary>
        /// Generates random skip list node level number.
        /// </summary>
        /// <param name="maxLevel">
        /// The maximum number of levels allowed.
        /// </param>
        /// <returns>
        /// The random skip list node level number.
        /// </returns>
        int Generate(int maxLevel);
    }
}