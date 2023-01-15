using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
/// </summary>
public sealed class SAV9SV : SaveFile, ISaveBlock9Main, ISCBlockArray, ISaveFileRevision
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {LastSaved.DisplayValue}";
    public override string Extension => string.Empty;

    public SAV9SV(byte[] data) : this(SwishCrypto.Decrypt(data)) { }

    private SAV9SV(IReadOnlyList<SCBlock> blocks) : base(Array.Empty<byte>())
    {
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor9SV(this);
        Initialize();
    }

    public SAV9SV()
    {
        AllBlocks = Meta9.GetBlankDataSV();
        Blocks = new SaveBlockAccessor9SV(this);
        Initialize();
        ClearBoxes();
    }

    public override void CopyChangesFrom(SaveFile sav)
    {
        // Absorb changes from all blocks
        var z = (SAV9SV)sav;
        var mine = AllBlocks;
        var newB = z.AllBlocks;
        for (int i = 0; i < mine.Count; i++)
            mine[i].CopyFrom(newB[i]);
        State.Edited = true;
    }

    public int SaveRevision => 0; // No DLC (yet?)

    public string SaveRevisionString => SaveRevision switch
    {
        0 => "-Base", // Vanilla
        _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
    };

    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    protected override void SetChecksums() { } // None!
    protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable9SV Personal => PersonalTable.SV;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SV;

    #region Blocks
    public SCBlockAccessor Accessor => Blocks;
    public SaveBlockAccessor9SV Blocks { get; }
    public IReadOnlyList<SCBlock> AllBlocks { get; }
    public T GetValue<T>(uint key) where T : struct => Blocks.GetBlockValueSafe<T>(key);
    public void SetValue<T>(uint key, T value) where T : struct => Blocks.SetBlockValueSafe(key, value);
    public Box8 BoxInfo => Blocks.BoxInfo;
    public Party9 PartyInfo => Blocks.PartyInfo;
    public MyItem9 Items => Blocks.Items;
    public MyStatus9 MyStatus => Blocks.MyStatus;
    public Zukan9 Zukan => Blocks.Zukan;
    public BoxLayout9 BoxLayout => Blocks.BoxLayout;
    public PlayTime9 Played => Blocks.Played;
    public ConfigSave9 Config => Blocks.Config;
    public TeamIndexes9 TeamIndexes => Blocks.TeamIndexes;
    public Epoch1970Value LastSaved => Blocks.LastSaved;
    public PlayerFashion9 PlayerFashion => Blocks.PlayerFashion;
    public PlayerAppearance9 PlayerAppearance => Blocks.PlayerAppearance;
    public RaidSpawnList9 Raid => Blocks.Raid;
    public RaidSevenStar9 RaidSevenStar => Blocks.RaidSevenStar;
    #endregion

    protected override SAV9SV CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new(blockCopy);
    }

    public override ushort MaxMoveID => Legal.MaxMoveID_9;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_9;
    public override int MaxItemID => Legal.MaxItemID_9;
    public override int MaxBallID => Legal.MaxBallID_9;
    public override int MaxGameID => Legal.MaxGameID_9;
    public override int MaxAbilityID => Legal.MaxAbilityID_9;

    private void Initialize()
    {
        Box = 0;
        Party = 0;
        PokeDex = 0;
        TeamIndexes.LoadBattleTeams();
    }

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen == 9;
    });

    // Configuration
    protected override int SIZE_STORED => PokeCrypto.SIZE_9STORED;
    protected override int SIZE_PARTY  => PokeCrypto.SIZE_9PARTY;
    public override int SIZE_BOXSLOT   => PokeCrypto.SIZE_9PARTY;
    public override PK9 BlankPKM => new();
    public override Type PKMType => typeof(PK9);

    public override int BoxCount => BoxLayout9.BoxCount;
    public override int MaxEV => 252;
    public override int Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public override int MaxStringLengthOT => 12;
    public override int MaxStringLengthNickname => 12;
    protected override PK9 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray9(data);

    public override GameVersion Version => Game switch
    {
        (int)GameVersion.SL => GameVersion.SL,
        (int)GameVersion.VL => GameVersion.VL,
        _ => GameVersion.Invalid,
    };

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter8.GetString(data);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);

    // Player Information
    public override uint ID32 { get => MyStatus.ID32; set => MyStatus.ID32 = value; }
    public override ushort TID16 { get => MyStatus.TID16; set => MyStatus.TID16 = value; }
    public override ushort SID16 { get => MyStatus.SID16; set => MyStatus.SID16 = value; }
    public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
    public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
    public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
    public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
    public override uint Money { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9SV.KMoney); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KMoney, value); }
    public uint LeaguePoints { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9SV.KLeaguePoints); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KLeaguePoints, value); }

    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

    // Inventory
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    public override int GetBoxOffset(int box) => Box + (SIZE_PARTY * box * 30);
    public override string GetBoxName(int box) => BoxLayout[box];
    public override void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        PK9 pk9 = (PK9)pk;
        // Apply to this Save File
        DateTime Date = DateTime.Now;
        pk9.Trade(this, Date.Day, Date.Month, Date.Year);

        if (FormArgumentUtil.IsFormArgumentTypeDatePair(pk9.Species, pk9.Form))
        {
            pk9.FormArgumentElapsed = pk9.FormArgumentMaximum = 0;
            pk9.FormArgumentRemain = (byte)GetFormArgument(pk9);
        }

        pk9.RefreshChecksum();
        AddCountAcquired(pk9);
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
            //Records.AddRecord(00);
        }
        else // capture, assume wild
        {
            //Records.AddRecord(01); // wild capture
            //Records.AddRecord(06); // total captured
            //Records.AddRecord(16); // wild encountered
            //Records.AddRecord(23); // total battled
        }
        if (pk.CurrentHandler == 1)
        {
            //Records.AddRecord(17, 2); // trade * 2 -- these games count 1 trade as 2 for some reason.
        }
    }

    protected override void SetDex(PKM pk) => Zukan.SetDex(pk);
    public override bool GetCaught(ushort species) => Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Zukan.GetSeen(species);

    public override int PartyCount
    {
        get => PartyInfo.PartyCount;
        protected set => PartyInfo.PartyCount = value;
    }

    protected override byte[] BoxBuffer => BoxInfo.Data;
    protected override byte[] PartyBuffer => PartyInfo.Data;
    public override PK9 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public override PK9 GetBoxSlot(int offset) => GetDecryptedPKM(GetData(BoxInfo.Data, offset, SIZE_PARTY)); // party format in boxes!

    //public int GetRecord(int recordID) => Records.GetRecord(recordID);
    //public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
    //public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
    //public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
    //public int RecordCount => Record9.RecordCount;

    public override StorageSlotSource GetSlotFlags(int index)
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
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor9SV.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KBoxesUnlocked, (byte)value); }
    public override bool HasBoxWallpapers => true;
    public override bool HasNamableBoxes => true;
    public Span<byte> Coordinates => Blocks.GetBlock(SaveBlockAccessor9SV.KCoordinates).Data;
    public float X { get => ReadSingleLittleEndian(Coordinates); set => WriteSingleLittleEndian(Coordinates, value); }
    public float Y { get => ReadSingleLittleEndian(Coordinates[4..]); set => WriteSingleLittleEndian(Coordinates[4..], value); }
    public float Z { get => ReadSingleLittleEndian(Coordinates[8..]); set => WriteSingleLittleEndian(Coordinates[8..], value); }

    public void SetCoordinates(float x, float y, float z)
    {
        // Only set coordinates if epsilon is different enough
        const float epsilon = 0.0001f;
        if (Math.Abs(X - x) < epsilon && Math.Abs(Y - y) < epsilon && Math.Abs(Z - z) < epsilon)
            return;
        X = x;
        Y = y;
        Z = z;
    }

    public override int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor9SV.KBoxWallpapers);
        return b.Data[box];
    }

    public override void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        var b = Blocks.GetBlock(SaveBlockAccessor9SV.KBoxWallpapers);
        b.Data[box] = (byte)value;
    }

    public byte BoxLegendWallpaperFlag
    {
        get => Blocks.GetBlock(SaveBlockAccessor9SV.KBoxWallpapers).Data[BoxLayout9.BoxCount];
        set => Blocks.GetBlock(SaveBlockAccessor9SV.KBoxWallpapers).Data[BoxLayout9.BoxCount] = value;
    }

    public void CollectAllStakes()
    {
        for (int i = 14; i <= 17; i++)
        {
            for (int f = 1; f <= 8; f++)
            {
                var flag = $"FEVT_SUB_{i:000}_KUI_{f:00}_RELEASE";
                var hash = (uint)FnvHash.HashFnv1a_64(flag);
                var block = Accessor.GetBlock(hash);
                block.ChangeBooleanType(SCTypeCode.Bool2);
            }
        }

        var blocks = new[]
        {
            "WEVT_SUB_014_EVENT_STATE_UTHUWA",
            "WEVT_SUB_015_EVENT_STATE_TSURUGI",
            "WEVT_SUB_016_EVENT_STATE_MOKKAN",
            "WEVT_SUB_017_EVENT_STATE_MAGATAMA",
        };

        foreach (var block in blocks)
            Accessor.GetBlock(block).SetValue(1); // lift seals from each shrine
    }

    public void UnlockAllTMRecipes()
    {
        for (int i = 1; i <= 171; i++)
        {
            var flag = $"FSYS_UI_WAZA_MACHINE_RELEASE_{i:000}";
            var hash = (uint)FnvHash.HashFnv1a_64(flag);
            var block = Accessor.GetBlock(hash);
            block.ChangeBooleanType(SCTypeCode.Bool2);
        }
    }
}
