using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class SetVersionEngineTests : TestingBase
    {
        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new SetVersionEngine(null, factory, patternApplier);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        [Fact]
        public void Ctor_ForNullFactory_ThrowsArgumentNullException()
        {
            Action act = () => new SetVersionEngine(fakeFileSystem, null, patternApplier);
            Assert.Throws<ArgumentNullException>("factory", act);
        }

        [Fact]
        public void Ctor_ForNullPatternApplier_ThrowsArgumentNullException()
        {
            Action act = () => new SetVersionEngine(fakeFileSystem, factory, null);
            Assert.Throws<ArgumentNullException>("patternApplier", act);
        }

        [Fact]
        public void Exeute_ForNullParsedArgs_ThrowsArgumentNullException()
        {
            var eng = new SetVersionEngine(fakeFileSystem, factory, patternApplier);
            Action act = () => eng.Execute(null);
            Assert.Throws<ArgumentNullException>("parsedArgs", act);
        }

        [Fact]
        public void Execute_ForInvalidParsedArgs_ThrowsArgumentException()
        {
            var eng = new SetVersionEngine(fakeFileSystem, factory, patternApplier);
            var args = new SetVersionCommandLineArguments() { ErrorMessage = "some error" };
            Action act = () => eng.Execute(args);
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Execute_CanReadFromInputFile_AndCanProduceExpectedOutputFile()
        {
            try
            {
                // Command line params should have priority over anything
                // read from the input file.
                Environment.SetEnvironmentVariable("EnvVar", "SomeValue");

                var args = new SetVersionCommandLineArguments()
                {
                    Infile = "sve1.cs",
                    Outfile = "sve1.cs",
                    VersionInfo = new VersionInfo()
                    {
                        AVPat = "1.2.{{Inc}}",
                        AVCur = "1.2.10",                  // This should override what is in the file.
                                                           // AFVPat should be read from the file and matches EnvVar.
                        AIVPat = "1.3.4 from commit 12gaef45 on branch master",
                        AIVCur = null,
                        WriteAV = true,
                        WriteAFV = true,
                        WriteAIV = true,
                    }
                };

                // Populate the file system with the file we will be adjusting.
                string contents = TestData.Get("SVE.input1");
                fakeFileSystem.WriteAllText("sve1.cs", contents);

                // Run the engine over it.
                var eng = new SetVersionEngine(fakeFileSystem, factory, patternApplier);
                eng.Execute(args);

                // File should have been mutated to what we expect.
                contents = fakeFileSystem.ReadAllText("sve1.cs");
                string expected = TestData.Get("SVE.expected1");
                Assert.Equal(expected, contents);
            }
            finally
            {
                Environment.SetEnvironmentVariable("EnvVar", "");
            }
        }

        [Fact]
        public void Execute_DoesNotNeedInputFile_AndCanProduceExpectedOutputFile()
        {
            var args = new SetVersionCommandLineArguments()
            {
                Outfile = "sve2.cs",
                VersionInfo = new VersionInfo()
                {
                    AVPat = "1.2.{{Inc}}",
                    AVCur = null,
                    AIVPat = "1.3.4 from commit 12gaef45 on branch master",
                    AIVCur = null,
                    WriteAV = true,
                    WriteAIV = true,
                }
            };

            // Populate the file system with the file we will be adjusting.
            string contents = TestData.Get("SVE.input2");
            fakeFileSystem.WriteAllText("sve2.cs", contents);

            // Run the engine over it.
            var eng = new SetVersionEngine(fakeFileSystem, factory, patternApplier);
            eng.Execute(args);

            // File should have been mutated to what we expect.
            contents = fakeFileSystem.ReadAllText("sve2.cs");
            string expected = TestData.Get("SVE.expected2");
            Assert.Equal(expected, contents);
        }
    }
}
