using System.Text;
using System.Text.RegularExpressions;

namespace SetVersion.Lib
{
    public static class RegexExtensions
    {
        /// <summary>
        /// Replaces the first matching group in an input string.
        /// </summary>
        /// <param name="regex">The regex. Should contain a named group.</param>
        /// <param name="input">The input.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="replacement">The replacement text.</param>
        /// <returns>String with the text replaced.</returns>
        public static string ReplaceFirstGroup(Regex regex, string input, string groupName, string replacement)
        {
            Val.ThrowIfNull(regex, nameof(regex));
            Val.ThrowIfNull(input, nameof(input));
            Val.ThrowIfNull(groupName, nameof(groupName));
            Val.ThrowIfNull(replacement, nameof(replacement));

            const int firstMatchOnly = 1;

            return regex.Replace(
                input,
                m =>
                {
                    var group = m.Groups[groupName];
                    var sb = new StringBuilder();
                    var previousCaptureEnd = 0;
                    foreach (var c in group.Captures)
                    {
                        Capture capture = c as Capture;
                        var currentCaptureEnd = capture.Index + capture.Length - m.Index;
                        var currentCaptureLength = capture.Index - m.Index - previousCaptureEnd;
                        sb.Append(m.Value.Substring(previousCaptureEnd, currentCaptureLength));
                        sb.Append(replacement);
                        previousCaptureEnd = currentCaptureEnd;
                    }

                    sb.Append(m.Value.Substring(previousCaptureEnd));

                    return sb.ToString();
                },
                firstMatchOnly
                );
        }
    }
}
