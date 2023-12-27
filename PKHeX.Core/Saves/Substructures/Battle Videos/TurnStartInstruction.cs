namespace PKHeX.Core;

public readonly record struct TurnStartInstruction(TurnStartCode TurnCode, int Count)
{
    public static TurnStartInstruction Get(byte Op)
    {
        var TurnCode = (TurnStartCode)(Op >> 4);
        var Count = Op & 0xF;
        return new TurnStartInstruction(TurnCode, Count);
    }

    public byte GetRawValue => (byte) ((Count & 0xF) | ((byte) TurnCode << 4));
}
