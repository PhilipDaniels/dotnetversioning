using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// Parses the command line passed to dotnet-setversion.
    /// </summary>
    public static class CommandLineParser
    {
        /// <summary>
        /// Parses the specified arguments and returns them as a structure.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Structure representing the arguments.</returns>
        public static SetVersionCommandLineArguments Parse(string[] args)
        {
            const string DefaultError = "No arguments specified";

            var result = new SetVersionCommandLineArguments()
            {
                ErrorMessage = DefaultError,
                VersionInfo = new VersionInfo(),
                Verbosity = Verbosity.Normal
            };

            if (args == null || args.Length == 0)
                return result;

            if (HelpRequired(args))
            {
                result.ShowHelp = true;
                return result;
            }

            int i = 0;
            while (i < args.Length)
            {
                if (result.ErrorMessage == DefaultError)
                    result.ErrorMessage = null;

                string arg = args[i];

                if (IsArg("--verbose", arg))
                {
                    result.Verbosity = Verbosity.Verbose;
                }
                else if (IsArg("--quiet", arg))
                {
                    result.Verbosity = Verbosity.Quiet;
                }
                else if (IsArg("--avpat", arg))
                {
                    result.VersionInfo.AVPat = GetArgValue(args, ++i);
                    if (result.VersionInfo.AVPat == null)
                    {
                        result.ErrorMessage = "No pattern specified for option --avpat";
                        return result;
                    }
                }
                else if (IsArg("--avcur", arg))
                {
                    result.VersionInfo.AVCur = GetArgValue(args, ++i);
                    if (result.VersionInfo.AVCur == null)
                    {
                        result.ErrorMessage = "No current value specified for option --avcur";
                        return result;
                    }
                }
                else if (IsArg("--afvpat", arg))
                {
                    result.VersionInfo.AFVPat = GetArgValue(args, ++i);
                    if (result.VersionInfo.AFVPat == null)
                    {
                        result.ErrorMessage = "No pattern specified for option --afvpat";
                        return result;
                    }
                }
                else if (IsArg("--afvcur", arg))
                {
                    result.VersionInfo.AFVCur = GetArgValue(args, ++i);
                    if (result.VersionInfo.AFVCur == null)
                    {
                        result.ErrorMessage = "No current value specified for option --afvcur";
                        return result;
                    }
                }
                else if (IsArg("--aivpat", arg))
                {
                    result.VersionInfo.AIVPat = GetArgValue(args, ++i);
                    if (result.VersionInfo.AIVPat == null)
                    {
                        result.ErrorMessage = "No pattern specified for option --aivpat";
                        return result;
                    }
                }
                else if (IsArg("--aivcur", arg))
                {
                    result.VersionInfo.AIVCur = GetArgValue(args, ++i);
                    if (result.VersionInfo.AIVCur == null)
                    {
                        result.ErrorMessage = "No current value specified for option --aivcur";
                        return result;
                    }
                }
                else if (IsArg("--read", arg))
                {
                    result.Infile = GetArgValue(args, ++i);
                    if (result.Infile == null)
                    {
                        result.ErrorMessage = "No file specified for option --read";
                        return result;
                    }
                }
                else if (IsArg("--write", arg))
                {
                    result.Outfile = GetArgValue(args, ++i);
                    if (result.Outfile == null)
                    {
                        result.ErrorMessage = "No file specified for option --write";
                        return result;
                    }
                }
                else if (IsArg("--what", arg))
                {
                    string what = GetArgValue(args, ++i);
                    GetWhatToWrite(what, result.VersionInfo);
                }
                else
                {
                    // Unexpected argument.
                    result.ErrorMessage = "Unexpected argument: " + arg;
                    return result;
                }

                i++;
            }

            // TODO: Probably it is more complicated than this, depends on exactly
            // what you are trying to do.
            bool valid = (result.VersionInfo.WriteAV || result.VersionInfo.WriteAFV || result.VersionInfo.WriteAIV) &&
                           (result.Infile != null ||
                               (
                               result.VersionInfo.AVPat != null || result.VersionInfo.AFVPat != null || result.VersionInfo.AIVPat != null
                               ));

            if (!valid)
                result.ErrorMessage = "Invalid argument combination";

            return result;
        }

        private static void GetWhatToWrite(string arg, VersionInfo versionInfo)
        {
            if (arg != null)
            {
                versionInfo.WriteAV = arg.Contains("av") || arg.Contains("all");
                versionInfo.WriteAIV = arg.Contains("aiv") || arg.Contains("all");
                versionInfo.WriteAFV = arg.Contains("afv") || arg.Contains("all");
            }

            if (!versionInfo.WriteAV && !versionInfo.WriteAFV && !versionInfo.WriteAIV)
                versionInfo.WriteAV = true;
        }

        /// <summary>
        /// Is help required? Check the first argument for typical help requests.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>True if it looks like the user is asking for help.</returns>
        public static bool HelpRequired(string[] args)
        {
            return args == null ||
                   args.Length == 0 ||
                   args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("-help", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("help", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("-?", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("?", StringComparison.OrdinalIgnoreCase) ||
                   args[0].Equals("/?", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetArgValue(string[] args, int i)
        {
            // Check against missing values by seeing if the next is also a flag.
            if (i >= args.Length || args[i].StartsWith("--", StringComparison.OrdinalIgnoreCase))
                return null;

            return args[i];
        }

        private static bool IsArg(string argName, string value)
        {
            return argName.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
