using System.Diagnostics;

namespace SetVersion.Lib
{
    /// <summary>
    /// Represents information extracted from a Version Control System.
    /// </summary>
    [DebuggerDisplay("{Branch}, {Commit}")]
    public class VCSInfo
    {
        /// <summary>
        /// Gets or sets the branch name.
        /// </summary>
        /// <value>
        /// The branch.
        /// </value>
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the commit id, such as as Git sha.
        /// </summary>
        /// <value>
        /// The commit.
        /// </value>
        public string Commit { get; set; }
    }
}
