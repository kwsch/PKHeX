using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// String Translation Utility
    /// </summary>
    public static class RibbonStrings
    {
        private static readonly Dictionary<string, string> RibbonNames = new Dictionary<string, string>();

        /// <summary>
        /// Resets the Ribbon Dictionary to use the supplied set of Ribbon (Property) Names.
        /// </summary>
        /// <param name="lines">Array of strings that are tab separated with Property Name, \t, and Display Name.</param>
        public static void ResetDictionary(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                string[] split = line.Split('\t');
                if (split.Length != 2)
                    continue;
                RibbonNames[split[0]] = split[1];
            }
        }

        /// <summary>
        /// Returns the Ribbon Display Name for the corresponding <see cref="PKM"/> ribbon property name.
        /// </summary>
        /// <param name="propertyName">Ribbon property name</param>
        /// <returns>Ribbon display name</returns>
        public static string GetName(string propertyName)
        {
            if (!RibbonNames.TryGetValue(propertyName, out string value))
                throw new ArgumentException(propertyName);
            return value;
        }
    }
}
