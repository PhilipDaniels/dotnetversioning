using System.Collections.Generic;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class CommandLineParserTests
    {
        [Fact]
        public void HelpRequired_ForTypicalHelp_ReturnsTrue()
        {
            Assert.True(CommandLineParser.HelpRequired(null));
            Assert.True(CommandLineParser.HelpRequired(new string[] { }));
            Assert.True(CommandLineParser.HelpRequired(new string[] { "--help" }));
            Assert.True(CommandLineParser.HelpRequired(new string[] { "-help" }));
            Assert.True(CommandLineParser.HelpRequired(new string[] { "-h" }));
            Assert.True(CommandLineParser.HelpRequired(new string[] { "?" }));
        }

        [Fact]
        public void Parse_ForNullArgs_ReturnsErrorMessage()
        {
            var result = CommandLineParser.Parse(null);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public void Parse_ForEmptyArgs_ReturnsInvalidArgs()
        {
            var result = CommandLineParser.Parse(new string[] { });
            Assert.NotNull(result.ErrorMessage);
        }

        class InputData
        {
            public string[] Args { get; set; }
            public SetVersionCommandLineArguments Expected { get; set; }
        }

        [Fact]
        public void Parse_ExtractsArgumentsCorrectly()
        {
            foreach (var data in TheInputData)
            {
                var result = CommandLineParser.Parse(data.Args);
                Check.ParsedArgsEqual(data.Expected, result);
            }
        }

        private IEnumerable<InputData> TheInputData
        {
            get
            {
                // We can test the "invalid argument" logic just using avpat since
                // they all follow the same pattern - the next argument must be a value
                // in order for it to be extracted.
                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "--ignored" },
                    Expected = new SetVersionCommandLineArguments() { VersionInfo = new VersionInfo(), ErrorMessage = "No pattern specified for option --avpat" }
                };

                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "--verbose" },
                    Expected = new SetVersionCommandLineArguments() { VersionInfo = new VersionInfo(), ErrorMessage = "No pattern specified for option --avpat" }
                    };

                yield return new InputData()
                {
                    Args = new string[] { "--pattern", "-x" },
                    Expected = new SetVersionCommandLineArguments() { VersionInfo = new VersionInfo(), ErrorMessage = "Unexpected argument: --pattern" }
                };

                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "1.2.3" },
                    Expected = new SetVersionCommandLineArguments() { VersionInfo = new VersionInfo() { AVPat = "1.2.3" }, ErrorMessage = "Invalid argument combination" }
                };

                // And now for some valid settings.

                // write with default what of "av".
                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "1.2.3", "--avcur", "4.5.6", "--write", "foo.cs", "--read", "bar.cs" },
                    Expected = new SetVersionCommandLineArguments()
                    {
                        Infile = "bar.cs",
                        Outfile = "foo.cs",
                        VersionInfo = new VersionInfo()
                        {
                            AVPat = "1.2.3",
                            AVCur = "4.5.6",
                            WriteAV = true
                        }
                    }
                };

                // Change order, specify what to write, turn on verbose.
                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "1.2.3", "--read", "bar.cs", "--avcur", "4.5.6", "--write", "foo.cs", "afv,aiv", "--verbose" },
                    Expected = new SetVersionCommandLineArguments()
                    {
                        Infile = "bar.cs",
                        Outfile = "foo.cs",
                        Verbose = true,
                        VersionInfo = new VersionInfo()
                        {
                            AVPat = "1.2.3",
                            AVCur = "4.5.6",
                            WriteAFV = true,
                            WriteAIV = true
                        }
                    }
                };

                // The full set. Use "all" for what to write.
                yield return new InputData()
                {
                    Args = new string[] { "--avpat", "1.2.3", "--read", "bar.cs", "--avcur", "4.5.6", "--write", "foo.cs", "all", "--verbose",
                        "--afvpat", "1.0.0", "--afvcur", "2.0.0",
                        "--aivpat", "3.0.0", "--aivcur", "4.0.0"
                    },
                    Expected = new SetVersionCommandLineArguments()
                    {
                        Infile = "bar.cs",
                        Outfile = "foo.cs",
                        Verbose = true,
                        VersionInfo = new VersionInfo()
                        {
                            AVPat = "1.2.3",
                            AVCur = "4.5.6",
                            AFVPat = "1.0.0",
                            AFVCur = "2.0.0",
                            AIVPat = "3.0.0",
                            AIVCur = "4.0.0",
                            WriteAV = true,
                            WriteAFV = true,
                            WriteAIV = true
                        }
                    }
                };
            }
        }
    }
}
