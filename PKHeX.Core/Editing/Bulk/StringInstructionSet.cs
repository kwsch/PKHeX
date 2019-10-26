using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Processes input of strings into a list of valid Filters and Instructions.
    /// </summary>
    public sealed class StringInstructionSet
    {
        public readonly IReadOnlyList<StringInstruction> Filters;
        public readonly IReadOnlyList<StringInstruction> Instructions;

        private const string SetSeparator = ";";

        public StringInstructionSet(IReadOnlyList<StringInstruction> filters, IReadOnlyList<StringInstruction> instructions)
        {
            Filters = filters;
            Instructions = instructions;
        }

        public StringInstructionSet(ICollection<string> set)
        {
            Filters = StringInstruction.GetFilters(set).ToList();
            Instructions = StringInstruction.GetInstructions(set).ToList();
        }

        public static IEnumerable<StringInstructionSet> GetBatchSets(IList<string> lines)
        {
            int start = 0;
            while (start < lines.Count)
            {
                var list = lines.Skip(start).TakeWhile(_ => !lines[start++].StartsWith(SetSeparator)).ToList();
                yield return new StringInstructionSet(list);
            }
        }
    }
}
