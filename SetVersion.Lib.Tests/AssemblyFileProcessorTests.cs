using System;
using System.Reflection;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class AssemblyFileProcessorTests : TestingBase
    {
        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new AssemblyFileProcessor(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        [Fact]
        public void Read_ForNullFileName_ThrowsArgumentNullException()
        {
            var proc = new AssemblyFileProcessor(fakeFileSystem);
            Action act = () => proc.Read(null);
            Assert.Throws<ArgumentNullException>("filename", act);
        }

        [Fact]
        public void Read_ForFileThatDoesNotExist_ReturnsDefaultVersionInfo()
        {
            var proc = new AssemblyFileProcessor(fakeFileSystem);
            var vi = proc.Read(@"C:\ver.dll");
            Check.VersionInfosEqual(vi, new VersionInfo());
        }

        [Fact]
        public void Read_ForDllThatExists_ExtractsCorrectAssemblyAttributes()
        {
            // Use this dll as a basis. The version is hardcoded.
            var proc = new AssemblyFileProcessor(new RealFileSystem());
            string path = Assembly.GetExecutingAssembly().Location;

            var vi = proc.Read(path);

            Assert.Equal("2.3.4.5", vi.AVCur);
            Assert.Equal("2.3.4.666", vi.AFVCur);
            Assert.Equal("2.3.4.8888-pre1963", vi.AIVCur);

            // Everything else should be unset.
            Assert.Null(vi.AVPat);
            Assert.Null(vi.AVNew);
            Assert.False(vi.WriteAV);
            Assert.Null(vi.AFVPat);
            Assert.Null(vi.AFVNew);
            Assert.False(vi.WriteAFV);
            Assert.Null(vi.AIVPat);
            Assert.Null(vi.AIVNew);
            Assert.False(vi.WriteAIV);
        }
    }
}
