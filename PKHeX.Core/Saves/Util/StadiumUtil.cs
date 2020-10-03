using System;

namespace PKHeX.Core
{
    public static class StadiumUtil
    {
        public static bool IsMagicPresentEither(byte[] data, int size, uint magic)
        {
            if (IsMagicPresent(data, size, magic))
                return true;

            if (IsMagicPresentSwap(data, size, magic))
                return true;

            return false;
        }

        public static bool IsMagicPresent(byte[] data, int size, uint magic)
        {
            // Check footers of first few teams to see if the magic value is there.
            for (int i = 0; i < 10; i++)
            {
                if (BitConverter.ToUInt32(data, size - 6 + (i * size)) != magic)
                    return false;
            }
            return true;
        }

        public static bool IsMagicPresentSwap(byte[] data, int size, uint magic)
        {
            // Check footers of first few teams to see if the magic value is there.
            var left = (ushort)magic;
            var right = (ushort)(magic >> 16);
            left = (ushort)((left >> 8) | (left << 8));
            right = (ushort)((right >> 8) | (right << 8));

            for (int i = 0; i < 10; i++)
            {
                var ofs = size - 6 + (i * size);
                if (BitConverter.ToUInt16(data, ofs - 2) != left) // OP
                    return false;
                if (BitConverter.ToUInt16(data, ofs + 4) != right) // EK
                    return false;
            }
            return true;
        }

        public static bool IsMagicPresentAbsolute(byte[] data, int offset, uint magic)
        {
            var actual = BitConverter.ToUInt32(data, offset);
            if (actual == magic) // POKE
                return true;

            var left = (ushort)magic;
            var right = (ushort)(magic >> 16);
            left = (ushort)((left >> 8) | (left << 8));
            right = (ushort)((right >> 8) | (right << 8));

            if (BitConverter.ToUInt16(data, offset - 2) != left) // OP
                return false;
            if (BitConverter.ToUInt16(data, offset + 4) != right) // EK
                return false;

            return true;
        }
    }
}
