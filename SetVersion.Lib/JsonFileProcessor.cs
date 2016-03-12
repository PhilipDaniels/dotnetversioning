using System.Text;
using System.Text.RegularExpressions;

namespace SetVersion.Lib
{
    public class JsonFileProcessor : SourceCodeFileProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileProcessor"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public JsonFileProcessor(IFileSystem fileSystem)
            : base(fileSystem)
        {
            avRegex = new Regex(@"""version""\s*:\s*""(?<ver>.*?)""", RegexOptions.Compiled);
            afvRegex = null;
            aivRegex = null;
            avPatRegex = new Regex(@"""versionPattern""\s*:\s*""(.*?)""", RegexOptions.Compiled);
            afvPatRegex = null;
            aivPatRegex = null;
        }

        /// <summary>
        /// Gets the string that should be written when creating a new file.
        /// For Json files, this just includes a "version" tag.
        /// </summary>
        /// <param name="versionInfo">The version information.</param>
        /// <returns>
        /// A string to be written to the file.
        /// </returns>
        public override string GetNewFileContents(VersionInfo versionInfo)
        {
            Val.ThrowIfNull(versionInfo, nameof(versionInfo));

            var sb = new StringBuilder();

            sb.AppendLine("{");
            sb.Append("  ");
            StringBuilderExtensions.AddJson(sb, "version", versionInfo.AVNew);
            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
