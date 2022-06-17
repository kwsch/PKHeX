using System;

namespace PKHeX.Core;

/// <inheritdoc cref="ISuggestModification"/>
/// <typeparam name="T">Specific (or not) type</typeparam>
public sealed class TypeSuggestion<T> : ISuggestModification
{
    public readonly string Keyword;
    public readonly Action<T, string> Action;
    public readonly Func<T, bool> Criteria = _ => true;

    public TypeSuggestion(string keyword, Action<T> action)
    {
        Keyword = keyword;
        Action = (pk, _) => action(pk);
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
        var pk = info.Entity;
        if (pk is not T x)
            return ModifyResult.Invalid;
        if (!Criteria(x))
            return ModifyResult.Invalid;
        Action(x, value);
        return ModifyResult.Modified;
    }
}
