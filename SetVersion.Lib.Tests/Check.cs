using Xunit;

namespace SetVersion.Lib.Tests
{
    public static class Check
    {
        public static void VersionInfosEqual(VersionInfo lhs, VersionInfo rhs)
        {
            if (lhs == null && rhs == null)
                return;
            if (lhs == null && rhs != null)
                Assert.True(false, "lhs is null, rhs is not null so instances cannot be equal.");
            if (lhs != null && rhs == null)
                Assert.True(false, "lhs is not null, rhs is null so instances cannot be equal.");

            Assert.Equal(lhs.AVCur, rhs.AVCur);
            Assert.Equal(lhs.AVNew, rhs.AVNew);
            Assert.Equal(lhs.AVPat, rhs.AVPat);
            Assert.Equal(lhs.AFVCur, rhs.AFVCur);
            Assert.Equal(lhs.AFVPat, rhs.AFVPat);
            Assert.Equal(lhs.AFVNew, rhs.AFVNew);
            Assert.Equal(lhs.AIVCur, rhs.AIVCur);
            Assert.Equal(lhs.AIVPat, rhs.AIVPat);
            Assert.Equal(lhs.AIVNew, rhs.AIVNew);
            Assert.Equal(lhs.WriteAV, rhs.WriteAV);
            Assert.Equal(lhs.WriteAFV, rhs.WriteAFV);
            Assert.Equal(lhs.WriteAIV, rhs.WriteAIV);
        }

        public static void ParsedArgsEqual(SetVersionCommandLineArguments lhs, SetVersionCommandLineArguments rhs)
        {
            if (lhs == null && rhs == null)
                return;
            if (lhs == null && rhs != null)
                Assert.True(false, "lhs is null, rhs is not null so instances cannot be equal.");
            if (lhs != null && rhs == null)
                Assert.True(false, "lhs is not null, rhs is null so instances cannot be equal.");

            Assert.Equal(lhs.Infile, rhs.Infile);
            Assert.Equal(lhs.Outfile, rhs.Outfile);
            Assert.Equal(lhs.ShowHelp, rhs.ShowHelp);
            Assert.Equal(lhs.ErrorMessage, rhs.ErrorMessage);
            Assert.Equal(lhs.Verbosity, rhs.Verbosity);
            VersionInfosEqual(lhs.VersionInfo, rhs.VersionInfo);
        }
    }
}
