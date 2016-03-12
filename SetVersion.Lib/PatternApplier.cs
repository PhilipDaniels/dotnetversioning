using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SetVersion.Lib
{
    /// <summary>
    /// Applies a pattern to a current version to generate a new version number.
    /// </summary>
    public partial class PatternApplier : IPatternApplier
    {
        private static readonly Regex variableRegex = new Regex(@"(%%.*?%%)|(##.*?##)|({{.*?}})", RegexOptions.Compiled);
        private static readonly Regex integerRegex = new Regex(@"[0-9]+", RegexOptions.Compiled);

        private readonly ITimeProvider timeProvider;
        private readonly IFileSystem fileSystem;
        private readonly IVCSInterrogator gitInterrogator;
        private VCSInfo vcsInfo;
        private bool incReset;

        /// <summary>
        /// The default format string for date variables.
        /// See https://msdn.microsoft.com/en-us/library/az4se3k1%28v=vs.110%29.aspx
        /// This is designed to go down to the second, but to be short enough that
        /// you can still use it in a NuGet suffix, they are limited to 20 characters.
        /// </summary>
        public const string DefaultDateFormat = "yyMMdd-HHmmss";

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternApplier"/> class.
        /// </summary>
        public PatternApplier
            (
            ITimeProvider timeProvider,
            IFileSystem fileSystem,
            IVCSInterrogator gitInterrogator
            )
        {
            this.timeProvider = Val.ThrowIfNull(timeProvider, nameof(timeProvider));
            this.fileSystem = Val.ThrowIfNull(fileSystem, nameof(fileSystem));
            this.gitInterrogator = Val.ThrowIfNull(gitInterrogator, nameof(gitInterrogator));
        }

        /// <summary>
        /// Generate a new version number based on a pattern and an existing version number.
        /// </summary>
        /// <param name="versionPattern">The version pattern.</param>
        /// <param name="currentVersion">The current version. Used as the basis for any <c>{{Inc}}</c> variables.</param>
        /// <returns>A new version number.</returns>
        public string GetNewVersion(string versionPattern, string currentVersion)
        {
            Val.ThrowIfNull(versionPattern, nameof(versionPattern));

            currentVersion = currentVersion ?? "";
            var vnPattern = new VersionString(versionPattern);
            var vnCurrent = new VersionString(currentVersion);

            ApplyPattern(vnPattern, vnCurrent);

            return vnCurrent.ToString();
        }

        private void ApplyPattern(VersionString vnPattern, VersionString vnCurrent)
        {
            vnCurrent.Major = EvaluatePattern(vnPattern.Major, vnCurrent.Major);
            vnCurrent.Minor = EvaluatePattern(vnPattern.Minor, vnCurrent.Minor);
            vnCurrent.Revision = EvaluatePattern(vnPattern.Revision, vnCurrent.Revision);
            vnCurrent.Build = EvaluatePattern(vnPattern.Build, vnCurrent.Build);
            vnCurrent.Suffix = EvaluatePattern(vnPattern.Suffix, vnCurrent.Suffix);
        }

        private string EvaluatePattern(string pattern, string currentValue)
        {
            // Each pattern may contain multiple variables.
            // Current is used by {{Inc}} and {{Same}} only.

            if (pattern == null)
                return null;

            int incIndex = 0;

            string result = variableRegex.Replace(pattern,
                match =>
                {
                    string innerVariable = match.Value.Substring(2, match.Value.Length - 4);

                    if (MatchStarts(match, "%%"))
                    {
                        // Per https://msdn.microsoft.com/en-us/library/77zkk0b6%28v=vs.110%29.aspx,
                        // environment variable names are not case-sensitive.
                        return (Environment.GetEnvironmentVariable(innerVariable) ?? "").Trim();
                    }
                    else if (MatchStarts(match, "##"))
                    {
                        // This will throw if the file does not exist.
                        string[] lines = fileSystem.ReadAllLines(innerVariable);
                        if (lines == null || lines.Length == 0)
                            return "";
                        else
                            return lines[0].Trim();
                    }
                    else if (Matches(match, "{{NowDOY}}"))
                    {
                        return GetDayOfYearString(timeProvider.Now);
                    }
                    else if (Matches(match, "{{UtcNowDOY}}"))
                    {
                        return GetDayOfYearString(timeProvider.UtcNow);
                    }
                    else if (MatchStarts(match, "{{Now"))
                    {
                        return GetDateString(innerVariable, timeProvider.Now);
                    }
                    else if (MatchStarts(match, "{{UtcNow"))
                    {
                        return GetDateString(innerVariable, timeProvider.UtcNow);
                    }
                    else if (Matches(match, "{{Inc}}"))
                    {
                        if (incReset)
                            return "0";
                        else
                        {
                            int cur = GetNextIntegerFromCurrentValue(currentValue, ref incIndex);
                            return (cur + 1).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else if (Matches(match, "{{Inc:Reset}}"))
                    {
                        incReset = true;
                        int cur = GetNextIntegerFromCurrentValue(currentValue, ref incIndex);
                        return (cur + 1).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (Matches(match, "{{MachineName}}"))
                    {
                        return Environment.MachineName;
                    }
                    else if (Matches(match, "{{OSVersion}}"))
                    {
                        return Environment.OSVersion.ToString();
                    }
                    else if (Matches(match, "{{UserName}}"))
                    {
                        return Environment.UserName;
                    }
                    else if (Matches(match, "{{UserDomainName}}"))
                    {
                        return Environment.UserDomainName;
                    }
                    else if (Matches(match, "{{Same}}"))
                    {
                        return currentValue;
                    }
                    else if (MatchStarts(match, "{{GitCommit"))
                    {
                        if (GitInfo.Commit == null)
                            return "";
                        else
                            return GetCommitString(innerVariable, GitInfo.Commit);
                    }
                    else if (Matches(match, "{{GitBranch}}"))
                    {
                        return GitInfo.Branch ?? "";
                    }
                    else
                    {
                        throw new ArgumentException("Unhandled variable pattern " + match.Value);
                    }
                });

            return result;
        }

        private VCSInfo GitInfo
        {
            get
            {
                if (vcsInfo == null)
                    vcsInfo = gitInterrogator.GetInfo();

                return vcsInfo;
            }
        }

        private bool Matches(Match match, string variable)
        {
            return match.Value.Equals(variable, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchStarts(Match match, string variableStart)
        {
            return match.Value.StartsWith(variableStart, StringComparison.OrdinalIgnoreCase);
        }

        private int GetNextIntegerFromCurrentValue(string currentValue, ref int incIndex)
        {
            int result = -1;

            if (currentValue != null)
            {
                var m = integerRegex.Match(currentValue, incIndex);
                if (m.Success)
                {
                    incIndex = m.Index + m.Length;
                    result = Convert.ToInt32(m.Value);
                }
            }

            return result;
        }

        private static string GetDayOfYearString(DateTime date)
        {
            return date.ToString("yy", CultureInfo.InvariantCulture) + date.DayOfYear.ToString("000");
        }

        private static string GetDateString(string innerVariable, DateTime date)
        {
            string variable;
            string format;
            StringExtensions.SplitOnFirst(innerVariable, ':', out variable, out format);
            if (format == null)
                format = DefaultDateFormat;

            return date.ToString(format, CultureInfo.InvariantCulture);
        }

        private static string GetCommitString(string innerVariable, string commit)
        {
            string variable;
            string length;
            StringExtensions.SplitOnFirst(innerVariable, ':', out variable, out length);
            if (length == null)
                return commit;

            int len = Convert.ToInt32(length);
            return commit.Substring(0, len);
        }
    }
}
