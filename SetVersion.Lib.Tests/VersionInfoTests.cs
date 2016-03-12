using System;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class VersionInfoTests
    {
        [Fact]
        public void MergeCurrentAndPatterns_ForNullInstance_ThrowsArgumentNullException()
        {
            var lhs = new VersionInfo();
            Action act = () => lhs.MergeCurrentAndPatterns(null);
            Assert.Throws<ArgumentNullException>("instanceHavingPriority", act);
        }

        [Fact]
        public void MergeCurrentAndPatterns_WhenSecondIsNullUsesFirst()
        {
            var lhs = new VersionInfo()
            {
                AVCur = "a", AFVCur = "b", AIVCur = "c", AVPat = "d", AFVPat = "e", AIVPat = "f",
                WriteAV = true, WriteAFV = false, WriteAIV = true
            };
            var rhs = new VersionInfo() {  };

            lhs.MergeCurrentAndPatterns(rhs);

            Assert.Equal("a", lhs.AVCur);
            Assert.Equal("b", lhs.AFVCur);
            Assert.Equal("c", lhs.AIVCur);
            Assert.Equal("d", lhs.AVPat);
            Assert.Equal("e", lhs.AFVPat);
            Assert.Equal("f", lhs.AIVPat);
            // Because bools aren't null and hence the second always has priority.
            Assert.False(lhs.WriteAV);
            Assert.False(lhs.WriteAFV);
            Assert.False(lhs.WriteAIV);
        }

        [Fact]
        public void MergeCurrentAndPatterns_WhenFirstIsNullUsesSecond()
        {
            var lhs = new VersionInfo() { };
            var rhs = new VersionInfo()
            {
                AVCur = "a", AFVCur = "b", AIVCur = "c", AVPat = "d", AFVPat = "e", AIVPat = "f",
                WriteAV = true, WriteAFV = false, WriteAIV = true
            };

            lhs.MergeCurrentAndPatterns(rhs);

            Assert.Equal("a", lhs.AVCur);
            Assert.Equal("b", lhs.AFVCur);
            Assert.Equal("c", lhs.AIVCur);
            Assert.Equal("d", lhs.AVPat);
            Assert.Equal("e", lhs.AFVPat);
            Assert.Equal("f", lhs.AIVPat);
            Assert.True(lhs.WriteAV);
            Assert.False(lhs.WriteAFV);
            Assert.True(lhs.WriteAIV);
        }

        [Fact]
        public void MergeCurrentAndPatterns_GivesPriorityToTheArgument()
        {
            var lhs = new VersionInfo() { AVCur = "a", AFVCur = "b", AIVCur = "c", AVPat = "d" };
            var rhs = new VersionInfo() {              AFVCur = "e", AIVCur = "f", AVPat = "g", AFVPat = "h", AIVPat = "i" };

            lhs.MergeCurrentAndPatterns(rhs);

            Assert.Equal("a", lhs.AVCur);
            Assert.Equal("e", lhs.AFVCur);
            Assert.Equal("f", lhs.AIVCur);
            Assert.Equal("g", lhs.AVPat);
            Assert.Equal("h", lhs.AFVPat);
            Assert.Equal("i", lhs.AIVPat);
        }
    }
}
