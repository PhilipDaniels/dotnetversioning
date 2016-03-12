namespace SetVersion.Lib
{
    public class VisualBasicFileProcessor : SourceCodeFileProcessor
    {
        public VisualBasicFileProcessor(IFileSystem fileSystem)
            : base(fileSystem)
        {
            usingSystemReflectionDeclaration = "Imports System.Reflection";
            attributePrefix = "[<Assembly: ";
            attributeSuffix = ">]";
        }
    }
}
