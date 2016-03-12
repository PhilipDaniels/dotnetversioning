using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// Used to create the appropriate type of IFileProcessor.
    /// </summary>
    public class FileProcessorFactory
    {
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessorFactory"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system. Passed down into the created file processor instance.</param>
        /// <exception cref="System.ArgumentNullException">If fileSystem is null.</exception>
        public FileProcessorFactory(IFileSystem fileSystem)
        {
            this.fileSystem = Val.ThrowIfNull(fileSystem, nameof(fileSystem));
        }

        /// <summary>
        /// Create a file processor of the appropriate type for the input file.
        /// The appropriate type is determined from the file extension.
        /// </summary>
        /// <param name="filename">The file that you want a processor for.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If filename is null.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public IFileProcessor MakeFileProcessor(string filename)
        {
            Val.ThrowIfNull(filename, nameof(filename));

            var ft = GetFileType(filename);
            Logger.Log("FileProcessorFactory: being asked to create a processor for {0}, file type is {1}", filename, ft);

            switch (ft)
            {
                case FileType.Json:
                    return new JsonFileProcessor(fileSystem);
                case FileType.CSharp:
                    return new CSharpFileProcessor(fileSystem);
                case FileType.VisualBasic:
                    return new VisualBasicFileProcessor(fileSystem);
                case FileType.FSharp:
                    return new FSharpFileProcessor(fileSystem);
                case FileType.Assembly:
                    return new AssemblyFileProcessor(fileSystem);
                default:
                    throw new InvalidOperationException("Unhandled file type: " + filename);
            }
        }

        /// <summary>
        /// Gets the type of the file, based on its extension.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="filename"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Unknown file type.</exception>
        public static FileType GetFileType(string filename)
        {
            Val.ThrowIfNull(filename, nameof(filename));

            if (filename.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.CSharp;
            }
            else if (filename.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Assembly;
            }
            else if (filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Json;
            }
            else if (filename.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.VisualBasic;
            }
            else if (filename.EndsWith(".fs", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.FSharp;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(filename), "Unknown file type.");
            }
        }
    }
}
