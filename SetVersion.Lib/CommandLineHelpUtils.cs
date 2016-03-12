using System;
using System.Reflection;

namespace SetVersion.Lib
{
    /// <summary>
    /// Utilities to print out embedded md help files.
    /// </summary>
    public static class CommandLineHelpUtils
    {
        /// <summary>
        /// Reads a document embedded in an assembly and writes it to the
        /// standard output, then exits.
        /// </summary>
        /// <param name="assembly">The assembly containing the embedded markdown file..</param>
        /// <param name="embeddedFileName">Name of the embedded file.</param>
        public static void ShowHelpAndExit(Assembly assembly, string embeddedFileName)
        {
            string help = AssemblyExtensions.GetResourceAsString(assembly, embeddedFileName);
            Console.WriteLine(help);
            Environment.Exit(1);
        }
    }
}
