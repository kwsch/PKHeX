using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// String Translation Utility
    /// </summary>
    public static class RibbonStrings
    {
        private static readonly Dictionary<string, string> RibbonNames = new();

        /// <summary>
        /// Resets the Ribbon Dictionary to use the supplied set of Ribbon (Property) Names.
        /// </summary>
        /// <param name="lines">Array of strings that are tab separated with Property Name, \t, and Display Name.</param>
        public static void ResetDictionary(IEnumerable<string> lines)
        {
            // Don't clear existing keys on reset; only update.
            // A language will have the same keys (hopefully), only with differing values.
            foreach (var line in lines)
            {
                var index = line.IndexOf('\t');
                if (index < 0)
                    continue;
                var name = line[..index];
                var text = line[(index + 1)..];
                RibbonNames[name] = text;
            }
        }

        /// <summary>
        /// Returns the Ribbon Display Name for the corresponding <see cref="PKM"/> ribbon property name.
        /// </summary>
        /// <param name="propertyName">Ribbon property name</param>
        /// <returns>Ribbon display name</returns>
        public static string GetName(string propertyName)
        {
            // Throw an exception with the requested property name as the message, rather than an ambiguous "key not present" message.
            // We should ALWAYS have the key present as the input arguments are not user-defined, rather, they are from PKM property names.
            if (!RibbonNames.TryGetValue(propertyName, out string value))
                throw new KeyNotFoundException(propertyName);
            return value;
        }
    }
}
