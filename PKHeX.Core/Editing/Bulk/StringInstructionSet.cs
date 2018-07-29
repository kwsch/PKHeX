using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class StringInstructionSet
    {
        public IList<StringInstruction> Filters { get; private set; }
        public IList<StringInstruction> Instructions { get; private set; }

        private const string SetSeparator = ";";

        public static IEnumerable<StringInstructionSet> GetBatchSets(IList<string> lines)
        {
            int start = 0;
            while (start < lines.Count)
            {
                var list = lines.Skip(start).TakeWhile(_ => !lines[start++].StartsWith(SetSeparator)).ToList();
                yield return GetBatchSet(list);
            }
        }

        private static StringInstructionSet GetBatchSet(ICollection<string> set)
        {
            return new StringInstructionSet
            {
                Filters = StringInstruction.GetFilters(set).ToList(),
                Instructions = StringInstruction.GetInstructions(set).ToList(),
            };
        }
    }
}
