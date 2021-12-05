namespace PKHeX.Core
{
    public enum TurnActionCode
    {
        None = 0,
        Fight = 1,
        Switch = 3,
        Run = 4,
        UNK5 = 5,
        Rotate = 6,
        UNK7 = 7,
        MegaEvolve = 8,
    }

    public readonly record struct TurnActionInstruction(int PlayerID, int Count, int Bit)
    {
        public TurnActionInstruction(byte Op) : this()
        {
            PlayerID = Op >> 5;
            Bit = (Op >> 4) & 1;
            Count = Op & 0xF;
        }

        public byte GetRawValue => (byte)((Count & 0xF) | ((byte)Bit << 4) | (PlayerID << 5));
    }
}