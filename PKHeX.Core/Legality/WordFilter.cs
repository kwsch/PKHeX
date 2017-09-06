using System.Text.RegularExpressions;

namespace PKHeX.Core
{
    public static class WordFilter
    {
        private static readonly string[] Patterns = Util.GetStringList("badwords");
        public static bool IsFiltered(string message, out string regMatch)
        {
            foreach (var pattern in Patterns)
            {
                if (!Regex.IsMatch(message, pattern))
                    continue;
                regMatch = pattern;
                return true;
            }
            regMatch = null;
            return false;
        }
    }
}
