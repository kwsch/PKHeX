using System;

namespace PKHeX.Core;

/// <inheritdoc cref="ISuggestModification"/>
public sealed class ComplexSuggestion(
    string Keyword,
    Func<string, string, BatchInfo, ModifyResult> Action)
    : ISuggestModification
{
    public readonly string Keyword = Keyword;
    public readonly Func<PKM, bool> Criteria = _ => true;
    public readonly Func<string, string, BatchInfo, ModifyResult> Action = Action;

    public ComplexSuggestion(
        string Keyword,
        Func<PKM, bool> criteria,
        Func<string, string, BatchInfo, ModifyResult> action) : this(Keyword, action)
    {
        Criteria = criteria;
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
