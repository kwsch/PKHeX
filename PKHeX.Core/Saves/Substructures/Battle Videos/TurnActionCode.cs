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

    public readonly struct TurnActionInstruction
    {
        public readonly int PlayerID;
        public readonly int Count;
        public readonly int Bit;

        public TurnActionInstruction(byte Op)
        {
            PlayerID = Op >> 5;
            Bit = (Op >> 4) & 1;
            Count = Op & 0xF;
        }

        public byte GetRawValue => (byte)((Count & 0xF) | ((byte)Bit << 4) | (PlayerID << 5));

        public override bool Equals(object obj) => obj is TurnStartInstruction t && t.GetRawValue == GetRawValue;
        public override int GetHashCode() => GetRawValue;
        public static bool operator ==(TurnActionInstruction left, TurnActionInstruction right) => left.Equals(right);
        public static bool operator !=(TurnActionInstruction left, TurnActionInstruction right) => !(left == right);
    }
}