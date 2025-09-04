using static PKHeX.Core.PIDType;

namespace PKHeX.Core;

/// <summary>
/// Logic to revise a <see cref="PIDIV"/> based on observed <see cref="PIDType"/> and constraining <see cref="PIDType"/>
/// </summary>
public static class CommonEvent3Checker
{
    /// <summary>
    /// Checks if the observed type and seed value match the constraints of the encounter.
    /// </summary>
    /// <remarks>Regular Antishiny Table constraint.</remarks>
    /// <param name="value">PID/IV observed</param>
    /// <param name="observed">Observed PID Type</param>
    /// <param name="species">Encounter Species</param>
    /// <param name="isWish">Encounter moveset contains the move Wish</param>
    /// <returns>True if is match; mutation not indicated.</returns>
    public static bool IsRestrictedTable2(ref PIDIV value, PIDType observed, ushort species, bool isWish)
    {
        // Same method as IsRestrictedTable3, except shiny is disallowed.
        if (observed is not (BACD or BACD_A or BACD_EA)) // regular/anti-shiny, or anti-shiny before trade & hatch
            return false;
        return IsRestrictedTableMatch(ref value, species, isWish, false);
    }

    /// <remarks>Force Shiny Table constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsRestrictedTable3(ref PIDIV value, PIDType observed, ushort species, bool isWish)
    {
        // Same method as IsRestrictedTable2, except it should be shiny.
        if (observed is not (BACD_S or BACD_ES)) // shiny or shiny before trade & hatch
            return false;
        return IsRestrictedTableMatch(ref value, species, isWish, true);
    }

    private static bool IsRestrictedTableMatch(ref PIDIV value, ushort species, bool isWish, bool isShiny)
    {
        var seed = value.OriginSeed;
        var origin = LCRNG.Prev2(seed);
        if (origin > ushort.MaxValue)
            return false;

        // Only 2 events distributed with table rand.
        // Negai Boshi Jirachi was filled with the same entry.
        // PCJP Fifth Anniversary had a few entries, will need to check.
        if (species is not (ushort)Species.Jirachi)
        {
            // Otherwise, check the table to see if it is valid.
            if (!IsMatchTableRand(species, isWish, (ushort)origin, isShiny))
                return false;
        }
        value = value.AsMutated(BACD_R, origin);
        return true;
    }

    private static bool IsMatchTableRand(ushort species, bool isWish, ushort seed16, bool isShiny)
    {
        var index = PCJPFifthAnniversary.GetIndex(species);
        var result = PCJPFifthAnniversary.GetResult(seed16);
        if (result.Shiny != isShiny)
            return false;
        if (result.Index != index)
            return false;
        if (result.Wish != isWish)
            return false;
        return true;
    }

    /// <remarks>Mystry Mew constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsMystryMew(ref PIDIV value, PIDType observed)
    {
        if (observed is not BACD)
            return false;
        var seed = value.OriginSeed;
        // Reverse back to origin seed.
        var info = MystryMew.GetSeedIndexes(seed);
        if (info.Index == -1)
            return false;
        seed = MystryMew.IsRestrictedIndex0th(info.SubIndex) ? seed : MystryMew.GetSeed(info.Index);
        if (!MystryMew.IsValidSeed(seed, info.SubIndex))
            return false;
        value = value.AsMutated(BACD_R, seed);
        return true;
    }

    /// <remarks>CHANNEL Jirachi constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsChannelJirachi(ref PIDIV value, PIDType observed)
    {
        if (observed is not Channel)
            return false;
        var seed = value.OriginSeed;

        var chk = ChannelJirachi.GetPossible(seed);
        if (chk.Pattern is ChannelJirachiRandomResult.None)
            return false;
        value = value.AsMutated(Channel, chk.Seed);
        return true;
    }

    /// <remarks>Regular BACD (WISHMKR Jirachi, Eggs) constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsRestrictedSimple(ref PIDIV value, PIDType observed)
    {
        if (observed is not BACD)
            return false;
        if (value.OriginSeed > ushort.MaxValue)
            return false;
        value = value.AsMutated(BACD_R);
        return true;
    }

    /// <remarks>Regular Antishiny without seed restriction constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsUnrestrictedAnti(ref PIDIV value, PIDType observed)
    {
        if (observed is not (BACD or BACD_A or BACD_EA))
            return false;
        value = value.AsMutated(BACD_U);
        return true;
    }

    /// <remarks>Regular Antishiny with seed restriction constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsRestrictedAnti(ref PIDIV value, PIDType observed)
    {
        if (observed is not (BACD or BACD_A or BACD_EA))
            return false;
        if (value.OriginSeed > ushort.MaxValue)
            return false;
        value = value.AsMutated(BACD_R);
        return true;
    }

    /// <remarks>Regular Antishiny without seed restriction constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsUnrestrictedAntiX(ref PIDIV value, PIDType observed)
    {
        if (observed is not (BACD_AX))
            return false;
        value = value.AsMutated(BACD_AX);
        return true;
    }

    /// <remarks>Berry Fix Zigzagoon seed restriction constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsBerryFixShiny(ref PIDIV value, PIDType observed)
    {
        if (observed is not BACD_S)
            return false;
        if (value.OriginSeed is < 3 or > 213) // Binary Coded Decimal sum of timestamp must be possible.
            return false;
        value = value.AsMutated(BACD_R);
        return true;
    }
}
