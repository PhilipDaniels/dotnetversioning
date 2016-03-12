namespace SetVersion.Lib.Tests.Utils
{
    public class FakeGitInterrogator : IVCSInterrogator
    {
        public VCSInfo GetInfo()
        {
            return new VCSInfo() { Branch = "fakemaster", Commit = "1234567890abcdef" };
        }
    }
}
