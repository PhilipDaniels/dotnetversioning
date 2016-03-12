using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class PatternApplierTests : TestingBase
    {
        public PatternApplierTests()
        {
            Environment.SetEnvironmentVariable("Foo", "FooValue");
            Environment.SetEnvironmentVariable("MinorEnv", "99");
            Environment.SetEnvironmentVariable("FullVersion", "1.10.20");

            fakeFileSystem.WriteAllText(@"rel.txt", "from rel file\r\nshould be ignored");
            fakeFileSystem.WriteAllText(@"C:\abs.txt", "from abs file\r\nshould be ignored");
        }

        [Fact]
        public void Ctor_ForNullTimeProvider_ThrowsArgumentNullException()
        {
            Action act = () => new PatternApplier(null, fakeFileSystem, fakeGitInterrogator);
            Assert.Throws<ArgumentNullException>("timeProvider", act);
        }

        [Fact]
        public void Ctor_ForNullFileSystem_ThrowsArgumentNullException()
        {
            Action act = () => new PatternApplier(fakeTimeProvider, null, fakeGitInterrogator);
            Assert.Throws<ArgumentNullException>("fileSystem", act);
        }

        [Fact]
        public void Ctor_ForNullGitInterrogator_ThrowsArgumentNullException()
        {
            Action act = () => new PatternApplier(fakeTimeProvider, fakeFileSystem, null);
            Assert.Throws<ArgumentNullException>("gitInterrogator", act);
        }

        [Fact]
        public void GetNewVersion_ForNullVersionPattern_ThrowsArgumentNullException()
        {
            Action act = () => patternApplier.GetNewVersion(null, "1.0.0");
            Assert.Throws<ArgumentNullException>("versionPattern", act);
        }

        [Theory]
        [InlineData("", null, "")]
        [InlineData("", "", "")]
        [InlineData("", "1.2.3", "")]
        [InlineData("1", "", "1")]
        [InlineData("1.2", "", "1.2")]
        [InlineData("1.2.3", "4.5.6", "1.2.3")]
        [InlineData("1.2.{Inc}", "", "1.2.{Inc}")]                                   // Not enough '{' !
        [InlineData("1.2.{{Inc}}", "", "1.2.0")]                                    // Initializing a new version number where none is set.
        [InlineData("1.2.{{Inc}}", "4.5.6", "1.2.7")]                               // Typical use of {{Inc}}
        [InlineData("1.{{Inc}}.{{Inc}}", "4.5.6", "1.6.7")]
        [InlineData("{{Inc}}.{{Inc}}.{{Inc}}", "4.5.6", "5.6.7")]
        [InlineData("1.2.{{Inc}}-pre{{Inc}}", "4.5.6-pre98", "1.2.7-pre99")]        // Multiple uses in the suffix.
        [InlineData("1.2.{{Inc}}-pre{{Inc}}-{{Inc}}", "4.5.6-pre98-56", "1.2.7-pre99-57")]
        [InlineData("1.2.{{Inc}}-pre{{Inc}}-{{Inc}}-{{Inc}}", "4.5.6-pre98-56", "1.2.7-pre99-57-0")]
        [InlineData("1.{{Inc}}.{{Same}}", "4.6.8", "1.7.8")]
        [InlineData("1.{{Inc}}.pre{{Same}}", "4.6.8", "1.7.pre8")]
        [InlineData("1.{{Inc:Reset}}.{{Inc}}", "4.6.8", "1.7.0")]
        // We don't care what current is for these tests.
        [InlineData("%%Foo%%", "", "FooValue")]                                     // An environment variable that has a value.
        [InlineData("1.%%Foo%%", "", "1.FooValue")]
        [InlineData("1.%%Foo%%.%%Foo%%", "", "1.FooValue.FooValue")]
        [InlineData("%%NotExist%%", "", "")]                                        // And one that doesn't.
        [InlineData("1.%%NotExist%%", "", "1.")]
        [InlineData("1.%%NotExist%%.%%NotExist%%", "", "1..")]
        [InlineData("%%FullVersion%%", "", "1.10.20")]                              // Pull entire version number from one environment variable.
        [InlineData("1.##rel.txt##.3", "", "1.from rel file.3")]
        [InlineData(@"1.##C:\abs.txt##.3", "", "1.from abs file.3")]
        [InlineData("1.2.{{NowDOY}}", "", "1.2.11216")]                             // This works because fakeTimeProvider has a constant, hardcoded Now and UtcNow value.
        [InlineData("1.2.{{UtcNowDOY}}", "", "1.2.15307")]
        [InlineData("1.2.3-{{Now}}", "", "1.2.3-110804-151617")]
        [InlineData("1.2.3-{{UtcNow}}", "", "1.2.3-151103-232221")]
        [InlineData("1.2.3-{{Now:HH:mm}}", "", "1.2.3-15:16")]
        [InlineData("1.2.3-{{Now:D}}", "", "1.2.3-Thursday, 04 August 2011")]
        [InlineData("1.2.3-{{UtcNow:HH:mm}}", "", "1.2.3-23:22")]
        [InlineData("1.2.3-pre-{{UtcNow:D}}", "", "1.2.3-pre-Tuesday, 03 November 2015")]
        [InlineData("{{GitBranch}}", "", "fakemaster")]
        [InlineData("{{GitCommit}}", "", "1234567890abcdef")]
        [InlineData("{{GitCommit:6}}", "", "123456")]
        // Combos!
        [InlineData("%%MinorEnv%%.##rel.txt##.{{NowDOY}}-pre{{Inc}}-v{{Inc}}", "1.0.pre-42-v20", "99.from rel file.11216-pre43-v21")]
        public void GetNewVersion_ReturnsExpectedResult(string pattern, string current, string expected)
        {
            string result = patternApplier.GetNewVersion(pattern, current);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetNewVersion_CanResolveVariablesFromTheEnvironmentClass()
        {
            Assert.Equal(Environment.MachineName, patternApplier.GetNewVersion("{{MachineName}}", ""));
            Assert.Equal(Environment.OSVersion.ToString(), patternApplier.GetNewVersion("{{OSVersion}}", ""));
            Assert.Equal(Environment.UserName, patternApplier.GetNewVersion("{{UserName}}", ""));
            Assert.Equal(Environment.UserDomainName, patternApplier.GetNewVersion("{{UserDomainName}}", ""));
        }
    }
}
