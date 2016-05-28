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

            CalculateNewVersions(vi);
            Logger.LogNewVersionInfo("New versions: ", vi, parsedArgs.Outfile);

            // Do we have an output file? If so update it.
            if (parsedArgs.Outfile != null)
            {
                var outputFileProcessor = factory.MakeFileProcessor(parsedArgs.Outfile);
                outputFileProcessor.Write(vi, parsedArgs.Outfile);
            }
            else
            {
                if (parsedArgs.Infile != null)
                {
                    // If we read from a file, assume that the user just wants to see the current attribute values
                    // and dump them to stdout.
                    if (vi.WriteAV && vi.AVCur != null)
                        Console.WriteLine(vi.AVCur);
                    if (vi.WriteAFV && vi.AFVCur != null)
                        Console.WriteLine(vi.AFVCur);
                    if (vi.WriteAIV && vi.AIVCur != null)
                        Console.WriteLine(vi.AIVCur);
                }
                else
                {
                    // We didn't read a file, so work in "eval mode", which allows the user to enter a pattern
                    // on the command line for evaluation and printing.
                    if (vi.WriteAV && vi.AVNew != null)
                        Console.WriteLine(vi.AVNew);
                    if (vi.WriteAFV && vi.AFVNew != null)
                        Console.WriteLine(vi.AFVNew);
                    if (vi.WriteAIV && vi.AIVNew != null)
                        Console.WriteLine(vi.AIVNew);
                }
            }
        }

        private void CalculateNewVersions(VersionInfo vi)
        {
            if (vi.AVPat != null)
            {
                vi.AVNew = patternApplier.GetNewVersion(vi.AVPat, vi.AVCur);
                LogResult("AssemblyVersion", vi.AVPat, vi.AVCur, vi.AVNew);
            }

            if (vi.AFVPat != null)
            {
                vi.AFVNew = patternApplier.GetNewVersion(vi.AFVPat, vi.AFVCur);
                LogResult("AssemblyFileVersion", vi.AFVPat, vi.AFVCur, vi.AFVNew);
            }

            if (vi.AIVPat != null)
            {
                vi.AIVNew = patternApplier.GetNewVersion(vi.AIVPat, vi.AIVCur);
                LogResult("AssemblyInformationalVersion", vi.AIVPat, vi.AIVCur, vi.AIVNew);
            }
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
