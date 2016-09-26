using System;

namespace PKHeX
{
    internal static class BigEndian
    {
        internal static uint ToUInt32(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 24;
            val |= data[offset + 1] << 16;
            val |= data[offset + 2] << 8;
            val |= data[offset + 3] << 0;
            return (uint)val;
        }
        internal static ushort ToUInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1] << 0;
            return (ushort)val;
        }
        internal static int ToInt32(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 24;
            val |= data[offset + 1] << 16;
            val |= data[offset + 2] << 8;
            val |= data[offset + 3] << 0;
            return val;
        }
        internal static short ToInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1] << 0;
            return (short)val;
        }

        internal static byte[] GetBytes(int value)
        {
            return Invert(BitConverter.GetBytes(value));
        }
        internal static byte[] GetBytes(short value)
        {
            return Invert(BitConverter.GetBytes(value));
        }
        internal static byte[] GetBytes(uint value)
        {
            return Invert(BitConverter.GetBytes(value));
        }
        internal static byte[] GetBytes(ushort value)
        {
            return Invert(BitConverter.GetBytes(value));
        }

        private static byte[] Invert(byte[] data)
        {
            byte[] result = new byte[data.Length];
            int o = 0;
            int i = data.Length;
            while (o != data.Length)
                result[--i] = data[o++];
            return result;
        }
    }
}
