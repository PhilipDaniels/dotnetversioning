using System;
using System.Diagnostics;

namespace SetVersion.Lib
{
    /// <summary>
    /// Represents a version number string, such as "1.2.3-pre" or "1.2.3.4-pre".
    /// The full format is "MAJOR.MINOR.REVISION.BUILD"
    /// Note that this class is also used to parse patterns, so the components
    /// can be variables such as "{{Inc}}".
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class VersionString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionString"/> class.
        /// </summary>
        /// <param name="versionNumber">The version number.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="versionNumber"/> is null.</exception>
        public VersionString(string versionNumber)
        {
            Val.ThrowIfNull(versionNumber, nameof(versionNumber));

            int idx = 0;
            Major = Eat(versionNumber, ref idx);
            if (Major == null)
            {
                Major = "";
                return;
            }

            Minor = Eat(versionNumber, ref idx);
            Revision = Eat(versionNumber, ref idx);
            // Consume the remainder.
            if (idx < versionNumber.Length)
                Build = versionNumber.Substring(idx);
        }

        /// <summary>
        /// Represents the Major component of the version string.
        /// For example, in the string "1.2.3.4-pre" Major is "1".
        /// </summary>
        /// <value>
        /// The Major component of the version string.
        /// </value>
        public string Major { get; set; }

        /// <summary>
        /// Represents the Minor component of the version string.
        /// For example, in the string "1.2.3.4-pre" Minor is "2".
        /// </summary>
        /// <value>
        /// The Minor component of the version string.
        /// </value>
        public string Minor { get; set; }

        /// <summary>
        /// Represents the Revision component of the version string.
        /// For example, in the string "1.2.3.4-pre" Revision is "3".
        /// </summary>
        /// <value>
        /// The Revision component of the version string.
        /// </value>
        public string Revision { get; set; }

        /// <summary>
        /// Represents the Build component of the version string.
        /// For example, in the string "1.2.3.4-pre" Build is "4-pre".
        /// </summary>
        /// <value>
        /// The Build component of the version string.
        /// </value>
        public string Build { get; set; }

        /// <summary>
        /// The full version number.
        /// </summary>
        /// <returns>
        /// The full version number.
        /// </returns>
        public override string ToString()
        {
            string s = Major;
            if (Minor != null)
                s += "." + Minor;
            if (Revision != null)
                s += "." + Revision;
            if (Build != null)
                s += "." + Build;

            return s;
        }

        private string Eat(string versionNumber, ref int startIdx)
        {
            if (startIdx >= versionNumber.Length)
                return null;

            int start = startIdx;

        again:
            int idx = StringExtensions.IndexOfAny(versionNumber, startIdx, ".", "{{", "##", "%%");
            if (idx == -1)
            {
                startIdx = versionNumber.Length;
                return versionNumber.Substring(start);
            }

            char c = versionNumber[idx];
            if (c == '.')
            {
                // The first thing we found was a field separator. Just eat the front bit.
                startIdx = idx + 1;
                return versionNumber.Substring(start, idx - start);
            }
            else if (c == '{' || c == '#' || c == '%')
            {
                // The first thing we found was a variable start. Try and skip to the
                // end of the variable and start again from there.
                string end = VarEnd(c);
                idx = versionNumber.IndexOf(end, idx + 2);
                if (idx == -1)
                {
                    // Could not find end of the variable, consume all the remaining input.
                    startIdx = versionNumber.Length;
                    return versionNumber.Substring(start);
                }
                else
                {
                    // Found the end of the variable, so start looking for a '.' again just after it.
                    startIdx = idx + 2;
                    goto again;
                }
            }
            else
            {
                throw new InvalidOperationException("Should not reach here.");
            }
        }

        private string VarEnd(char c)
        {
            if (c == '{')
                return "}}";
            else if (c == '#')
                return "##";
            else if (c == '%')
                return "%%";

            throw new ArgumentOutOfRangeException(nameof(c));
        }
    }
}
