using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PKHeX.Core
{
    /// <summary>
    /// Bad-word Filter class containing logic to check against unsavory regular expressions.
    /// </summary>
    public static class WordFilter
    {
        /// <summary>
        /// Source pattern regexes to check with
        /// </summary>
        private static readonly string[] Patterns = Util.GetStringList("badwords");

        /// <summary>
        /// Due to some messages repeating (Trainer names), keep a list of repeated values for faster lookup.
        /// </summary>
        private static readonly Dictionary<string, string> Lookup = new(INIT_COUNT);

        private const string NoMatch = "";

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
                regMatch = NoMatch;
                return false;
            }

            var msg = message.ToLower();
            // Check dictionary
            lock (dictLock)
            {
                if (Lookup.TryGetValue(msg, out regMatch))
                    return !ReferenceEquals(regMatch, NoMatch);
            }

            // not in dictionary, check patterns
            foreach (var pattern in Patterns)
            {
                if (!Regex.IsMatch(msg, pattern))
                    continue;

                // match found, cache result
                regMatch = pattern;
                lock (dictLock)
                    Lookup[msg] = regMatch;
                return true;
            }

            // didn't match any pattern, cache result
            lock (dictLock)
            {
                if ((Lookup.Count & ~MAX_COUNT) != 0)
                    Lookup.Clear(); // reset
                Lookup[msg] = regMatch = NoMatch;
            }
            return false;
        }

        private static readonly object dictLock = new();
        private const int MAX_COUNT = (1 << 17) - 1; // arbitrary cap for max dictionary size
        private const int INIT_COUNT = 1 << 10; // arbitrary init size to limit future doublings
    }
}
