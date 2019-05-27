using System;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV7 : SaveFile, ITrainerStatRecord, ISecureValueStorage
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => string.Empty;

        public override string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen <= 7 && f[1] != 'b'; // ignore PB7
        }).ToArray();

        protected SAV7(byte[] data) : base(data)
        {
            Blocks = BlockInfo3DS.GetBlockInfoData(Data, out BlockInfoOffset, Checksums.CRC16);
            CanReadChecksums();
            Initialize();
        }

        protected SAV7(int size) : base(size)
        {
            Blocks = BlockInfo3DS.GetBlockInfoData(Data, out BlockInfoOffset, Checksums.CRC16);
            Initialize();
            ClearBoxes();
        }

        private void Initialize()
        {
            GetSAVOffsets();

            HeldItems = USUM ? Legal.HeldItems_USUM : Legal.HeldItems_SM;
            Personal = USUM ? PersonalTable.USUM : PersonalTable.SM;

            var demo = !USUM && Data.Skip(PCLayout).Take(0x4C4).All(z => z == 0); // up to Battle Box values
            if (demo || !Exportable)
            {
                BoxLayout.ClearBattleTeams();
            }
            else // Valid slot locking info present
            {
                BoxLayout.LoadBattleTeams();
            }
        }

        // Configuration
        public override int SIZE_STORED => PKX.SIZE_6STORED;
        protected override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK7();
        public override Type PKMType => typeof(PK7);

        public override int BoxCount => 32;
        public override int MaxEV => 252;
        public override int Generation => 7;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventConstMax => 1000;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxBallID => Legal.MaxBallID_7; // 26
        public override int MaxGameID => Legal.MaxGameID_7;
        protected override PKM GetPKM(byte[] data) => new PK7(data);
        protected override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray(data);

        // Feature Overrides

        // Blocks & Offsets
        private readonly int BlockInfoOffset;
        private readonly BlockInfo[] Blocks;
        private bool IsMemeCryptoApplied = true;
        private const int MemeCryptoBlock = 36;
        public override bool ChecksumsValid => CanReadChecksums() && Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => CanReadChecksums() ? Blocks.GetChecksumInfo(Data) : string.Empty;

        private bool CanReadChecksums()
        {
            if (Blocks.Length <= MemeCryptoBlock)
            { Debug.WriteLine($"Not enough blocks ({Blocks.Length}), aborting {nameof(CanReadChecksums)}"); return false; }
            if (!IsMemeCryptoApplied)
                return true;
            // clear memecrypto sig
            new byte[0x80].CopyTo(Data, Blocks[MemeCryptoBlock].Offset + 0x100);
            IsMemeCryptoApplied = false;
            return true;
        }

        protected override void SetChecksums()
        {
            if (!CanReadChecksums())
                return;
            Blocks.SetChecksums(Data);
            BoxLayout.SaveBattleTeams();
            Data = MemeCrypto.Resign7(Data);
            IsMemeCryptoApplied = true;
        }

        public ulong TimeStampCurrent
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0x14);
        }

        public ulong TimeStampPrevious
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0xC);
        }

        private void GetSAVOffsets()
        {
            /* 00 */ Bag            = Blocks[00].Offset; // 0x00000  // [DE0]    MyItem
            /* 01 */ Trainer1       = Blocks[01].Offset; // 0x00E00  // [07C]    Situation
            /* 02 */            //  = Blocks[02].Offset; // 0x01000  // [014]    RandomGroup
            /* 03 */ TrainerCard    = Blocks[03].Offset; // 0x01200  // [0C0]    MyStatus
            /* 04 */ Party          = Blocks[04].Offset; // 0x01400  // [61C]    PokePartySave
            /* 05 */ EventConst     = Blocks[05].Offset; // 0x01C00  // [E00]    EventWork
            /* 06 */ PokeDex        = Blocks[06].Offset; // 0x02A00  // [F78]    ZukanData
            /* 07 */ GTS            = Blocks[07].Offset; // 0x03A00  // [228]    GtsData
            /* 08 */ Fused          = Blocks[08].Offset; // 0x03E00  // [104]    UnionPokemon
            /* 09 */ Misc           = Blocks[09].Offset; // 0x04000  // [200]    Misc
            /* 10 */ Trainer2       = Blocks[10].Offset; // 0x04200  // [020]    FieldMenu
            /* 11 */ ConfigSave     = Blocks[11].Offset; // 0x04400  // [004]    ConfigSave
            /* 12 */ AdventureInfo  = Blocks[12].Offset; // 0x04600  // [058]    GameTime
            /* 13 */ PCLayout       = Blocks[13].Offset; // 0x04800  // [5E6]    BOX
            /* 14 */ Box            = Blocks[14].Offset; // 0x04E00  // [36600]  BoxPokemon
            /* 15 */ Resort         = Blocks[15].Offset; // 0x3B400  // [572C]   ResortSave
            /* 16 */ PlayTime       = Blocks[16].Offset; // 0x40C00  // [008]    PlayTime
            /* 17 */ Overworld      = Blocks[17].Offset; // 0x40E00  // [1080]   FieldMoveModelSave
            /* 18 */ Fashion        = Blocks[18].Offset; // 0x42000  // [1A08]   Fashion
            /* 19 */            //  = Blocks[19].Offset; // 0x43C00  // [6408]   JoinFestaPersonalSave
            /* 20 */            //  = Blocks[20].Offset; // 0x4A200  // [6408]   JoinFestaPersonalSave
            /* 21 */ JoinFestaData  = Blocks[21].Offset; // 0x50800  // [3998]   JoinFestaDataSave
            /* 22 */            //  = Blocks[22].Offset; // 0x54200  // [100]    BerrySpot
            /* 23 */            //  = Blocks[23].Offset; // 0x54400  // [100]    FishingSpot
            /* 24 */            //  = Blocks[24].Offset; // 0x54600  // [10528]  LiveMatchData
            /* 25 */            //  = Blocks[25].Offset; // 0x64C00  // [204]    BattleSpotData
            /* 26 */ PokeFinderSave = Blocks[26].Offset; // 0x65000  // [B60]    PokeFinderSave
            /* 27 */ WondercardFlags= Blocks[27].Offset; // 0x65C00  // [3F50]   MysteryGiftSave
            /* 28 */ Record         = Blocks[28].Offset; // 0x69C00  // [358]    Record
            /* 29 */            //  = Blocks[29].Offset; // 0x6A000  // [728]    ValidationSave
            /* 30 */            //  = Blocks[30].Offset; // 0x6A800  // [200]    GameSyncSave
            /* 31 */            //  = Blocks[31].Offset; // 0x6AA00  // [718]    PokeDiarySave
            /* 32 */ BattleTree     = Blocks[32].Offset; // 0x6B200  // [1FC]    BattleInstSave
            /* 33 */ Daycare        = Blocks[33].Offset; // 0x6B400  // [200]    Sodateya
            /* 34 */            //  = Blocks[34].Offset; // 0x6B600  // [120]    WeatherSave
            /* 35 */ QRSaveData     = Blocks[35].Offset; // 0x6B800  // [1C8]    QRReaderSaveData
            /* 36 */            //  = Blocks[36].Offset; // 0x6BA00  // [200]    TurtleSalmonSave

            // USUM only
            /* 37 */            //  = Blocks[37].Offset;   BattleFesSave
            /* 38 */            //  = Blocks[38].Offset;   FinderStudioSave

            EventFlag = EventConst + (EventConstMax * 2); // After Event Const (u16)*n
            HoF = EventFlag + (EventFlagMax / 8); // After Event Flags (1b)*(1u8/8b)*n

            PokeDexLanguageFlags =  PokeDex + 0x550;
            WondercardData = WondercardFlags + 0x100;

            Played = new PlayTime6(this, PlayTime);
            MysteryBlock = new MysteryBlock7(this, WondercardFlags);
            PokeFinder = new PokeFinder7(this, PokeFinderSave);
            Festa = new JoinFesta7(this, JoinFestaData);
            DaycareBlock = new Daycare7(this, Daycare);
            Situation = new Situation7(this, Overworld);
            MyStatus = new MyStatus7(this, TrainerCard);
            OverworldBlock = new FieldMoveModelSave7(this, Overworld);
            Config = new ConfigSave7(this, ConfigSave);
            GameTime = new GameTime7(this, AdventureInfo);
            MiscBlock = new Misc7(this, Misc);
            BoxLayout = new BoxLayout7(this, PCLayout);
            BattleTreeBlock = new BattleTree7(this, BattleTree);
            ResortSave = new ResortSave7(this, Resort);
            FieldMenu = new FieldMenu7(this, Trainer2);
            FashionBlock = new FashionBlock7(this, Fashion);

            TeamSlots = BoxLayout.TeamSlots;
        }

        // Private Only
        protected int Bag { get; set; } = int.MinValue;
        private int AdventureInfo { get; set; } = int.MinValue;
        private int Trainer2 { get; set; } = int.MinValue;
        public int Misc { get; private set; } = int.MinValue;
        private int WondercardFlags { get; set; } = int.MinValue;
        private int PlayTime { get; set; } = int.MinValue;
        private int Overworld { get; set; } = int.MinValue;
        public int JoinFestaData { get; private set; } = int.MinValue;
        private int PokeFinderSave { get; set; } = int.MinValue;
        private int BattleTree { get; set; } = int.MinValue;
        private int ConfigSave { get; set; } = int.MinValue;
        public int QRSaveData { get; set; } = int.MinValue;

        protected MyItem Items { private get; set; }
        protected MysteryBlock7 MysteryBlock { private get; set; }
        public PokeFinder7 PokeFinder { get; private set; }
        public JoinFesta7 Festa { get; private set; }
        private Daycare7 DaycareBlock { get; set; }
        protected Record6 Records { get; set; }
        public PlayTime6 Played { get; set; }
        public MyStatus7 MyStatus { get; private set; }
        public FieldMoveModelSave7 OverworldBlock { get; private set; }
        public Situation7 Situation { get; private set; }
        public ConfigSave7 Config { get; private set; }
        public GameTime7 GameTime { get; private set; }
        public Misc7 MiscBlock { get; private set; }
        public Zukan7 Zukan { get; protected set; }
        private BoxLayout7 BoxLayout { get; set; }
        public BattleTree7 BattleTreeBlock { get; private set; }
        public ResortSave7 ResortSave { get; private set; }
        public FieldMenu7 FieldMenu { get; private set; }
        public FashionBlock7 FashionBlock { get; private set; }

        // Accessible as SAV7
        private int TrainerCard { get; set; } = 0x14000;
        private int Resort { get; set; }
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Fashion { get; set; } = int.MinValue;
        protected int Record { get; set; } = int.MinValue;

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case 30: return GameVersion.SN;
                    case 31: return GameVersion.MN;
                    case 32: return GameVersion.US;
                    case 33: return GameVersion.UM;
                }
                return GameVersion.Invalid;
            }
        }

        public override string MiscSaveInfo() => string.Join(Environment.NewLine, Blocks.Select(b => b.Summary));
        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7(value, maxLength, Language, PadToSize, PadWith);
        }

        // Player Information
        public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
        public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
        public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
        public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
        public override int GameSyncIDSize => MyStatus7.GameSyncIDSize; // 64 bits
        public override string GameSyncID { get => MyStatus.GameSyncID; set => MyStatus.GameSyncID = value; }
        public override int SubRegion { get => MyStatus.SubRegion; set => MyStatus.SubRegion = value; }
        public override int Country { get => MyStatus.Country; set => MyStatus.Country = value; }
        public override int ConsoleRegion { get => MyStatus.ConsoleRegion; set => MyStatus.ConsoleRegion = value; }
        public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
        public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
        public override int MultiplayerSpriteID { get => MyStatus.MultiplayerSpriteID; set => MyStatus.MultiplayerSpriteID = value; }
        public override uint Money { get => MiscBlock.Money; set => MiscBlock.Money = value; }
        
        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }
        public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
        public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }

        // Stat Records
        public int RecordCount => 200;
        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);

        // Inventory
        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);
        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
        public override void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
        public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = value; }

        protected override void SetPKM(PKM pkm)
        {
            PK7 pk7 = (PK7)pkm;
            // Apply to this Save File
            int CT = pk7.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk7.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk7.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk7.Moves.Contains(216)) // Return
                    pk7.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk7.OppositeFriendship;
            }
            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            Records.AddRecord(pkm.WasEgg ? 008 : 006); // egg, capture
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(011); // trade
            if (!pkm.WasEgg)
                Records.AddRecord(004); // wild encounters
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK7)pkm).FormDuration = GetFormDuration(pkm, isParty);
        }

        private static uint GetFormDuration(PKM pkm, bool isParty)
        {
            if (!isParty || pkm.AltForm == 0)
                return 0;
            switch (pkm.Species)
            {
                case 676: return 5; // Furfrou
                case 720: return 3; // Hoopa
                default: return 0;
            }
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            int team = Array.IndexOf(TeamSlots, index);
            if (team < 0)
                return StorageSlotFlag.None;

            team /= 6;
            var val = (StorageSlotFlag)((int)StorageSlotFlag.BattleTeam1 << team);
            if (BoxLayout.GetIsTeamLocked(team))
                val |= StorageSlotFlag.Locked;
            return val;
        }

        private int FusedCount => USUM ? 3 : 1;

        public int GetFusedSlotOffset(int slot)
        {
            if (Fused < 0 || slot < 0 || slot >= FusedCount)
                return -1;
            return Fused + (PKX.SIZE_6PARTY * slot); // 0x104*slot
        }

        public override int DaycareSeedSize => Daycare7.DaycareSeedSize; // 128 bits
        public override int GetDaycareSlotOffset(int loc, int slot) => DaycareBlock.GetDaycareSlotOffset(slot);
        public override bool? IsDaycareOccupied(int loc, int slot) => DaycareBlock.GetIsOccupied(slot);
        public override string GetDaycareRNGSeed(int loc) => DaycareBlock.RNGSeed;
        public override bool? IsDaycareHasEgg(int loc) => DaycareBlock.HasEgg;
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) => DaycareBlock.SetOccupied(slot, occupied);
        public override void SetDaycareRNGSeed(int loc, string seed) => DaycareBlock.RNGSeed = seed;
        public override void SetDaycareHasEgg(int loc, bool hasEgg) => DaycareBlock.HasEgg = hasEgg;

        protected override bool[] MysteryGiftReceivedFlags { get => MysteryBlock.MysteryGiftReceivedFlags; set => MysteryBlock.MysteryGiftReceivedFlags = value; }
        protected override MysteryGift[] MysteryGiftCards { get => MysteryBlock.MysteryGiftCards; set => MysteryBlock.MysteryGiftCards = value; }
    }
}
