using System;
using System.Reflection;

namespace SetVersion.Lib
{
    /// <summary>
    /// Global logger implementation.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The flag which controls whether any logging is done.
        /// </summary>
        public static Verbosity Verbosity { get; set; }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            if (Verbosity == Verbosity.Verbose)
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
            if (Verbosity == Verbosity.Verbose)
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
            if (Verbosity == Verbosity.Verbose)
            {
                Console.WriteLine("{0}: Current values are av = \"{1}\", afv = \"{2}\", aiv = \"{3}\"",
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
            if (Verbosity == Verbosity.Verbose)
            {
                Console.WriteLine("{0}: Patterns are avpat = \"{1}\", afvpat = \"{2}\", aivpat = \"{3}\"",
                    prefix,
                    versionInfo.AVPat,
                    versionInfo.AFVPat,
                    versionInfo.AIVPat);
            }
        }

        /// <summary>
        /// Logs the new version number information from the VersionInfo structure, but only if
        /// it exists and there was an outfile.
        /// </summary>
        /// <param name="prefix">The message prefix.</param>
        /// <param name="versionInfo">The version information.</param>
        public static void LogNewVersionInfo(string prefix, VersionInfo versionInfo, string outFile)
        {
            outFile = outFile ?? string.Empty;

            if ((Verbosity == Verbosity.Verbose || Verbosity == Verbosity.Normal) &&
               (!string.IsNullOrEmpty(versionInfo.AVNew) || !string.IsNullOrEmpty(versionInfo.AFVNew) || !string.IsNullOrEmpty(versionInfo.AFVNew)) &&
               (!string.IsNullOrEmpty(outFile)))
            {
                Console.WriteLine("{0}: Wrote AV[\"{1}\"], AFV[\"{2}\"], AIV[\"{3}\"] to {4}",
                        ExeName,
                        versionInfo.AVNew,
                        versionInfo.AFVNew,
                        versionInfo.AIVNew,
                        outFile);
            }
        }

        /// <summary>
        /// Logs the done message.
        /// </summary>
        /// <param name="msec">The msec.</param>
        public static void LogDoneMessage(long msec)
        {
            if (Verbosity == Verbosity.Verbose)
            {
                Console.WriteLine("{0}: Completed in {1} msec.", ExeName, msec);
            }
        }

        private static string ExeName
        {
            get
            {
                return Assembly.GetEntryAssembly().ManifestModule.Name;
            }
        }
    }
}
