using System;
using System.Collections.Generic;

namespace SetVersion.Lib.Tests.Utils
{
    /// <summary>
    /// Fake file system, we can test without hitting the real file system.
    /// </summary>
    /// <seealso cref="VersionStamping.IFileSystem" />
    public class FakeFileSystem : IFileSystem
    {
        private Dictionary<string, string> files;

        public FakeFileSystem()
        {
            files = new Dictionary<string, string>();
        }

        public bool Exists(string path)
        {
            return files.ContainsKey(path);
        }

        public string[] ReadAllLines(string path)
        {
            return files[path].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public string ReadAllText(string path)
        {
            return files[path];
        }

        public void WriteAllText(string path, string contents)
        {
            files[path] = contents;
        }
    }
}
