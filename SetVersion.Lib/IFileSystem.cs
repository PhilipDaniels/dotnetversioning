namespace SetVersion.Lib
{
    /// <summary>
    /// An interface containing the file system operations the project uses.
    /// To allow mocking and testing.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Checks to see if a file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        bool Exists(string path);

        /// <summary>
        /// Reads all text from the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Contents as a string.</returns>
        string ReadAllText(string path);

        /// <summary>
        /// Writes all text to the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        void WriteAllText(string path, string contents);

        /// <summary>
        /// Reads all lines from the file and returns them as an array.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Array of lines in the file.</returns>
        string[] ReadAllLines(string path);
    }
}
