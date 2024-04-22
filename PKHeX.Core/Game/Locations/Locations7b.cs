using System;

namespace PKHeX.Core;

/// <summary>
/// Locations for <see cref="GameVersion.GG"/>.
/// </summary>
public static class Locations7b
{
    /// <summary>
    /// Available location list for the 00000 set of location names.
    /// </summary>
    public static ReadOnlySpan<byte> Met0 =>
    [
                  002, 003, 004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053,
    ];

    /// <summary>
    /// Available location list for the 30000 set of location names.
    /// </summary>
    public static ReadOnlySpan<ushort> Met3 =>
    [
               30001,        30003, 30004, 30005, 30006, 30007, 30008, 30009,
        30010, 30011, 30012, 30013, 30014, 30015, 30016, 30017,
    ];

    /// <summary>
    /// Available location list for the 40000 set of location names.
    /// </summary>
    public static ReadOnlySpan<ushort> Met4 =>
    [
               40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009,
        40010, 40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019,
        40020, 40021, 40022, 40023, 40024, 40025, 40026, 40027, 40028, 40029,
        40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039,
        40040, 40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049,
        40050, 40051, 40052, 40053, 40054, 40055, 40056, 40057, 40058, 40059,
        40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069,
        40070, 40071, 40072, 40073, 40074, 40075, 40076, 40077,
    ];

    /// <summary>
    /// Available location list for the 60000 set of location names.
    /// </summary>
    public static ReadOnlySpan<ushort> Met6 => [/* X/Y */ 60001, 60003, /* OR/AS */ 60004];
}
