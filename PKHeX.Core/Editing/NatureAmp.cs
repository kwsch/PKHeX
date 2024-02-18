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
    public static Nature GetNewNature(this NatureAmpRequest type, int statIndex, Nature currentNature)
    {
        if ((uint)currentNature >= NatureCount)
            return Nature.Random;

        var (up, dn) = GetNatureModification(currentNature);

        return GetNewNature(type, statIndex, up, dn);
    }

    /// <inheritdoc cref="GetNewNature(NatureAmpRequest,int,Nature)"/>
    public static Nature GetNewNature(NatureAmpRequest type, int statIndex, int up, int dn)
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
                return Nature.Random; // failure
        }

        return CreateNatureFromAmps(up, dn);
    }

    /// <summary>
    /// Recombine the stat amps into a nature value.
    /// </summary>
    /// <param name="up">Increased stat</param>
    /// <param name="dn">Decreased stat</param>
    /// <returns>Nature</returns>
    public static Nature CreateNatureFromAmps(int up, int dn)
    {
        if ((uint)up > 5 || (uint)dn > 5)
            return Nature.Random;
        return (Nature)((up * 5) + dn);
    }

    /// <summary>
    /// Decompose the nature to the two stat indexes that are modified
    /// </summary>
    public static (int up, int dn) GetNatureModification(Nature nature)
    {
        var up = ((byte)nature / 5);
        var dn = ((byte)nature % 5);
        return (up, dn);
    }

    /// <summary>
    /// Checks if the nature is out of range or the stat amplifications are not neutral.
    /// </summary>
    /// <param name="nature">Nature</param>
    /// <param name="up">Increased stat</param>
    /// <param name="dn">Decreased stat</param>
    /// <returns>True if nature modification values are equal or the Nature is out of range.</returns>
    public static bool IsNeutralOrInvalid(Nature nature, int up, int dn)
    {
        return up == dn || (byte)nature >= 25; // invalid
    }

    /// <inheritdoc cref="IsNeutralOrInvalid(Nature, int, int)"/>
    public static bool IsNeutralOrInvalid(Nature nature)
    {
        var (up, dn) = GetNatureModification(nature);
        return IsNeutralOrInvalid(nature, up, dn);
    }

    /// <summary>
    /// Updates stats according to the specified nature.
    /// </summary>
    /// <param name="stats">Current stats to amplify if appropriate</param>
    /// <param name="nature">Nature</param>
    public static void ModifyStatsForNature(Span<ushort> stats, Nature nature)
    {
        var (up, dn) = GetNatureModification(nature);
        if (IsNeutralOrInvalid(nature, up, dn))
            return;

        ref var upStat = ref stats[up + 1];
        ref var dnStat = ref stats[dn + 1];
        upStat = (ushort)((upStat * 11) / 10);
        dnStat = (ushort)((dnStat * 9) / 10);
    }

    /// <summary>
    /// Nature Amplification Table
    /// </summary>
    /// <remarks>-1 is 90%, 0 is 100%, 1 is 110%.</remarks>
    public static ReadOnlySpan<sbyte> Table =>
    [
        0, 0, 0, 0, 0, // Hardy
        1,-1, 0, 0, 0, // Lonely
        1, 0, 0, 0,-1, // Brave
        1, 0,-1, 0, 0, // Adamant
        1, 0, 0,-1, 0, // Naughty
       -1, 1, 0, 0, 0, // Bold
        0, 0, 0, 0, 0, // Docile
        0, 1, 0, 0,-1, // Relaxed
        0, 1,-1, 0, 0, // Impish
        0, 1, 0,-1, 0, // Lax
       -1, 0, 0, 0, 1, // Timid
        0,-1, 0, 0, 1, // Hasty
        0, 0, 0, 0, 0, // Serious
        0, 0,-1, 0, 1, // Jolly
        0, 0, 0,-1, 1, // Naive
       -1, 0, 1, 0, 0, // Modest
        0,-1, 1, 0, 0, // Mild
        0, 0, 1, 0,-1, // Quiet
        0, 0, 0, 0, 0, // Bashful
        0, 0, 1,-1, 0, // Rash
       -1, 0, 0, 1, 0, // Calm
        0,-1, 0, 1, 0, // Gentle
        0, 0, 0, 1,-1, // Sassy
        0, 0,-1, 1, 0, // Careful
        0, 0, 0, 0, 0, // Quirky
    ];

    private const byte NatureCount = 25;
    private const int AmpWidth = 5;

    public static int AmplifyStat(Nature nature, int index, int initial) => GetNatureAmp(nature, index) switch
    {
        1 => 110 * initial / 100, // 110%
        -1 => 90 * initial / 100, // 90%
        _ => initial,
    };

    private static sbyte GetNatureAmp(Nature nature, int index)
    {
        if ((uint)nature >= NatureCount)
            return -1;
        var amps = GetAmps(nature);
        return amps[index];
    }

    public static ReadOnlySpan<sbyte> GetAmps(Nature nature)
    {
        if ((uint)nature >= NatureCount)
            nature = 0;
        return Table.Slice(AmpWidth * (byte)nature, AmpWidth);
    }
}

public enum NatureAmpRequest
{
    Neutral,
    Increase,
    Decrease,
}
