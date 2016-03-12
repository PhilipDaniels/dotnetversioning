using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class SourceCodeFileProcessorTests : TestingBase
    {
        class FakeFileProcessor : SourceCodeFileProcessor
        {
            public FakeFileProcessor(IFileSystem fileSystem)
                : base(fileSystem)
            {
            }

            public void SetUsingSystemReflection(string s)
            {
                usingSystemReflectionDeclaration = s;
            }
        }

        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new FakeFileProcessor(null);
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Read_ForNullFilename_ThrowsArgumentNullException()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            Action act = () => proc.Read(null);
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Read_ForFileThatDoesNotExist_ReturnsDefaultVersionInfo()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var vi = proc.Read("ver.txt");

            var expected = new VersionInfo();
            Check.VersionInfosEqual(expected, vi);
        }

        [Fact]
        public void Read_ForFileThatExistsAndOnlyHasVersions_ExtractsExpectedVersions()
        {
            // By splitting this Read test over two files we effectively
            // test that absent attributes are handled as expected.
            var proc = new FakeFileProcessor(fakeFileSystem);
            string contents = TestData.Get("SCFP.input1");
            fakeFileSystem.WriteAllText("ver.txt", contents);
            var vi = proc.Read("ver.txt");

            Assert.Equal(vi.AVCur, "1.0.0.0");
            Assert.Equal(vi.AFVCur, "1.2.3.4");
            Assert.Equal(vi.AIVCur, "some info");
            Assert.Null(vi.AVPat);
            Assert.Null(vi.AFVPat);
            Assert.Null(vi.AIVPat);
        }

        [Fact]
        public void Read_ForFileThatExistsAndOnlyHasPatterns_ExtractsExpectedPatterns()
        {
            // By splitting this Read test over two files we effectively
            // test that absent attributes are handled as expected.
            var proc = new FakeFileProcessor(fakeFileSystem);
            string contents = TestData.Get("SCFP.input2");
            fakeFileSystem.WriteAllText("ver.txt", contents);
            var vi = proc.Read("ver.txt");

            Assert.Null(vi.AVCur);
            Assert.Null(vi.AFVCur);
            Assert.Null(vi.AIVCur);
            Assert.Equal(vi.AVPat, "{{MAJOR}}.{{MINOR}}");
            Assert.Equal(vi.AFVPat, "1.0.{{Inc}}");
            Assert.Equal(vi.AIVPat, "##somefile.txt##");
        }

        [Fact]
        public void Write_ForNullVersionInfo_ThrowsArgumentNullException()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            Action act = () => proc.Write(null, "ver.txt");
            Assert.Throws<ArgumentNullException>("versionInfo", act);
        }

        [Fact]
        public void Write_ForNullFilename_ThrowsArgumentNullException()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            Action act = () => proc.Write(new VersionInfo(), null);
            Assert.Throws<ArgumentNullException>("filename", act);
        }

        [Fact]
        public void Write_ForExistingFile_ReplacesAVAttribute()
        {
            string contents = TestData.Get("SCFP.input3");
            fakeFileSystem.WriteAllText("ver.txt", contents);
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AVNew = "1.2.3.4",
                WriteAV = true
            };

            proc.Write(newVi, "ver.txt");

            contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected1");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForExistingFile_ReplacesAFVAttribute()
        {
            string contents = TestData.Get("SCFP.input3");
            fakeFileSystem.WriteAllText("ver.txt", contents);
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AFVNew = "1.2.3.4",
                WriteAFV = true
            };

            proc.Write(newVi, "ver.txt");

            contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected2");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForExistingFile_ReplacesAIVAttribute()
        {
            string contents = TestData.Get("SCFP.input3");
            fakeFileSystem.WriteAllText("ver.txt", contents);
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AIVNew = "1.2.3.4",
                WriteAIV = true
            };

            proc.Write(newVi, "ver.txt");

            contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected3");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForNewFile_WritesAVAttribute()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AVNew = "1.2.3.4",
                WriteAV = true
            };

            proc.Write(newVi, "ver.txt");

            string contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected4");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForNewFile_WritesAFVAttribute()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AFVNew = "1.2.3.4",
                WriteAFV = true
            };

            proc.Write(newVi, "ver.txt");

            string contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected5");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForNewFile_WritesAIVAttribute()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AIVNew = "1.2.3.4",
                WriteAIV = true
            };

            proc.Write(newVi, "ver.txt");

            string contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected6");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForNewFile_WritesUsingSystem()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AVNew = "1.2.3.4",
                WriteAV = true
            };
            proc.Write(newVi, "ver.txt");

            string contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected7");
            Assert.Equal(expected, contents);
        }

        [Fact]
        public void Write_ForNewFile_WritesUsingSystemReflection()
        {
            var proc = new FakeFileProcessor(fakeFileSystem);
            var newVi = new VersionInfo()
            {
                AVNew = "1.2.3.4",
                WriteAV = true
            };
            proc.SetUsingSystemReflection("using System.Reflection;");
            proc.Write(newVi, "ver.txt");

            string contents = fakeFileSystem.ReadAllText("ver.txt");
            string expected = TestData.Get("SCFP.expected8");
            Assert.Equal(expected, contents);
        }
    }
}
