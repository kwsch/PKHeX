using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 2-3 specific met location name holder.
/// </summary>
/// <remarks>Single segment, no shift bias.</remarks>
public sealed record LocationSet0(string[] Met0) : ILocationSet
{
    public ReadOnlySpan<string> GetLocationNames(int bankID) => bankID switch
    {
        0 => Met0,
        _ => [],
    };

    public string GetLocationName(int locationID)
    {
        if ((uint)locationID >= Met0.Length)
            return string.Empty;
        return Met0[locationID];
    }

    public IEnumerable<(int Bank, ReadOnlyMemory<string> Names)> GetAll()
    {
        yield return (0, Met0);
    }
}
