using System;
using System.Globalization;
using SetVersion.Lib.Tests.Utils;

namespace SetVersion.Lib.Tests
{
    /// <summary>
    /// Use a base class to make these things available, since it results in
    /// much shorter variable names in the test code.
    /// </summary>
    public abstract class TestingBase
    {
        public FakeTimeProvider fakeTimeProvider;
        public FakeFileSystem fakeFileSystem;
        public PatternApplier patternApplier;
        public FileProcessorFactory factory;
        public IVCSInterrogator fakeGitInterrogator;

        /// <summary>
        /// Create some classes to test, using fixed dates instead of DateTime.Now.
        /// </summary>
        protected TestingBase()
        {
            fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.Now = new DateTime(2011, 8, 4, 15, 16, 17);
            fakeTimeProvider.UtcNow = new DateTime(2015, 11, 3, 23, 22, 21);
            fakeFileSystem = new FakeFileSystem();
            factory = new FileProcessorFactory(fakeFileSystem);
            fakeGitInterrogator = new FakeGitInterrogator();
            //variableResolver = new VariableResolver(fakeTimeProvider, fakeFileSystem, fakeGitInterrogator);
            patternApplier = new PatternApplier(fakeTimeProvider, fakeFileSystem, fakeGitInterrogator);
        }

        public string ExpectedNowDOY
        {
            get
            {
                return DateDOY(fakeTimeProvider.Now);
            }
        }

        public string ExpectedUtcNowDOY
        {
            get
            {
                return DateDOY(fakeTimeProvider.UtcNow);
            }
        }

        private static string DateDOY(DateTime date)
        {
            return date.ToString("yy", CultureInfo.InvariantCulture) + date.DayOfYear.ToString("000", CultureInfo.InvariantCulture);
        }
    }
}
