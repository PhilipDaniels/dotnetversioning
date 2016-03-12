namespace SetVersion.Lib
{
    public class CSharpFileProcessor : SourceCodeFileProcessor
    {
        public CSharpFileProcessor(IFileSystem fileSystem)
            : base(fileSystem)
        {
            usingSystemReflectionDeclaration = "using System.Reflection;";
            attributePrefix = "[assembly: ";
            attributeSuffix = "]";
        }
    }
}
