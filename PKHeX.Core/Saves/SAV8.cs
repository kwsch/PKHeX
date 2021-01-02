using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV8 : SaveFile, ISaveBlock8Main
    {
        // Save Data Attributes
        protected internal override string ShortSummary => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Extension => string.Empty;

        public override IReadOnlyList<string> PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen <= 8; // future: change to <= when HOME released
        }).ToArray();

        protected SAV8(byte[] data) : base(data) { }
        protected SAV8() { }

        // Configuration
        protected override int SIZE_STORED => PokeCrypto.SIZE_8STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
        public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8PARTY;
        public override PKM BlankPKM => new PK8();
        public override Type PKMType => typeof(PK8);

        public override int BoxCount => BoxLayout8.BoxCount;
        public override int MaxEV => 252;
        public override int Generation => 8;
        // protected override int GiftCountMax => 48;
        // protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventConstMax => 1000;
        public override int OTLength => 12;
        public override int NickLength => 12;
        protected override PKM GetPKM(byte[] data) => new PK8(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray8(data);

        #region Blocks
        public abstract Box8 BoxInfo { get; }
        public abstract Party8 PartyInfo { get; }
        public abstract MyItem Items { get; }
        public abstract PlayTime8 Played { get; }
        public abstract MyStatus8 MyStatus { get; }
        public abstract Misc8 Misc { get; }
        public abstract Zukan8 Zukan { get; }
        public abstract BoxLayout8 BoxLayout { get; }
        public abstract Fused8 Fused { get; }
        public abstract Daycare8 Daycare { get; }
        public abstract Record8 Records { get; }
        public abstract TrainerCard8 TrainerCard { get; }
        public abstract RaidSpawnList8 Raid { get; }
        public abstract RaidSpawnList8 RaidArmor { get; }
        public abstract RaidSpawnList8 RaidCrown { get; }
        public abstract TitleScreen8 TitleScreen { get; }
        public abstract TeamIndexes8 TeamIndexes { get; }
        #endregion

        public override GameVersion Version => Game switch
        {
            (int)GameVersion.SW => GameVersion.SW,
            (int)GameVersion.SH => GameVersion.SH,
            _ => GameVersion.Invalid
        };

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7b(value, maxLength, Language, PadToSize, PadWith);
        }

        // Player Information
        public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
        public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
        public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
        public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
        public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
        public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
        public override uint Money { get => Misc.Money; set => Misc.Money = value; }
        public int Badges { get => Misc.Badges; set => Misc.Badges = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

        // Inventory
        public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_PARTY * box * 30);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override byte[] GetDataForBox(PKM pkm) => pkm.EncryptedPartyData;

        protected override void SetPKM(PKM pkm)
        {
            PK8 pk = (PK8)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            pk.Trade(this, Date.Day, Date.Month, Date.Year);
            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            if (pkm.WasEgg)
            {
                Records.AddRecord(00);
            }
            else // capture, assume wild
            {
                Records.AddRecord(01); // wild capture
                Records.AddRecord(06); // total captured
                Records.AddRecord(16); // wild encountered
            }
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(17, 2); // trade * 2 -- these games count 1 trade as 2 for some reason.
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override int PartyCount
        {
            get => PartyInfo.PartyCount;
            protected set => PartyInfo.PartyCount = value;
        }

        protected override byte[] BoxBuffer => BoxInfo.Data;
        protected override byte[] PartyBuffer => PartyInfo.Data;
        public override PKM GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
        public override PKM GetBoxSlot(int offset) => GetDecryptedPKM(GetData(BoxInfo.Data, offset, SIZE_PARTY)); // party format in boxes!
    }
}