namespace SetVersion.Lib
{
    /// <summary>
    /// Base class for file processors.
    /// </summary>
    /// <seealso cref="SetVersion.Lib.IFileProcessor" />
    public abstract class FileProcessorBase : IFileProcessor
    {
        public IFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessorBase"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <exception cref="System.ArgumentNullException">If fileSystem is null.</exception>
        protected FileProcessorBase(IFileSystem fileSystem)
        {
            FileSystem = Val.ThrowIfNull(fileSystem, nameof(fileSystem));
        }

        /// <summary>
        /// Parses the specified input file, extracting the <c>Pattern</c> and <c>Current</c> values
        /// and returning the corresponding VersionInfo object. Any properties not found in the
        /// input file will be left as null. It is acceptable for the file to not exist.
        /// </summary>
        /// <param name="filename">The input file path.</param>
        /// <returns>Correctly initialised VersionInfo object.</returns>
        public abstract VersionInfo Read(string filename);

        /// <summary>
        /// Writes the new version to the specified file.
        /// </summary>
        /// <param name="versionInfo">The version information object containing the new version.</param>
        /// <param name="filename">The output file path. It is acceptable for
        /// the file to not exist.</param>
        public abstract void Write(VersionInfo versionInfo, string filename);
    }
}
