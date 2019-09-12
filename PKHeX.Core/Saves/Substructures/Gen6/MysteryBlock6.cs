using System;

namespace PKHeX.Core
{
    public class MysteryBlock6 : SaveBlock
    {
        private const int FlagStart = 0;
        private const int MaxReceivedFlag = 2048;
        private const int MaxCardsPresent = 24;
        // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
        private const int CardStart = FlagStart + (MaxReceivedFlag / 8);

        public MysteryBlock6(SAV6 sav, int offset) : base(sav) => Offset = offset;

        public bool[] MysteryGiftReceivedFlags
        {
            get => ArrayUtil.GitBitFlagArray(Data, Offset + FlagStart, MaxReceivedFlag);
            set
            {
                if (value?.Length != MaxReceivedFlag)
                    return;
                ArrayUtil.SetBitFlagArray(Data, Offset + FlagStart, value);
                SAV.Edited = true;
            }
        }

        public MysteryGift[] MysteryGiftCards
        {
            get
            {
                var cards = new MysteryGift[MaxCardsPresent];
                for (int i = 0; i < cards.Length; i++)
                    cards[i] = GetGift(i);
                return cards;
            }
            set
            {
                int count = Math.Min(MaxCardsPresent, value.Length);
                for (int i = 0; i < count; i++)
                    SetGift(value[i], i);
                for (int i = value.Length; i < MaxCardsPresent; i++)
                    SetGift(new WC6(), i);
            }
        }

        public MysteryGift GetGift(int index)
        {
            if ((uint)index > MaxCardsPresent)
                throw new ArgumentOutOfRangeException(nameof(index));

            var offset = GetGiftOffset(index);
            var data = SAV.GetData(offset, WC6.Size);
            return new WC6(data);
        }

        private int GetGiftOffset(int index) => Offset + CardStart + (index * WC6.Size);

        public void SetGift(MysteryGift wc6, int index)
        {
            if ((uint)index > MaxCardsPresent)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (wc6.Data.Length != WC6.Size)
                throw new InvalidCastException(nameof(wc6));

            if (wc6.CardID == 2048 && wc6.ItemID == 726) // Eon Ticket (OR/AS)
            {
                if (!(SAV is SAV6AO ao))
                    return;
                // Set the special received data
                var info = ao.Sango;
                info.ReceiveEon();
                info.EnableSendEon();
            }

            SAV.SetData(wc6.Data, GetGiftOffset(index));
        }
    }
}