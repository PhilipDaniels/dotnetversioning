using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// Global logger implementation.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The flag which controls whether any logging is done. If false,
        /// no output is produced by any of the logging methods.
        /// </summary>
        public static bool Verbose { get; set; }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            if (Verbose)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Logs the specified formatted message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Log(string format, params object[] args)
        {
            if (Verbose)
            {
                Console.WriteLine(format, args);
            }
        }

        /// <summary>
        /// Logs the current information from the VersionInfo structure.
        /// </summary>
        /// <param name="prefix">The message prefix.</param>
        /// <param name="versionInfo">The version information.</param>
        public static void LogCurrentInfo(string prefix, VersionInfo versionInfo)
        {
            if (Verbose)
            {
                Log("{0}: Current values are av = \"{1}\", afv = \"{2}\", aiv = \"{3}\"",
                    prefix,
                    versionInfo.AVCur,
                    versionInfo.AFVCur,
                    versionInfo.AIVCur);
            }
        }

        /// <summary>
        /// Logs the pattern information from the VersionInfo structure.
        /// </summary>
        /// <param name="prefix">The message prefix.</param>
        /// <param name="versionInfo">The version information.</param>
        public static void LogPatternInfo(string prefix, VersionInfo versionInfo)
        {
            if (Verbose)
            {
                Log("{0}: Patterns are avpat = \"{1}\", afvpat = \"{2}\", aivpat = \"{3}\"",
                    prefix,
                    versionInfo.AVPat,
                    versionInfo.AFVPat,
                    versionInfo.AIVPat);
            }
        }

        /// <summary>
        /// Logs the new version number information from the VersionInfo structure.
        /// </summary>
        /// <param name="prefix">The message prefix.</param>
        /// <param name="versionInfo">The version information.</param>
        public static void LogNewVersionInfo(string prefix, VersionInfo versionInfo)
        {
            if (Verbose)
            {
                Log("{0}: New version numbers are avnew = \"{1}\", afvnew = \"{2}\", aivnew = \"{3}\"",
                    prefix,
                    versionInfo.AVNew,
                    versionInfo.AFVNew,
                    versionInfo.AIVNew);
            }
        }
    }
}
