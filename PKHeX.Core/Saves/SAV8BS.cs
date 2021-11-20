using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PKHeX.Core
{
    public class SAV8BS : SaveFile, ISaveFileRevision, ITrainerStatRecord
    {
        // Save Data Attributes
        protected internal override string ShortSummary => $"{OT} ({Version})";
        public override string Extension => string.Empty;

        public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
        {
            int gen = f[^1] - 0x30;
            return gen <= 8;
        });

        public SAV8BS(byte[] data, bool exportable = true) : base(data, exportable)
        {
            Work = new FlagWork8b(this, 0x00004);
            Items = new MyItem8b(this, 0x0563C);
            PartyInfo = new Party8b(this, 0x14098);
            BoxLayout = new BoxLayout8b(this, 0x148AA);
            Config = new ConfigSave8b(this, 0x79B74); // size: 0x40
            MyStatus = new MyStatus8b(this, 0x79BB4); // size: 0x50
            Played = new PlayTime8b(this, 0x79C04); // size: 0x04
            Contest = new Contest8b(this, 0x79C08); // size: 0x720
            Zukan = new Zukan8b(this, 0x7A328); // size: 0x30B8
            // 0x7D3E0 - Trainer Battle Data (bool,bool)[707]
            // 0x7E9F8 - Menu selections (TopMenuItemTypeInt32, bool IsNew)[8], TopMenuItemTypeInt32 LastSelected
            // 0x7EA3C - _FIELDOBJ_SAVE Objects[1000] (sizeof (0x44, 17 int fields), total size 0x109A0
            Records = new Record8b(this, 0x8F3DC);

            Initialize();
        }

        public SAV8BS() : this(new byte[SaveUtil.SIZE_G8BDSP_1], false) => SaveRevision = (int)Gem8Version.V1_1;

        private void Initialize()
        {
            Box = 0x14EF4;
            Party = PartyInfo.Offset;
            PokeDex = Zukan.PokeDex;
            BoxLayout.LoadBattleTeams();
        }

        public override bool HasEvents => true;

        // Configuration
        protected override int SIZE_STORED => PokeCrypto.SIZE_8STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
        public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8PARTY;
        public override PKM BlankPKM => new PB8();
        public override Type PKMType => typeof(PB8);

        public override int BoxCount => BoxLayout8b.BoxCount;
        public override int MaxEV => 252;

        public override int Generation => 8;
        protected override int EventConstMax => 500;
        public override PersonalTable Personal => PersonalTable.BDSP;
        public override int OTLength => 12;
        public override int NickLength => 12;
        public override int MaxMoveID => Legal.MaxMoveID_8b;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_8b;
        public override int MaxItemID => Legal.MaxItemID_8b;
        public override int MaxBallID => Legal.MaxBallID_8b;
        public override int MaxGameID => Legal.MaxGameID_8b;
        public override int MaxAbilityID => Legal.MaxAbilityID_8b;

        public int SaveRevision
        {
            get => BitConverter.ToInt32(Data, 0);
            init => BitConverter.GetBytes(value).CopyTo(Data, 0);
        }

        public string SaveRevisionString => (Gem8Version)SaveRevision switch
        {
            Gem8Version.V1_0 => "-1.0.0", // Launch Revision
            Gem8Version.V1_1 => "-1.1.0", // 1.1.0
            _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
        };

        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_BS;
        protected override SaveFile CloneInternal() => new SAV8BS((byte[])(Data.Clone()));

        protected override byte[] GetFinalData()
        {
            BoxLayout.SaveBattleTeams();
            return base.GetFinalData();
        }

        protected void ReloadBattleTeams()
        {
            if (!State.Exportable)
                BoxLayout.ClearBattleTeams();
            else // Valid slot locking info present
                BoxLayout.LoadBattleTeams();
        }

        #region Checksums

        private const int HashOffset = SaveUtil.SIZE_G8BDSP - 0x10;
        private Span<byte> CurrentHash => Data.AsSpan(SaveUtil.SIZE_G8BDSP - 0x10, 0x10);

        private byte[] ComputeHash()
        {
            CurrentHash.Fill(0);
            using var md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(Data);
        }

        protected override void SetChecksums() => ComputeHash().CopyTo(Data, HashOffset);
        public override string ChecksumInfo => !ChecksumsValid ? "MD5 Hash Invalid" : string.Empty;

        public override bool ChecksumsValid
        {
            get
            {
                // Cache hash and restore it after computation
                var original = CurrentHash.ToArray();
                var newHash = ComputeHash();
                var result = newHash.AsSpan().SequenceEqual(original);
                original.AsSpan().CopyTo(CurrentHash);
                return result;
            }
        }

        #endregion

        protected override PKM GetPKM(byte[] data) => new PB8(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray8(data);

        #region Blocks
        // public Box8 BoxInfo { get; }
        public FlagWork8b Work { get; }
        public MyItem8b Items { get; }
        public Party8b PartyInfo { get; }
        // public MyItem Items { get; }
        public BoxLayout8b BoxLayout { get; }
        public ConfigSave8b Config { get; }
        public MyStatus8b MyStatus { get; }
        public PlayTime8b Played { get; }
        public Contest8b Contest { get; }
        // public Misc8 Misc { get; }
        public Zukan8b Zukan { get; }
        public Record8b Records { get; }
        #endregion

        public override GameVersion Version => Game switch
        {
            (int)GameVersion.BD => GameVersion.BD,
            (int)GameVersion.SP => GameVersion.SP,
            _ => GameVersion.Invalid,
        };

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7b(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7b(value, maxLength, PadToSize, PadWith);
        }

        public override bool GetEventFlag(int flagNumber) => Work.GetFlag(flagNumber);
        public override void SetEventFlag(int flagNumber, bool value) => Work.SetFlag(flagNumber, value);

        // Player Information
        public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
        public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
        public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
        public override int Gender { get => MyStatus.Male ? 0 : 1; set => MyStatus.Male = value == 0; }
        public override int Language { get => Config.Language; set => Config.Language = value; }
        public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
        public override uint Money { get => MyStatus.Money; set => MyStatus.Money = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = (ushort)value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = (byte)value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = (byte)value; }

        // Inventory
        public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_PARTY * box * 30);
        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
        public override void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override byte[] GetDataForBox(PKM pkm) => pkm.EncryptedPartyData;
        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = (byte)value; }
        public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = (byte)value; }

        public string Rival
        {
            get => GetString(0x55F4, 0x1A);
            set => SetString(value, OTLength).CopyTo(Data, 0x55F4);
        }

        public short ZoneID // map
        {
            get => BitConverter.ToInt16(Data, 0x5634);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x5634);
        }

        public float TimeScale
        {
            get => BitConverter.ToSingle(Data, 0x5638);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x5638);
        }

        protected override void SetPKM(PKM pkm, bool isParty = false)
        {
            var pk = (PB8)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            pk.Trade(this, Date.Day, Date.Month, Date.Year);

            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            // There aren't many records, and they only track Capture/Fish/Hatch/Defeat.
            Records.AddRecord(pkm.WasEgg ? 004 : 002); // egg, capture
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override int PartyCount
        {
            get => PartyInfo.PartyCount;
            protected set => PartyInfo.PartyCount = value;
        }

        public override PKM GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
        public override PKM GetBoxSlot(int offset) => GetDecryptedPKM(GetData(Data, offset, SIZE_PARTY)); // party format in boxes!

        public enum TopMenuItemType
        {
            Zukan = 0,
            Pokemon = 1,
            Bag = 2,
            Card = 3,
            Map = 4,
            Seal = 5,
            Setting = 6,
            Gift = 7,
        }

        public int RecordCount => Record8b.RecordCount;
        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
        public int GetRecordMax(int recordID) => Record8b.RecordMaxValue;
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
    }
}
