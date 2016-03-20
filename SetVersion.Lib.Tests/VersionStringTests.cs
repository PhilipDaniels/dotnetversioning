using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class VersionStringTests
    {
        [Fact]
        public void Ctor_ForNullVersionNumberString_ThrowsArgumentNullException()
        {
            Action act = () => new VersionString(null);
            Assert.Throws<ArgumentNullException>("versionNumber", act);
        }

        [Theory]
        // An empty input string should be round trippable, sets Major to empty string.
        [InlineData("", "", null, null, null)]
        // Normal cases. "1.2" is minimalist number sometimes seen for AssemblyVersion.
        [InlineData("1", "1", null, null, null)]
        [InlineData("1.2", "1", "2", null, null)]
        [InlineData("1.2.3", "1", "2", "3", null)]
        [InlineData("1.2.3.4", "1", "2", "3", "4")]
        // Cases including a typical suffix.
        [InlineData("1.2.3-pre99", "1", "2", "3-pre99", null)]
        [InlineData("1.2.3.4-pre99", "1", "2", "3", "4-pre99")]
        // Cases that might be seen in AssemblyInformationalVersion
        [InlineData("2.1.0, Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}", "2", "1", "0, Commit {{GitCommit:12}} on branch {{GitBranch}}, at {{UtcNow:yyyy-MM-dd HH:mm:ss}} UTC by {{UserDomainName}}\\{{UserName}} on {{MachineName}}", null)]
        [InlineData("2.1.0.4.5", "2", "1", "0", "4.5")]
        // Cases where the suffix is too early. It will be assumed to be a part of the MAJOR or MINOR.
        [InlineData("1-pre99", "1-pre99", null, null, null)]
        [InlineData("1.2-pre99", "1", "2-pre99", null, null)]
        // Cases with integers of more than 1 digit.
        [InlineData("10.20.30", "10", "20", "30", null)]
        [InlineData("10.20.30.40", "10", "20", "30", "40")]
        [InlineData("10.20.30-pre99", "10", "20", "30-pre99", null)]
        [InlineData("10.20.30.40-pre99", "10", "20", "30", "40-pre99")]
        // Cases where the suffix includes repeated use of '-'.
        [InlineData("1.2.3-pre99-57", "1", "2", "3-pre99-57", null)]
        [InlineData("1.2.3.4-pre99-57", "1", "2", "3", "4-pre99-57")]
        // Typical pattern seen in project.json.
        [InlineData("1.2.3-*", "1", "2", "3-*", null)]
        [InlineData("1.2.3.4-*", "1", "2", "3", "4-*")]
        // Simple, correct variable use. We need to be able to parse patterns as well as normal version strings.
        [InlineData("{{Inc}}", "{{Inc}}", null, null, null)]
        [InlineData("1.{{Inc}}", "1", "{{Inc}}", null, null)]
        [InlineData("1.2.{{Inc}}", "1", "2", "{{Inc}}", null)]
        [InlineData("1.2.3.{{Inc}}", "1", "2", "3", "{{Inc}}")]
        [InlineData("1.2.3.4-{{Inc}}", "1", "2", "3", "4-{{Inc}}")]
        [InlineData("1.2.3.4-{{Inc}}-pre99", "1", "2", "3", "4-{{Inc}}-pre99")]
        // Well formed variables that include an embedded '.' character.
        [InlineData("##rel.txt##", "##rel.txt##", null, null, null)]
        [InlineData("1.##rel.txt##", "1", "##rel.txt##", null, null)]
        [InlineData("##rel.txt##.3", "##rel.txt##", "3", null, null)]
        [InlineData("1.##rel.txt##.3", "1", "##rel.txt##", "3", null)]
        [InlineData("{{Now:hh.mm}}", "{{Now:hh.mm}}", null, null, null)]
        [InlineData("1.{{Now:hh.mm}}", "1", "{{Now:hh.mm}}", null, null)]
        [InlineData("{{Now:hh.mm}}.3", "{{Now:hh.mm}}", "3", null, null)]
        [InlineData("1.{{Now:hh.mm}}.3", "1", "{{Now:hh.mm}}", "3", null)]
        [InlineData("%%Bamboo.BuildNumber%%", "%%Bamboo.BuildNumber%%", null, null, null)]
        [InlineData("1.%%Bamboo.BuildNumber%%", "1", "%%Bamboo.BuildNumber%%", null, null)]
        [InlineData("%%Bamboo.BuildNumber%%.3", "%%Bamboo.BuildNumber%%", "3", null, null)]
        [InlineData("1.%%Bamboo.BuildNumber%%.3", "1", "%%Bamboo.BuildNumber%%", "3", null)]
        // Badly closed variables will consume to the end of the string.
        [InlineData("{{Inc}", "{{Inc}", null, null, null)]
        [InlineData("{{Inc}.3", "{{Inc}.3", null, null, null)]
        [InlineData("1.{{Inc}.3", "1", "{{Inc}.3", null, null)]
        // Badly opened variables. This is more a statement of what will happen than a statement of correct behaviour.
        [InlineData("{Inc}}", "{Inc}}", null, null, null)]
        [InlineData("1.{Inc}}", "1", "{Inc}}", null, null)]
        [InlineData("{Inc}}.3", "{Inc}}", "3", null, null)]
        [InlineData("1.{Inc}}.3", "1", "{Inc}}", "3", null)]
        public void Ctor_SetsPropertiesCorrectly(string input, string major, string minor, string revision, string build)
        {
            var vs = new VersionString(input);
            Assert.Equal(major, vs.Major);
            Assert.Equal(minor, vs.Minor);
            Assert.Equal(revision, vs.Revision);
            Assert.Equal(build, vs.Build);

            // Also test round-trippability.
            Assert.Equal(input, vs.ToString());
        }
    }
}
