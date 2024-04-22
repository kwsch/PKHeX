using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 specific met location name holder.
/// </summary>
/// <remarks>Multi-segment, small gaps.</remarks>
public sealed record LocationSet4(string[] Met0, string[] Met2, string[] Met3) : ILocationSet
{
    public ReadOnlySpan<string> GetLocationNames(int bankID) => bankID switch
    {
        0 => Met0,
        2 => Met2,
        3 => Met3,
        _ => [],
    };

    public string GetLocationName(int locationID) => locationID switch
    {
        >= 3000 => Get(Met3, locationID - 3000),
        >= 2000 => Get(Met2, locationID - 2000),
        _       => Get(Met0, locationID),
    };

    private static string Get(ReadOnlySpan<string> names, int index)
    {
        if ((uint)index >= names.Length)
            return string.Empty;
        return names[index];
    }

    public IEnumerable<(int Bank, ReadOnlyMemory<string> Names)> GetAll()
    {
        yield return (0, Met0);
        yield return (2, Met2);
        yield return (3, Met3);
    }
}
