using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Byte array reusable logic
    /// </summary>
    public static class ArrayUtil
    {
        public static bool IsRangeAll(this byte[] data, int value, int offset, int length)
        {
            int start = offset + length - 1;
            int end = offset;
            for (int i = start; i >= end; i--)
            {
                if (data[i] != value)
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

        public static bool WithinRange(int value, int min, int max) => min <= value && value < max;

        public static byte[][] Split(this byte[] data, int size)
        {
            byte[][] result = new byte[data.Length / size][];
            for (int i = 0; i < data.Length; i += size)
                result[i / size] = data.Slice(i, size);
            return result;
        }

        public static IEnumerable<byte[]> EnumerateSplit(byte[] bin, int size, int start = 0)
        {
            for (int i = start; i < bin.Length; i += size)
                yield return bin.Slice(i, size);
        }

        public static IEnumerable<byte[]> EnumerateSplit(byte[] bin, int size, int start, int end)
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
                if (value[i])
                    data[offset + (i >> 3)] |= (byte)(1 << (i & 7));
            }
        }

        public static byte[] SetBitFlagArray(bool[] value)
        {
            byte[] data = new byte[value.Length / 8];
            SetBitFlagArray(data, 0, value);
            return data;
        }
    }
}