using System;

namespace PKHeX.Core;

public sealed class MysteryBlock7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw), IMysteryGiftStorage, IMysteryGiftFlags
{
    private const int FlagStart = 0;
    private const int MaxReceivedFlag = 2048;
    private const int MaxCardsPresent = 48;
    // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    private const int CardStart = FlagStart + (MaxReceivedFlag / 8);
    public void ClearReceivedFlags() => Data[..(MaxReceivedFlag / 8)].Clear();

    private static int GetGiftOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCardsPresent);
        return CardStart + (index * WC7.Size);
    }

    private Span<byte> GetCardSpan(int index) => Data.Slice(GetGiftOffset(index), WC7.Size);

    public WC7 GetMysteryGift(int index) => new(GetCardSpan(index).ToArray());

    public void SetMysteryGift(int index, WC7 wc7)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (wc7.Data.Length != WC7.Size)
            throw new InvalidCastException(nameof(wc7));

        SAV.SetData(Data[GetGiftOffset(index)..], wc7.Data);
    }

    public int MysteryGiftReceivedFlagMax => MaxReceivedFlag;
    public bool GetMysteryGiftReceivedFlag(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        return FlagUtil.GetFlag(Data, index); // offset 0
    }

    public void SetMysteryGiftReceivedFlag(int index, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxReceivedFlag);
        FlagUtil.SetFlag(Data, index, value); // offset 0
    }

    public int GiftCountMax => MaxCardsPresent;
    DataMysteryGift IMysteryGiftStorage.GetMysteryGift(int index) => GetMysteryGift(index);
    void IMysteryGiftStorage.SetMysteryGift(int index, DataMysteryGift gift) => SetMysteryGift(index, (WC7)gift);
}
