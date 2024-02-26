using System;

namespace PKHeX.Core;

public sealed class MysteryBlock7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    private const int FlagStart = 0;
    private const int MaxReceivedFlag = 2048;
    private const int MaxCardsPresent = 48;
    // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
    private const int CardStart = FlagStart + (MaxReceivedFlag / 8);

    // Mystery Gift
    public bool[] MysteryGiftReceivedFlags
    {
        get => FlagUtil.GetBitFlagArray(Data, MaxReceivedFlag);
        set
        {
            if (value.Length != MaxReceivedFlag)
                return;
            FlagUtil.SetBitFlagArray(Data, value);
            SAV.State.Edited = true;
        }
    }

    public DataMysteryGift[] MysteryGiftCards
    {
        get
        {
            var cards = new DataMysteryGift[MaxCardsPresent];
            for (int i = 0; i < cards.Length; i++)
                cards[i] = GetGift(i);
            return cards;
        }
        set
        {
            int count = Math.Min(MaxCardsPresent, value.Length);
            for (int i = 0; i < count; i++)
                SetGift((WC7)value[i], i);
            for (int i = value.Length; i < MaxCardsPresent; i++)
                SetGift(new WC7(), i);
        }
    }

    private WC7 GetGift(int index)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));

        var offset = GetGiftOffset(index);
        var data = Data.Slice(offset, WC7.Size).ToArray();
        return new WC7(data);
    }

    private static int GetGiftOffset(int index) => CardStart + (index * WC7.Size);

    private void SetGift(WC7 wc7, int index)
    {
        if ((uint)index > MaxCardsPresent)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (wc7.Data.Length != WC7.Size)
            throw new InvalidCastException(nameof(wc7));

        SAV.SetData(Data[GetGiftOffset(index)..], wc7.Data);
    }
}
