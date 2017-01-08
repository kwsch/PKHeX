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
            val |= data[offset + 3] << 0;
            return (uint)val;
        }
        public static ushort ToUInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1] << 0;
            return (ushort)val;
        }
        public static int ToInt32(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 24;
            val |= data[offset + 1] << 16;
            val |= data[offset + 2] << 8;
            val |= data[offset + 3] << 0;
            return val;
        }
        public static short ToInt16(byte[] data, int offset)
        {
            int val = 0;
            val |= data[offset + 0] << 8;
            val |= data[offset + 1] << 0;
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
            byte[] result = new byte[data.Length];
            int o = 0;
            int i = data.Length;
            while (o != data.Length)
                result[--i] = data[o++];
            return result;
        }
    }
}
