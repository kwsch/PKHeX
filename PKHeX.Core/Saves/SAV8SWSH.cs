using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
/// </summary>
public sealed class SAV8SWSH : SaveFile, ISaveBlock8SWSH, ITrainerStatRecord, ISaveFileRevision, ISCBlockArray, IBoxDetailName, IBoxDetailWallpaper
{
    public SAV8SWSH(byte[] data) : this(SwishCrypto.Decrypt(data)) { }

    private SAV8SWSH(IReadOnlyList<SCBlock> blocks) : base([])
    {
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor8SWSH(this);
        SaveRevision = Zukan.GetRevision();
        Initialize();
    }

    public SAV8SWSH()
    {
        AllBlocks = BlankBlocks8.GetBlankBlocks();
        Blocks = new SaveBlockAccessor8SWSH(this);
        SaveRevision = Zukan.GetRevision();
        Initialize();
        ClearBoxes();
    }

    public override void CopyChangesFrom(SaveFile sav)
    {
        // Absorb changes from all blocks
        var z = (SAV8SWSH)sav;
        var mine = AllBlocks;
        var newB = z.AllBlocks;
        for (int i = 0; i < mine.Count; i++)
            mine[i].CopyFrom(newB[i]);
        State.Edited = true;
    }

    public int SaveRevision { get; }

    public string SaveRevisionString => SaveRevision switch
    {
        0 => "-Base", // Vanilla
        1 => "-IoA", // DLC 1: Isle of Armor
        2 => "-CT", // DLC 2: Crown Tundra
        _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
    };

    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    protected override void SetChecksums() { } // None!
    protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable8SWSH Personal => PersonalTable.SWSH;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_SWSH;

    #region Blocks
    public SCBlockAccessor Accessor => Blocks;
    public SaveBlockAccessor8SWSH Blocks { get; }
    public IReadOnlyList<SCBlock> AllBlocks { get; }
    public T GetValue<T>(uint key) where T : struct => Blocks.GetBlockValueSafe<T>(key);
    public void SetValue<T>(uint key, T value) where T : struct => Blocks.SetBlockValueSafe(key, value);
    public Box8 BoxInfo => Blocks.BoxInfo;
    public Party8 PartyInfo => Blocks.PartyInfo;
    public MyItem8 Items => Blocks.Items;
    public MyStatus8 MyStatus => Blocks.MyStatus;
    public Coordinates8 Coordinates => Blocks.Coordinates;
    public Misc8 Misc => Blocks.Misc;
    public Zukan8 Zukan => Blocks.Zukan;
    public BoxLayout8 BoxLayout => Blocks.BoxLayout;
    public PlayTime7b Played => Blocks.Played;
    public Fused8 Fused => Blocks.Fused;
    public Daycare8 Daycare => Blocks.Daycare;
    public Record8 Records => Blocks.Records;
    public TrainerCard8 TrainerCard => Blocks.TrainerCard;
    public FashionUnlock8 Fashion => Blocks.Fashion;
    public RaidSpawnList8 RaidGalar => Blocks.RaidGalar;
    public RaidSpawnList8 RaidArmor => Blocks.RaidArmor;
    public RaidSpawnList8 RaidCrown => Blocks.RaidCrown;
    public TitleScreen8 TitleScreen => Blocks.TitleScreen;
    public TeamIndexes8 TeamIndexes => Blocks.TeamIndexes;
    #endregion

    protected override SAV8SWSH CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new(blockCopy);
    }

    private ushort m_spec, m_item, m_move, m_abil;
    public override ushort MaxMoveID => m_move;
    public override ushort MaxSpeciesID => m_spec;
    public override int MaxItemID => m_item;
    public override int MaxBallID => Legal.MaxBallID_8;
    public override GameVersion MaxGameID => Legal.MaxGameID_8;
    public override int MaxAbilityID => m_abil;

    public override bool HasPokeDex => true;
    private void Initialize()
    {
        Box = 0;
        Party = 0;
        TeamIndexes.LoadBattleTeams();

        int rev = SaveRevision;
        if (rev == 0)
        {
            m_move = Legal.MaxMoveID_8_O0;
            m_spec = Legal.MaxSpeciesID_8_O0;
            m_item = Legal.MaxItemID_8_O0;
            m_abil = Legal.MaxAbilityID_8_O0;
        }
        else if (rev == 1)
        {
            m_move = Legal.MaxMoveID_8_R1;
            m_spec = Legal.MaxSpeciesID_8_R1;
            m_item = Legal.MaxItemID_8_R1;
            m_abil = Legal.MaxAbilityID_8_R1;
        }
        else
        {
            m_move = Legal.MaxMoveID_8_R2;
            m_spec = Legal.MaxSpeciesID_8_R2;
            m_item = Legal.MaxItemID_8_R2;
            m_abil = Legal.MaxAbilityID_8_R2;
        }
    }

    // Save Data Attributes
    protected internal override string ShortSummary => $"{OT} ({Version}) - {Played.LastSavedTime}";
    public override string Extension => string.Empty;

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen <= 8;
    });

    // Configuration
    protected override int SIZE_STORED => PokeCrypto.SIZE_8STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8PARTY;
    public override PK8 BlankPKM => new();
    public override Type PKMType => typeof(PK8);

    public override int BoxCount => BoxLayout8.BoxCount;
    public override int MaxEV => EffortValues.Max252;
    public override byte Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;
    protected override PK8 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray8(data);

    public override bool IsVersionValid() => Version is GameVersion.SW or GameVersion.SH;

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
    public override GameVersion Version { get => (GameVersion)MyStatus.Game; set => MyStatus.Game = (byte)value; }
    public override byte Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
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
    public string GetBoxName(int box) => BoxLayout[box];
    public void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        PK8 pk8 = (PK8)pk;
        // Apply to this Save File
        pk8.UpdateHandler(this);

        if (FormArgumentUtil.IsFormArgumentTypeDatePair(pk8.Species, pk8.Form))
        {
            pk8.FormArgumentElapsed = pk8.FormArgumentMaximum = 0;
            pk8.FormArgumentRemain = (byte)GetFormArgument(pk8);
        }

        pk8.RefreshChecksum();
        if (SetUpdateRecords != PKMImportSetting.Skip)
            AddCountAcquired(pk8);
    }

    private static uint GetFormArgument(PKM pk)
    {
        if (pk.Form == 0)
            return 0;
        return pk.Species switch
        {
            (int)Species.Furfrou => 5u, // Furfrou
            (int)Species.Hoopa => 3u, // Hoopa
            _ => 0u,
        };
    }

    private void AddCountAcquired(PKM pk)
    {
        if (pk.WasEgg)
        {
            Records.AddRecord(00);
        }
        else // capture, assume wild
        {
            Records.AddRecord(01); // wild capture
            Records.AddRecord(06); // total captured
            Records.AddRecord(16); // wild encountered
            Records.AddRecord(23); // total battled
        }
        if (pk.CurrentHandler == 1)
            Records.AddRecord(17, 2); // trade * 2 -- these games count 1 trade as 2 for some reason.
    }

    protected override void SetDex(PKM pk) => Zukan.SetDex(pk);
    public override bool GetCaught(ushort species) => Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Zukan.GetSeen(species);

    public override int PartyCount
    {
        get => PartyInfo.PartyCount;
        protected set => PartyInfo.PartyCount = value;
    }

    protected override Span<byte> BoxBuffer => BoxInfo.Data;
    protected override Span<byte> PartyBuffer => PartyInfo.Data;
    public override PK8 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public override PK8 GetBoxSlot(int offset) => GetDecryptedPKM(BoxInfo.Data.Slice(offset, SIZE_PARTY).ToArray()); // party format in boxes!

    public int GetRecord(int recordID) => Records.GetRecord(recordID);
    public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
    public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
    public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
    public int RecordCount => Record8.RecordCount;

    public override StorageSlotSource GetBoxSlotFlags(int index)
    {
        int team = Array.IndexOf(TeamIndexes.TeamSlots, index);
        if (team < 0)
            return StorageSlotSource.None;

        team /= 6;
        var result = (StorageSlotSource)((int)StorageSlotSource.BattleTeam1 << team);
        if (TeamIndexes.GetIsTeamLocked(team))
            result |= StorageSlotSource.Locked;
        return result;
    }

    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor8SWSH.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor8SWSH.KBoxesUnlocked, (byte)value); }

    public override byte[] BoxFlags
    {
        get =>
        [
            (byte)(Blocks.GetBlock(SaveBlockAccessor8SWSH.KSecretBoxUnlocked).Type - 1),
        ];
        set
        {
            if (value.Length != 1)
                return;
            var block = Blocks.GetBlock(SaveBlockAccessor8SWSH.KSecretBoxUnlocked);
            block.ChangeBooleanType((SCTypeCode)(value[0] & 1) + 1);
        }
    }

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor8SWSH.KBoxWallpapers);
        return b.Data[box];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        var b = Blocks.GetBlock(SaveBlockAccessor8SWSH.KBoxWallpapers);
        b.Data[box] = (byte)value;
    }

    public void UnlockAllDiglett()
    {
        if (SaveRevision == 0)
            return; // no blocks

        // Zone specific values
        (int Zone, int Max)[] zones =
        [
            (0201, 16), // Fields of Honor
            (0202, 18), // Soothing Wetlands
            (0203, 6), // Forest of Focus
            (0204, 7), // Challenge Beach
            (0205, 5), // Brawlers' Cave
            (0206, 6), // Challenge Road
            (0207, 5), // Courageous Cavern
            (0208, 5), // Loop Lagoon
            (0209, 13), // Training Lowlands
            (0210, 1), // Warm-Up Tunnel
            (0211, 8), // Potbottom Desert
            (0221, 9), // Workout Sea
            (0222, 5), // Stepping-Stone Sea
            (0223, 3), // Insular Sea
            (0224, 1), // Honeycalm Sea
            (0231, 9), // Honeycalm Island
        ];
        var s = Blocks;
        foreach (var (zone, max) in zones)
        {
            var baseName = $"z_wr{zone:0000}_F_DHIGUDA";
            s.GetBlock(baseName).ChangeBooleanType(SCTypeCode.Bool2);
            for (int i = 0; i <= max; i++)
            {
                var otherName = $"{baseName}_{i}";
                s.GetBlock(otherName).ChangeBooleanType(SCTypeCode.Bool2);
            }

            var countName = $"WK_EV_R1_DHIG_WR{zone:0000}";
            var value = max + 2;
            if (zone == 0223) // trio
                value += 2;
            s.GetBlock(countName).SetValue((uint)value);
        }

        // Atypical named values
        const string TRIO1 = "z_wr0223_F_TRIO";
        const string TRIO2 = "FE_R1_DHIGUDA_TRIO";
        s.GetBlock(TRIO1).ChangeBooleanType(SCTypeCode.Bool2);
        s.GetBlock(TRIO2).ChangeBooleanType(SCTypeCode.Bool2);

        // Overall named values
        const string unreported = "WK_EV_R1_DHIGUDA_ADD";
        const string totalCount = "WK_EV_R1_DHIGUDA_COUNT";
        const string progressCt = "WK_EV_R1_DHIGUDA_PROGRESS";
        s.GetBlock(unreported).SetValue((uint)150); // 150 unreported
        s.GetBlock(totalCount).SetValue((uint)1); // obtained count
        s.GetBlock(progressCt).SetValue((uint)1); // none obtained progress value
    }
}
