using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class JsonFileProcessorTests : TestingBase
    {
        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new JsonFileProcessor(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        [Fact]
        public void GetNewFileContents_ForNullVersionInfo_ThrowsArgumentNullException()
        {
            var proc = new JsonFileProcessor(fakeFileSystem);
            Action act = () => proc.GetNewFileContents(null);
            Assert.Throws<ArgumentNullException>("versionInfo", act);
        }

        [Fact]
        public void GetNewFileContents_WritesOnlyVersion()
        {
            var proc = new JsonFileProcessor(fakeFileSystem);
            var vi = new VersionInfo();
            vi.AVNew = "1.2.3";
            string contents = proc.GetNewFileContents(vi);

            Assert.Contains("version", contents);
            Assert.Contains("1.2.3", contents);
        }
    }
}
