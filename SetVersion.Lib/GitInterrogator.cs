using System.Diagnostics;
using System.IO;

namespace SetVersion.Lib
{
    /// <summary>
    /// A class capable of getting information from Git.
    /// </summary>
    public class GitInterrogator : IVCSInterrogator
    {
        private readonly string workingDirectory;
        private string locatedGitExe;

        private string[] GitExeSearchPaths =
            {
            @"git.exe",                                 // On the path?
            @"C:\Program Files (x86)\Git\bin\git.exe",  // The usual suspects.
            @"C:\Program Files\Git\bin\git.exe",
            @"C:\cygwin\bin\git.exe",
            @"C:\cygwin64\bin\git.exe"
        };

        /// <summary>
        /// Initialise a new instance.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        public GitInterrogator(string workingDirectory)
        {
            Val.ThrowIfNull(workingDirectory, nameof(workingDirectory));

            if (!Directory.Exists(workingDirectory))
                throw new DirectoryNotFoundException("The working directory " + workingDirectory + " does not exist.");

            this.workingDirectory = workingDirectory;
        }

        /// <summary>
        /// Gets information from a VCS for a specific working directory.
        /// </summary>
        /// <returns>Appropriate <seealso cref="VCSInfo"/>, will not be null, but the properties
        /// of it might be.</returns>
        public VCSInfo GetInfo()
        {
            var info = new VCSInfo();
            info.Branch = RunGit("symbolic-ref --short head", workingDirectory);
            if (info.Branch != null)
            {
                info.Commit = RunGit("rev-parse head", workingDirectory);
            }

            return info;
        }

        private string RunGit(string arguments, string workingDirectory)
        {
            int i = 0;
            while (i < GitExeSearchPaths.Length)
            {
                string exe = locatedGitExe == null ? GitExeSearchPaths[i++] : locatedGitExe;
                if (exe != "git.exe" && !File.Exists(exe))
                    continue;

                try
                {
                    var psi = new ProcessStartInfo();
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardError = true;
                    psi.RedirectStandardOutput = true;
                    psi.FileName = exe;
                    psi.UseShellExecute = false;

                    var gitProcess = new Process();
                    psi.Arguments = arguments;
                    psi.WorkingDirectory = workingDirectory;
                    gitProcess.StartInfo = psi;
                    gitProcess.Start();
                    string stderr_str = gitProcess.StandardError.ReadToEnd();
                    string stdout_str = gitProcess.StandardOutput.ReadToEnd();

                    gitProcess.WaitForExit();
                    gitProcess.Close();

                    if (string.IsNullOrEmpty(stdout_str))
                        stdout_str = "NOGITOUTPUT";
                    else
                        stdout_str = stdout_str.Replace("\r", "").Replace("\n", "");

                    if (locatedGitExe == null)
                    {
                        locatedGitExe = exe;
                        Logger.Log("GitInterrogator: Located git at {0}", exe);
                    }

                    return stdout_str;
                }
                catch
                {
                }
            }

            return "NOGITOUTPUT";
        }
    }
}
