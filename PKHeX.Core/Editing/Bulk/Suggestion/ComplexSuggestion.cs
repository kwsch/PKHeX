using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <inheritdoc cref="ISuggestModification"/>
public sealed class ComplexSuggestion(
    [ConstantExpected] string Keyword,
    Func<ReadOnlySpan<char>, ReadOnlySpan<char>, BatchInfo, ModifyResult> Action)
    : ISuggestModification
{
    public readonly string Keyword = Keyword;
    public readonly Func<PKM, bool> Criteria = _ => true;
    public readonly Func<ReadOnlySpan<char>, ReadOnlySpan<char>, BatchInfo, ModifyResult> Action = Action;

    public ComplexSuggestion(
        [ConstantExpected] string Keyword,
        Func<PKM, bool> criteria,
        Func<ReadOnlySpan<char>, ReadOnlySpan<char>, BatchInfo, ModifyResult> action) : this(Keyword, action)
    {
        Criteria = criteria;
    }

    public bool IsMatch(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info)
    {
        return name.SequenceEqual(Keyword) && Criteria(info.Entity);
    }

    public ModifyResult Modify(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info)
    {
        return Action(name, value, info);
    }
}
