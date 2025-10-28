using System;

namespace PKHeX.Core;

public sealed class MysteryBlock6 : SaveBlock<SAV6>, IMysteryGiftFlags, IMysteryGiftStorage
{
    private const int FlagStart = 0;
    private const int MaxReceivedFlag = 2048;
    private const int MaxCardsPresent = 24;
    // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    private const int CardStart = FlagStart + (MaxReceivedFlag / 8);
    public void ClearReceivedFlags() => Data[..(MaxReceivedFlag / 8)].Clear();

    public MysteryBlock6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public MysteryBlock6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    private static int GetGiftOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxCardsPresent);
        return CardStart + (index * WC6.Size);
    }

    private Span<byte> GetCardSpan(int index) => Data.Slice(GetGiftOffset(index), WC6.Size);

    public WC6 GetMysteryGift(int index) => new(GetCardSpan(index).ToArray());

    public void SetMysteryGift(int index, WC6 wc6)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (wc6.Data.Length != WC6.Size)
            throw new InvalidCastException(nameof(wc6));

        if (wc6 is { CardID: 2048, ItemID: 726 }) // Eon Ticket (OR/AS)
        {
            if (SAV is not SAV6AO ao)
                return;
            // Set the special received data
            var info = ao.Blocks.Sango;
            info.ReceiveEon();
            info.EnableSendEon();
        }

        SAV.SetData(GetCardSpan(index), wc6.Data);
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
    void IMysteryGiftStorage.SetMysteryGift(int index, DataMysteryGift gift) => SetMysteryGift(index, (WC6)gift);
}
