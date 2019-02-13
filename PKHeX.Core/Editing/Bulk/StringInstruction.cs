using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public class StringInstruction
    {
        public string PropertyName { get; private set; }
        public string PropertyValue { get; private set; }
        public bool Evaluator { get; private set; }

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
        public const char SplitInstruction = '=';

        // Extra Functionality
        private int Min, Max;
        public bool Random { get; private set; }
        public int RandomValue => Util.Rand.Next(Min, Max + 1);

        public void SetRandRange(string pv)
        {
            string str = pv.Substring(1);
            var split = str.Split(SplitRange);
            int.TryParse(split[0], out Min);
            int.TryParse(split[1], out Max);

            if (Min == Max)
            {
                PropertyValue = Min.ToString();
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
                select new StringInstruction { PropertyName = split[0], PropertyValue = split[1], Evaluator = eval };
        }

        public static IEnumerable<StringInstruction> GetInstructions(IEnumerable<string> lines)
        {
            var raw = GetRelevantStrings(lines, Apply).Select(line => line.Substring(1));
            return from line in raw
                select line.Split(SplitInstruction) into split
                where split.Length == 2
                select new StringInstruction { PropertyName = split[0], PropertyValue = split[1] };
        }

        private static IEnumerable<string> GetRelevantStrings(IEnumerable<string> lines, params char[] pieces)
        {
            return lines.Where(line => !string.IsNullOrEmpty(line) && pieces.Any(z => z == line[0]));
        }
    }
}
