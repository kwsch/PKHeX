using System;

namespace PKHeX.Core
{
    /// <inheritdoc cref="ISuggestModification"/>
    /// <typeparam name="T">Specific (or not) type</typeparam>
    public class TypeSuggestion<T> : ISuggestModification where T : PKM
    {
        public readonly string Keyword;
        public readonly Action<T, string> Action;
        public readonly Func<T, bool> Criteria = _ => true;

        public TypeSuggestion(string keyword, Action<T> action)
        {
            Keyword = keyword;
            Action = (pkm, _) => action(pkm);
        }

        public TypeSuggestion(string keyword, Func<T, bool> criteria, Action<T> action) : this(keyword, action)
        {
            Criteria = criteria;
        }

        public bool IsMatch(string name, string value, BatchInfo info)
        {
            return name == Keyword && info.Entity is T;
        }

        public ModifyResult Modify(string name, string value, BatchInfo info)
        {
            var pk = (T) info.Entity;
            if (!Criteria(pk))
                return ModifyResult.Invalid;
            Action(pk, value);
            return ModifyResult.Modified;
        }
    }
}