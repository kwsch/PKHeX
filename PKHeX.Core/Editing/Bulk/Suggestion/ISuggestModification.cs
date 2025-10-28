using System;

namespace PKHeX.Core;

/// <summary>
/// Modifies a property to have a "correct" value based on derived legality.
/// </summary>
public interface ISuggestModification
{
    public bool IsMatch(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info);
    public ModifyResult Modify(ReadOnlySpan<char> name, ReadOnlySpan<char> value, BatchInfo info);
}
