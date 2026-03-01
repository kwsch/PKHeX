using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <inheritdoc cref="ISuggestModification"/>
/// <typeparam name="T">Specific (or not) type</typeparam>
public sealed class TypeSuggestion<T>([ConstantExpected] string Keyword, Action<T> Action) : ISuggestModification
{
    public readonly string Keyword = Keyword;
    public readonly Action<T, ReadOnlySpan<char>> Action = (pk, _) => Action(pk);
    public readonly Func<T, bool> Criteria = _ => true;

    public TypeSuggestion([ConstantExpected] string Keyword, Func<T, bool> criteria, Action<T> action) : this(Keyword, action)
    {
        Criteria = criteria;
    }

    public bool IsMatch(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info)
    {
        return name.SequenceEqual(Keyword) && info.Entity is T;
    }

    public ModifyResult Modify(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info)
    {
        var pk = info.Entity;
        if (pk is not T x)
            return ModifyResult.Skipped;
        if (!Criteria(x))
            return ModifyResult.Skipped;
        Action(x, value);
        return ModifyResult.Modified;
    }
}
