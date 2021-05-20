namespace PKHeX.Core
{
    /// <summary>
    /// Genius Sonority's logic for <see cref="GameVersion.XD"/> and <see cref="GameVersion.BATREV"/> encryption.
    /// </summary>
    public static class GeniusCrypto
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
            var k3 = keys[3] + 0x13;
            var k2 = keys[2] + 0x17;
            var k1 = keys[1] + 0x29;
            var k0 = keys[0] + 0x43;

            // Rotate 4bit groups across the diagonal [ / ] after biasing each u16 (no overflow):
            // 0123           FB73 
            // 4567           EA62
            // 89AB  becomes  D951
            // CDEF           C840
            // We can leave our intermediary types as int as the bit-masks remove any overflow.

            keys[3] = (ushort)((k0 >> 12 & 0xf) | (k1 >> 8 & 0xf0) | (k2 >> 4 & 0xf00) | (k3       & 0xf000));
            keys[2] = (ushort)((k0 >> 08 & 0xf) | (k1 >> 4 & 0xf0) | (k2      & 0xf00) | (k3 << 04 & 0xf000));
            keys[1] = (ushort)((k0 >> 04 & 0xf) | (k1      & 0xf0) | (k2 << 4 & 0xf00) | (k3 << 08 & 0xf000));
            keys[0] = (ushort)((k0       & 0xf) | (k1 << 4 & 0xf0) | (k2 << 8 & 0xf00) | (k3 << 12 & 0xf000));
        }
    }
}
