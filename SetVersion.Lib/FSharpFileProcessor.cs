using System.Text;

namespace SetVersion.Lib
{
    public class FSharpFileProcessor : SourceCodeFileProcessor
    {
        public FSharpFileProcessor(IFileSystem fileSystem)
            : base(fileSystem)
        {
            usingSystemReflectionDeclaration = "open System.Reflection";
            attributePrefix = "[<assembly: ";
            attributeSuffix = ">]";
        }

        public override string GetNewFileContents(VersionInfo versionInfo)
        {
            Val.ThrowIfNull(versionInfo, nameof(versionInfo));

            var sb = new StringBuilder(base.GetNewFileContents(versionInfo));
            sb.AppendLine();
            sb.AppendLine("do");
            sb.AppendLine("    ()");

            return sb.ToString();
        }
    }
}
