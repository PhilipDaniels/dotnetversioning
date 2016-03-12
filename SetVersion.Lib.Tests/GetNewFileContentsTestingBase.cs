using Xunit;

namespace SetVersion.Lib.Tests
{
    public abstract class GetNewFileContentsTestingBase : TestingBase
    {
        protected string AVAttr;
        protected string AFVAttr;
        protected string AIVAttr;
        protected string UsingSystemRef;

        protected abstract SourceCodeFileProcessor GetProcessor();

        [Fact]
        public void GetNewFileContents_WhenWriteAVIsTrue_WritesAssemblyVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AVNew = "1.2.3", WriteAV = true };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.Contains(AVAttr, result);
        }

        [Fact]
        public void GetNewFileContents_WhenWriteAVIsFalse_OmitsAssemblyVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AVNew = "1.2.3", WriteAV = false };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.DoesNotContain(AVAttr, result);
        }

        [Fact]
        public void GetNewFileContents_WhenWriteAFVIsTrue_WritesAssemblyFileVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AFVNew = "1.2.3", WriteAFV = true };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.Contains(AFVAttr, result);
        }

        [Fact]
        public void GetNewFileContents_WhenWriteAFVIsFalse_OmitsAssemblyFileVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AFVNew = "1.2.3", WriteAFV = false };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.DoesNotContain(AFVAttr, result);
        }

        [Fact]
        public void GetNewFileContents_WhenWriteAIVIsTrue_WritesAssemblyInformationalVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AIVNew = "1.2.3", WriteAIV = true };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.Contains(AIVAttr, result);
        }

        [Fact]
        public void GetNewFileContents_WhenWriteAIVIsFalse_OmitsAssemblyInformationalVersionAttribute()
        {
            var proc = GetProcessor();

            var vi = new VersionInfo() { AIVNew = "1.2.3", WriteAIV = false };
            string result = proc.GetNewFileContents(vi);

            CheckUsings(result);
            Assert.DoesNotContain(AIVAttr, result);
        }

        private void CheckUsings(string result)
        {
            Assert.Contains(UsingSystemRef, result);
        }
    }
}
