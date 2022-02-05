using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.PLA"/> games.
/// </summary>
public sealed class SAV8LA : SaveFile, ISaveBlock8LA, ISCBlockArray
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {LastSaved.LastSavedTime}";
    public override string Extension => string.Empty;

    public SAV8LA(byte[] data) : base(data)
    {
        Data = Array.Empty<byte>();
        AllBlocks = SwishCrypto.Decrypt(data);
        Blocks = new SaveBlockAccessor8LA(this);
        Initialize();
    }

    private SAV8LA(byte[] data, IReadOnlyList<SCBlock> blocks) : base(data)
    {
        Data = Array.Empty<byte>();
        AllBlocks = blocks;
        Blocks = new SaveBlockAccessor8LA(this);
        Initialize();
    }

    public SAV8LA()
    {
        AllBlocks = Meta8.GetBlankDataLA();
        Blocks = new SaveBlockAccessor8LA(this);
        Initialize();
        ClearBoxes();
    }

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter8.GetString(data);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option) => StringConverter8.SetString(destBuffer, value, maxLength, option);

    public override void CopyChangesFrom(SaveFile sav)
    {
        // Absorb changes from all blocks
        var z = (SAV8LA)sav;
        var mine = AllBlocks;
        var newB = z.AllBlocks;
        for (int i = 0; i < mine.Count; i++)
            newB[i].Data.CopyTo(mine[i].Data, 0);
        State.Edited = true;
    }

    protected override int SIZE_STORED => PokeCrypto.SIZE_8ASTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_8APARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_8ASTORED;
    protected override PKM GetPKM(byte[] data) => new PA8(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray8A(data);

    public override PKM BlankPKM => new PA8();
    public override Type PKMType => typeof(PA8);
    public override int MaxEV => 252;
    public override int Generation => 8;
    public override int OTLength => 12;
    public override int NickLength => 12;

    public SCBlockAccessor Accessor => Blocks;
    public IReadOnlyList<SCBlock> AllBlocks { get; }
    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    public override int BoxCount => BoxLayout8a.BoxCount; // 32
    public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
    public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
    public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
    public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
    public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
    public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }

    public override GameVersion Version => Game switch
    {
        (int)GameVersion.PLA => GameVersion.PLA,
        _ => GameVersion.Invalid,
    };

    protected override void SetChecksums() { } // None!
    protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

    public override PersonalTable Personal => PersonalTable.LA;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SWSH;

    #region Blocks
    public SaveBlockAccessor8LA Blocks { get; }

    public T GetValue<T>(uint key) where T : struct
    {
        if (!State.Exportable)
            return default;
        var value = Blocks.GetBlockValue(key);
        if (value is T v)
            return v;
        throw new ArgumentException($"Incorrect type request! Expected {typeof(T).Name}, received {value.GetType().Name}", nameof(T));
    }

    public void SetValue<T>(uint key, T value) where T : struct
    {
        if (!State.Exportable)
            return;
        Blocks.SetBlockValue(key, value);
    }

    #endregion
    protected override SaveFile CloneInternal()
    {
        var blockCopy = new SCBlock[AllBlocks.Count];
        for (int i = 0; i < AllBlocks.Count; i++)
            blockCopy[i] = AllBlocks[i].Clone();
        return new SAV8LA(State.BAK, blockCopy);
    }

    public override int MaxMoveID => Legal.MaxMoveID_8a;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override int MaxGameID => Legal.MaxGameID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;

    public Box8 BoxInfo => Blocks.BoxInfo;
    public Party8a PartyInfo => Blocks.PartyInfo;
    public MyStatus8a MyStatus => Blocks.MyStatus;
    public PokedexSave8a PokedexSave => Blocks.PokedexSave;
    public BoxLayout8a BoxLayout => Blocks.BoxLayout;
    public MyItem8a Items => Blocks.Items;
    public AdventureStart8a AdventureStart => Blocks.AdventureStart;
    public LastSaved8a LastSaved => Blocks.LastSaved;
    public PlayTime8a Played => Blocks.Played;
    public override uint SecondsToStart { get => (uint)AdventureStart.Seconds; set => AdventureStart.Seconds = value; }
    public override uint Money { get => (uint)Blocks.GetBlockValue(SaveBlockAccessor8LA.KMoney); set => Blocks.SetBlockValue(SaveBlockAccessor8LA.KMoney, value); }
    public override int MaxMoney => 9_999_999;

    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = (ushort)value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = (byte)value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = (byte)value; }

    protected override byte[] BoxBuffer => BoxInfo.Data;
    protected override byte[] PartyBuffer => PartyInfo.Data;

    private void Initialize()
    {
        Box = 0;
        Party = 0;
        PokeDex = 0;
    }

    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
    public override int PartyCount
    {
        get => PartyInfo.PartyCount;
        protected set => PartyInfo.PartyCount = value;
    }

    // Zukan
    protected override void SetDex(PKM pkm)
    {
        // TODO: Seen in wild?
        // Accessor.SetPokeSeenInWild(pkm);

        // TODO: Should this update research? What research should it be updating?
        // TODO: Should this be passing "caught=true" to set caught flags and not just obtain flags?
        // For now, if we have never obtained the poke, treat this pkm as obtained-via-trade.
        PokedexSave.OnPokeGet_TradeWithoutEvolution(pkm);
    }

    public override bool GetCaught(int species)
    {
        if (species > Personal.MaxSpeciesID)
            return false;

        var formCount = Personal[species].FormCount;
        for (var form = 0; form < formCount; form++)
        {
            if (PokedexSave.HasAnyPokeObtainFlags(species, form))
                return true;
        }
        return false;
    }

    public override bool GetSeen(int species) => PokedexSave.HasPokeEverBeenUpdated(species);

    // Inventory
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    #region Boxes
    public override bool HasBoxWallpapers => false;
    public override bool HasNamableBoxes => true;
    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
    public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor8LA.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor8LA.KBoxesUnlocked, (byte)value); }

    public override byte[] BoxFlags
    {
        get => new[]
        {
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox01).Type - 1),
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox02).Type - 1),
            Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox03).Type - 1),
        };
        set
        {
            if (value.Length != 1)
                return;

            var blocks = new[]
            {
                Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox01),
                Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox02),
                Blocks.GetBlock(SaveBlockAccessor8LA.KUnlockedSecretBox03),
            };

            foreach (var block in blocks)
            {
                block.ChangeBooleanType((SCTypeCode)(value[0] & 1) + 1);
            }
        }
    }

    public override int GetBoxOffset(int box) => Box + (SIZE_BOXSLOT * box * 30);
    public override string GetBoxName(int box) => BoxLayout.GetBoxName(box);
    public override void SetBoxName(int box, string value) => BoxLayout.SetBoxName(box, value);

    public override int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return box;
        var b = Blocks.GetBlock(SaveBlockAccessor8LA.KBoxWallpapersUnused);
        return b.Data[box];
    }

    public override void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        var b = Blocks.GetBlock(SaveBlockAccessor8LA.KBoxWallpapersUnused);
        b.Data[box] = (byte)value;
    }
    #endregion
}
