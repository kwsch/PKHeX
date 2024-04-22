using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 <see cref="SaveFile"/> object that reads Generation 4 PokeStock .stk dumps.
/// </summary>
public sealed class Bank4 : BulkStorage, IBoxDetailNameRead
{
    public Bank4(byte[] data) : base(data, typeof(PK4), 0) => Version = GameVersion.HGSS;

    public override GameVersion Version { get => GameVersion.HGSS; set { } }
    public override PersonalTable4 Personal => PersonalTable.HGSS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_HGSS;
    protected override Bank4 CloneInternal() => new((byte[])Data.Clone());
    public override string PlayTimeString => Checksums.CRC16Invert(Data).ToString("X4");
    protected internal override string ShortSummary => PlayTimeString;
    public override string Extension => ".stk";

    public override int BoxCount => 64;
    private const int BoxNameSize = 0x18;

    private int BoxDataSize => SlotsPerBox * SIZE_STORED;
    public override int GetBoxOffset(int box) => Box + (BoxDataSize * box);
    public string GetBoxName(int box) => GetString(Data.AsSpan(GetBoxNameOffset(box), BoxNameSize / 2));
    private static int GetBoxNameOffset(int box) => 0x3FC00 + (0x19 * box);
}
