using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.BDSP"/> games.
/// </summary>
public sealed class SAV8BS : SaveFile, ISaveFileRevision, ITrainerStatRecord, IEventFlagArray, IEventWorkArray<int>
{
    // Save Data Attributes
    protected internal override string ShortSummary => $"{OT} ({Version}) - {System.LastSavedTime}";
    public override string Extension => string.Empty;

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen <= 8;
    });

    public SAV8BS(byte[] data, bool exportable = true) : base(data, exportable)
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

        Zukan = new Zukan8b(this, 0x7A328); // size: 0x30B8
        BattleTrainer = new BattleTrainerStatus8b(this, 0x7D3E0); // size: 0x1618
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

    public SAV8BS() : this(new byte[SaveUtil.SIZE_G8BDSP_3], false) => SaveRevision = (int)Gem8Version.V1_3;

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
    public override PKM BlankPKM => new PB8();
    public override Type PKMType => typeof(PB8);

    public override int BoxCount => BoxLayout8b.BoxCount;
    public override int MaxEV => 252;

    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;
    public override IPersonalTable Personal => PersonalTable.BDSP;
    public override int OTLength => 12;
    public override int NickLength => 12;
    public override int MaxMoveID => Legal.MaxMoveID_8b;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_8b;
    public override int MaxItemID => Legal.MaxItemID_8b;
    public override int MaxBallID => Legal.MaxBallID_8b;
    public override int MaxGameID => Legal.MaxGameID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8b;

    public bool HasFirstSaveFileExpansion => (Gem8Version)SaveRevision >= Gem8Version.V1_1;
    public bool HasSecondSaveFileExpansion => (Gem8Version)SaveRevision >= Gem8Version.V1_2;

    public int SaveRevision
    {
        get => ReadInt32LittleEndian(Data.AsSpan(0));
        init => WriteInt32LittleEndian(Data.AsSpan(0), value);
    }

    public string SaveRevisionString => ((Gem8Version)SaveRevision).GetSuffixString();

    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_BS;
    protected override SaveFile CloneInternal() => new SAV8BS((byte[])(Data.Clone()));

    protected override byte[] GetFinalData()
    {
        BoxLayout.SaveBattleTeams();
        return base.GetFinalData();
    }

    private void ReloadBattleTeams()
    {
        if (!State.Exportable)
            BoxLayout.ClearBattleTeams();
        else // Valid slot locking info present
            BoxLayout.LoadBattleTeams();
    }

    public override StorageSlotSource GetSlotFlags(int index)
    {
        int team = Array.IndexOf(TeamSlots, index);
        if (team < 0)
            return StorageSlotSource.None;

        team /= 6;
        var result = (StorageSlotSource)((int)StorageSlotSource.BattleTeam1 << team);
        if (BoxLayout.GetIsTeamLocked(team))
            result |= StorageSlotSource.Locked;
        return result;
    }

    #region Checksums

    private const int HashOffset = SaveUtil.SIZE_G8BDSP - 0x10;
    private Span<byte> CurrentHash => Data.AsSpan(SaveUtil.SIZE_G8BDSP - 0x10, 0x10);

    private byte[] ComputeHash()
    {
        CurrentHash.Clear();
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
    public FlagWork8b FlagWork { get; }
    public MyItem8b Items { get; }
    public UndergroundItemList8b Underground { get; }
    public SaveItemShortcut8b SelectBoundItems { get; }
    public Party8b PartyInfo { get; }
    // public MyItem Items { get; }
    public BoxLayout8b BoxLayout { get; }
    public ConfigSave8b Config { get; }
    public MyStatus8b MyStatus { get; }
    public PlayTime8b Played { get; }
    public Contest8b Contest { get; }
    // public Misc8 Misc { get; }
    public Zukan8b Zukan { get; }
    public BattleTrainerStatus8b BattleTrainer { get; }
    public MenuSelect8b MenuSelection { get; }
    public FieldObjectSave8b FieldObjects { get; }
    public Record8b Records { get; }
    public EncounterSave8b Encounter { get; }
    public PlayerData8b Player { get; }
    public SealBallDecoData8b SealDeco { get; }
    public SealList8b SealList { get; }
    public RandomGroup8b Random { get; }
    public FieldGimmickSave8b FieldGimmick { get; }
    public BerryTreeGrowSave8b BerryTrees { get; }
    public PoffinSaveData8b Poffins { get; }
    public BattleTowerWork8b BattleTower { get; }
    public SystemData8b System { get; }
    public Poketch8b Poketch { get; }
    public Daycare8b Daycare { get; }
    public UgSaveData8b UgSaveData { get; }
    public UnionSaveData8b UnionSave { get; }
    public ContestPhotoLanguage8b ContestPhotoLanguage { get; }
    public ZukanSpinda8b ZukanExtra { get; }
    public UgCountRecord8b UgCount { get; }

    // First Savedata Expansion!
    public RecordAddData8b RecordAdd { get; }
    public MysteryBlock8b MysteryRecords { get; }
    #endregion

    public override GameVersion Version => Game switch
    {
        (int)GameVersion.BD => GameVersion.BD,
        (int)GameVersion.SP => GameVersion.SP,
        _ => GameVersion.Invalid,
    };

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter8.GetString(data);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter8.SetString(destBuffer, value, maxLength, option);
    }

    public int EventFlagCount => FlagWork8b.COUNT_FLAG;
    public bool GetEventFlag(int flagNumber) => FlagWork.GetFlag(flagNumber);
    public void SetEventFlag(int flagNumber, bool value) => FlagWork.SetFlag(flagNumber, value);

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
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;
    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = (byte)value; }
    public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = (byte)value; }

    public string Rival
    {
        get => GetString(0x55F4, 0x1A);
        set => SetString(Data.AsSpan(0x55F4, 0x1A), value.AsSpan(), OTLength, StringConverterOption.ClearZero);
    }

    public short ZoneID // map
    {
        get => ReadInt16LittleEndian(Data.AsSpan(0x5634));
        set => WriteInt16LittleEndian(Data.AsSpan(0x5634), value);
    }

    public float TimeScale // default 1440.0f
    {
        get => ReadSingleLittleEndian(Data.AsSpan(0x5638));
        set => WriteSingleLittleEndian(Data.AsSpan(0x5638), value);
    }

    public uint UnionRoomPenaltyTime // move this into the UnionSaveData block once reversed.
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0xCEA14));
        set => WriteSingleLittleEndian(Data.AsSpan(0xCEA14), value);
    }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pb8 = (PB8)pk;
        // Apply to this Save File
        DateTime Date = DateTime.Now;
        pb8.Trade(this, Date.Day, Date.Month, Date.Year);

        pb8.RefreshChecksum();
        AddCountAcquired(pb8);
    }

    private void AddCountAcquired(PKM pk)
    {
        // There aren't many records, and they only track Capture/Fish/Hatch/Defeat.
        Records.AddRecord(pk.WasEgg ? 004 : 002); // egg, capture
    }

    protected override void SetDex(PKM pk) => Zukan.SetDex(pk);
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
    public int GetRecordMax(int recordID) => Record8b.GetMax(recordID);
    public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);

    #region Daycare
    public override int DaycareSeedSize => 16; // 8byte
    public override int GetDaycareSlotOffset(int loc, int slot) => Daycare.GetParentSlotOffset(slot);
    public override uint? GetDaycareEXP(int loc, int slot) => 0;
    public override bool? IsDaycareOccupied(int loc, int slot) => Daycare.GetDaycareSlotOccupied(slot);
    public override bool? IsDaycareHasEgg(int loc) => Daycare.IsEggAvailable;
    public override void SetDaycareEXP(int loc, int slot, uint EXP) { }
    public override void SetDaycareOccupied(int loc, int slot, bool occupied) { }
    public override void SetDaycareHasEgg(int loc, bool hasEgg) => Daycare.IsEggAvailable = hasEgg;
    public override string GetDaycareRNGSeed(int loc) => $"{Daycare.DaycareSeed:X16}";
    public override void SetDaycareRNGSeed(int loc, string seed) => Daycare.DaycareSeed = Util.GetHexValue64(seed);
    #endregion

    public int EventWorkCount => FlagWork8b.COUNT_WORK;
    public int GetWork(int index) => FlagWork.GetWork(index);
    public void SetWork(int index, int value = default) => FlagWork.SetWork(index, value);
}
