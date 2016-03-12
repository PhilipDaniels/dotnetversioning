using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace SetVersion.Lib.Tests
{
    public class GitInterrogatorTests
    {
        [Fact]
        public void GetInfo_CanExtractActualGitInfo()
        {
            // We need a real Git interrogator, we actually want to check that git extraction works.
            string dir = GitDir();
            var interrogator = new GitInterrogator(dir);

            var vi = interrogator.GetInfo();

            Assert.True(vi.Branch.Length > 0);
            Assert.True(vi.Commit.Length > 0);
        }

        private string GitDir([CallerFilePath] string sourcePath = "")
        {
            // Clever huh? Avoids problems with the shadow copy into the temp dir.
            return Path.GetDirectoryName(sourcePath);
        }
    }
}
