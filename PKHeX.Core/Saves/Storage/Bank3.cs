using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object that reads exported data for Generation 3 PokeStock .gst dumps.
/// </summary>
public sealed class Bank3 : BulkStorage, IBoxDetailNameRead
{
    public Bank3(byte[] data) : base(data, typeof(PK3), 0) => Version = GameVersion.RS;

    public override GameVersion Version { get => GameVersion.RS; set { } }
    public override PersonalTable3 Personal => PersonalTable.RS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;
    protected override Bank3 CloneInternal() => new((byte[])Data.Clone());
    public override string PlayTimeString => Checksums.CRC16Invert(Data).ToString("X4");
    protected internal override string ShortSummary => PlayTimeString;
    public override string Extension => ".gst";

    public override int BoxCount => 64;
    private const int BoxNameSize = 9;

    private int BoxDataSize => SlotsPerBox * SIZE_STORED;
    public override int GetBoxOffset(int box) => Box + (BoxDataSize * box);
    public string GetBoxName(int box) => GetString(Data.AsSpan(GetBoxNameOffset(box), BoxNameSize));
    private static int GetBoxNameOffset(int box) => 0x25800 + (9 * box);
}
