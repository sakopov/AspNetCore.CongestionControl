namespace AspNetCore.CongestionControl.Configuration
{
    /// <summary>
    /// This class defines abstract configuration class which must be
    /// inherited by all classes providing configuration options.
    /// </summary>
    public abstract class BaseConfiguration
    {
        /// <summary>
        /// Validates configuration options.
        /// </summary>
        internal abstract void Validate();
    }
}