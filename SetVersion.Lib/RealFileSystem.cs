using System.IO;

namespace SetVersion.Lib
{
    /// <summary>
    /// An implementation of IFileSystem that hits the real file system.
    /// </summary>
    /// <seealso cref="VersionStamping.IFileSystem" />
    public class RealFileSystem : IFileSystem
    {
        /// <summary>
        /// Checks to see if a file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// True if the file exists, false otherwise.
        /// </returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Reads all text from the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// Contents as a string.
        /// </returns>
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Writes all text to the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        /// <summary>
        /// Reads all lines from the file and returns them as an array.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// Array of lines in the file.
        /// </returns>
        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}
