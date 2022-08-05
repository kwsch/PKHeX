using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for working with Effort Values
/// </summary>
public static class EffortValues
{
    /// <summary>
    /// Gets randomized EVs for a given generation format
    /// </summary>
    /// <param name="evs">Array to store the resulting EVs</param>
    /// <param name="generation">Generation specific formatting option</param>
    public static void SetRandom(Span<int> evs, int generation)
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
        const int maxTotal = 510;
        const int maxStat = 252;
        const int maxStatPlusBias = 300; // weight more towards the high end of received EVs
        while (true) // loop until we get a valid set of 6 stats
        {
            int remain = maxTotal;
            for (int i = 0; i < evs.Length - 1; i++)
            {
                var max = Math.Min(maxStatPlusBias, remain);
                var amount = rnd.Next(0, max);
                if (amount > maxStat)
                    amount = maxStat;
                remain -= (evs[i] = (byte)amount);
            }
            if (remain > maxStat)
                continue; // try again! must have had really low rand rolls.

            evs[5] = remain;
            break; // done!
        }

        Util.Shuffle(evs, 0, evs.Length, rnd);
    }

    private static void SetRandom12(Span<int> evs, Random rnd)
    {
        // In generation 1/2, EVs can be 0-65535.
        for (int i = 0; i < evs.Length; i++)
            evs[i] = rnd.Next(ushort.MaxValue + 1);
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

        evs[tuples[0].Index] = 252;
        evs[tuples[1].Index] = 252;
        evs[tuples[2].Index] = 6;
    }

    private static void SetMax12(Span<int> evs)
    {
        for (int i = 0; i < evs.Length; i++)
            evs[i] = ushort.MaxValue;
    }

    /// <summary>
    /// Sets all the EVs to zero.
    /// </summary>
    /// <param name="evs">Array to store the resulting EVs</param>
    public static void Clear(Span<int> evs) => evs.Clear();
}
