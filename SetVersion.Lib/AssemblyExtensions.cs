using System;
using System.IO;
using System.Reflection;

namespace SetVersion.Lib
{
    /// <summary>
    /// Extensions to the <code>System.Reflection.Assembly</code> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the full manifest filename of a manifest resource. The <paramref name="fileName" />
        /// is of the format "folder.folder.filename.ext" and is case sensitive. The method first
        /// tries to find an exact matching name based on the name of the <paramref name="assembly"/>
        /// and the <paramref name="fileName"/>. Normally, this will find the resource, however
        /// if you have used ILMerge the name won't be correct, so the method then tries a fallback
        /// method where it tries to find a resource whose name ends with <paramref name="fileName"/>.
        /// This should work with ILMerged assemblies as long as your embedded resources have
        /// sufficiently unique names. If your name is not sufficiently unique, an exception will
        /// be thrown.
        /// </summary>
        /// <param name="assembly">The assembly in which to form the filename.</param>
        /// <param name="fileName">Filename you want to refer to.</param>
        /// <returns>Fully qualified manifest resource filename.</returns>
        public static string GetResourceFileName(Assembly assembly, string fileName)
        {
            Val.ThrowIfNull(assembly, nameof(assembly));
            Val.ThrowIfNull(fileName, nameof(fileName));

            string[] allNames = assembly.GetManifestResourceNames();
            string candidateName = string.Concat(assembly.GetName().Name, ".", fileName);

            // Exact match?
            string actualName = Linq.SingleOrDefault(allNames, s => s == candidateName);

            // If not, look for a unique suffix match.
            if (actualName == null)
            {
                actualName = Linq.SingleOrDefault(allNames, s => s.EndsWith(fileName, StringComparison.Ordinal));
            }

            return actualName;
        }

        /// <summary>
        /// Gets an open stream on the specified embedded resource. It is the
        /// caller's responsibility to call Dispose() on the stream.
        /// The filename is of the format "folder.folder.filename.ext"
        /// and is case sensitive.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the Stream.</param>
        /// <param name="fileName">Filename whose contents you want.</param>
        /// <returns>Stream object.</returns>
        public static Stream GetResourceStream(Assembly assembly, string fileName)
        {
            string name = GetResourceFileName(assembly, fileName);
            Stream s = assembly.GetManifestResourceStream(name);
            return s;
        }

        /// <summary>
        /// Get the contents of an embedded file as a string.
        /// The filename is of the format "folder.folder.filename.ext"
        /// and is case sensitive.
        /// </summary>
        /// <param name="assembly">The assembly from which to retrieve the file.</param>
        /// <param name="fileName">Filename whose contents you want.</param>
        /// <returns>String object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Known to be safe")]
        public static string GetResourceAsString(Assembly assembly, string fileName)
        {
            using (Stream s = GetResourceStream(assembly, fileName))
            using (StreamReader sr = new StreamReader(s))
            {
                string fileContents = sr.ReadToEnd();
                return fileContents;
            }
        }
    }
}
