namespace PKHeX.Core;

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
    public static TurnActionInstruction Get(byte Op)
    {
        var PlayerID = Op >> 5;
        var Bit = (Op >> 4) & 1;
        var Count = Op & 0xF;
        return new TurnActionInstruction(PlayerID, Count, Bit);
    }

    public byte GetRawValue => (byte)((Count & 0xF) | ((byte)Bit << 4) | (PlayerID << 5));
}
