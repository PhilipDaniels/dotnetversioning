using System;
using System.Globalization;
using System.Reflection;
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
            patternApplier = new PatternApplier(fakeTimeProvider, fakeFileSystem, fakeGitInterrogator);

            // This is needed because some methods refer to it. They will fail under unit
            // testing, because the entry assembly is not set.
            SetAsEntryAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Sets an assembly to be the entry assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void SetAsEntryAssembly(Assembly assembly)
        {
            AppDomainManager appDomainManager = new AppDomainManager();
            FieldInfo field = appDomainManager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(appDomainManager, assembly);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            FieldInfo field2 = currentDomain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            field2.SetValue(currentDomain, appDomainManager);
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
