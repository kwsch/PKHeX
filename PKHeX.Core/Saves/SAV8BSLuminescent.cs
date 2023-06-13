using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.BDSPLUMI"/> games.
    /// </summary>
    public sealed class SAV8BSLuminescent : SAV8BS
    {
        public SAV8BSLuminescent(byte[] data, bool exportable = true) : base(data, exportable)
        {
            FlagWork = new FlagWork8b(this, 0x00004);
            Items = new MyItem8b(this, 0x0563C);
            Underground = new UndergroundItemList8b(this, 0x111BC);
            SelectBoundItems = new SaveItemShortcut8b(this, 0x14090); // size: 0x8
            PartyInfo = new Party8b(this, 0x14098);
            BoxLayout = new BoxLayout8b(this, 0x148AA); // size: 0x64A
            // 0x14EF4 - Box[40]

            // PLAYER_DATA:
            Config = new ConfigSave8b(this, 0x79B74); // size: 0x40
            MyStatus = new MyStatus8b(this, 0x79BB4); // size: 0x50
            Played = new PlayTime8b(this, 0x79C04); // size: 0x04
            Contest = new Contest8b(this, 0x79C08); // size: 0x720

            Zukan = (SaveRevision & 0x0100) == 0x0100 ? new Zukan8bLumi(this, 0x7A328) : new Zukan8b(this, 0x7A328); // size: 0x30B8
            BattleTrainer = new BattleTrainerStatus8bLumi(this, 0x7D3E0); // size: 0x1618
            MenuSelection = new MenuSelect8b(this, 0x7E9F8); // size: 0x44
            FieldObjects = new FieldObjectSave8b(this, 0x7EA3C); // size: 0x109A0 (1000 * 0x44)
            Records = new Record8b(this, 0x8F3DC); // size: 0x78 * 12
            Encounter = new EncounterSave8b(this, 0x8F97C); // size: 0x188
            Player = new PlayerData8b(this, 0x8FB04); // 0x80
            SealDeco = new SealBallDecoData8b(this, 0x8FB84); // size: 0x4288
            SealList = new SealList8b(this, 0x93E0C); // size: 0x960 SaveSealData[200]
            Random = new RandomGroup8b(this, 0x9476C); // size: 0x630
            FieldGimmick = new FieldGimmickSave8b(this, 0x94D9C); // FieldGimmickSaveData; int[3] gearRotate
            BerryTrees = new BerryTreeGrowSave8b(this, 0x94DA8); // size: 0x808
            Poffins = new PoffinSaveData8b(this, 0x955B0); // size: 0x644
            BattleTower = new BattleTowerWork8b(this, 0x95BF4); // size: 0x1B8
            System = new SystemData8b(this, 0x95DAC); // size: 0x138
            Poketch = new Poketch8b(this, 0x95EE4); // todo
            Daycare = new Daycare8b(this, 0x96080); // 0x2C0
            // 0x96340 - _DENDOU_SAVEDATA; DENDOU_RECORD[30], POKEMON_DATA_INSIDE[6], ushort[4] ?
            // BadgeSaveData; byte[8]
            // BoukenNote; byte[24]
            // TV_DATA (int[48], TV_STR_DATA[42]), (int[37], bool[37])*2, (int[8], int[8]), TV_STR_DATA[10]; 144 128bit zeroed (900 bytes?)? 
            UgSaveData = new UgSaveData8b(this, 0x9A89C); // size: 0x27A0
            // 0x9D03C - GMS_DATA // size: 0x31304, (GMS_POINT_DATA[650], ushort, ushort, byte)?; substructure GMS_POINT_HISTORY_DATA[5]
            // 0xCE340 - PLAYER_NETWORK_DATA; bcatFlagArray byte[1300]
            UnionSave = new UnionSaveData8b(this, 0xCEA10); // size: 0xC
            ContestPhotoLanguage = new ContestPhotoLanguage8b(this, 0xCEA1C); // size: 0x18
            ZukanExtra = new ZukanSpinda8b(this, 0xCEA34); // size: 0x64 (100)
            // CON_PHOTO_EXT_DATA[5]
            // GMS_POINT_HISTORY_EXT_DATA[3250]
            UgCount = new UgCountRecord8b(this, 0xE8178); // size: 0x20
            // 0xE8198 - ReBuffnameData; RE_DENDOU_RECORD[30], RE_DENDOU_POKEMON_DATA_INSIDE[6] (0x20) = 0x1680
            // 0xE9818 -- 0x10 byte[] MD5 hash of all savedata;

            // v1.1 additions
            RecordAdd = new RecordAddData8b(this, 0xE9828); // size: 0x3C0
            MysteryRecords = new MysteryBlock8b(this, 0xE9BE8); // size: ???
                                                                // POKETCH_POKETORE_COUNT_ARRAY -- (u16 species, u16 unused, i32 count, i32 reserved, i32 reserved)[3] = 0x10bytes
                                                                // PLAYREPORT_DATA -- reporting player progress online? 248 bytes?
                                                                // MT_DATA mtData; -- 0x400 bytes
                                                                // DENDOU_SAVE_ADD -- language tracking of members (hall of fame?); ADD_POKE_MEMBER[30], ADD_POKE[6]

            // v1.2 additions
            // ReBuffnameData reBuffNameDat -- RE_DENDOU_RECORD[], RE_DENDOU_RECORD is an RE_DENDOU_POKEMON_DATA_INSIDE[] with nicknames
            // PLAYREPORT_DATA playReportData    sizeof(0xF8)
            // PLAYREPORT_DATA playReportDataRef sizeof(0xF8)

            Initialize();
        }

        public SAV8BSLuminescent() : this(new byte[SaveUtil.SIZE_G8BDSPLUMI_3], false) => SaveRevision = (int)Gem8LumiVersion.V1_3rv1;

        private void Initialize()
        {
            Box = 0x14EF4;
            Party = PartyInfo.Offset;
            PokeDex = Zukan.PokeDex;
            DaycareOffset = Daycare.Offset;

            ReloadBattleTeams();
            TeamSlots = BoxLayout.TeamSlots;
        }

        // Configuration
        protected override int SIZE_STORED => PokeCrypto.SIZE_8STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
        public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8PARTY;
        public override PB8LUMI BlankPKM => new();
        public override Type PKMType => typeof(PB8LUMI);

        public override int BoxCount => BoxLayout8b.BoxCount;
        public override int MaxEV => 252;

        public override int Generation => 8;
        public override EntityContext Context => EntityContext.Gen8b;
        public override PersonalTable8BDSP Personal => PersonalTable.BDSPLUMI;
        public override int MaxStringLengthOT => 12;
        public override int MaxStringLengthNickname => 12;
        public override ushort MaxMoveID => Legal.MaxMoveID_8b;
        public override ushort MaxSpeciesID => (ushort)Species.MAX_COUNT - 1;
        public override int MaxItemID => 1835;
        public override int MaxBallID => Legal.MaxBallID_8b;
        public override int MaxGameID => Legal.MaxGameID_8b;
        public override int MaxAbilityID => Legal.MaxAbilityID_8b;

        public new int SaveRevision
        {
            get => ReadUInt16LittleEndian(Data) switch
            {
                0x00 when Data.Length == SaveUtil.SIZE_G8BDSPLUMI_1 => (int)Gem8LumiVersion.V1_1, // 1.1.0-Luminescent
                0x00 or (int)Gem8LumiVersion.V1_3 when Data.Length == SaveUtil.SIZE_G8BDSPLUMI_3 => (int)Gem8LumiVersion.V1_3, // 1.3.0-Luminescent
                _ => (int)Gem8LumiVersion.V1_3rv1,
            };
            init => WriteUInt32LittleEndian(Data.AsSpan(0), (uint)(0xFFFF0000 | value));
        }

        public override string SaveRevisionString => ((Gem8LumiVersion)SaveRevision).GetSuffixString();

        protected override SAV8BSLuminescent CloneInternal() => new((byte[])(Data.Clone()));

        private void ReloadBattleTeams()
        {
            if (!State.Exportable)
                BoxLayout.ClearBattleTeams();
            else // Valid slot locking info present
                BoxLayout.LoadBattleTeams();
        }

        protected override PB8LUMI GetPKM(byte[] data) => new(data);

        #region Blocks
        // public Box8 BoxInfo { get; }
        public new FlagWork8b FlagWork { get; }
        public new MyItem8b Items { get; }
        public new UndergroundItemList8b Underground { get; }
        public new SaveItemShortcut8b SelectBoundItems { get; }
        public new Party8b PartyInfo { get; }
        // public MyItem Items { get; }
        public new BoxLayout8b BoxLayout { get; }
        public new ConfigSave8b Config { get; }
        public new MyStatus8b MyStatus { get; }
        public new PlayTime8b Played { get; }
        public new Contest8b Contest { get; }
        // public Misc8 Misc { get; }
        public new Zukan8b Zukan { get; }
        public new BattleTrainerStatus8bLumi BattleTrainer { get; }
        public new MenuSelect8b MenuSelection { get; }
        public new FieldObjectSave8b FieldObjects { get; }
        public new Record8b Records { get; }
        public new EncounterSave8b Encounter { get; }
        public new PlayerData8b Player { get; }
        public new SealBallDecoData8b SealDeco { get; }
        public new SealList8b SealList { get; }
        public new RandomGroup8b Random { get; }
        public new FieldGimmickSave8b FieldGimmick { get; }
        public new BerryTreeGrowSave8b BerryTrees { get; }
        public new PoffinSaveData8b Poffins { get; }
        public new BattleTowerWork8b BattleTower { get; }
        public new SystemData8b System { get; }
        public new Poketch8b Poketch { get; }
        public new Daycare8b Daycare { get; }
        public new UgSaveData8b UgSaveData { get; }
        public new UnionSaveData8b UnionSave { get; }
        public new ContestPhotoLanguage8b ContestPhotoLanguage { get; }
        public new ZukanSpinda8b ZukanExtra { get; }
        public new UgCountRecord8b UgCount { get; }

        // First Savedata Expansion!
        public new RecordAddData8b RecordAdd { get; }
        public new MysteryBlock8b MysteryRecords { get; }
        #endregion

        protected override void SetPKM(PKM pkm, bool isParty = false)
        {
            var pk = (PB8LUMI)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            pk.Trade(this, Date.Day, Date.Month, Date.Year);

            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }
    }
}
