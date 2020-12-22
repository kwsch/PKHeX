using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Batch Editing instruction
    /// </summary>
    /// <remarks>
    /// Can be a filter (skip), or a modification instruction (modify)
    /// </remarks>
    /// <see cref="Exclude"/>
    /// <see cref="Require"/>
    /// <see cref="Apply"/>
    public sealed class StringInstruction
    {
        public string PropertyName { get; }
        public string PropertyValue { get; private set; }
        public bool Evaluator { get; private init; }

        public StringInstruction(string name, string value)
        {
            PropertyName = name;
            PropertyValue = value;
        }

        public void SetScreenedValue(string[] arr)
        {
            int index = Array.IndexOf(arr, PropertyValue);
            PropertyValue = index > -1 ? index.ToString() : PropertyValue;
        }

        public static readonly IReadOnlyList<char> Prefixes = new[] { Apply, Require, Exclude };
        private const char Exclude = '!';
        private const char Require = '=';
        private const char Apply = '.';
        private const char SplitRange = ',';

        /// <summary>
        /// Character which divides a property and a value.
        /// </summary>
        /// <remarks>
        /// Example:
        /// =Species=1
        /// The second = is the split.
        /// </remarks>
        public const char SplitInstruction = '=';

        // Extra Functionality
        private int RandomMinimum, RandomMaximum;
        public bool Random { get; private set; }
        public int RandomValue => Util.Rand.Next(RandomMinimum, RandomMaximum + 1);

        public void SetRandRange(string pv)
        {
            string str = pv.Substring(1);
            var split = str.Split(SplitRange);
            int.TryParse(split[0], out RandomMinimum);
            int.TryParse(split[1], out RandomMaximum);

            if (RandomMinimum == RandomMaximum)
            {
                PropertyValue = RandomMinimum.ToString();
                Debug.WriteLine(PropertyName + " randomization range Min/Max same?");
            }
            else
            {
                Random = true;
            }
        }

        public static IEnumerable<StringInstruction> GetFilters(IEnumerable<string> lines)
        {
            var raw = GetRelevantStrings(lines, Exclude, Require);
            return from line in raw
                let eval = line[0] == Require
                let split = line.Substring(1).Split(SplitInstruction)
                where split.Length == 2 && !string.IsNullOrWhiteSpace(split[0])
                select new StringInstruction(split[0], split[1]) { Evaluator = eval };
        }

        public static IEnumerable<StringInstruction> GetInstructions(IEnumerable<string> lines)
        {
            var raw = GetRelevantStrings(lines, Apply).Select(line => line.Substring(1));
            return from line in raw
                select line.Split(SplitInstruction) into split
                where split.Length == 2
                select new StringInstruction(split[0], split[1]);
        }

        /// <summary>
        /// Weeds out invalid lines and only returns those with a valid first character.
        /// </summary>
        private static IEnumerable<string> GetRelevantStrings(IEnumerable<string> lines, params char[] pieces)
        {
            return lines.Where(line => !string.IsNullOrEmpty(line) && pieces.Any(z => z == line[0]));
        }
    }
}
