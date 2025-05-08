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
        if (observed is not (BACD or BACD_A or BACD_EA))
            return false;

        var seed = value.OriginSeed;
        var prev2 = LCRNG.Prev2(seed);
        if (prev2 > ushort.MaxValue)
            return false;
        if (species is (ushort)Species.Jirachi)
            return true;

        var index = PCJPFifthAnniversary.GetIndexPCJP(species);
        var combined = WeightedTable3.GetRandom32(prev2);
        var result = PCJPFifthAnniversary.GetResultPCJP(combined);
        if (result.Shiny)
            return false;
        if (result.Index != index)
            return false;
        if (result.Wish != isWish)
            return false;
        value = value.AsMutated(BACD_R, prev2);
        return true;
    }

    /// <remarks>Force Shiny Table constraint.</remarks>
    /// <inheritdoc cref="IsRestrictedTable2"/>
    public static bool IsRestrictedTable3(ref PIDIV value, PIDType observed, ushort species, bool isWish)
    {
        if (observed is not (BACD_S or BACD_ES))
            return false;
        var seed = value.OriginSeed;
        var prev3 = LCRNG.Prev2(seed);
        if (prev3 > ushort.MaxValue)
            return false;
        if (species is (ushort)Species.Jirachi)
            return true;

        var index = PCJPFifthAnniversary.GetIndexPCJP(species);
        seed = prev3;
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        var combined = (rand1 << 16) | rand2;
        var result = PCJPFifthAnniversary.GetResultPCJP(combined);
        if (!result.Shiny)
            return false;
        if (result.Index != index)
            return false;
        if (result.Wish != isWish)
            return false;
        value = value.AsMutated(BACD_R, prev3);
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
