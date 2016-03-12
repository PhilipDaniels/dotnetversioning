using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// A default time provider which simply returns the real time.
    /// </summary>
    public class DefaultTimeProvider : ITimeProvider
    {
        DateTime now;
        DateTime utcNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTimeProvider"/> class.
        /// Takes a record of the current time and stores it, so that the time of "Now" and "UtcNow" is frozen
        /// for the entire duration of the program, which prevents different attributes getting written
        /// with different timestamps.
        /// </summary>
        public DefaultTimeProvider()
        {
            now = DateTime.Now;
            utcNow = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the current wall-clock time.
        /// </summary>
        public DateTime Now
        {
            get { return now; }
        }

        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        public DateTime UtcNow
        {
            get { return utcNow; }
        }
    }
}
