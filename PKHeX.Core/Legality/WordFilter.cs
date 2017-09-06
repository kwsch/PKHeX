using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PKHeX.Core
{
    public static class WordFilter
    {
        /// <summary>
        /// Source pattern regexes to check with
        /// </summary>
        private static readonly string[] Patterns = Util.GetStringList("badwords");

        /// <summary>
        /// Due to some messages repeating (Trainer names), keep a list of repeated values for faster lookup.
        /// </summary>
        private static readonly Dictionary<string, string> Lookup = new Dictionary<string, string>();

        /// <summary>
        /// Checks to see if a phrase contains filtered content.
        /// </summary>
        /// <param name="message">Phrase to check for</param>
        /// <param name="regMatch">Matching regex that filters the phrase.</param>
        /// <returns>Boolean result if the message is filtered or not.</returns>
        public static bool IsFiltered(string message, out string regMatch)
        {
            if (string.IsNullOrWhiteSpace(message) || message.Length <= 1)
            {
                regMatch = null;
                return false;
            }

            var msg = message.ToLower();
            // Check dictionary
            if (Lookup.TryGetValue(msg, out regMatch))
                return regMatch != null;

            foreach (var pattern in Patterns)
            {
                if (!Regex.IsMatch(msg, pattern))
                    continue;
                regMatch = pattern;
                Lookup.Add(msg, regMatch);
                return true;
            }

            if (Lookup.Count > 100_000) // arbitrary cap
                Lookup.Clear(); // reset
            Lookup.Add(msg, regMatch = null);
            return false;
        }
    }
}
