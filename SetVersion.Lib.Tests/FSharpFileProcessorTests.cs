using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class FSharpFileProcessorTests : GetNewFileContentsTestingBase
    {
        public FSharpFileProcessorTests()
        {
            AVAttr = "[<assembly: AssemblyVersion(\"1.2.3\")>]";
            AFVAttr = "[<assembly: AssemblyFileVersion(\"1.2.3\")>]";
            AIVAttr = "[<assembly: AssemblyInformationalVersion(\"1.2.3\")>]";

            UsingSystemRef = "open System.Reflection";
        }

        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new FSharpFileProcessor(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        protected override SourceCodeFileProcessor GetProcessor()
        {
            return new FSharpFileProcessor(fakeFileSystem);
        }
    }
}
