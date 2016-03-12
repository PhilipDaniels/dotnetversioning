using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// Interface to allows the current time to be faked for testing purposes.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the current wall-clock time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        DateTime UtcNow { get; }
    }
}
