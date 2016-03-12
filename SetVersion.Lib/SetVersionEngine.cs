using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// The engine that orchestrates the entire process.
    /// </summary>
    public class SetVersionEngine
    {
        private readonly IFileSystem fileSystem;
        private readonly FileProcessorFactory factory;
        private readonly IPatternApplier patternApplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetVersionEngine"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="patternApplier">The pattern applier.</param>
        /// <exception cref="System.ArgumentNullException">If any of the supplied dependencies are null.</exception>
        public SetVersionEngine
            (
            IFileSystem fileSystem,
            FileProcessorFactory factory,
            IPatternApplier patternApplier
            )
        {
            this.fileSystem = Val.ThrowIfNull(fileSystem, nameof(fileSystem));
            this.factory = Val.ThrowIfNull(factory, nameof(factory));
            this.patternApplier = Val.ThrowIfNull(patternApplier, nameof(patternApplier));
        }

        public void Execute(SetVersionCommandLineArguments parsedArgs)
        {
            Val.ThrowIfNull(parsedArgs, nameof(parsedArgs));

            if (parsedArgs.ErrorMessage != null)
                throw new ArgumentException("The parsedArgs are not valid. Error message = " + parsedArgs.ErrorMessage);

            // If an input file is specified and we can extract information
            // from it then use the values, but any arguments specified on the
            // command line have precedence.
            VersionInfo vi;
            if (parsedArgs.Infile != null)
            {
                var inputFileProcessor = factory.MakeFileProcessor(parsedArgs.Infile);
                vi = inputFileProcessor.Read(parsedArgs.Infile);
                vi.MergeCurrentAndPatterns(instanceHavingPriority: parsedArgs.VersionInfo);
            }
            else
            {
                vi = parsedArgs.VersionInfo;
            }

            Logger.LogPatternInfo("SetVersionEngine - effective", vi);

            // Actually make the new version strings, but only if we are going
            // to be writing them out.
            if (vi.WriteAV && vi.AVPat != null)
            {
                vi.AVNew = patternApplier.GetNewVersion(vi.AVPat, vi.AVCur);
                LogResult("AssemblyVersion", vi.AVPat, vi.AVCur, vi.AVNew);
            }

            if (vi.WriteAFV && vi.AFVPat != null)
            {
                vi.AFVNew = patternApplier.GetNewVersion(vi.AFVPat, vi.AFVCur);
                LogResult("AssemblyFileVersion", vi.AFVPat, vi.AFVCur, vi.AFVNew);
            }

            if (vi.WriteAIV && vi.AIVPat != null)
            {
                vi.AIVNew = patternApplier.GetNewVersion(vi.AIVPat, vi.AIVCur);
                LogResult("AssemblyInformationalVersion", vi.AIVPat, vi.AIVCur, vi.AIVNew);
            }

            // And finally write them to the destination.
            var outputFileProcessor = factory.MakeFileProcessor(parsedArgs.Outfile);
            outputFileProcessor.Write(vi, parsedArgs.Outfile);
        }

        private void LogResult(string attributeName, string patter, string currentVersion, string newVersion)
        {
            Logger.Log
                (
                "SetVersionEngine: For attribute {0}, the pattern \"{1}\" applied to current version \"{2}\" produced new version \"{3}\"",
                attributeName, patter, currentVersion, newVersion
                );
        }
    }
}
