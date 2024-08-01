using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 <see cref="SaveFile"/> object for <see cref="GameVersion.SV"/> games.
/// </summary>
public sealed class SAV9SV : SaveFile, ISaveBlock9Main, ISCBlockArray, ISaveFileRevision, IBoxDetailName, IBoxDetailWallpaper
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {LastSaved.DisplayValue}";
    public override string Extension => string.Empty;

    public SAV9SV(byte[] data) : this(SwishCrypto.Decrypt(data)) { }

    private SAV9SV(IReadOnlyList<SCBlock> blocks) : base([])
    {
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor9SV(this);
        SaveRevision = Blocks.HasBlock(SaveBlockAccessor9SV.KBlueberryPoints) ? 2 : RaidKitakami.Data.Length != 0 ? 1 : 0;
        Initialize();
    }

    public SAV9SV()
    {
        AllBlocks = BlankBlocks9.GetBlankBlocks();
        Blocks = new SaveBlockAccessor9SV(this);
        SaveRevision = BlankBlocks9.BlankRevision;
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

    public int SaveRevision { get; }

    public string SaveRevisionString => SaveRevision switch
    {
        0 => "-Base", // Vanilla
        1 => "-TM", // Teal Mask
        2 => "-ID", // Indigo Disk
        _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
    };

    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    protected override void SetChecksums() { } // None!
    protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable9SV Personal => PersonalTable.SV;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_SV;

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
    public TeamIndexes8 TeamIndexes => Blocks.TeamIndexes;
    public Epoch1900DateTimeValue LastSaved => Blocks.LastSaved;
    public Epoch1970Value LastDateCycle => Blocks.LastDateCycle;
    public PlayerFashion9 PlayerFashion => Blocks.PlayerFashion;
    public PlayerAppearance9 PlayerAppearance => Blocks.PlayerAppearance;
    public RaidSpawnList9 RaidPaldea => Blocks.RaidPaldea;
    public RaidSpawnList9 RaidKitakami => Blocks.RaidKitakami;
    public RaidSpawnList9 RaidBlueberry => Blocks.RaidBlueberry;
    public RaidSevenStar9 RaidSevenStar => Blocks.RaidSevenStar;
    public Epoch1900DateValue EnrollmentDate => Blocks.EnrollmentDate;
    public BlueberryQuestRecord9 BlueberryQuestRecord => Blocks.BlueberryQuestRecord;
    public BlueberryClubRoom9 BlueberryClubRoom => Blocks.BlueberryClubRoom;
    #endregion

    protected override SAV9SV CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new(blockCopy);
    }

    private ushort m_spec, m_item, m_move, m_abil;
    public override int MaxBallID => Legal.MaxBallID_9;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME;
    public override ushort MaxMoveID => m_move;
    public override ushort MaxSpeciesID => m_spec;
    public override int MaxItemID => m_item;
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
            m_move = Legal.MaxMoveID_9_T0;
            m_spec = Legal.MaxSpeciesID_9_T0;
            m_item = Legal.MaxItemID_9_T0;
            m_abil = Legal.MaxAbilityID_9_T0;
        }
        else if (rev == 1)
        {
            m_move = Legal.MaxMoveID_9_T1;
            m_spec = Legal.MaxSpeciesID_9_T1;
            m_item = Legal.MaxItemID_9_T1;
            m_abil = Legal.MaxAbilityID_9_T1;
        }
        else if (rev == 2)
        {
            m_move = Legal.MaxMoveID_9_T2;
            m_spec = Legal.MaxSpeciesID_9_T2;
            m_item = Legal.MaxItemID_9_T2;
            m_abil = Legal.MaxAbilityID_9_T2;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(SaveRevision));
        }
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
    public override int MaxEV => EffortValues.Max252;
    public override byte Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;
    protected override PK9 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray9(data);

    public override bool IsVersionValid() => Version is GameVersion.SL or GameVersion.VL;

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
    public override uint Money { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9SV.KMoney); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KMoney, value); }
    public uint LeaguePoints { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9SV.KLeaguePoints); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KLeaguePoints, value); }
    public uint BlueberryPoints { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9SV.KBlueberryPoints); set => Blocks.SetBlockValueSafe(SaveBlockAccessor9SV.KBlueberryPoints, value); }

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
        PK9 pk9 = (PK9)pk;
        // Apply to this Save File
        pk9.UpdateHandler(this);

        if (FormArgumentUtil.IsFormArgumentTypeDatePair(pk9.Species, pk9.Form))
        {
            pk9.FormArgumentElapsed = pk9.FormArgumentMaximum = 0;
            pk9.FormArgumentRemain = (byte)GetFormArgument(pk9);
        }

        pk9.RefreshChecksum();
        if (SetUpdateRecords != PKMImportSetting.Skip)
            AddCountAcquired(pk9);
    }

    private static uint GetFormArgument(PKM pk)
    {
        if (pk.Form == 0)
            return 0;
        return pk.Species switch
        {
            (int)Species.Furfrou => 5u, // Furfrou
            // Hoopa no longer sets Form Argument for Unbound form. Let it set 0.
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

    protected override Span<byte> BoxBuffer => BoxInfo.Data;
    protected override Span<byte> PartyBuffer => PartyInfo.Data;
    public override PK9 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public override PK9 GetBoxSlot(int offset) => GetDecryptedPKM(BoxInfo.Data.Slice(offset, SIZE_PARTY).ToArray()); // party format in boxes!

    //public int GetRecord(int recordID) => Records.GetRecord(recordID);
    //public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
    //public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
    //public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
    //public int RecordCount => Record9.RecordCount;

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
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor9SV.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor9SV.KBoxesUnlocked, (byte)value); }
    public Span<byte> Coordinates => Blocks.GetBlock(SaveBlockAccessor9SV.KCoordinates).Data;
    public float X { get => ReadSingleLittleEndian(Coordinates); set => WriteSingleLittleEndian(Coordinates, value); }
    public float Y { get => ReadSingleLittleEndian(Coordinates[4..]); set => WriteSingleLittleEndian(Coordinates[4..], value); }
    public float Z { get => ReadSingleLittleEndian(Coordinates[8..]); set => WriteSingleLittleEndian(Coordinates[8..], value); }
    public Span<byte> PlayerRotation => Blocks.GetBlock(SaveBlockAccessor9SV.KPlayerRotation).Data;
    public float RX { get => ReadSingleLittleEndian(PlayerRotation); set => WriteSingleLittleEndian(PlayerRotation, value); }
    public float RY { get => ReadSingleLittleEndian(PlayerRotation[4..]); set => WriteSingleLittleEndian(PlayerRotation[4..], value); }
    public float RZ { get => ReadSingleLittleEndian(PlayerRotation[8..]); set => WriteSingleLittleEndian(PlayerRotation[8..], value); }
    public float RW { get => ReadSingleLittleEndian(PlayerRotation[12..]); set => WriteSingleLittleEndian(PlayerRotation[12..], value); }

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

    public void SetPlayerRotation(float rx, float ry, float rz, float rw)
    {
        // Only set coordinates if epsilon is different enough
        const float epsilon = 0.0001f;
        if (Math.Abs(RX - rx) < epsilon && Math.Abs(RY - ry) < epsilon && Math.Abs(RZ - rz) < epsilon && Math.Abs(RW - rw) < epsilon)
            return;
        RX = rx;
        RY = ry;
        RZ = rz;
        RW = rw;
    }

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor9SV.KBoxWallpapers);
        return b.Data[box];
    }

    public void SetBoxWallpaper(int box, int value)
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

    public ThrowStyle9 ThrowStyle {
        get {
            if(Blocks.TryGetBlock(SaveBlockAccessor9SV.KThrowStyle, out var throwStyleBlock))
                return (ThrowStyle9)throwStyleBlock.Data[0];
            return ThrowStyle9.OriginalStyle;
        }
        set
        {
            if (Blocks.TryGetBlock(SaveBlockAccessor9SV.KThrowStyle, out var throwStyleBlock))
                throwStyleBlock.ChangeData([(byte)value]);
        }
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

        string[] blocks =
        [
            "WEVT_SUB_014_EVENT_STATE_UTHUWA",
            "WEVT_SUB_015_EVENT_STATE_TSURUGI",
            "WEVT_SUB_016_EVENT_STATE_MOKKAN",
            "WEVT_SUB_017_EVENT_STATE_MAGATAMA",
        ];

        foreach (var block in blocks)
            Accessor.GetBlock(block).SetValue(1); // lift seals from each shrine

        // remove chains from each shrine
        Accessor.GetBlock(SaveBlockAccessor9SV.KStakesRemovedTingLu).SetValue((ulong)10);
        Accessor.GetBlock(SaveBlockAccessor9SV.KStakesRemovedChienPao).SetValue((ulong)10);
        Accessor.GetBlock(SaveBlockAccessor9SV.KStakesRemovedWoChien).SetValue((ulong)10);
        Accessor.GetBlock(SaveBlockAccessor9SV.KStakesRemovedChiYu).SetValue((ulong)10);
    }

    public void UnlockAllTMRecipes()
    {
        for (int i = 1; i <= 229; i++)
        {
            var flag = $"FSYS_UI_WAZA_MACHINE_RELEASE_{i:000}";
            var hash = (uint)FnvHash.HashFnv1a_64(flag);
            if (Accessor.TryGetBlock(hash, out var block))
                block.ChangeBooleanType(SCTypeCode.Bool2);
        }
    }

    public void ActivateSnacksworthLegendaries()
    {
        for (int i = 13; i <= 37; i++)
        {
            var flag = $"WEVT_S2_SUB_{i:000}_STATE";
            var hash = (uint)FnvHash.HashFnv1a_64(flag);
            if (Accessor.TryGetBlock(hash, out var block))
                block.SetValue(1); // appeared, not captured
        }
    }

    public void UnlockAllCoaches()
    {
        string[] blocks =
        [
            "FSYS_CLUB_HUD_COACH_BOTAN",
            "FSYS_CLUB_HUD_COACH_CHAMP_HAGANE",
            "FSYS_CLUB_HUD_COACH_CHAMP_JIMEN",
            "FSYS_CLUB_HUD_COACH_CHAMP_TOP",
            "FSYS_CLUB_HUD_COACH_FRIEND",
            "FSYS_CLUB_HUD_COACH_RIVAL",
            "FSYS_CLUB_HUD_COACH_TEACHER_ART",
            "FSYS_CLUB_HUD_COACH_TEACHER_ATHLETIC",
            "FSYS_CLUB_HUD_COACH_TEACHER_BIOLOGY",
            "FSYS_CLUB_HUD_COACH_TEACHER_HEAD",
            "FSYS_CLUB_HUD_COACH_TEACHER_HEALTH",
            "FSYS_CLUB_HUD_COACH_TEACHER_HISTORY",
            "FSYS_CLUB_HUD_COACH_TEACHER_HOME",
            "FSYS_CLUB_HUD_COACH_TEACHER_LANGUAGE",
            "FSYS_CLUB_HUD_COACH_TEACHER_MATH",
        ];

        // extra safety
        foreach (var block in blocks)
        {
            var hash = (uint)FnvHash.HashFnv1a_64(block);
            if (Accessor.TryGetBlock(hash, out var flag))
                flag.ChangeBooleanType(SCTypeCode.Bool2);
        }
    }

    public void UnlockAllThrowStyles()
    {
        // Unlock Styles
        for (int i = 1; i <= 3; i++)
        {
            var flag = $"FSYS_CLUB_ROOM_BALL_THROW_FORM_0{i}";
            var hash = (uint)FnvHash.HashFnv1a_64(flag);
            if (Accessor.TryGetBlock(hash, out var block))
                block.ChangeBooleanType(SCTypeCode.Bool2);
        }

        // Update Support Board
        var board = BlueberryClubRoom.SupportBoard;
        board.BaseballClub1SmugElegantPurchased = true;
        board.BaseballClub1SmugElegantUnread = false;
        board.BaseballClub2TwirlingNinjaPurchased = true;
        board.BaseballClub2TwirlingNinjaUnread = false;
        board.BaseballClub3ChampionPurchased = true;
        board.BaseballClub3ChampionUnread = false;
    }
}
