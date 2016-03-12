using System.Reflection;

namespace SetVersion.Lib.Tests
{
    public static class TestData
    {
        public static string Get(string embeddedFilename)
        {
            embeddedFilename = "Data." + embeddedFilename;
            return AssemblyExtensions.GetResourceAsString(Assembly.GetExecutingAssembly(), embeddedFilename);
        }
    }
}
