using System;

namespace PKHeX.Core;

/// <summary>
/// Complex modification of data to a string value.
/// </summary>
public interface IComplexSet
{
    bool IsMatch(ReadOnlySpan<char> name, ReadOnlySpan<char> value);
    void Modify(PKM pk, StringInstruction instr);
}
