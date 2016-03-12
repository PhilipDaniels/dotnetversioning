using System.Text;

namespace SetVersion.Lib
{
    public static class StringBuilderExtensions
    {
        public static void AddJson(StringBuilder sb, string key, string value)
        {
            Val.ThrowIfNull(sb, nameof(sb));
            Val.ThrowIfNull(key, nameof(key));
            Val.ThrowIfNull(value, nameof(value));

            sb.Append('"');
            sb.Append(key);
            sb.Append('"');
            sb.Append(": ");
            sb.Append('"');
            sb.Append(value);
            sb.Append('"');
        }
    }
}
