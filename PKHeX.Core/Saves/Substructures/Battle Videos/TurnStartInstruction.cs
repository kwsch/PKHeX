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
    }
}