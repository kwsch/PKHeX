using System;

namespace PKHeX.Core;

/// <inheritdoc cref="ISuggestModification"/>
/// <typeparam name="T">Specific (or not) type</typeparam>
public sealed class TypeSuggestion<T>(string Keyword, Action<T> Action) : ISuggestModification
{
    public readonly string Keyword = Keyword;
    public readonly Action<T, string> Action = (pk, _) => Action(pk);
    public readonly Func<T, bool> Criteria = _ => true;

    public TypeSuggestion(string Keyword, Func<T, bool> criteria, Action<T> action) : this(Keyword, action)
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
