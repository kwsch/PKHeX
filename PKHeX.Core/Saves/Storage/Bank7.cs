using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 <see cref="SaveFile"/> object that reads from Pokémon Bank savedata (stored on AWS).
/// </summary>
public sealed class Bank7 : BulkStorage, IBoxDetailNameRead
{
    public Bank7(byte[] data, Type t, [ConstantExpected] int start, int slotsPerBox = 30) : base(data, t, start, slotsPerBox) => Version = GameVersion.USUM;

    public override GameVersion Version { get => GameVersion.USUM; set { } }
    public override PersonalTable7 Personal => PersonalTable.USUM;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_SM;
    protected override Bank7 CloneInternal() => new((byte[])Data.Clone(), PKMType, BoxStart, SlotsPerBox);
    public override string PlayTimeString => $"{Year:00}{Month:00}{Day:00}_{Hours:00}ː{Minutes:00}";
    protected internal override string ShortSummary => PlayTimeString;
    private const int GroupNameSize = 0x20;
    private const int BankNameSize = 0x24;
    private const int GroupNameSpacing = GroupNameSize + 2;
    private const int BankNameSpacing = BankNameSize + 2;

    public ulong UID => ReadUInt64LittleEndian(Data.AsSpan(0));

    public string GetGroupName(int group)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>((uint)group, 10);
        int offset = 0x8 + (GroupNameSpacing * group) + 2; // skip over " "
        return GetString(Data.AsSpan(offset, GroupNameSize / 2));
    }

    public override int BoxCount => BankCount;

    private int BankCount
    {
        get => Data[0x15E];
        set => Data[0x15E] = (byte)value;
    }

    private int Year => ReadUInt16LittleEndian(Data.AsSpan(0x160));
    private int Month => Data[0x162];
    private int Day => Data[0x163];
    private int Hours => Data[0x164];
    private int Minutes => Data[0x165];

    private int BoxDataSize => (SlotsPerBox * SIZE_STORED) + BankNameSpacing;
    public override int GetBoxOffset(int box) => Box + (BoxDataSize * box);
    public string GetBoxName(int box) => GetString(Data.AsSpan(GetBoxNameOffset(box), BankNameSize / 2));
    public int GetBoxNameOffset(int box) => GetBoxOffset(box) + (SlotsPerBox * SIZE_STORED);
    public int GetBoxIndex(int box) => ReadUInt16LittleEndian(Data.AsSpan(GetBoxNameOffset(box) + BankNameSize));

    private const int BoxStart = 0x17C;
    public static Bank7 GetBank7(byte[] data) => new(data, typeof(PK7), BoxStart);
}
