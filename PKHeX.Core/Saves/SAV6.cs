using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV6 : SAV_BEEF, ITrainerStatRecord, ISaveBlock6Core, IRegionOrigin, IGameSync
    {
        // Save Data Attributes
        protected internal override string ShortSummary => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Extension => string.Empty;

        protected SAV6(byte[] data, int biOffset) : base(data, biOffset) { }
        protected SAV6(int size, int biOffset) : base(size, biOffset) { }

        // Configuration
        protected override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
        public override PKM BlankPKM => new PK6();
        public override Type PKMType => typeof(PK6);

        public override int BoxCount => 31;
        public override int MaxEV => 252;
        public override int Generation => 6;
        protected override int GiftCountMax => 24;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 8 * 0x1A0;
        protected override int EventConstMax => (EventFlag - EventConst) / sizeof(ushort);
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxSpeciesID => Legal.MaxSpeciesID_6;
        public override int MaxBallID => Legal.MaxBallID_6;
        public override int MaxGameID => Legal.MaxGameID_6; // OR

        protected override PKM GetPKM(byte[] data) => new PK6(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);

        protected int WondercardFlags { get; set; } = int.MinValue;
        protected int JPEG { get; set; } = int.MinValue;
        public int PSS { get; protected set; } = int.MinValue;
        public int BerryField { get; protected set; } = int.MinValue;
        public int HoF { get; protected set; } = int.MinValue;
        protected int PCLayout { private get; set; } = int.MinValue;
        protected int BattleBoxOffset { get; set; } = int.MinValue;
        public int GetBattleBoxSlot(int slot) => BattleBoxOffset + (slot * SIZE_STORED);

        public virtual string JPEGTitle => string.Empty;
        public virtual byte[] GetJPEGData() => Array.Empty<byte>();

        protected internal const int LongStringLength = 0x22; // bytes, not characters
        protected internal const int ShortStringLength = 0x1A; // bytes, not characters

        // Player Information
        public override int TID { get => Status.TID; set => Status.TID = value; }
        public override int SID { get => Status.SID; set => Status.SID = value; }
        public override int Game { get => Status.Game; set => Status.Game = value; }
        public override int Gender { get => Status.Gender; set => Status.Gender = value; }
        public override int Language { get => Status.Language; set => Status.Language = value; }
        public override string OT { get => Status.OT; set => Status.OT = value; }
        public int Region { get => Status.SubRegion; set => Status.SubRegion = value; }
        public int Country { get => Status.Country; set => Status.Country = value; }
        public int ConsoleRegion { get => Status.ConsoleRegion; set => Status.ConsoleRegion = value; }
        public int GameSyncIDSize => MyStatus6.GameSyncIDSize; // 64 bits
        public string GameSyncID { get => Status.GameSyncID; set => Status.GameSyncID = value; }
        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

        public abstract int Badges { get; set; }
        public abstract int Vivillon { get; set; }
        public abstract int BP { get; set; }
        // Money

        public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
        public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }
        public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Daycare
        public override int DaycareSeedSize => 16;

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);

        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

        private int GetBoxNameOffset(int box) => PCLayout + (LongStringLength * box);

        public override string GetBoxName(int box)
        {
            if (PCLayout < 0)
                return $"B{box + 1}";
            return GetString(Data, GetBoxNameOffset(box), LongStringLength);
        }

        public override void SetBoxName(int box, string value)
        {
            var data = SetString(value, LongStringLength / 2, LongStringLength / 2);
            SetData(data, PCLayout + (LongStringLength * box));
        }

        protected override void SetPKM(PKM pkm, bool isParty = false)
        {
            PK6 pk6 = (PK6)pkm;
            // Apply to this Save File
            int CT = pk6.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk6.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk6.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk6.HasMove(216)) // Return
                    pk6.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.HasMove(218)) // Frustration
                    pkm.CurrentFriendship = pk6.OppositeFriendship;
            }

            pk6.FormArgumentElapsed = pk6.FormArgumentMaximum = 0;
            pk6.FormArgumentRemain = (byte)GetFormArgument(pkm, isParty);
            if (!isParty && pkm.Form != 0)
            {
                switch (pkm.Species)
                {
                    case (int) Species.Furfrou:
                        pkm.Form = 0;
                        break;
                    case (int) Species.Hoopa:
                    {
                        pkm.Form = 0;
                        var hsf = Array.IndexOf(pkm.Moves, (int) Move.HyperspaceFury);
                        if (hsf != -1)
                            pkm.SetMove(hsf, (int) Move.HyperspaceHole);
                        break;
                    }
                }
            }

            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            Records.AddRecord(pkm.WasEgg ? 009 : 007); // egg, capture
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(012); // trade
            if (!pkm.WasEgg)
            {
                Records.AddRecord(004); // total battles
                Records.AddRecord(005); // wild encounters
            }
        }

        private static uint GetFormArgument(PKM pkm, bool isParty)
        {
            if (!isParty || pkm.Form == 0)
                return 0;
            return pkm.Species switch
            {
                (int)Species.Furfrou => 5u, // Furfrou
                (int)Species.Hoopa => 3u, // Hoopa
                _ => 0u
            };
        }

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString6(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString6(value, maxLength, PadToSize, PadWith);
        }

        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int RecordCount => RecordBlock6.RecordCount;
        public abstract MyItem Items { get; }
        public abstract ItemInfo6 ItemInfo { get; }
        public abstract GameTime6 GameTime { get; }
        public abstract Situation6 Situation { get; }
        public abstract PlayTime6 Played { get; }
        public abstract MyStatus6 Status { get; }
        public abstract RecordBlock6 Records { get; }
    }
}
