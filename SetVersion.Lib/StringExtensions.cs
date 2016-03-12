namespace SetVersion.Lib
{
    public static class StringExtensions
    {
        public static void SplitOnFirst(string value, char splitChar, out string before, out string after)
        {
            int idx = value.IndexOf(splitChar);
            if (idx == -1)
            {
                before = value;
                after = null;
            }
            else
            {
                before = value.Substring(0, idx);
                after = value.Substring(idx + 1);
            }
        }

        public static int IndexOfAny(string value, int start, params string[] searchStrings)
        {
            int idx = -1;

            foreach (var s in searchStrings)
            {
                int i = value.IndexOf(s, start);
                if (idx == -1)
                {
                    idx = i;
                }
                else
                {
                    if (i != -1 && i < idx)
                        idx = i;
                }
            }

            return idx;
        }
    }
}
