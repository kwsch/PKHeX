using System;

namespace PKHeX.Core
{
    public sealed class MysteryBlock6 : SaveBlock
    {
        private const int FlagStart = 0;
        private const int MaxReceivedFlag = 2048;
        private const int MaxCardsPresent = 24;
        // private const int FlagRegionSize = (MaxReceivedFlag / 8); // 0x100
        private const int CardStart = FlagStart + (MaxReceivedFlag / 8);

        public MysteryBlock6(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public MysteryBlock6(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        public bool[] GetReceivedFlags() => ArrayUtil.GitBitFlagArray(Data, Offset + FlagStart, MaxReceivedFlag);

        public void SetReceivedFlags(bool[] value)
        {
            if (value.Length != MaxReceivedFlag)
                return;
            ArrayUtil.SetBitFlagArray(Data, Offset + FlagStart, value);
            SAV.State.Edited = true;
        }

        public DataMysteryGift[] GetGifts()
        {
            var cards = new DataMysteryGift[MaxCardsPresent];
            for (int i = 0; i < cards.Length; i++)
                cards[i] = GetGift(i);
            return cards;
        }

        public void SetGifts(DataMysteryGift[] value)
        {
            int count = Math.Min(MaxCardsPresent, value.Length);
            for (int i = 0; i < count; i++)
                SetGift(value[i], i);
            for (int i = value.Length; i < MaxCardsPresent; i++)
                SetGift(new WC6(), i);
        }

        public DataMysteryGift GetGift(int index)
        {
            if ((uint)index > MaxCardsPresent)
                throw new ArgumentOutOfRangeException(nameof(index));

            var offset = GetGiftOffset(index);
            var data = SAV.GetData(offset, WC6.Size);
            return new WC6(data);
        }

        private int GetGiftOffset(int index) => Offset + CardStart + (index * WC6.Size);

        public void SetGift(DataMysteryGift wc6, int index)
        {
            if ((uint)index > MaxCardsPresent)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (wc6.Data.Length != WC6.Size)
                throw new InvalidCastException(nameof(wc6));

            if (wc6.CardID == 2048 && wc6.ItemID == 726) // Eon Ticket (OR/AS)
            {
                if (SAV is not SAV6AO ao)
                    return;
                // Set the special received data
                var info = ao.Blocks.Sango;
                info.ReceiveEon();
                info.EnableSendEon();
            }

            SAV.SetData(wc6.Data, GetGiftOffset(index));
        }
    }
}