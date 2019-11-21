using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Array reusable logic
    /// </summary>
    public static class ArrayUtil
    {
        public static bool IsRangeAll<T>(this T[] data, T value, int offset, int length) where T : IEquatable<T>
        {
            int start = offset + length - 1;
            int end = offset;
            for (int i = start; i >= end; i--)
            {
                if (!data[i].Equals(value))
                    return false;
            }

            return true;
        }

        public static byte[] Slice(this byte[] src, int offset, int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(src, offset, data, 0, data.Length);
            return data;
        }

        public static byte[] SliceEnd(this byte[] src, int offset)
        {
            int length = src.Length - offset;
            byte[] data = new byte[length];
            Buffer.BlockCopy(src, offset, data, 0, data.Length);
            return data;
        }

        public static T[] Slice<T>(this T[] src, int offset, int length)
        {
            var data = new T[length];
            Array.Copy(src, offset, data, 0, data.Length);
            return data;
        }

        public static T[] SliceEnd<T>(this T[] src, int offset)
        {
            int length = src.Length - offset;
            var data = new T[length];
            Array.Copy(src, offset, data, 0, data.Length);
            return data;
        }

        public static bool WithinRange(int value, int min, int max) => min <= value && value < max;

        public static T[][] Split<T>(this T[] data, int size)
        {
            var result = new T[data.Length / size][];
            for (int i = 0; i < data.Length; i += size)
                result[i / size] = data.Slice(i, size);
            return result;
        }

        public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start = 0)
        {
            for (int i = start; i < bin.Length; i += size)
                yield return bin.Slice(i, size);
        }

        public static IEnumerable<T[]> EnumerateSplit<T>(T[] bin, int size, int start, int end)
        {
            if (end < 0)
                end = bin.Length;
            for (int i = start; i < end; i += size)
                yield return bin.Slice(i, size);
        }

        public static bool[] GitBitFlagArray(byte[] data, int offset, int count)
        {
            bool[] result = new bool[count];
            for (int i = 0; i < result.Length; i++)
                result[i] = (data[offset + (i >> 3)] >> (i & 7) & 0x1) == 1;
            return result;
        }

        public static void SetBitFlagArray(byte[] data, int offset, bool[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                var ofs = offset + (i >> 3);
                var mask = (1 << (i & 7));
                if (value[i])
                    data[ofs] |= (byte)mask;
                else
                    data[ofs] &= (byte)~mask;
            }
        }

        public static byte[] SetBitFlagArray(bool[] value)
        {
            byte[] data = new byte[value.Length / 8];
            SetBitFlagArray(data, 0, value);
            return data;
        }

        /// <summary>
        /// Copies a <see cref="T"/> list to the destination list, with an option to copy to a starting point.
        /// </summary>
        /// <param name="list">Source list to copy from</param>
        /// <param name="dest">Destination list/array</param>
        /// <param name="skip">Criteria for skipping a slot</param>
        /// <param name="start">Starting point to copy to</param>
        /// <returns>Count of <see cref="T"/> copied.</returns>
        public static int CopyTo<T>(this IEnumerable<T> list, IList<T> dest, Func<T, bool> skip, int start = 0)
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

        public static int FindNextValidIndex<T>(IList<T> dest, Func<T, bool> skip, int ctr)
        {
            while (true)
            {
                if ((uint)ctr >= dest.Count)
                    return -1;
                var exist = dest[ctr];
                if (exist == null || !skip(exist))
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
    }
}