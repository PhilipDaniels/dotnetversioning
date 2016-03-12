using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class CSharpFileProcessorTests : GetNewFileContentsTestingBase
    {
        public CSharpFileProcessorTests()
        {
            AVAttr = "[assembly: AssemblyVersion(\"1.2.3\")]";
            AFVAttr = "[assembly: AssemblyFileVersion(\"1.2.3\")]";
            AIVAttr = "[assembly: AssemblyInformationalVersion(\"1.2.3\")]";

            UsingSystemRef = "using System.Reflection;";
        }

        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new CSharpFileProcessor(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        protected override SourceCodeFileProcessor GetProcessor()
        {
            return new CSharpFileProcessor(fakeFileSystem);
        }
    }
}
