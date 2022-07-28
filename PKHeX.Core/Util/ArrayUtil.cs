using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Array reusable logic
/// </summary>
public static class ArrayUtil
{
    public static bool IsRangeEmpty(this ReadOnlySpan<byte> data, byte value = 0)
    {
        for (int i = data.Length - 1; i >= 0; i--)
        {
            if (data[i] != value)
                return false;
        }
        return true;
    }

    public static int Count<T>(this Span<T> data, T value) where T : IEquatable<T>
    {
        return ((ReadOnlySpan<T>)data).Count(value);
    }

    public static T Find<T>(this Span<T> data, Func<T, bool> value) where T : unmanaged
    {
        foreach (var x in data)
        {
            if (value(x))
                return x;
        }
        return default;
    }

    public static int Count<T>(this ReadOnlySpan<T> data, T value) where T : IEquatable<T>
    {
        int count = 0;
        for (int i = data.Length - 1; i >= 0; i--)
        {
            if (data[i].Equals(value))
                count++;
        }
        return count;
    }

    public static byte[] Slice(this byte[] src, int offset, int length) => src.AsSpan(offset, length).ToArray();
    public static T[] Slice<T>(this T[] src, int offset, int length) => src.AsSpan(offset, length).ToArray();

    public static bool WithinRange(int value, int min, int max) => min <= value && value < max;

    public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start = 0)
    {
        for (int i = start; i < bin.Length; i += size)
            yield return bin.Slice(i, size);
    }

    public static bool[] GitBitFlagArray(ReadOnlySpan<byte> data, int count)
    {
        bool[] result = new bool[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ((data[i >> 3] >> (i & 7)) & 0x1) == 1;
        return result;
    }

    public static void SetBitFlagArray(Span<byte> data, ReadOnlySpan<bool> value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var ofs = i >> 3;
            var mask = (1 << (i & 7));
            if (value[i])
                data[ofs] |= (byte)mask;
            else
                data[ofs] &= (byte)~mask;
        }
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

    /// <summary>
    /// Copies an <see cref="IEnumerable{T}"/> list to the destination list, with an option to copy to a starting point.
    /// </summary>
    /// <typeparam name="T">Typed object to copy</typeparam>
    /// <param name="list">Source list to copy from</param>
    /// <param name="dest">Destination list/array</param>
    /// <param name="start">Starting point to copy to</param>
    /// <returns>Count of <see cref="T"/> copied.</returns>
    public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, int start = 0)
    {
        int ctr = start;
        foreach (var z in list)
        {
            if ((uint)ctr >= dest.Count)
                break;
            dest[ctr++] = z;
        }
        return ctr - start;
    }

    internal static T[] ConcatAll<T>(params T[][] arr)
    {
        int len = 0;
        foreach (var a in arr)
            len += a.Length;

        var result = new T[len];

        int ctr = 0;
        foreach (var a in arr)
        {
            a.CopyTo(result, ctr);
            ctr += a.Length;
        }

        return result;
    }

    internal static T[] ConcatAll<T>(T[] arr1, T[] arr2)
    {
        int len = arr1.Length + arr2.Length;
        var result = new T[len];
        arr1.CopyTo(result, 0);
        arr2.CopyTo(result, arr1.Length);
        return result;
    }

    internal static T[] ConcatAll<T>(T[] arr1, T[] arr2, T[] arr3)
    {
        int len = arr1.Length + arr2.Length + arr3.Length;
        var result = new T[len];
        arr1.CopyTo(result, 0);
        arr2.CopyTo(result, arr1.Length);
        arr3.CopyTo(result, arr1.Length + arr2.Length);
        return result;
    }

    internal static T[] ConcatAll<T>(T[] arr1, T[] arr2, ReadOnlySpan<T> arr3)
    {
        int len = arr1.Length + arr2.Length + arr3.Length;
        var result = new T[len];
        arr1.CopyTo(result, 0);
        arr2.CopyTo(result, arr1.Length);
        arr3.CopyTo(result.AsSpan(arr1.Length + arr2.Length));
        return result;
    }
}
