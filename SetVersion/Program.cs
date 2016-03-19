using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SetVersion.Lib;

namespace SetVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var parsedArgs = CommandLineParser.Parse(args);
            if (parsedArgs.ShowHelp)
                CommandLineHelpUtils.ShowHelpAndExit(Assembly.GetEntryAssembly(), "usage.md");
            if (parsedArgs.ErrorMessage != null)
            {
                ShowMiniHelp(parsedArgs);
                Environment.Exit(1);
            }
                
            Logger.Verbose = parsedArgs.Verbose;

            var fs = new RealFileSystem();
            var tp = new DefaultTimeProvider();
            string workingDirectory = Path.IsPathRooted(parsedArgs.Outfile) ? Path.GetDirectoryName(parsedArgs.Outfile) : Environment.CurrentDirectory;
            var patternApplier = new PatternApplier(tp, fs, new GitInterrogator(workingDirectory));
            var factory = new FileProcessorFactory(fs);
            var engine = new SetVersionEngine(fs, factory, patternApplier);

            engine.Execute(parsedArgs);

            sw.Stop();
            Logger.Log("{0}: completed in {1} msec.", ExeName, sw.ElapsedMilliseconds);
        }

        private static void ShowMiniHelp(SetVersionCommandLineArguments parsedArgs)
        {
            Console.WriteLine("Error: " + parsedArgs.ErrorMessage);
            Console.WriteLine("Try --help to see the full help.");
        }

        private static string ExeName
        {
            get
            {
                return Assembly.GetEntryAssembly().ManifestModule.Name;
            }
        }
    }
}
