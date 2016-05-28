namespace SetVersion.Lib
{
    /// <summary>
    /// Logging verbosity options.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// Logs one line with the new attributes and time taken.
        /// </summary>
        Normal,

        /// <summary>
        /// Logs nothing.
        /// </summary>
        Quiet,

        /// <summary>
        /// Verbose logging, lots of information about each step.
        /// </summary>
        Verbose
    }
}
