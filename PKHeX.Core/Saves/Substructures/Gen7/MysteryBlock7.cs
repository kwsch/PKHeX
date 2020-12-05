using System;

namespace PKHeX.Core
{
    public sealed class MysteryBlock7 : SaveBlock
    {
        private const int FlagStart = 0;
        private const int MaxReceivedFlag = 2048;
        private const int MaxCardsPresent = 48;
        // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
        private const int CardStart = FlagStart + (MaxReceivedFlag / 8);

        public MysteryBlock7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public MysteryBlock7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // Mystery Gift
        public bool[] MysteryGiftReceivedFlags
        {
            get => ArrayUtil.GitBitFlagArray(Data, Offset + FlagStart, MaxReceivedFlag);
            set
            {
                if (value.Length != MaxReceivedFlag)
                    return;
                ArrayUtil.SetBitFlagArray(Data, Offset + FlagStart, value);
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
            var data = SAV.GetData(offset, WC7.Size);
            return new WC7(data);
        }

        private int GetGiftOffset(int index) => Offset + CardStart + (index * WC7.Size);

        private void SetGift(WC7 wc7, int index)
        {
            if ((uint)index > MaxCardsPresent)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (wc7.Data.Length != WC7.Size)
                throw new InvalidCastException(nameof(wc7));

            SAV.SetData(wc7.Data, GetGiftOffset(index));
        }
    }
}