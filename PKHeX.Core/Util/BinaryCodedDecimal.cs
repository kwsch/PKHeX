using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// 4-bit decimal encoding used by some Generation 1 save file values.
    /// </summary>
    public static class BinaryCodedDecimal
    {
        /// <summary>
        /// Returns a 32-bit signed integer converted from bytes in a Binary Coded Decimal format byte array.
        /// </summary>
        /// <param name="input">Input byte array to read from.</param>
        /// <param name="offset">Offset to start reading at.</param>
        /// <param name="length">Length of array to read.</param>
        public static int ToInt32BE(byte[] input, int offset, int length)
        {
            var span = new ReadOnlySpan<byte>(input, offset, length);
            return ToInt32BE(span);
        }

        /// <inheritdoc cref="ToInt32BE(byte[],int,int)"/>
        public static int ToInt32BE(ReadOnlySpan<byte> input)
        {
            int result = 0;
            foreach (var b in input)
                PushDigits(ref result, b);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushDigits(ref int result, byte b) => result = (result * 100) + (10 * (b >> 4)) + (b & 0xf);

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of Binary Coded Decimal format bytes.
        /// </summary>
        /// <param name="value">32-bit signed integer to convert.</param>
        /// <param name="size">Desired size of returned array.</param>
        public static byte[] GetBytesBE(int value, int size)
        {
            byte[] data = new byte[size];
            WriteBytesBE(data, value);
            return data;
        }

        public static void WriteBytesBE(Span<byte> data, int value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int p = value % 100;
                value /= 100;
                data[^(1+i)] = (byte) (p / 10 << 4 | p % 10);
            }
        }

        /// <inheritdoc cref="ToInt32BE(byte[],int,int)"/>
        /// <remarks>Big Endian instead of Little Endian</remarks>
        public static int ToInt32LE(byte[] data, int offset, int length)
        {
            var span = new ReadOnlySpan<byte>(data, offset, length);
            return ToInt32LE(span);
        }

        /// <inheritdoc cref="ToInt32BE(byte[],int,int)"/>
        public static int ToInt32LE(ReadOnlySpan<byte> input)
        {
            int result = 0;
            for (int i = input.Length - 1; i >= 0; i--)
                PushDigits(ref result, input[i]);
            return result;
        }

        /// <inheritdoc cref="GetBytesBE"/>
        /// <remarks>Little Endian instead of Big Endian</remarks>
        public static byte[] GetBytesLE(int value, int size)
        {
            byte[] data = new byte[size];
            WriteBytesLE(data, value);
            return data;
        }

        /// <summary>
        /// Writes the <see cref="value"/> to the <see cref="data"/> buffer.
        /// </summary>
        public static void WriteBytesLE(Span<byte> data, int value)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int p = value % 100;
                value /= 100;
                data[i] = (byte) (p / 10 << 4 | p % 10);
            }
        }
    }
}
