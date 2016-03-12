using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class VisualBasicFileProcessorTests : GetNewFileContentsTestingBase
    {
        public VisualBasicFileProcessorTests()
        {
            AVAttr = "[<Assembly: AssemblyVersion(\"1.2.3\")>]";
            AFVAttr = "[<Assembly: AssemblyFileVersion(\"1.2.3\")>]";
            AIVAttr = "[<Assembly: AssemblyInformationalVersion(\"1.2.3\")>]";

            UsingSystemRef = "Imports System.Reflection";
        }

        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new CSharpFileProcessor(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        protected override SourceCodeFileProcessor GetProcessor()
        {
            return new VisualBasicFileProcessor(fakeFileSystem);
        }
    }
}
