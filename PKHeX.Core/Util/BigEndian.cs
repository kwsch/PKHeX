using System;

namespace PKHeX.Core
{
    public static class BigEndian
    {
        public static uint ToUInt32(ReadOnlySpan<byte> data)
        {
            int val = 0;
            val |= data[0] << 24;
            val |= data[1] << 16;
            val |= data[2] << 8;
            val |= data[3];
            return (uint)val;
        }

        public static ushort ToUInt16(ReadOnlySpan<byte> data)
        {
            int val = 0;
            val |= data[0] << 8;
            val |= data[1];
            return (ushort)val;
        }

        public static int ToInt32(ReadOnlySpan<byte> data)
        {
            int val = 0;
            val |= data[0] << 24;
            val |= data[1] << 16;
            val |= data[2] << 8;
            val |= data[3];
            return val;
        }

        public static short ToInt16(ReadOnlySpan<byte> data)
        {
            int val = 0;
            val |= data[0] << 8;
            val |= data[1];
            return (short)val;
        }

        public static uint ToUInt32(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 24;
            val |= data[offset + 1] << 16;
            val |= data[offset + 2] << 8;
            val |= data[offset + 3];
            return (uint)val;
        }

        public static ushort ToUInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1];
            return (ushort)val;
        }

        public static int ToInt32(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 24;
            val |= data[offset + 1] << 16;
            val |= data[offset + 2] << 8;
            val |= data[offset + 3];
            return val;
        }

        public static short ToInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1];
            return (short)val;
        }

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
                var temp = data[i];
                data[i] = data[j];
                data[j] = temp;
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
                byte tmp = data[0 + i];
                data[0 + i] = data[3 + i];
                data[3 + i] = tmp;

                byte tmp1 = data[1 + i];
                data[1 + i] = data[2 + i];
                data[2 + i] = tmp1;
            }
        }
    }
}
