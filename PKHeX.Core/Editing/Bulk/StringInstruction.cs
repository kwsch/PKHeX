using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        /// <summary> True if ==, false if != </summary>
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
            string str = pv[1..];
            var split = str.Split(SplitRange);
            int.TryParse(split[0], out RandomMinimum);
            int.TryParse(split[1], out RandomMaximum);

            if (RandomMinimum == RandomMaximum)
            {
                PropertyValue = RandomMinimum.ToString();
                Debug.WriteLine($"{PropertyName} randomization range Min/Max same?");
            }
            else
            {
                Random = true;
            }
        }

        public static IEnumerable<StringInstruction> GetFilters(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (line.Length is 0 || line[0] is not (Exclude or Require))
                    continue;

                const int start = 1;
                var splitIndex = line.IndexOf(SplitInstruction, start);
                if (splitIndex == -1)
                    continue;
                var noExtra = line.IndexOf(SplitInstruction, splitIndex + 1);
                if (noExtra != -1)
                    continue;

                var name = line.AsSpan(start, splitIndex - start);
                if (name.IsWhiteSpace())
                    continue;

                bool eval = line[0] == Require;
                var value = line[(splitIndex + 1)..];
                yield return new StringInstruction(name.ToString(), value) { Evaluator = eval };
            }
        }

        public static IEnumerable<StringInstruction> GetInstructions(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (line.Length is 0 || line[0] is not Apply)
                    continue;

                const int start = 1;
                var splitIndex = line.IndexOf(SplitInstruction, start);
                if (splitIndex == -1)
                    continue;
                var noExtra = line.IndexOf(SplitInstruction, splitIndex + 1);
                if (noExtra != -1)
                    continue;

                var name = line.AsSpan(start, splitIndex - start);
                if (name.IsWhiteSpace())
                    continue;

                var value = line[(splitIndex + 1)..];
                yield return new StringInstruction(name.ToString(), value);
            }
        }
    }
}
