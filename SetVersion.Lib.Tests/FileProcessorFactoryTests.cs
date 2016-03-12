using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class FileProcessorFactoryTests : TestingBase
    {
        [Fact]
        public void Ctor_WhenFileSystemIsNull_ThrowsArgumentNullException()
        {
            Action act = () => new FileProcessorFactory(null);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        [Fact]
        public void GetFileType_ForNullFilename_ThrowsArgumentNullException()
        {
            Action act = () => FileProcessorFactory.GetFileType(null);
            Assert.Throws<ArgumentNullException>("filename", act);
        }

        [Fact]
        public void GetFileType_ForUnknownFileType_ThrowsArgumentOutOfRangeException()
        {
            Action act = () => FileProcessorFactory.GetFileType("foo.csproj");
            Assert.Throws<ArgumentOutOfRangeException>(act);
        }

        [Theory]
        [InlineData("C:\temp\foo.CS", FileType.CSharp)]
        [InlineData("\\share\foo.CS", FileType.CSharp)]
        [InlineData("foo.CS", FileType.CSharp)]
        [InlineData("foo.cs", FileType.CSharp)]
        [InlineData("foo.VB", FileType.VisualBasic)]
        [InlineData("foo.vb", FileType.VisualBasic)]
        [InlineData("foo.FS", FileType.FSharp)]
        [InlineData("foo.fs", FileType.FSharp)]
        [InlineData("foo.DLL", FileType.Assembly)]
        [InlineData("foo.dll", FileType.Assembly)]
        [InlineData("foo.EXE", FileType.Assembly)]
        [InlineData("foo.exe", FileType.Assembly)]
        [InlineData("foo.JSON", FileType.Json)]
        [InlineData("foo.json", FileType.Json)]
        public void GetFileType_ForValidFileTypes_ReturnsExpectedType(string path, FileType expected)
        {
            var result = FileProcessorFactory.GetFileType(path);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MakeFileProcessor_ForNullFilename_ThrowsArgumentNullException()
        {
            var factory = new FileProcessorFactory(fakeFileSystem);
            Action act = () => factory.MakeFileProcessor(null);
            Assert.Throws<ArgumentNullException>("filename", act);
        }

        [Theory]
        [InlineData("C:\temp\foo.CS", typeof(CSharpFileProcessor))]
        [InlineData("\\share\foo.CS", typeof(CSharpFileProcessor))]
        [InlineData("foo.CS", typeof(CSharpFileProcessor))]
        [InlineData("foo.cs", typeof(CSharpFileProcessor))]
        [InlineData("foo.VB", typeof(VisualBasicFileProcessor))]
        [InlineData("foo.vb", typeof(VisualBasicFileProcessor))]
        [InlineData("foo.FS", typeof(FSharpFileProcessor))]
        [InlineData("foo.fs", typeof(FSharpFileProcessor))]
        [InlineData("foo.DLL", typeof(AssemblyFileProcessor))]
        [InlineData("foo.dll", typeof(AssemblyFileProcessor))]
        [InlineData("foo.EXE", typeof(AssemblyFileProcessor))]
        [InlineData("foo.exe", typeof(AssemblyFileProcessor))]
        [InlineData("foo.JSON", typeof(JsonFileProcessor))]
        [InlineData("foo.json", typeof(JsonFileProcessor))]
        public void MakeFileProcessor_ForValidFileTypes_ReturnsExpectedProcessorType(string path, Type expected)
        {
            var factory = new FileProcessorFactory(fakeFileSystem);
            var result = factory.MakeFileProcessor(path);
            Assert.Equal(expected, result.GetType());
        }
    }
}
