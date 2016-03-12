using System;
using System.Diagnostics;
using System.Reflection;

namespace SetVersion.Lib
{
    /// <summary>
    /// The implementation of IFileProcessor for compiled assemblies.
    /// </summary>
    /// <seealso cref="SetVersion.Lib.FileProcessorBase" />
    public class AssemblyFileProcessor : FileProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFileProcessor"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public AssemblyFileProcessor(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        /// <summary>
        /// Parses the specified input file, extracting the <c>Pattern</c> and <c>Current</c> values
        /// and returning the corresponding VersionInfo object. Any properties not found in the
        /// input file will be left as null. It is acceptable for the file to not exist.
        /// </summary>
        /// <param name="filename">The input file path.</param>
        /// <returns>Correctly initialised VersionInfo object.</returns>
        public override VersionInfo Read(string filename)
        {
            Val.ThrowIfNull(filename, nameof(filename));

            var vi = new VersionInfo();

            if (FileSystem.Exists(filename))
            {
                Logger.Log("AssemblyFileProcessor: reading from file {0}", filename);

                var name = AssemblyName.GetAssemblyName(filename);
                vi.AVCur = name.Version.ToString();
                var fileVI = FileVersionInfo.GetVersionInfo(filename);
                vi.AFVCur = fileVI.FileVersion;
                vi.AIVCur = fileVI.ProductVersion;

                Logger.LogCurrentInfo("AssemblyFileProcessor.Read", vi);
            }
            else
            {
                Logger.Log("AssemblyFileProcessor: the file {0} does not exist.", filename);
            }

            return vi;
        }

        /// <summary>
        /// Throws an exception - assemblies cannot be written to.
        /// </summary>
        /// <param name="versionInfo">The version information object containing the new version.</param>
        /// <param name="filename">The output file path.</param>
        /// <exception cref="System.NotImplementedException">The AssemblyFileProcessor cannot write to the file.</exception>
        public override void Write(VersionInfo versionInfo, string filename)
        {
            throw new NotImplementedException("The AssemblyFileProcessor cannot write to the file.");
        }
    }
}
