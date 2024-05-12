using System;

namespace PKHeX.Core;

/// <summary>
/// Simple Storage Binary wrapper for a concatenated list of <see cref="PKM"/> data.
/// </summary>
public abstract class BulkStorage : SaveFile
{
    protected BulkStorage(byte[] data, Type t, int start, int slotsPerBox = 30) : base(data)
    {
        Box = start;
        SlotsPerBox = slotsPerBox;

        blank = EntityBlank.GetBlank(t);
        var slots = (Data.Length - Box) / blank.SIZE_STORED;
        BoxCount = slots / SlotsPerBox;
    }

    protected readonly int SlotsPerBox;

    protected internal override string ShortSummary => $"{Checksums.CRC16Invert(Data.AsSpan(Box)):X4}";
    public override string Extension => ".bin";
    public sealed override bool ChecksumsValid => true;
    public sealed override string ChecksumInfo => "No Info.";

    private readonly PKM blank;
    public sealed override Type PKMType => blank.GetType();
    public sealed override PKM BlankPKM => blank.Clone();

    protected override PKM GetPKM(byte[] data) => EntityFormat.GetFromBytes(data, prefer: Context) ?? blank;
    protected override byte[] DecryptPKM(byte[] data) => GetPKM(data).Data;

    protected override int SIZE_STORED => blank.SIZE_STORED;
    protected override int SIZE_PARTY => blank.SIZE_PARTY;
    public sealed override int MaxEV => blank.MaxEV;
    public sealed override byte Generation => blank.Format;
    public sealed override EntityContext Context => blank.Context;
    public sealed override ushort MaxMoveID => blank.MaxMoveID;
    public sealed override ushort MaxSpeciesID => blank.MaxSpeciesID;
    public sealed override int MaxAbilityID => blank.MaxAbilityID;
    public sealed override int MaxItemID => blank.MaxItemID;
    public sealed override int MaxBallID => blank.MaxBallID;
    public sealed override GameVersion MaxGameID => blank.MaxGameID;
    public sealed override int MaxStringLengthTrainer => blank.MaxStringLengthTrainer;
    public sealed override int MaxStringLengthNickname => blank.MaxStringLengthNickname;
    public bool IsBigEndian => blank is BK4 or XK3 or CK3;

    public override int BoxCount { get; }
    protected override void SetChecksums() { }

    public override int GetBoxOffset(int box) => Box + (box * (SlotsPerBox * SIZE_STORED));
    public sealed override int GetPartyOffset(int slot) => int.MinValue;

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter.GetString(data, Generation, blank.Japanese, IsBigEndian, Language);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter.LoadString(data, destBuffer, Generation, blank.Japanese, IsBigEndian, Language);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter.SetString(destBuffer, value, maxLength, option: option, generation: Generation, jp: blank.Japanese, isBigEndian: IsBigEndian, language: Language);
}
