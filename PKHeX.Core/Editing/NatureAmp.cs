using System;
using static PKHeX.Core.NatureAmpRequest;

namespace PKHeX.Core;

/// <summary>
/// Logic for mutating a nature to amplify certain stats.
/// </summary>
public static class NatureAmp
{
    /// <summary>
    /// Mutate the nature amp indexes to match the request
    /// </summary>
    /// <param name="type">Request type to modify the provided <see cref="statIndex"/></param>
    /// <param name="statIndex">Stat Index to mutate</param>
    /// <param name="currentNature">Current nature to derive the current amps from</param>
    /// <returns>New nature value</returns>
    public static int GetNewNature(this NatureAmpRequest type, int statIndex, int currentNature)
    {
        if (currentNature > (int)Nature.Quirky)
            return -1;

        var (up, dn) = GetNatureModification(currentNature);

        return GetNewNature(type, statIndex, up, dn);
    }

    /// <inheritdoc cref="GetNewNature(NatureAmpRequest,int,int)"/>
    public static int GetNewNature(NatureAmpRequest type, int statIndex, int up, int dn)
    {
        // 
        switch (type)
        {
            case Increase when up != statIndex:
                up = statIndex;
                break;
            case Decrease when dn != statIndex:
                dn = statIndex;
                break;
            case Neutral when up != statIndex && dn != statIndex:
                up = dn = statIndex;
                break;
            default:
                return -1; // failure
        }

        return CreateNatureFromAmps(up, dn);
    }

    /// <summary>
    /// Recombine the stat amps into a nature value.
    /// </summary>
    /// <param name="up">Increased stat</param>
    /// <param name="dn">Decreased stat</param>
    /// <returns>Nature</returns>
    public static int CreateNatureFromAmps(int up, int dn)
    {
        if ((uint)up > 5 || (uint)dn > 5)
            return -1;
        return (up * 5) + dn;
    }

    /// <summary>
    /// Decompose the nature to the two stat indexes that are modified
    /// </summary>
    public static (int up, int dn) GetNatureModification(int nature)
    {
        var up = (nature / 5);
        var dn = (nature % 5);
        return (up, dn);
    }

    /// <summary>
    /// Checks if the nature is out of range or the stat amplifications are not neutral.
    /// </summary>
    /// <param name="nature">Nature</param>
    /// <param name="up">Increased stat</param>
    /// <param name="dn">Decreased stat</param>
    /// <returns>True if nature modification values are equal or the Nature is out of range.</returns>
    public static bool IsNeutralOrInvalid(int nature, int up, int dn)
    {
        return up == dn || nature >= 25; // invalid
    }

    /// <inheritdoc cref="IsNeutralOrInvalid(int, int, int)"/>
    public static bool IsNeutralOrInvalid(int nature)
    {
        var (up, dn) = GetNatureModification(nature);
        return IsNeutralOrInvalid(nature, up, dn);
    }

    /// <summary>
    /// Updates stats according to the specified nature.
    /// </summary>
    /// <param name="stats">Current stats to amplify if appropriate</param>
    /// <param name="nature">Nature</param>
    public static void ModifyStatsForNature(Span<ushort> stats, int nature)
    {
        var (up, dn) = GetNatureModification(nature);
        if (IsNeutralOrInvalid(nature, up, dn))
            return;

        ref var upStat = ref stats[up + 1];
        ref var dnStat = ref stats[dn + 1];
        upStat = (ushort)((upStat * 11) / 10);
        dnStat = (ushort)((dnStat * 9) / 10);
    }
}

public enum NatureAmpRequest
{
    Neutral,
    Increase,
    Decrease,
}
