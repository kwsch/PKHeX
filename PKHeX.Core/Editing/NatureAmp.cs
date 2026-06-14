using System;
using static PKHeX.Core.NatureAmpRequest;

namespace PKHeX.Core;

/// <summary>
/// Logic for mutating a nature to amplify certain stats.
/// </summary>
public static class NatureAmp
{
    extension(NatureAmpRequest type)
    {
        /// <summary>
        /// Mutate the nature amp indexes to match the request
        /// </summary>
        /// <param name="statIndex">Stat Index to mutate, internal order</param>
        /// <param name="currentNature">Current nature to derive the current amps from</param>
        /// <returns>New nature value</returns>
        public Nature GetNewNature(int statIndex, Nature currentNature)
        {
            if ((uint)currentNature >= NatureCount)
                return Nature.Random;

            var (up, dn) = currentNature.GetNatureModification();

            return type.GetNewNature(statIndex, up, dn);
        }

        /// <param name="statIndex">Stat Index to mutate, internal order</param>
        /// <param name="up">Current increased stat index, internal order</param>
        /// <param name="dn">Current decreased stat index, internal order</param>
        /// <inheritdoc cref="GetNewNature(NatureAmpRequest,int,Nature)"/>
        public Nature GetNewNature(int statIndex, int up, int dn)
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
    }

    extension(Nature nature)
    {
        /// <summary>
        /// Decompose the nature to the two stat indexes that are modified
        /// </summary>
        /// <returns>Tuple containing the increased and decreased stat indexes, internal order</returns>
        public (int up, int dn) GetNatureModification()
        {
            var up = ((byte)nature / 5);
            var dn = ((byte)nature % 5);
            return (up, dn);
        }

        /// <inheritdoc cref="IsNeutralOrInvalid(Nature, int, int)"/>
        public bool IsNeutralOrInvalid()
        {
            var (up, dn) = nature.GetNatureModification();
            return nature.IsNeutralOrInvalid(up, dn);
        }

        /// <summary>
        /// Checks if the nature is out of range or the stat amplifications are not neutral.
        /// </summary>
        /// <param name="up">Increased stat, internal order</param>
        /// <param name="dn">Decreased stat, internal order</param>
        /// <returns>True if nature modification values are equal or the Nature is out of range.</returns>
        public bool IsNeutralOrInvalid(int up, int dn)
        {
            return up == dn || (byte)nature >= 25; // invalid
        }

        /// <summary>
        /// Updates stats according to the specified nature.
        /// </summary>
        /// <param name="stats">Current stats to amplify if appropriate</param>
        public void ModifyStatsForAlignment(Span<ushort> stats)
        {
            var (up, dn) = nature.GetNatureModification();
            if (nature.IsNeutralOrInvalid(up, dn))
                return;

            ref var upStat = ref stats[up + 1];
            ref var dnStat = ref stats[dn + 1];
            upStat = (ushort)((upStat * 11) / 10);
            dnStat = (ushort)((dnStat * 9) / 10);
        }
    }

    /// <summary>
    /// Recombine the stat amps into a nature value.
    /// </summary>
    /// <param name="up">Increased stat, internal order</param>
    /// <param name="dn">Decreased stat, internal order</param>
    /// <returns>Nature</returns>
    public static Nature CreateNatureFromAmps(int up, int dn)
    {
        if ((uint)up > 5 || (uint)dn > 5)
            return Nature.Random;
        return (Nature)((up * 5) + dn);
    }

    /// <summary>
    /// Nature / Stat Alignment Amplification Table, speed last (visual order).
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

    /// <summary>
    /// Amplify the stat according to the nature. If the nature is out of range, it will be treated as neutral.
    /// </summary>
    /// <param name="nature">Nature to use for amplification</param>
    /// <param name="index">Stat index to amplify (0-4), visual index</param>
    /// <param name="initial">Initial stat value</param>
    /// <returns>Amplified stat value</returns>
    public static int AmplifyStat(Nature nature, int index, int initial) => GetNatureAmp(nature, index) switch
    {
        1 => 110 * initial / 100, // 110%
        -1 => 90 * initial / 100, // 90%
        _ => initial,
    };

    /// <summary>
    /// Get the nature amp for the specified stat index. If the nature is out of range, it will be treated as neutral.
    /// </summary>
    /// <param name="nature">Nature to use for amplification</param>
    /// <param name="index">Stat index to amplify (0-4), visual index</param>
    /// <returns>Nature amp value: 1 for 110%, -1 for 90%, 0 for 100%.</returns>
    private static sbyte GetNatureAmp(Nature nature, int index)
    {
        if ((uint)nature >= NatureCount)
            return -1;
        var amps = GetAmps(nature);
        return amps[index];
    }

    /// <summary>
    /// Get the nature amps for all stats. If the nature is out of range, it will be treated as neutral.
    /// </summary>
    /// <param name="nature">Nature to use for amplification</param>
    /// <returns>ReadOnlySpan of stat amps for all stats (visual order)</returns>
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
