using System;
using System.IO;
using System.Reflection;
using SetVersion.Lib;

namespace GetVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            if (CommandLineParser.HelpRequired(args))
                ShowHelpAndExit();

            string what;
            string filename;
            if (args[0].StartsWith("--"))
            {
                what = args[0].ToLowerInvariant();
                if (args.Length < 2)
                    ShowHelpAndExit();
                filename = args[1];
            }
            else
            {
                what = "--av";
                filename = args[0];
            }
            
            if (!File.Exists(filename))
            {
                Console.WriteLine("File " + filename + " does not exist.");
                Environment.Exit(1);
            }

            var fs = new RealFileSystem();
            var factory = new FileProcessorFactory(fs);
            var inputFileProcessor = factory.MakeFileProcessor(filename);
            var vi = inputFileProcessor.Read(filename);

            if (what == "--av")
            {
                Console.WriteLine(vi.AVCur);
            }
            else if (what == "--afv")
            {
                Console.WriteLine(vi.AFVCur);
            }
            else if (what == "--aiv")
            {
                Console.WriteLine(vi.AIVCur);
            }
            else
            {
                ShowHelpAndExit();
            }
        }

        static void ShowHelpAndExit()
        {
            CommandLineHelpUtils.ShowHelpAndExit(Assembly.GetEntryAssembly(), "usage.md");
        }
    }
}
