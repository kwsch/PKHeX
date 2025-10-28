using System;

namespace PKHeX.Core;

public sealed class WB7Records(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw), IMysteryGiftStorage, IMysteryGiftFlags
{
    private const int CardStart = 0;
    private const int FlagStart = (MaxCardsPresent * WR7.Size);

    private const int MaxCardsPresent = 10; // 0xE90 > (0x140 * 0xA = 0xC80)
    private const int MaxReceivedFlag = 0x1080; // (4224) end of the block -- max ever distributed was 2001 (Mew)

    public void ClearReceivedFlags() => Data[..(MaxReceivedFlag / 8)].Clear();

    private static int GetGiftOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCardsPresent);
        return CardStart + (index * WR7.Size);
    }

    private Span<byte> GetCardSpan(int index) => Data.Slice(GetGiftOffset(index), WR7.Size);

    public WR7 GetMysteryGift(int index) => new(GetCardSpan(index).ToArray());

    public void SetMysteryGift(int index, WR7 wr7)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (wr7.Data.Length != WR7.Size)
            throw new InvalidCastException(nameof(wr7));

        SAV.SetData(Data[GetGiftOffset(index)..], wr7.Data);
    }

    public int MysteryGiftReceivedFlagMax => MaxReceivedFlag;
    public bool GetMysteryGiftReceivedFlag(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        return FlagUtil.GetFlag(Data[FlagStart..], index); // offset 0
    }

    public void SetMysteryGiftReceivedFlag(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        FlagUtil.SetFlag(Data[FlagStart..], index, value); // offset 0
    }

    public int GiftCountMax => MaxCardsPresent;
    DataMysteryGift IMysteryGiftStorage.GetMysteryGift(int index) => GetMysteryGift(index);
    void IMysteryGiftStorage.SetMysteryGift(int index, DataMysteryGift gift) => SetMysteryGift(index, (WR7)gift);
}
