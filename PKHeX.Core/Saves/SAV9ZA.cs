using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class SAV9ZA : SaveFile, ISCBlockArray, ISaveFileRevision, IBoxDetailName, IBoxDetailWallpaper
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {LastSaved.DisplayValue}";
    public override string Extension => string.Empty;

    public SAV9ZA(Memory<byte> data) : this(SwishCrypto.Decrypt(data.Span)) { }

    private SAV9ZA(IReadOnlyList<SCBlock> blocks) : base(Memory<byte>.Empty)
    {
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor9ZA(this);
        Initialize();
    }

    public SAV9ZA()
    {
        AllBlocks = BlankBlocks9a.GetBlankBlocks();
        Blocks = new SaveBlockAccessor9ZA(this);

        var revision = Blocks.GetBlock(SaveBlockAccessor9ZA.KSaveRevision);
        revision.ChangeStoredType(SCTypeCode.UInt64);
        revision.SetValue((ulong)BlankBlocks9a.BlankRevision);

        Initialize();
        ClearBoxes();
    }

    public override void CopyChangesFrom(SaveFile sav)
    {
        // Absorb changes from all blocks
        var z = (SAV9ZA)sav;
        var mine = AllBlocks;
        var newB = z.AllBlocks;
        for (int i = 0; i < mine.Count; i++)
            mine[i].CopyFrom(newB[i]);
        State.Edited = true;
    }


    public int SaveRevision => (int)GetValue<ulong>(SaveBlockAccessor9ZA.KSaveRevision);

    public string SaveRevisionString => SaveRevision switch
    {
        0 => "-Base", // Vanilla
        1 => "-MD", // Mega Dimension
        _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
    };

    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    protected override void SetChecksums() { } // None!
    protected override Memory<byte> GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable9ZA Personal => PersonalTable.ZA;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_ZA;

    #region Blocks
    public SCBlockAccessor Accessor => Blocks;
    public SaveBlockAccessor9ZA Blocks { get; }
    public IReadOnlyList<SCBlock> AllBlocks { get; }
    public T GetValue<T>(uint key) where T : struct => Blocks.GetBlockValueSafe<T>(key);
    public void SetValue<T>(uint key, T value) where T : struct => Blocks.SetBlockValueSafe(key, value);
    public Box8 BoxInfo => Blocks.BoxInfo;
    public Party9a PartyInfo => Blocks.PartyInfo;
    public MyItem9a Items => Blocks.Items;
    public MyStatus9a MyStatus => Blocks.MyStatus;
    public Zukan9a Zukan => Blocks.Zukan;
    public BoxLayout9a BoxLayout => Blocks.BoxLayout;
    public PlayTime9a Played => Blocks.Played;
    public TeamIndexes8 TeamIndexes => Blocks.TeamIndexes;
    public Epoch1900DateTimeValue LastSaved => Blocks.LastSaved;
    public Epoch1900DateTimeValue StartTime => Blocks.EnrollmentDate;
    public Coordinates9a Coordinates => Blocks.Coordinates;
    public InfiniteRoyale9a InfiniteRoyale => Blocks.InfiniteRoyale;
    public PlayerAppearance9a PlayerAppearance => Blocks.PlayerAppearance;
    public PlayerFashion9a PlayerFashion => Blocks.PlayerFashion;
    public ConfigSave9a Config => Blocks.Config;
    public DonutPocket9a Donuts => Blocks.Donuts;
    #endregion

    protected override SAV9ZA CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new(blockCopy);
    }

    private ushort m_spec, m_item, m_move, m_abil;
    public override int MaxBallID => Legal.MaxBallID_9a;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME2;
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
            m_move = Legal.MaxMoveID_9a_IK;
            m_spec = Legal.MaxSpeciesID_9a_IK;
            m_item = Legal.MaxItemID_9a_IK;
            m_abil = Legal.MaxAbilityID_9a_IK;
        }
        else
        {
            m_move = Legal.MaxMoveID_9a_MD;
            m_spec = Legal.MaxSpeciesID_9a_MD;
            m_item = Legal.MaxItemID_9a_MD;
            m_abil = Legal.MaxAbilityID_9a_MD;
        }
    }

    public override IReadOnlyList<string> PKMExtensions => EntityFileExtension.GetExtensionsHOME();

    // Configuration
    protected override int SIZE_STORED => PokeCrypto.SIZE_9STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_9PARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_9PARTY + GapBoxSlot;
    public override PA9 BlankPKM => new();
    public override Type PKMType => typeof(PA9);

    public override int BoxCount => BoxLayout9.BoxCount;
    public override int MaxEV => EffortValues.Max252;
    public override byte Generation => 9;
    public override EntityContext Context => EntityContext.Gen9a;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;
    protected override PA9 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray9(data);

    public override bool IsVersionValid() => Version is GameVersion.ZA;

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
    public override uint Money { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor9ZA.KMoney); set => Blocks.SetBlockValue(SaveBlockAccessor9ZA.KMoney, value); }

    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

    // Inventory
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    // Storage
    private const int GapBoxSlot = 0x40;
    private const int GapPartySlot = 0x88; // 0x40 + 0x48
    public override int GetPartyOffset(int slot) => Party + ((SIZE_PARTY + GapPartySlot) * slot);
    public override int GetBoxOffset(int box) => Box + (SIZE_BOXSLOT * box * 30);
    public string GetBoxName(int box) => BoxLayout[box];
    public void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);
    public override byte[] GetDataForBox(PKM pk) => [..pk.EncryptedPartyData, ..new byte[GapBoxSlot]];

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        PA9 pa9 = (PA9)pk;
        // Apply to this Save File
        pa9.UpdateHandler(this);

        if (FormArgumentUtil.IsFormArgumentTypeDatePair(pa9.Species, pa9.Form))
        {
            pa9.FormArgumentElapsed = pa9.FormArgumentMaximum = 0;
            pa9.FormArgumentRemain = (byte)GetFormArgument(pa9);
        }

        pa9.RefreshChecksum();
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
    public override PA9 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));
    public override PA9 GetBoxSlot(int offset) => GetDecryptedPKM(BoxInfo.Data.Slice(offset, SIZE_PARTY).ToArray()); // party format in boxes!

    public override StorageSlotSource GetBoxSlotFlags(int index)
    {
        int team = TeamIndexes.TeamSlots.IndexOf(index);
        if (team < 0)
            return StorageSlotSource.None;

        team /= 6;
        var result = (StorageSlotSource)((int)StorageSlotSource.BattleTeam1 << team);
        if (TeamIndexes.GetIsTeamLocked(team))
            result |= StorageSlotSource.Locked;
        return result;
    }

    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor9ZA.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor9ZA.KBoxesUnlocked, (byte)value); }

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor9ZA.KBoxWallpapers);
        return b.Data[box];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        var b = Blocks.GetBlock(SaveBlockAccessor9ZA.KBoxWallpapers);
        b.Data[box] = (byte)value;
    }

    public uint TicketPointsRoyale
    {
        get => Blocks.GetBlockValue<uint>(SaveBlockAccessor9ZA.KTicketPointsZARoyale);
        set => Blocks.SetBlockValue(SaveBlockAccessor9ZA.KTicketPointsZARoyale, value);
    }

    public uint TicketPointsRoyaleInfinite
    {
        get => Blocks.GetBlockValue<uint>(SaveBlockAccessor9ZA.KTicketPointsZARoyaleInfinite);
        set => Blocks.SetBlockValue(SaveBlockAccessor9ZA.KTicketPointsZARoyaleInfinite, value);
    }
}
