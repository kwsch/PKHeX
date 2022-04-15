namespace PKHeX.Core
{
    public sealed class Musical5 : SaveBlock<SAV5>
    {
        public Musical5(SAV5BW SAV, int offset) : base(SAV) => Offset = offset;
        public Musical5(SAV5B2W2 SAV, int offset) : base(SAV) => Offset = offset;

        private const int PropOffset = 0x258;

        public void UnlockAllMusicalProps()
        {
            // 101 props, which is 12.X bytes of bitflags.
            var bitFieldOffset = Offset + PropOffset;
            for (int i = 0; i < 0xC; i++)
                Data[bitFieldOffset + i] = 0xFF;
            Data[bitFieldOffset + 0xC] = 0x1F; // top 3 bits unset, to complete multiple of 8 (101=>104 bits).
        }

        public bool GetHasProp(int prop)
        {
            var bitFieldOffset = Offset + PropOffset;
            var bitOffset = prop >> 3;
            return SAV.GetFlag(bitFieldOffset + bitOffset, prop & 7);
        }

        public void SetHasProp(int prop, bool value = true)
        {
            var bitFieldOffset = Offset + PropOffset;
            var bitOffset = prop >> 3;
            SAV.SetFlag(bitFieldOffset + bitOffset, prop & 7, value);
        }
    }
}
