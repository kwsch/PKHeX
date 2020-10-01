using System;

namespace PKHeX.Core
{
    public static class BigEndian
    {
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

        /// <summary>
        /// Returns a 32-bit signed integer converted from bytes in a Binary Coded Decimal format byte array.
        /// </summary>
        /// <param name="input">Input byte array to read from.</param>
        /// <param name="offset">Offset to start reading at.</param>
        /// <param name="length">Length of array to read.</param>
        public static int BCDToInt32(byte[] input, int offset, int length)
        {
            int result = 0;
            for (int i = offset; i < offset + length; i++)
            {
                byte p = input[i];
                result *= 100;
                result += 10 * (p >> 4);
                result += p & 0xf;
            }
            return result;
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of Binary Coded Decimal format bytes.
        /// </summary>
        /// <param name="input">32-bit signed integer to convert.</param>
        /// <param name="size">Desired size of returned array.</param>
        public static byte[] Int32ToBCD(int input, int size)
        {
            byte[] result = new byte[size];
            for (int i = 0; i < size; i++)
            {
                int p = input%100;
                input /= 100;
                result[size - i - 1] = (byte)(p/10 << 4 | p%10);
            }
            return result;
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from bytes in a Binary Coded Decimal format byte array.
        /// </summary>
        /// <remarks>Little Endian instead of Big Endian</remarks>
        /// <param name="input">Input byte array to read from.</param>
        /// <param name="offset">Offset to start reading at.</param>
        /// <param name="length">Length of array to read.</param>
        public static int BCDToInt32_LE(byte[] input, int offset, int length)
        {
            int result = 0;
            for (int i = offset + length - 1; i >= offset; i--)
            {
                byte p = input[i];
                result *= 100;
                result += 10 * (p >> 4);
                result += p & 0xf;
            }
            return result;
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of Binary Coded Decimal format bytes.
        /// </summary>
        /// <remarks>Little Endian instead of Big Endian</remarks>
        /// <param name="input">32-bit signed integer to convert.</param>
        /// <param name="size">Desired size of returned array.</param>
        public static byte[] Int32ToBCD_LE(int input, int size)
        {
            byte[] result = new byte[size];
            for (int i = size - 1; i >= 0; i--)
            {
                int p = input % 100;
                input /= 100;
                result[size - i - 1] = (byte)(p / 10 << 4 | p % 10);
            }
            return result;
        }
    }
}
