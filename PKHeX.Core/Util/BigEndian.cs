using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public static class BigEndian
    {
        public static uint ToUInt32(ReadOnlySpan<byte> data) => ReadUInt32BigEndian(data);
        public static ushort ToUInt16(ReadOnlySpan<byte> data) => ReadUInt16BigEndian(data);
        public static int ToInt32(ReadOnlySpan<byte> data) => ReadInt32BigEndian(data);
        public static short ToInt16(ReadOnlySpan<byte> data) => ReadInt16BigEndian(data);
        public static uint ToUInt32(byte[] data, int offset) => ReadUInt32BigEndian(data.AsSpan(offset));
        public static ushort ToUInt16(byte[] data, int offset) => ReadUInt16BigEndian(data.AsSpan(offset));
        public static int ToInt32(byte[] data, int offset) => ReadInt32BigEndian(data.AsSpan(offset));
        public static short ToInt16(byte[] data, int offset) => ReadInt16BigEndian(data.AsSpan(offset));

        public static byte[] GetBytes(int value)
        {
            return Invert(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(short value)
        {
            return Invert(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(uint value)
        {
            return Invert(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(ushort value)
        {
            return Invert(BitConverter.GetBytes(value));
        }

        private static byte[] Invert(byte[] data)
        {
            int i = 0;
            int j = 0 + data.Length - 1;
            while (i < j)
            {
                (data[i], data[j]) = (data[j], data[i]);
                i++;
                j--;
            }
            return data;
        }

        /// <summary>
        /// Swaps byte ordering in a byte array based on 32bit value writes.
        /// </summary>
        /// <remarks>The <see cref="data"/> is reversed in-place.</remarks>
        public static void SwapBytes32(byte[] data)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                (data[0 + i], data[3 + i]) = (data[3 + i], data[0 + i]);
                (data[1 + i], data[2 + i]) = (data[2 + i], data[1 + i]);
            }
        }
    }
}
