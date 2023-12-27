using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Array reusable logic
/// </summary>
public static class ArrayUtil
{
    public static T Find<T>(this Span<T> data, Func<T, bool> value) where T : unmanaged
    {
        foreach (var x in data)
        {
            if (value(x))
                return x;
        }
        return default;
    }

    /// <summary>
    /// Checks the range (exclusive max) if the <see cref="value"/> is inside.
    /// </summary>
    public static bool WithinRange(int value, int min, int max) => min <= value && value < max;

    public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start = 0)
    {
        for (int i = start; i < bin.Length; i += size)
            yield return bin.AsSpan(i, size).ToArray();
    }

    /// <summary>
    /// Copies a <see cref="T"/> list to the destination list, with an option to copy to a starting point.
    /// </summary>
    /// <param name="list">Source list to copy from</param>
    /// <param name="dest">Destination list/array</param>
    /// <param name="skip">Criteria for skipping a slot</param>
    /// <param name="start">Starting point to copy to</param>
    /// <returns>Count of <see cref="T"/> copied.</returns>
    public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, Func<int, bool> skip, int start = 0)
    {
        int ctr = start;
        int skipped = 0;
        foreach (var z in list)
        {
            // seek forward to next open slot
            int next = FindNextValidIndex(dest, skip, ctr);
            if (next == -1)
                break;
            skipped += next - ctr;
            ctr = next;
            dest[ctr++] = z;
        }
        return ctr - start - skipped;
    }

    public static int FindNextValidIndex<T>(IList<T> dest, Func<int, bool> skip, int ctr)
    {
        while (true)
        {
            if ((uint)ctr >= dest.Count)
                return -1;
            var exist = dest[ctr];
            if (exist == null || !skip(ctr))
                return ctr;
            ctr++;
        }
    }
}
