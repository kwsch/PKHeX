using System;

namespace PKHeX.Core
{
    /// <inheritdoc cref="ISuggestModification"/>
    public class ComplexSuggestion : ISuggestModification
    {
        public readonly string Keyword;
        public readonly Func<PKM, bool> Criteria = _ => true;
        public readonly Func<string, string, BatchInfo, ModifyResult> Action;

        public ComplexSuggestion(
            string keyword,
            Func<PKM, bool> criteria,
            Func<string, string, BatchInfo, ModifyResult> action) : this(keyword, action)
        {
            Criteria = criteria;
        }

        public ComplexSuggestion(
            string keyword,
            Func<string, string, BatchInfo, ModifyResult> action)
        {
            Keyword = keyword;
            Action = action;
        }

        public bool IsMatch(string name, string value, BatchInfo info)
        {
            return name == Keyword && Criteria(info.Entity);
        }

        public ModifyResult Modify(string name, string value, BatchInfo info)
        {
            return Action(name, value, info);
        }
    }
}
