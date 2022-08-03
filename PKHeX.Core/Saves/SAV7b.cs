using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 <see cref="SaveFile"/> object for <see cref="GameVersion.GG"/> games.
/// </summary>
public sealed class SAV7b : SAV_BEEF, ISaveBlock7b, IGameSync, IEventFlagArray
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {Blocks.Played.LastSavedTime}";
    public override string Extension => ".bin";
    public override IReadOnlyList<string> PKMExtensions => EntityFileExtension.Extensions7b;

    public override Type PKMType => typeof(PB7);
    public override PKM BlankPKM => new PB7();
    protected override int SIZE_STORED => PokeCrypto.SIZE_6PARTY;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_6PARTY;
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;

    public override IPersonalTable Personal => PersonalTable.GG;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_GG;

    protected override SaveFile CloneInternal() => new SAV7b((byte[])Data.Clone());

    public SaveBlockAccessor7b Blocks { get; }
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;

    public SAV7b() : base(SaveUtil.SIZE_G7GG, 0xB8800)
    {
        Blocks = new SaveBlockAccessor7b(this);
        Initialize();
        ClearBoxes();
    }

    public SAV7b(byte[] data) : base(data, 0xB8800)
    {
        Blocks = new SaveBlockAccessor7b(this);
        Initialize();
    }

    private void Initialize()
    {
        Box = Blocks.GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
        Party = Blocks.GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
        PokeDex = Blocks.GetBlockOffset(BelugaBlockIndex.Zukan);

        WondercardData = Blocks.GiftRecords.Offset;
    }

    // Save Block accessors
    public MyItem Items => Blocks.Items;
    public Misc7b Misc => Blocks.Misc;
    public Zukan7b Zukan => Blocks.Zukan;
    public MyStatus7b Status => Blocks.Status;
    public PlayTime7b Played => Blocks.Played;
    public ConfigSave7b Config => Blocks.Config;
    public EventWork7b EventWork => Blocks.EventWork;
    public PokeListHeader Storage => Blocks.Storage;
    public WB7Records GiftRecords => Blocks.GiftRecords;
    public CaptureRecords Captured => Blocks.Captured;

    public override IReadOnlyList<InventoryPouch> Inventory { get => Blocks.Items.Inventory; set => Blocks.Items.Inventory = value; }

    // Feature Overrides
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;
    public override int MaxMoveID => Legal.MaxMoveID_7b;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_7b;
    public override int MaxItemID => Legal.MaxItemID_7b;
    public override int MaxBallID => Legal.MaxBallID_7b;
    public override int MaxGameID => Legal.MaxGameID_7b;
    public override int MaxAbilityID => Legal.MaxAbilityID_7b;

    public override int MaxIV => 31;
    public override int MaxEV => 252;
    public override int OTLength => 12;
    public override int NickLength => 12;
    protected override int GiftCountMax => 48;
    protected override int GiftFlagMax => 0x100 * 8;
    public int EventFlagCount => 4160; // 0xDC0 (true max may be up to 0x7F less. 23A8 starts u64 hashvals)

    public override bool HasParty => false; // handled via team slots

    // BoxSlotCount => 1000 -- why couldn't this be a multiple of 30...
    public override int BoxSlotCount => 25;
    public override int BoxCount => 40; // 1000/25

    public bool FixPreWrite() => Blocks.Storage.CompressStorage();

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pb7 = (PB7)pk;
        // Apply to this Save File
        var Date = DateTime.Now;
        pb7.Trade(this, Date.Day, Date.Month, Date.Year);
        pb7.RefreshChecksum();
    }

    protected override void SetDex(PKM pk) => Blocks.Zukan.SetDex(pk);
    public override bool GetCaught(int species) => Blocks.Zukan.GetCaught(species);
    public override bool GetSeen(int species) => Blocks.Zukan.GetSeen(species);

    protected override PKM GetPKM(byte[] data) => new PB7(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);
    public override int GetBoxOffset(int box) => Box + (box * BoxSlotCount * SIZE_BOXSLOT);
    protected override IList<int>[] SlotPointers => new[] { Blocks.Storage.PokeListInfo };

    public override int GetPartyOffset(int slot) => Blocks.Storage.GetPartyOffset(slot);
    public override int PartyCount { get => Blocks.Storage.PartyCount; protected set => Blocks.Storage.PartyCount = value; }
    protected override void SetPartyValues(PKM pk, bool isParty) => base.SetPartyValues(pk, true);

    public override StorageSlotSource GetSlotFlags(int index)
    {
        var result = StorageSlotSource.None;
        var header = Blocks.Storage.PokeListInfo;
        int position = Array.IndexOf(header, index, 0, 6);
        if (position >= 0)
            result = (StorageSlotSource)((int)StorageSlotSource.Party1 << position);
        if (header[PokeListHeader.STARTER] == index)
            result |= StorageSlotSource.Starter;
        return result;
    }

    public override string GetBoxName(int box) => $"Box {box + 1}";
    public override void SetBoxName(int box, string value) { }

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter8.GetString(data);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter8.SetString(destBuffer, value, maxLength);
    }

    public override GameVersion Version => Game switch
    {
        (int)GameVersion.GP => GameVersion.GP,
        (int)GameVersion.GE => GameVersion.GE,
        _ => GameVersion.Invalid,
    };

    // Player Information
    public override int TID { get => Blocks.Status.TID; set => Blocks.Status.TID = value; }
    public override int SID { get => Blocks.Status.SID; set => Blocks.Status.SID = value; }
    public override int Game { get => Blocks.Status.Game; set => Blocks.Status.Game = value; }
    public override int Gender { get => Blocks.Status.Gender; set => Blocks.Status.Gender = value; }
    public override int Language { get => Blocks.Status.Language; set => Blocks.Config.Language = Blocks.Status.Language = value; } // stored in multiple places
    public override string OT { get => Blocks.Status.OT; set => Blocks.Status.OT = value; }
    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }

    public override int PlayedHours { get => Blocks.Played.PlayedHours; set => Blocks.Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Blocks.Played.PlayedMinutes; set => Blocks.Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Blocks.Played.PlayedSeconds; set => Blocks.Played.PlayedSeconds = value; }

    /// <summary>
    /// Gets the <see cref="bool"/> status of a desired Event Flag
    /// </summary>
    /// <param name="flagNumber">Event Flag to check</param>
    /// <returns>Flag is Set (true) or not Set (false)</returns>
    public bool GetEventFlag(int flagNumber) => Blocks.EventWork.GetFlag(flagNumber);

    /// <summary>
    /// Sets the <see cref="bool"/> status of a desired Event Flag
    /// </summary>
    /// <param name="flagNumber">Event Flag to check</param>
    /// <param name="value">Event Flag status to set</param>
    /// <remarks>Flag is Set (true) or not Set (false)</remarks>
    public void SetEventFlag(int flagNumber, bool value) => Blocks.EventWork.SetFlag(flagNumber, value);

    protected override bool[] MysteryGiftReceivedFlags { get => Blocks.GiftRecords.GetFlags(); set => Blocks.GiftRecords.SetFlags(value); }
    protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.GiftRecords.GetRecords(); set => Blocks.GiftRecords.SetRecords((WR7[])value); }

    public int GameSyncIDSize => MyStatus7b.GameSyncIDSize; // 64 bits
    public string GameSyncID { get => Blocks.Status.GameSyncID; set => Blocks.Status.GameSyncID = value; }
}
