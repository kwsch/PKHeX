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

    public struct TurnActionInstruction
    {
        public int PlayerID;
        public int Count;

        public TurnActionInstruction(byte Op)
        {
            PlayerID = Op >> 5;
            Count = Op & 0xF;
        }
    }
}