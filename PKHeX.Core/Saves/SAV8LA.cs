using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.PLA"/> games.
/// </summary>
public sealed class SAV8LA : SaveFile, ISaveBlock8LA, ISCBlockArray, ISaveFileRevision, IBoxDetailName, IBoxDetailWallpaper
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {LastSaved.DisplayValue}";
    public override string Extension => string.Empty;

    public SAV8LA(byte[] data) : this(SwishCrypto.Decrypt(data)) { }

    private SAV8LA(IReadOnlyList<SCBlock> blocks) : base([])
    {
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor8LA(this);
        SaveRevision = Blocks.DetectRevision();
        Initialize();
    }

    public SAV8LA()
    {
        AllBlocks = BlankBlocks8a.GetBlankBlocks();
        Blocks = new SaveBlockAccessor8LA(this);
        SaveRevision = Blocks.DetectRevision();
        Initialize();
        ClearBoxes();
    }

    public int SaveRevision { get; }
    public string SaveRevisionString => SaveRevision switch
    {
        0 => "-Base", // Vanilla
        1 => "-DB", // DLC 1: Daybreak
        _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
    };

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);

    public override void CopyChangesFrom(SaveFile sav)
    {
        // Absorb changes from all blocks
        var z = (SAV8LA)sav;
        var mine = AllBlocks;
        var newB = z.AllBlocks;
        for (int i = 0; i < mine.Count; i++)
            mine[i].CopyFrom(newB[i]);
        State.Edited = true;
    }

    protected override int SIZE_STORED => PokeCrypto.SIZE_8ASTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_8APARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8ASTORED;
    protected override PA8 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray8A(data);

    public override PA8 BlankPKM => new();
    public override Type PKMType => typeof(PA8);
    public override int MaxEV => EffortValues.Max252;
    public override byte Generation => 8;
    public override EntityContext Context => EntityContext.Gen8a;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;

    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    public override int BoxCount => BoxLayout8a.BoxCount; // 32
    public override uint ID32 { get => MyStatus.ID32; set => MyStatus.ID32 = value; }
    public override ushort TID16 { get => MyStatus.TID16; set => MyStatus.TID16 = value; }
    public override ushort SID16 { get => MyStatus.SID16; set => MyStatus.SID16 = value; }
    public override GameVersion Version { get => (GameVersion)MyStatus.Game; set => MyStatus.Game = (byte)value; }
    public override byte Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
    public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
    public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }

    public override bool IsVersionValid() => Version is GameVersion.PLA;

    protected override void SetChecksums() { } // None!
    protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable8LA Personal => PersonalTable.LA;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_LA;

    protected override SAV8LA CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new(blockCopy);
    }

    public override ushort MaxMoveID => Legal.MaxMoveID_8a;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;

    #region Blocks
    public SCBlockAccessor Accessor => Blocks;
    public SaveBlockAccessor8LA Blocks { get; }
    public IReadOnlyList<SCBlock> AllBlocks { get; }
    public T GetValue<T>(uint key) where T : struct => Blocks.GetBlockValueSafe<T>(key);
    public void SetValue<T>(uint key, T value) where T : struct => Blocks.SetBlockValueSafe(key, value);
    public Box8 BoxInfo => Blocks.BoxInfo;
    public Party8a PartyInfo => Blocks.PartyInfo;
    public MyStatus8a MyStatus => Blocks.MyStatus;
    public PokedexSave8a PokedexSave => Blocks.PokedexSave;
    public BoxLayout8a BoxLayout => Blocks.BoxLayout;
    public MyItem8a Items => Blocks.Items;
    public Epoch1970Value AdventureStart => Blocks.AdventureStart;
    public Coordinates8a Coordinates => Blocks.Coordinates;
    public Epoch1900DateTimeValue LastSaved => Blocks.LastSaved;
    public PlayTime8b Played => Blocks.Played;
    public AreaSpawnerSet8a AreaSpawners => new(Blocks.GetBlock(SaveBlockAccessor8LA.KSpawners));
    #endregion

    public override uint SecondsToStart { get => (uint)AdventureStart.Seconds; set => AdventureStart.Seconds = value; }
    public override uint Money { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor8LA.KMoney); set => Blocks.SetBlockValue(SaveBlockAccessor8LA.KMoney, value); }
    public override int MaxMoney => 9_999_999;

    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

    protected override Span<byte> BoxBuffer => BoxInfo.Data;
    protected override Span<byte> PartyBuffer => PartyInfo.Data;

    public override bool HasPokeDex => true;
    private void Initialize()
    {
        Box = 0;
        Party = 0;
    }

    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    public override int PartyCount
    {
        get => PartyInfo.PartyCount;
        protected set => PartyInfo.PartyCount = value;
    }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pa8 = (PA8)pk;
        // Apply to this Save File
        pa8.UpdateHandler(this);
        pa8.RefreshChecksum();
    }

    // Zukan
    protected override void SetDex(PKM pk)
    {
        // TODO: Seen in wild?
        // Accessor.SetPokeSeenInWild(pk);

        // TODO: Should this update research? What research should it be updating?
        // TODO: Should this be passing "caught=true" to set caught flags and not just obtain flags?
        // For now, if we have never obtained the poke, treat this pk as obtained-via-trade.
        PokedexSave.OnPokeGet_TradeWithoutEvolution(pk);
    }

    public override bool GetCaught(ushort species)
    {
        if (species > Personal.MaxSpeciesID)
            return false;

        var formCount = Personal[species].FormCount;
        for (byte form = 0; form < formCount; form++)
        {
            if (PokedexSave.HasAnyPokeObtainFlags(species, form))
                return true;
        }
        return false;
    }

    public override bool GetSeen(ushort species) => PokedexSave.HasPokeEverBeenUpdated(species);

    // Inventory
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    #region Boxes
    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor8LA.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor8LA.KBoxesUnlocked, (byte)value); }

    public override byte[] BoxFlags
    {
        get =>
        [
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox01).Type - 1),
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox02).Type - 1),
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox03).Type - 1),
        ];
        set
        {
            if (value.Length != 3)
                return;

            Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox01).ChangeBooleanType((SCTypeCode)(value[0] & 1) + 1);
            Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox02).ChangeBooleanType((SCTypeCode)(value[1] & 1) + 1);
            Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox03).ChangeBooleanType((SCTypeCode)(value[2] & 1) + 1);
        }
    }

    public override int GetBoxOffset(int box) => Box + (SIZE_BOXSLOT * box * 30);
    public string GetBoxName(int box) => BoxLayout[box];
    public void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor8LA.KBoxWallpapersUnused);
        return b.Data[box];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        var b = Blocks.GetBlock(SaveBlockAccessor8LA.KBoxWallpapersUnused);
        b.Data[box] = (byte)value;
    }
    #endregion
}
