using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 5+ specific met location name holder.
/// </summary>
/// <remarks>Multi-segment, large gaps.</remarks>
/// <param name="Met0">List of location names for bank 0.</param>
/// <param name="Met3">List of location names for bank 3 (30000).</param>
/// <param name="Met4">List of location names for bank 4 (40000).</param>
/// <param name="Met6">List of location names for bank 6 (60000).</param>
public sealed record LocationSet6(string[] Met0, string[] Met3, string[] Met4, string[] Met6) : ILocationSet
{
    public ReadOnlySpan<string> GetLocationNames(int bankID) => bankID switch
    {
        0 => Met0,
        3 => Met3,
        4 => Met4,
        6 => Met6,
        _ => [],
    };

    public string GetLocationName(int locationID) => locationID switch
    {
        >= 60000 => Get(Met6, locationID - 60000),
        >= 40000 => Get(Met4, locationID - 40000),
        >= 30000 => Get(Met3, locationID - 30000),
        _ => Get(Met0, locationID),
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
        yield return (3, Met3);
        yield return (4, Met4);
        yield return (6, Met6);
    }
}
