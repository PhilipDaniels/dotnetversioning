namespace SetVersion.Lib
{
    /// <summary>
    /// Represents a service that can read and write version information from files.
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// A file system abstraction, to facilitate testing.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Parses the specified input file, extracting the <c>Pattern</c> and <c>Current</c> values
        /// and returning the corresponding VersionInfo object. Any properties not found in the
        /// input file will be left as null. It is acceptable for the file to not exist.
        /// </summary>
        /// <param name="filename">The input file path.</param>
        /// <returns>Correctly initialised VersionInfo object.</returns>
        VersionInfo Read(string filename);

        /// <summary>
        /// Writes the new version to the specified file.
        /// </summary>
        /// <param name="versionInfo">The version information object containing the new version.</param>
        /// <param name="filename">The output file path. It is acceptable for
        /// the file to not exist.</param>
        void Write(VersionInfo versionInfo, string filename);
    }
}
