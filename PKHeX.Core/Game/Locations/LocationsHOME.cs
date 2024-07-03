using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for SW/SH met locations from HOME.
/// </summary>
public static class LocationsHOME
{
    // 60000 - (version - PLA)
    private const int RemapCount = 5; // Count of future game version IDs that can transfer back into SW/SH.
    public const ushort SHVL = 59996; // VL traded to (SW)SH
    public const ushort SWSL = 59997; // SL traded to SW(SH)
    public const ushort SHSP = 59998; // SP traded to (SW)SH
    public const ushort SWBD = 59999; // BD traded to SW(SH)
    public const ushort SWLA = 60000; // PLA traded to SW(SH)

    public const ushort SWSHEgg = 65534; // -2 = 8bNone-1..

    /// <summary>
    /// Gets the external entity version needs to be remapped into a location for SW/SH.
    /// </summary>
    /// <param name="version">Origin Game ID to be stored directly/indirectly in the PK8.</param>
    /// <returns>True if a known remap exists.</returns>
    public static bool IsVersionRemapNeeded(GameVersion version) => GetRemapIndex(version) < RemapCount;

    private static int GetRemapIndex(GameVersion version) => version - PLA;

    /// <summary>
    /// Checks if the SW/SH-context Met Location is one of the remapped HOME locations.
    /// </summary>
    public static bool IsLocationSWSH(ushort met) => met switch
    {
        SHVL or SWSL or SHSP or SWBD or SWLA => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the SW/SH-context Egg Location is valid with respect to the <see cref="original"/> location.
    /// </summary>
    public static bool IsLocationSWSHEgg(GameVersion version, ushort met, int egg, ushort original)
    {
        if (original > SWLA && egg == SWSHEgg)
            return true;

        // >60000 can be reset to Link Trade (30001), then altered differently.
        var expect = GetMetSWSH(original, version);
        return expect == met && expect == egg;
    }

    /// <summary>
    /// Gets the SW/SH-context Egg Location when an external entity from the input <see cref="version"/> resides in SW/SH.
    /// </summary>
    public static ushort GetLocationSWSHEgg(GameVersion version, ushort egg)
    {
        if (egg == 0)
            return 0;
        if (egg > SWLA)
            return SWSHEgg;
        // >60000 can be reset to Link Trade (30001), then altered differently.
        return GetMetSWSH(egg, version);
    }

    /// <summary>
    /// Gets the SW/SH-context <see cref="GameVersion"/> when an external entity from the input <see cref="version"/> resides in SW/SH.
    /// </summary>
    public static GameVersion GetVersionSWSH(GameVersion version) => version switch
    {
        GameVersion.PLA => GameVersion.SW,
        GameVersion.BD  => GameVersion.SW,
        GameVersion.SP  => GameVersion.SH,
        GameVersion.SL  => GameVersion.SW,
        GameVersion.VL  => GameVersion.SH,
        _ => version,
    };

    /// <summary>
    /// Gets the SW/SH-context Met Location when an external entity from the input <see cref="version"/> resides in SW/SH.
    /// </summary>
    public static ushort GetMetSWSH(ushort loc, GameVersion version) => version switch
    {
        GameVersion.PLA => SWLA,
        GameVersion.BD => SWBD,
        GameVersion.SP => SHSP,
        GameVersion.SL => SWSL,
        GameVersion.VL => SHVL,
        _ => loc,
    };

    public static GameVersion GetVersionSWSHOriginal(ushort loc) => loc switch
    {
        SWLA => GameVersion.PLA,
        SWBD => GameVersion.BD,
        SHSP => GameVersion.SP,
        SWSL => GameVersion.SL,
        SHVL => GameVersion.VL,
        _ => GameVersion.SW,
    };

    /// <summary>
    /// Checks if the met location is a valid location for the input <see cref="version"/>.
    /// </summary>
    /// <remarks>Relevant when an entity from BD/SP is transferred to SW/SH.</remarks>
    public static bool IsValidMetBDSP(ushort loc, GameVersion version) => loc switch
    {
        SHSP when version == SH => true,
        SWBD when version == SW => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the met location is a valid location for the input <see cref="version"/>.
    /// </summary>
    /// <remarks>Relevant when an entity from S/V is transferred to SW/SH.</remarks>
    public static bool IsValidMetSV(ushort loc, GameVersion version) => loc switch
    {
        SHVL when version == SH => true,
        SWSL when version == SW => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the location is (potentially) remapped based on visitation options.
    /// </summary>
    /// <remarks>Relevant when a side data yields SW/SH side data with a higher priority than the original (by version) side data.</remarks>
    /// <param name="original">Original context</param>
    /// <param name="current">Current context</param>
    public static LocationRemapState GetRemapState(EntityContext original, EntityContext current)
    {
        if (current == original)
            return LocationRemapState.Original;
        if (current == EntityContext.Gen8)
            return LocationRemapState.Remapped;
        return original.Generation() switch
        {
            < 8 => LocationRemapState.Original,
            8 => LocationRemapState.Either,
            _ => current is (EntityContext.Gen8a or EntityContext.Gen8b) // down
                ? LocationRemapState.Either
                : LocationRemapState.Original,
        };
    }

    public static bool IsMatchLocation(EntityContext original, EntityContext current, ushort met, int expect, GameVersion version)
    {
        var state = GetRemapState(original, current);
        return state switch
        {
            LocationRemapState.Original => met == expect,
            LocationRemapState.Remapped => met == GetMetSWSH((ushort)expect, version),
            LocationRemapState.Either => met == expect || met == GetMetSWSH((ushort)expect, version),
            _ => false,
        };
    }
}

[Flags]
public enum LocationRemapState
{
    None,
    Original = 1 << 0,
    Remapped = 1 << 1,
    Either = Original | Remapped,
}
