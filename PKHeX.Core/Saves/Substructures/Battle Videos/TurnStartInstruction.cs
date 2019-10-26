namespace PKHeX.Core
{
    public struct TurnStartInstruction
    {
        public TurnStartCode TurnCode;
        public int Count;

        public TurnStartInstruction(byte Op)
        {
            TurnCode = (TurnStartCode)(Op >> 4);
            Count = Op & 0xF;
        }

        public byte GetRawValue => (byte) ((Count & 0xF) | ((byte) TurnCode << 4));

        public override bool Equals(object obj) => obj is TurnStartInstruction t && t.GetRawValue == GetRawValue;
        public override int GetHashCode() => GetRawValue;
        public static bool operator ==(TurnStartInstruction left, TurnStartInstruction right) => left.Equals(right);
        public static bool operator !=(TurnStartInstruction left, TurnStartInstruction right) => !(left == right);
    }
}