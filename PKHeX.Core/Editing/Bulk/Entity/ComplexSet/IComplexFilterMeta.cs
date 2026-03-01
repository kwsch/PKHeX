using System;

namespace PKHeX.Core;

/// <summary>
/// Complex filter of data based on a string value.
/// </summary>
public interface IComplexFilterMeta
{
    bool IsMatch(ReadOnlySpan<char> prop);
    bool IsFiltered(object cache, StringInstruction value);
}
