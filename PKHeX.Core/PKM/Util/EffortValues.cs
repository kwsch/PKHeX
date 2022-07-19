using System;

namespace PKHeX.Core;

public static class EffortValues
{
    /// <summary>
    /// Gets randomized EVs for a given generation format
    /// </summary>
    /// <param name="evs">Array containing randomized EVs (H/A/B/S/C/D)</param>
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
        do
        {
            int max = 510;
            for (int i = 0; i < evs.Length - 1; i++)
                max -= evs[i] = (byte)Math.Min(rnd.Next(Math.Min(300, max)), 252);
            evs[5] = max;
        } while (evs[5] > 252);

        Util.Shuffle(evs, 0, evs.Length, rnd);
    }

    private static void SetRandom12(Span<int> evs, Random rnd)
    {
        for (int i = 0; i < evs.Length; i++)
            evs[i] = rnd.Next(ushort.MaxValue + 1);
    }

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

    public static void Clear(Span<int> values) => values.Clear();
}
