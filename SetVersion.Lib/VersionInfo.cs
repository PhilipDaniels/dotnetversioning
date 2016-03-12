namespace SetVersion.Lib
{
    /// <summary>
    /// A DTO to represent current version information and the corresponding patterns.
    /// Properties named oddly, but done to match the mnemonics of the command line arguments.
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// The current value of the <c>AssemblyVersion</c> attribute.
        /// </summary>
        public string AVCur { get; set; }

        /// <summary>
        /// The new value of the <c>AssemblyVersion</c> attribute.
        /// </summary>
        public string AVNew { get; set; }

        /// <summary>
        /// The pattern to be applied to <c>AssemblyVersion</c>.
        /// </summary>
        public string AVPat { get; set; }

        /// <summary>
        /// The current value of the <c>AssemblyFileVersion</c> attribute.
        /// </summary>
        public string AFVCur { get; set; }

        /// <summary>
        /// The pattern to be applied to <c>AssemblyFileVersion</c>.
        /// </summary>
        public string AFVPat { get; set; }

        /// <summary>
        /// The new value of the <c>AssemblyFileVersion</c> attribute.
        /// </summary>
        public string AFVNew { get; set; }

        /// <summary>
        /// The current value of the <c>AssemblyInformationalVersion</c> attribute.
        /// </summary>
        public string AIVCur { get; set; }

        /// <summary>
        /// The pattern to be applied to <c>AssemblyInformationalVersion</c>.
        /// </summary>
        public string AIVPat { get; set; }

        /// <summary>
        /// The new value of the <c>AssemblyInformationalVersion</c> attribute.
        /// </summary>
        public string AIVNew { get; set; }

        /// <summary>
        /// Whether to write the <c>AssemblyVersion</c> attribute.
        /// </summary>
        public bool WriteAV { get; set; }

        /// <summary>
        /// Whether to write the <c>AssemblyFileVersion</c> attribute.
        /// </summary>
        public bool WriteAFV { get; set; }

        /// <summary>
        /// Whether to write the <c>AssemblyInformationalVersion</c> attribute.
        /// </summary>
        public bool WriteAIV { get; set; }

        /// <summary>
        /// Merges one VersionInfo object with another. Used to merge any attributes
        /// read from file with those specified on the command line.
        /// </summary>
        /// <param name="instanceHavingPriority">The instance having priority.</param>
        public void MergeCurrentAndPatterns(VersionInfo instanceHavingPriority)
        {
            Val.ThrowIfNull(instanceHavingPriority, nameof(instanceHavingPriority));

            AVCur = instanceHavingPriority.AVCur ?? AVCur;
            AFVCur = instanceHavingPriority.AFVCur ?? AFVCur;
            AIVCur = instanceHavingPriority.AIVCur ?? AIVCur;
            AVPat = instanceHavingPriority.AVPat ?? AVPat;
            AFVPat = instanceHavingPriority.AFVPat ?? AFVPat;
            AIVPat = instanceHavingPriority.AIVPat ?? AIVPat;
            WriteAV = instanceHavingPriority.WriteAV;
            WriteAFV = instanceHavingPriority.WriteAFV;
            WriteAIV = instanceHavingPriority.WriteAIV;
        }
    }
}
