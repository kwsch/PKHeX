using System;

namespace PKHeX.Core
{
    /// <summary> Generation 6 <see cref="PKM"/> format. </summary>
    public abstract class _K6 : PKM
    {
        private static readonly byte[] Unused =
        {
            0x36, 0x37, // Unused Ribbons
            0x58, 0x59, 0x73, 0x90, 0x91, 0x9E, 0x9F, 0xA0, 0xA1, 0xA7, 0xAA, 0xAB, 0xAC, 0xAD, 0xC8, 0xC9, 0xD7, 0xE4, 0xE5, 0xE6, 0xE7
        };

        public override byte[] ExtraBytes => Unused;

        public override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override int SIZE_STORED => PKX.SIZE_6STORED;

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x40, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0x40); } }
        public override byte[] HT_Trash { get => GetData(0x78, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0x78); } }
        public override byte[] OT_Trash { get => GetData(0xB0, 24); set { if (value?.Length == 24) value.CopyTo(Data, 0xB0); } }

        protected override ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < 0xE8; i += 2) // fixed value; pb7 overrides stored size
                chk += BitConverter.ToUInt16(Data, i);
            return chk;
        }

        // Simple Generated Attributes
        public override int CurrentFriendship
        {
            get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
        }

        public int OppositeFriendship
        {
            get => CurrentHandler == 1 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 1) OT_Friendship = value; else HT_Friendship = value; }
        }

        public override int SuperTrainingMedalCount(int maxCount = 30)
        {
            uint value = BitConverter.ToUInt32(Data, 0x2C);
            int TrainCount = 0;
            value >>= 2;
            for (int i = 0; i < maxCount; i++)
            {
                if ((value & 1) != 0)
                    TrainCount++;
                value >>= 1;
            }

            return TrainCount;
        }

        public override int PSV => (int)((PID >> 16 ^ (PID & 0xFFFF)) >> 4);
        public override int TSV => (TID ^ SID) >> 4;
        public override bool IsUntraded => Data[0x78] == 0 && Data[0x78 + 1] == 0 && Format == GenNumber; // immediately terminated HT_Name data (\0)

        // Complex Generated Attributes
        public override int Characteristic
        {
            get
            {
                int pm6 = (int)(EncryptionConstant % 6);
                int maxIV = MaximumIV;
                int pm6stat = 0;
                for (int i = 0; i < 6; i++)
                {
                    pm6stat = (pm6 + i) % 6;
                    if (GetIV(pm6stat) == maxIV)
                        break;
                }
                return (pm6stat * 5) + (maxIV % 5);
            }
        }

        // Methods
        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PKX.EncryptArray(Data);
        }

        // General User-error Fixes
        public void FixRelearn()
        {
            while (true)
            {
                if (RelearnMove4 != 0 && RelearnMove3 == 0)
                {
                    RelearnMove3 = RelearnMove4;
                    RelearnMove4 = 0;
                }
                if (RelearnMove3 != 0 && RelearnMove2 == 0)
                {
                    RelearnMove2 = RelearnMove3;
                    RelearnMove3 = 0;
                    continue;
                }
                if (RelearnMove2 != 0 && RelearnMove1 == 0)
                {
                    RelearnMove1 = RelearnMove2;
                    RelearnMove2 = 0;
                    continue;
                }
                break;
            }
        }

        // Synthetic Trading Logic
        public void Trade(ITrainerInfo tr, int Day = 1, int Month = 1, int Year = 2015)
        {
            if (IsEgg)
            {
                // Eggs do not have any modifications done if they are traded
                // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc)
                if ((tr.OT != OT_Name) || (tr.TID != TID) || (tr.SID != SID) || (tr.Gender != OT_Gender))
                    SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade6);
                return;
            }

            // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
            if (!TradeOT(tr))
                TradeHT(tr);
        }

        protected abstract bool TradeOT(ITrainerInfo tr);
        protected abstract void TradeHT(ITrainerInfo tr);

        // Misc Updates
        public virtual void TradeMemory(bool Bank)
        {
            HT_Memory = 4; // Link trade to [VAR: General Location]
            HT_TextVar = Bank ? 0 : 9; // Somewhere (Bank) : Pokécenter (Trade)
            HT_Intensity = 1;
            HT_Feeling = Memories.GetRandomFeeling(HT_Memory, Bank ? 10 : 20); // 0-9 Bank, 0-19 Trade
        }

        // Legality Properties
        public override bool WasLink => Met_Location == Locations.LinkGift6;
        public override bool WasEvent => Locations.IsEventLocation5(Met_Location) || FatefulEncounter;
        public override bool WasEventEgg => GenNumber < 5 ? base.WasEventEgg : (Locations.IsEventLocation5(Egg_Location) || (FatefulEncounter && Egg_Location == Locations.LinkTrade6)) && Met_Level == 1;

        // Maximums
        public override int MaxIV => 31;
        public override int MaxEV => 252;
        public override int OTLength => 12;
        public override int NickLength => 12;
    }
}
