using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.BDSP"/> games.
/// </summary>
public sealed class SAV8BS : SaveFile, ISaveFileRevision, ITrainerStatRecord, IEventWorkArray<int>, IBoxDetailName, IBoxDetailWallpaper, IDaycareStorage, IDaycareEggState, IDaycareRandomState<ulong>
{
    // Save Data Attributes
    protected internal override string ShortSummary => $"{OT} ({Version}) - {System.LastSavedTime}";
    public override string Extension => string.Empty;

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen <= 8;
    });

    public SAV8BS() : this(new byte[SaveUtil.SIZE_G8BDSP_3], false) => SaveRevision = (int)Gem8Version.V1_3;

    public SAV8BS(byte[] data, bool exportable = true) : base(data, exportable)
    {
        var Raw = Data.AsMemory();
        FlagWork = new FlagWork8b(this, Raw.Slice(0x00004, FlagWork8b.SIZE));
        Items = new MyItem8b(this, Raw.Slice(0x0563C, MyItem8b.SIZE));
        Underground = new UndergroundItemList8b(this, Raw.Slice(0x111BC, UndergroundItemList8b.SIZE));
        SelectBoundItems = new SaveItemShortcut8b(this, Raw.Slice(0x14090, 8));
        PartyInfo = new Party8b(this, Raw.Slice(0x14098, Party8b.SIZE));
        BoxLayout = new BoxLayout8b(this, Raw.Slice(0x148AA, 0x64A));
        // 0x14EF4 - Box[40]

        // PLAYER_DATA:
        Config = new ConfigSave8b(this, Raw.Slice(0x79B74, 0x40));
        MyStatus = new MyStatus8b(this, Raw.Slice(0x79BB4, 0x50));
        Played = new PlayTime8b(this, Raw.Slice(0x79C04, 0x04));
        Contest = new Contest8b(this, Raw.Slice(0x79C08, 0x720));

        Zukan = new Zukan8b(this, Raw.Slice(0x7A328, 0x30B8));
        BattleTrainer = new BattleTrainerStatus8b(this, Raw.Slice(0x7D3E0, 0x1618));
        MenuSelection = new MenuSelect8b(this, Raw.Slice(0x7E9F8, 0x44));
        FieldObjects = new FieldObjectSave8b(this, Raw.Slice(0x7EA3C, 0x109A0));
        Records = new Record8b(this, Raw.Slice(0x8F3DC, 0x78 * 12));
        Encounter = new EncounterSave8b(this, Raw.Slice(0x8F97C, 0x188));
        Player = new PlayerData8b(this, Raw.Slice(0x8FB04, PlayerData8b.SIZE));
        SealDeco = new SealBallDecoData8b(this, Raw.Slice(0x8FB84, SealBallDecoData8b.SIZE));
        SealList = new SealList8b(this, Raw.Slice(0x93E0C, 0x960)); // size: 0x960 SaveSealData[200]
        Random = new RandomGroup8b(this, Raw.Slice(0x9476C, 0x630)); // size: 0x630
        FieldGimmick = new FieldGimmickSave8b(this, Raw.Slice(0x94D9C, 0xC)); // FieldGimmickSaveData; int[3] gearRotate
        BerryTrees = new BerryTreeGrowSave8b(this, Raw.Slice(0x94DA8, 0x808)); // size: 0x808
        Poffins = new PoffinSaveData8b(this, Raw.Slice(0x955B0, 0x644)); // size: 0x644
        BattleTower = new BattleTowerWork8b(this, Raw.Slice(0x95BF4, 0x1B8)); // size: 0x1B8
        System = new SystemData8b(this, Raw.Slice(0x95DAC, SystemData8b.SIZE));
        Poketch = new Poketch8b(this, Raw.Slice(0x95EE4, Poketch8b.SIZE));
        Daycare = new Daycare8b(this, Raw.Slice(0x96080, Daycare8b.SIZE));
        // 0x96340 - _DENDOU_SAVEDATA; DENDOU_RECORD[30], POKEMON_DATA_INSIDE[6], ushort[4] ?
        // BadgeSaveData; byte[8]
        // BoukenNote; byte[24]
        // TV_DATA (int[48], TV_STR_DATA[42]), (int[37], bool[37])*2, (int[8], int[8]), TV_STR_DATA[10]; 144 128bit zeroed (900 bytes?)?
        UgSaveData = new UgSaveData8b(this, Raw.Slice(0x9A89C, 0x27A0));
        // 0x9D03C - GMS_DATA // size: 0x31304, (GMS_POINT_DATA[650], ushort, ushort, byte)?; substructure GMS_POINT_HISTORY_DATA[5]
        // 0xCE340 - PLAYER_NETWORK_DATA; bcatFlagArray byte[1300]
        UnionSave = new UnionSaveData8b(this, Raw.Slice(0xCEA10, 0xC));
        ContestPhotoLanguage = new ContestPhotoLanguage8b(this, Raw.Slice(0xCEA1C, 0x18));
        ZukanExtra = new ZukanSpinda8b(this, Raw.Slice(0xCEA34, 0x64));
        // CON_PHOTO_EXT_DATA[5]
        // GMS_POINT_HISTORY_EXT_DATA[3250]
        UgCount = new UgCountRecord8b(this, Raw.Slice(0xE8178, 0x20)); // size: 0x20
        // 0xE8198 - ReBuffnameData; RE_DENDOU_RECORD[30], RE_DENDOU_POKEMON_DATA_INSIDE[6] (0x20) = 0x1680
        // 0xE9818 -- 0x10 byte[] MD5 hash of all savedata;

        // v1.1 additions
        RecordAdd = new RecordAddData8b(this, GetSafe(Raw, 0xE9828, 0x3C0));
        MysteryRecords = new MysteryBlock8b(this, GetSafe(Raw, 0xE9BE8, MysteryBlock8b.MinSize)); // size: ???
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

    private static Memory<byte> GetSafe(Memory<byte> src, int ofs, int len)
    {
        if (ofs + len > src.Length)
            return new byte[len];
        return src.Slice(ofs, len);
    }

    public override bool HasPokeDex => true;

    private void Initialize()
    {
        Box = 0x14EF4;
        Party = 0;

        ReloadBattleTeams();
        TeamSlots = BoxLayout.TeamSlots;
    }

    // Configuration
    protected override int SIZE_STORED => PokeCrypto.SIZE_8STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8PARTY;
    public override PB8 BlankPKM => new();
    public override Type PKMType => typeof(PB8);

    public override int BoxCount => BoxLayout8b.BoxCount;
    public override int MaxEV => EffortValues.Max252;

    public override byte Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;
    public override PersonalTable8BDSP Personal => PersonalTable.BDSP;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;
    public override ushort MaxMoveID => Legal.MaxMoveID_8b;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8b;
    public override int MaxItemID => Legal.MaxItemID_8b;
    public override int MaxBallID => Legal.MaxBallID_8b;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME;
    public override int MaxAbilityID => Legal.MaxAbilityID_8b;

    public bool HasFirstSaveFileExpansion => (Gem8Version)SaveRevision >= Gem8Version.V1_1;
    public bool HasSecondSaveFileExpansion => (Gem8Version)SaveRevision >= Gem8Version.V1_2;

    public int SaveRevision
    {
        get => ReadInt32LittleEndian(Data.AsSpan(0));
        init => WriteInt32LittleEndian(Data.AsSpan(0), value);
    }

    public string SaveRevisionString => ((Gem8Version)SaveRevision).GetSuffixString();

    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_BS;
    protected override SAV8BS CloneInternal() => new((byte[])(Data.Clone()));

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

    public override StorageSlotSource GetBoxSlotFlags(int index)
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

    private const int HashLength = MD5.HashSizeInBytes;
    private const int HashOffset = SaveUtil.SIZE_G8BDSP - HashLength;
    private Span<byte> CurrentHash => Data.AsSpan(HashOffset, HashLength);

    // Checksum is stored in the middle of the save file, and is zeroed before computing.
    protected override void SetChecksums()
    {
        var current = CurrentHash;
        current.Clear();
        RuntimeCryptographyProvider.Md5.HashData(Data, current);
    }

    public override bool ChecksumsValid
    {
        get
        {
            // Cache existing checksum as computing will update it.
            var current = CurrentHash;
            Span<byte> exist = stackalloc byte[HashLength];
            current.CopyTo(exist);
            SetChecksums();
            var result = current.SequenceEqual(exist);
            if (!result)
                exist.CopyTo(current); // restore original bad checksum
            return result;
        }
    }

    public override string ChecksumInfo => !ChecksumsValid ? "MD5 Hash Invalid" : string.Empty;

    #endregion

    protected override PB8 GetPKM(byte[] data) => new(data);
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

    public override bool IsVersionValid() => Version is GameVersion.BD or GameVersion.SP;

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);

    // Player Information
    public override uint ID32 { get => MyStatus.ID32; set => MyStatus.ID32 = value; }
    public override ushort TID16 { get => MyStatus.TID16; set => MyStatus.TID16 = value; }
    public override ushort SID16 { get => MyStatus.SID16; set => MyStatus.SID16 = value; }
    public override GameVersion Version { get => MyStatus.Game; set => MyStatus.Game = value; }
    public override byte Gender { get => MyStatus.Male ? (byte)0 : (byte)1; set => MyStatus.Male = value == 0; }
    public override int Language { get => Config.Language; set => Config.Language = value; }
    public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
    public override uint Money { get => MyStatus.Money; set => MyStatus.Money = value; }

    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

    // Inventory
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    protected override Span<byte> PartyBuffer => PartyInfo.Data;
    public override int GetBoxOffset(int box) => Box + (SIZE_PARTY * box * 30);
    public int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
    public void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
    public string GetBoxName(int box) => BoxLayout[box];
    public void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;
    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = (byte)value; }
    public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = (byte)value; }

    public string Rival
    {
        get => GetString(Data.AsSpan(0x55F4, 0x1A));
        set => SetString(Data.AsSpan(0x55F4, 0x1A), value, MaxStringLengthTrainer, StringConverterOption.ClearZero);
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
        pb8.UpdateHandler(this);

        pb8.RefreshChecksum();
        if (SetUpdateRecords != PKMImportSetting.Skip)
            AddCountAcquired(pk);
    }

    private void AddCountAcquired(PKM pk)
    {
        // There aren't many records, and they only track Capture/Fish/Hatch/Defeat.
        Records.AddRecord(pk.WasEgg ? 004 : 002); // egg, capture
    }

    protected override void SetDex(PKM pk) => Zukan.SetDex(pk);
    public override bool GetCaught(ushort species) => Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Zukan.GetSeen(species);

    public override int PartyCount
    {
        get => PartyInfo.PartyCount;
        protected set => PartyInfo.PartyCount = value;
    }

    public override PB8 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public override PB8 GetBoxSlot(int offset) => GetDecryptedPKM(Data.AsSpan(offset, SIZE_PARTY).ToArray()); // party format in boxes!

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
    public int GetRecordOffset(int recordID) => Record8b.GetRecordOffset(recordID);
    public int GetRecordMax(int recordID) => Record8b.GetMax(recordID);
    public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);

    #region Daycare
    public int DaycareSlotCount => Daycare.DaycareSlotCount;
    public bool IsDaycareOccupied(int slot) => Daycare.IsDaycareOccupied(slot);
    public bool IsEggAvailable { get => Daycare.IsEggAvailable; set => Daycare.IsEggAvailable = value; }
    public void SetDaycareOccupied(int slot, bool occupied) { }
    public Memory<byte> GetDaycareSlot(int index) => Daycare.GetDaycareSlot(index);
    ulong IDaycareRandomState<ulong>.Seed { get => Daycare.Seed; set => Daycare.Seed = value; }
    #endregion

    public int EventWorkCount => FlagWork8b.COUNT_WORK;
    public int GetWork(int index) => FlagWork.GetWork(index);
    public void SetWork(int index, int value = default) => FlagWork.SetWork(index, value);
}
