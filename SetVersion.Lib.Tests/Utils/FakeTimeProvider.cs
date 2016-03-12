using System;

namespace SetVersion.Lib.Tests.Utils
{
    /// <summary>
    /// A time provider which allows you to set Now and UtcNow to specific times,
    /// handy for testing purposes.
    /// </summary>
    public class FakeTimeProvider : ITimeProvider
    {
        /// <summary>
        /// Gets the current wall-clock time.
        /// </summary>
        public DateTime Now { get; set; }

        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        public DateTime UtcNow { get; set; }
    }
}
