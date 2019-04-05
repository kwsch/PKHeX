namespace PKHeX.Core
{
    /// <summary>
    /// Logic for <see cref="GameVersion.XD"/> and <see cref="GameVersion.BATREV"/> encryption.
    /// </summary>
    public static class GCSaveUtil
    {
        public static byte[] Decrypt(byte[] input, int start, int end, ushort[] keys)
        {
            var output = (byte[])input.Clone();
            Decrypt(input, start, end, keys, output);
            return output;
        }

        public static void Decrypt(byte[] input, int start, int end, ushort[] keys, byte[] output)
        {
            for (int ofs = start; ofs < end; ofs += 8)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var index = ofs + (i * 2);
                    ushort val = BigEndian.ToUInt16(input, index);
                    val -= keys[i];
                    output[index] = (byte)(val >> 8);
                    output[index + 1] = (byte)val;
                }

                AdvanceKeys(keys);
            }
        }

        public static byte[] Encrypt(byte[] input, int start, int end, ushort[] keys)
        {
            var output = (byte[])input.Clone();
            Encrypt(input, start, end, keys, output);
            return output;
        }

        public static void Encrypt(byte[] input, int start, int end, ushort[] keys, byte[] output)
        {
            for (int ofs = start; ofs < end; ofs += 8)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var index = ofs + (i * 2);
                    ushort val = BigEndian.ToUInt16(input, index);
                    val += keys[i];
                    output[index] = (byte)(val >> 8);
                    output[index + 1] = (byte)val;
                }

                AdvanceKeys(keys);
            }
        }

        private static void AdvanceKeys(ushort[] keys)
        {
            keys[0] += 0x43;
            keys[1] += 0x29;
            keys[2] += 0x17;
            keys[3] += 0x13;

            var _0 = (ushort)((keys[0] >> 00 & 0xf) | (keys[1] << 4 & 0xf0) | (keys[2] << 8 & 0xf00) | (keys[3] << 12 & 0xf000));
            var _1 = (ushort)((keys[0] >> 04 & 0xf) | (keys[1] << 0 & 0xf0) | (keys[2] << 4 & 0xf00) | (keys[3] << 08 & 0xf000));
            var _2 = (ushort)((keys[0] >> 08 & 0xf) | (keys[1] >> 4 & 0xf0) | (keys[2] >> 0 & 0xf00) | (keys[3] << 04 & 0xf000));
            var _3 = (ushort)((keys[0] >> 12 & 0xf) | (keys[1] >> 8 & 0xf0) | (keys[2] >> 4 & 0xf00) | (keys[3] << 00 & 0xf000));

            keys[0] = _0;
            keys[1] = _1;
            keys[2] = _2;
            keys[3] = _3;
        }
    }
}