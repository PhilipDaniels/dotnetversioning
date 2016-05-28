namespace SetVersion.Lib
{
    /// <summary>
    /// Represents the command line arguments after parsing.
    /// </summary>
    public class SetVersionCommandLineArguments
    {
        /// <summary>
        /// Whether to show the full help.
        /// </summary>
        public bool ShowHelp { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// If non-null, means the arguments were not parsed correctly and the "mini-help"
        /// should be shown.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Verbosity to be applied.
        /// </summary>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// The name of the file to read existing version numbers from.
        /// </summary>
        public string Infile { get; set; }

        /// <summary>
        /// The name of the file to be written.
        /// </summary>
        public string Outfile { get; set; }

        /// <summary>
        /// The version information.
        /// </summary>
        public VersionInfo VersionInfo { get; set; }
    }
}
