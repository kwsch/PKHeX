using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class JoinAvenue5(SAV5B2W2 sav, Memory<byte> raw) : SaveBlock<SAV5B2W2>(sav, raw)
{
    public const int VisitorCount = 8;
    public const int FanCount = 12;
    public const int OccupantCount = 8;
    public const int AssistantCount = 4;

    public uint CountVisitor // always 8
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public JoinAvenueVisitor5 GetVisitor(int index) => new(Raw.Slice(GetSubstructureOffset(index, 0x08, VisitorCount, JoinAvenueVisitor5.SIZE), JoinAvenueVisitor5.SIZE));

    public uint CountFan // always 12
    {
        get => ReadUInt32LittleEndian(Data[0x628..]);
        set => WriteUInt32LittleEndian(Data[0x628..], value);
    }

    public JoinAvenueFan5 GetFan(int index) => new(Raw.Slice(GetSubstructureOffset(index, 0x62C, FanCount, JoinAvenueFan5.SIZE), JoinAvenueFan5.SIZE));

    public JoinAvenueVisitor5 GetOccupant(int index) => new(Raw.Slice(GetSubstructureOffset(index, 0xAAC, OccupantCount, JoinAvenueVisitor5.SIZE), JoinAvenueVisitor5.SIZE));
    public JoinAvenueAssistant5 GetAssistant(int index) => new(Raw.Slice(GetSubstructureOffset(index, 0x10CC, AssistantCount, JoinAvenueAssistant5.SIZE), JoinAvenueAssistant5.SIZE));

    /// <summary> Won't always have all values filled out. </summary>
    public JoinAvenueVisitor5 Self => new(Raw.Slice(0x122C, JoinAvenueVisitor5.SIZE));

    /// <summary>
    /// Internal flag used when scripting has triggered an update.
    /// </summary>
    public bool ScriptFlag
    {
        get => (Data[0x12F0] & 1) != 0;
        set => Data[0x12F0] = (byte)((Data[0x12F0] & ~1) | (value ? 1 : 0));
    }
    // 3 bytes alignment

    public JoinAvenueSettings5 Settings => new(Raw.Slice(0x12F4, JoinAvenueSettings5.SIZE), SAV);

    private static int GetSubstructureOffset(int index, int offset, int count, int size)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, count);
        return offset + (index * size);
    }

    // 0..15 max trivia value
    public static ReadOnlySpan<byte> TriviaMax => [4, 2, 5, 2, 4, 8, 2, 2, 3, 2, 9, 5, 2, 2, 2, 2];
}
