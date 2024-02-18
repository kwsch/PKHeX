using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for working with Effort Values
/// </summary>
public static class EffortValues
{
    /// <summary> Maximum value for a single stat in Generation 1/2 formats. </summary>
    public const ushort Max12 = ushort.MaxValue;
    /// <summary> Maximum value for a single stat in Generation 6+ formats. </summary>
    public const byte Max252 = 252;
    /// <summary> Maximum value for a single stat in Generation 3-5 formats. </summary>
    public const byte Max255 = 255;
    /// <summary> Maximum value for the sum of all stats in Generation 3+ formats. </summary>
    public const ushort Max510 = 510;
    /// <summary> Since EVs are effective in multiples of 4, the leftover EVs (2) have no impact regardless of stat gained. </summary>
    public const ushort MaxEffective = 508;
    /// <summary> The leftover EVs if two stats are <see cref="Max252"/>. </summary>
    public const byte LeftoverDual252 = 6;
    /// <summary> Vitamin Max for consideration in Gen3 & Gen4. </summary>
    public const ushort MaxVitamins34 = 100;

    /// <summary>
    /// Gets randomized EVs for a given generation format
    /// </summary>
    /// <param name="evs">Array to store the resulting EVs</param>
    /// <param name="generation">Generation specific formatting option</param>
    public static void SetRandom(Span<int> evs, byte generation)
    {
        var rnd = Util.Rand;
        if (generation > 2)
            SetRandom252(evs, rnd);
        else
            SetRandom12(evs, rnd);
    }

    private static void SetRandom252(Span<int> evs, Random rnd)
    {
        // Set random EVs (max 252 per stat) until we run out of EVs.
        // The last stat index receives the remaining EVs
        const int maxTotal = Max510;
        const int maxStat = Max252;
        const int maxStatPlusBias = 300; // weight more towards the high end of received EVs
        while (true) // loop until we get a valid set of 6 stats
        {
            int remain = maxTotal;
            for (int i = 0; i < evs.Length - 1; i++)
            {
                var max = Math.Min(maxStatPlusBias, remain);
                var amount = rnd.Next(0, max + 1);
                if (amount > maxStat)
                    amount = maxStat;
                remain -= (evs[i] = (byte)amount);
            }
            if (remain > maxStat)
                continue; // try again! must have had really low rand rolls.

            evs[5] = remain;
            break; // done!
        }

        rnd.Shuffle(evs);
    }

    private static void SetRandom12(Span<int> evs, Random rnd)
    {
        // In generation 1/2, EVs can be 0-65535.
        for (int i = 0; i < evs.Length; i++)
            evs[i] = rnd.Next(Max12 + 1);
    }

    /// <summary>
    /// Sets the Effort Values to a reasonable max value.
    /// </summary>
    /// <param name="evs">Array to store the resulting EVs</param>
    /// <param name="entity">Entity that will eventually receive the EVs</param>
    public static void SetMax(Span<int> evs, PKM entity)
    {
        if (entity.Format < 3)
            SetMax12(evs);
        else
            SetMax252(evs, entity);
    }

    private static void SetMax252(Span<int> evs, PKM entity)
    {
        // Get the 3 highest base stat indexes from the entity PersonalInfo
        var pi = entity.PersonalInfo;
        Span<(int Index, int Stat)> tuples = stackalloc (int, int)[6];
        pi.GetSortedStatIndexes(tuples);

        evs[tuples[0].Index] = Max252;
        evs[tuples[1].Index] = Max252;
        evs[tuples[2].Index] = LeftoverDual252;
    }

    private static void SetMax12(Span<int> evs)
    {
        for (int i = 0; i < evs.Length; i++)
            evs[i] = Max12;
    }

    /// <summary>
    /// Sets all the EVs to zero.
    /// </summary>
    /// <param name="evs">Array to store the resulting EVs</param>
    public static void Clear(Span<int> evs) => evs.Clear();

    public static EffortValueGrade GetGrade(int sum) => sum switch
    {
        0 => EffortValueGrade.None,
        <= 128 => EffortValueGrade.Quarter,
        <= 256 => EffortValueGrade.Half,

        < MaxEffective => EffortValueGrade.NearFull,
        MaxEffective   => EffortValueGrade.MaxEffective,
        Max510 - 1     => EffortValueGrade.MaxNearCap,
        Max510         => EffortValueGrade.MaxLegal,
        _ => EffortValueGrade.Illegal,
    };
}

/// <summary>
/// Assessment of the total EVs, compared to the maximum allowed.
/// </summary>
public enum EffortValueGrade
{
    /// <summary> No EVs </summary>
    None,
    /// <summary> 1-128 EVs </summary>
    Quarter,
    /// <summary> 129-256 EVs </summary>
    Half,
    /// <summary> 257-508 EVs </summary>
    NearFull,
    /// <summary> 508 EVs </summary>
    MaxEffective,
    /// <summary> 509 EVs </summary>
    MaxNearCap,
    /// <summary> 510 EVs </summary>
    MaxLegal,
    /// <summary> 511+ EVs </summary>
    Illegal,
}
