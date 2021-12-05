namespace PKHeX.Core
{
    public readonly record struct TurnStartInstruction(TurnStartCode TurnCode, int Count)
    {
        public TurnStartInstruction(byte Op) : this()
        {
            TurnCode = (TurnStartCode)(Op >> 4);
            Count = Op & 0xF;
        }

        public byte GetRawValue => (byte) ((Count & 0xF) | ((byte) TurnCode << 4));
    }
}