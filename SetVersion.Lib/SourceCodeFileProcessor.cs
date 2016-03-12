using System.Text;
using System.Text.RegularExpressions;

namespace SetVersion.Lib
{
    public abstract class SourceCodeFileProcessor : FileProcessorBase
    {
        // It's the same regex for C# and Visual Basic and even F#.
        // VB:  <Assembly: AssemblyVersion("1.0.0.0")>
        // C#:  [assembly: AssemblyVersion("1.0.0.0")]
        // F#:  [<assembly: AssemblyVersion("1.0.0.0")>]
        protected Regex avRegex = new Regex(@"AssemblyVersion\(""(?<ver>.*?)""\)", RegexOptions.Compiled);
        protected Regex afvRegex = new Regex(@"AssemblyFileVersion\(""(?<ver>.*?)""\)", RegexOptions.Compiled);
        protected Regex aivRegex = new Regex(@"AssemblyInformationalVersion\(""(?<ver>.*?)""\)", RegexOptions.Compiled);

        // These do not exist in the framework, but you can add them in comments.
        protected Regex avPatRegex = new Regex(@"AssemblyVersionPattern\(""(?<ver>.*?)""\)", RegexOptions.Compiled);
        protected Regex afvPatRegex = new Regex(@"AssemblyFileVersionPattern\(""(?<ver>.*?)""\)", RegexOptions.Compiled);
        protected Regex aivPatRegex = new Regex(@"AssemblyInformationalVersionPattern\(""(?<ver>.*?)""\)", RegexOptions.Compiled);

        // These are used when writing a new file.
        protected string usingSystemReflectionDeclaration;
        protected string attributePrefix = string.Empty;
        protected string attributeSuffix = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeFileProcessor"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public SourceCodeFileProcessor(IFileSystem fileSystem)
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
                Logger.Log("SourceCodeFileProcessor: reading from file {0}", filename);
                
                string contents = FileSystem.ReadAllText(filename);

                vi.AVCur = ExtractRegex(contents, avRegex);
                vi.AFVCur = ExtractRegex(contents, afvRegex);
                vi.AIVCur = ExtractRegex(contents, aivRegex);
                vi.AVPat = ExtractRegex(contents, avPatRegex);
                vi.AFVPat = ExtractRegex(contents, afvPatRegex);
                vi.AIVPat = ExtractRegex(contents, aivPatRegex);

                Logger.LogCurrentInfo("SourceCodeFileProcessor", vi);
                Logger.LogPatternInfo("SourceCodeFileProcessor", vi);
            }
            else
            {
                Logger.Log("SourceCodeFileProcessor: the file {0} does not exist", filename);
            }

            return vi;
        }

        /// <summary>
        /// Writes the new version to the specified file.
        /// </summary>
        /// <param name="versionInfo">The version information object containing the new version.</param>
        /// <param name="filename">The output file path. It is acceptable for
        /// the file to not exist.</param>
        public override void Write(VersionInfo versionInfo, string filename)
        {
            Val.ThrowIfNull(versionInfo, nameof(versionInfo));
            Val.ThrowIfNull(filename, nameof(filename));

            string contents;
            if (FileSystem.Exists(filename))
            {
                contents = FileSystem.ReadAllText(filename);
                contents = ReplaceAttributesInFileContents(contents, versionInfo);
                FileSystem.WriteAllText(filename, contents);
                Logger.Log("SourceCodeFileProcessor: updated file {0}", filename);
            }
            else
            {
                contents = GetNewFileContents(versionInfo);
                FileSystem.WriteAllText(filename, contents);
                Logger.Log("SourceCodeFileProcessor: wrote new file {0}", filename);
            }
        }

        /// <summary>
        /// Gets the string that should be written when creating a new file.
        /// </summary>
        /// <param name="versionInfo">The version information.</param>
        /// <returns>A string to be written to the file.</returns>
        public virtual string GetNewFileContents(VersionInfo versionInfo)
        {
            Val.ThrowIfNull(versionInfo, nameof(versionInfo));

            var sb = new StringBuilder();
            if (usingSystemReflectionDeclaration != null)
            {
                sb.AppendLine(usingSystemReflectionDeclaration);
                sb.AppendLine();
            }

            if (versionInfo.WriteAV && versionInfo.AVNew != null)
            {
                sb.AppendFormat("{0}AssemblyVersion(\"{1}\"){2}", attributePrefix, versionInfo.AVNew, attributeSuffix);
                sb.AppendLine();
            }

            if (versionInfo.WriteAFV && versionInfo.AFVNew != null)
            {
                sb.AppendFormat("{0}AssemblyFileVersion(\"{1}\"){2}", attributePrefix, versionInfo.AFVNew, attributeSuffix);
                sb.AppendLine();
            }

            if (versionInfo.WriteAIV && versionInfo.AIVNew != null)
            {
                sb.AppendFormat("{0}AssemblyInformationalVersion(\"{1}\"){2}", attributePrefix, versionInfo.AIVNew, attributeSuffix);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        protected virtual string ReplaceAttributesInFileContents(string contents, VersionInfo versionInfo)
        {
            Val.ThrowIfNull(contents, nameof(contents));
            Val.ThrowIfNull(versionInfo, nameof(versionInfo));

            if (versionInfo.WriteAV && versionInfo.AVNew != null)
                contents = ReplaceAttribute(contents, avRegex, versionInfo.AVNew, "AssemblyVersion");
            if (versionInfo.WriteAFV && versionInfo.AFVNew != null)
                contents = ReplaceAttribute(contents, afvRegex, versionInfo.AFVNew, "AssemblyFileVersion");
            if (versionInfo.WriteAIV && versionInfo.AIVNew != null)
                contents = ReplaceAttribute(contents, aivRegex, versionInfo.AIVNew, "AssemblyInformationalVersion");

            return contents;
        }

        protected virtual string ReplaceAttribute(string contents, Regex regex, string newValue, string attributeName)
        {
            Val.ThrowIfNull(contents, nameof(contents));
            Val.ThrowIfNull(regex, nameof(regex));
            Val.ThrowIfNull(newValue, nameof(newValue));

            var match = regex.Match(contents);
            if (match.Success)
            {
                contents = RegexExtensions.ReplaceFirstGroup(regex, contents, "ver", newValue);
                Logger.Log("SourceCodeFileProcessor: Replaced {0} with the new value \"{1}\"", attributeName, newValue);
            }
            else
            {
                Logger.Log("SourceCodeFileProcessor: A match for regular expression '{0}' was not found in the destination file", regex);
            }

            return contents;
        }

        private string ExtractRegex(string contents, Regex regex)
        {
            if (regex != null)
            {
                var match = regex.Match(contents);
                if (match.Success)
                    return match.Groups[1].Value;
            }

            return null;
        }
    }
}
