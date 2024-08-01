using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 <see cref="SaveFile"/> object for <see cref="GameVersion.GG"/> games.
/// </summary>
public sealed class SAV7b : SAV_BEEF, ISaveBlock7b, IGameSync, IMysteryGiftStorageProvider
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {Blocks.Played.LastSavedTime}";
    public override string Extension => ".bin";
    public override IReadOnlyList<string> PKMExtensions => EntityFileExtension.Extensions7b;

    public override Type PKMType => typeof(PB7);
    public override PB7 BlankPKM => new();
    protected override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
    public override int SIZE_BOXSLOT => PokeCrypto.SIZE_6PARTY;
    public override byte[] GetDataForBox(PKM pk) => pk.EncryptedPartyData;
    public override PB7 GetBoxSlot(int offset) => GetDecryptedPKM(Data.AsSpan(offset, SIZE_PARTY).ToArray()); // party format in boxes!
    public override PB7 GetDecryptedPKM(byte[] data) => GetPKM(DecryptPKM(data));

    public override PersonalTable7GG Personal => PersonalTable.GG;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_GG;

    protected override SAV7b CloneInternal() => new((byte[])Data.Clone());

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

    public override bool HasPokeDex => true;

    private void Initialize()
    {
        Box = Blocks.BlockInfo[(int)BelugaBlockIndex.PokeListPokemon].Offset;
        Party = Blocks.BlockInfo[(int)BelugaBlockIndex.PokeListPokemon].Offset;
    }

    // Save Block accessors
    public MyItem7b Items => Blocks.Items;
    public Coordinates7b Coordinates => Blocks.Coordinates;
    public Misc7b Misc => Blocks.Misc;
    public Zukan7b Zukan => Blocks.Zukan;
    public MyStatus7b Status => Blocks.Status;
    public PlayTime7b Played => Blocks.Played;
    public ConfigSave7b Config => Blocks.Config;
    public EventWork7b EventWork => Blocks.EventWork;
    public PokeListHeader Storage => Blocks.Storage;
    public WB7Records GiftRecords => Blocks.GiftRecords;
    public Daycare7b Daycare => Blocks.Daycare;
    public CaptureRecords Captured => Blocks.Captured;
    public GoParkStorage Park => Blocks.Park;
    public PlayerGeoLocation7b PlayerGeoLocation => Blocks.PlayerGeoLocation;

    public override IReadOnlyList<InventoryPouch> Inventory { get => Blocks.Items.Inventory; set => Blocks.Items.Inventory = value; }

    // Feature Overrides
    public override byte Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;
    public override ushort MaxMoveID => Legal.MaxMoveID_7b;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_7b;
    public override int MaxItemID => Legal.MaxItemID_7b;
    public override int MaxBallID => Legal.MaxBallID_7b;
    public override GameVersion MaxGameID => Legal.MaxGameID_7b;
    public override int MaxAbilityID => Legal.MaxAbilityID_7b;

    public override int MaxIV => 31;
    public override int MaxEV => EffortValues.Max252;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;

    public override bool HasParty => false; // handled via team slots

    // BoxSlotCount => 1000 -- why couldn't this be a multiple of 30...
    public override int BoxSlotCount => 25;
    public override int BoxCount => 40; // 1000/25

    public bool FixPreWrite() => Blocks.Storage.CompressStorage();

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pb7 = (PB7)pk;
        // Apply to this Save File
        pb7.UpdateHandler(this);
        pb7.RefreshChecksum();
    }

    protected override void SetDex(PKM pk) => Blocks.Zukan.SetDex(pk);
    public override bool GetCaught(ushort species) => Blocks.Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Blocks.Zukan.GetSeen(species);

    protected override PB7 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);
    public override int GetBoxOffset(int box) => Box + (box * BoxSlotCount * SIZE_BOXSLOT);
    protected override IList<int>[] SlotPointers => [ Blocks.Storage.PokeListInfo ];

    public override int GetPartyOffset(int slot) => Blocks.Storage.GetPartyOffset(slot);
    public override int PartyCount { get => Blocks.Storage.PartyCount; protected set => Blocks.Storage.PartyCount = value; }
    protected override void SetPartyValues(PKM pk, bool isParty) => base.SetPartyValues(pk, true);

    public override StorageSlotSource GetBoxSlotFlags(int index)
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

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength);

    public override bool IsVersionValid() => Version is GameVersion.GP or GameVersion.GE;

    // Player Information
    public override uint ID32 { get => Blocks.Status.ID32; set => Blocks.Status.ID32 = value; }
    public override ushort TID16 { get => Blocks.Status.TID16; set => Blocks.Status.TID16 = value; }
    public override ushort SID16 { get => Blocks.Status.SID16; set => Blocks.Status.SID16 = value; }
    public override GameVersion Version { get => (GameVersion)Blocks.Status.Game; set => Blocks.Status.Game = (byte)value; }
    public override byte Gender { get => Blocks.Status.Gender; set => Blocks.Status.Gender = value; }
    public override int Language { get => Blocks.Status.Language; set => Blocks.Config.Language = Blocks.Status.Language = value; } // stored in multiple places
    public override string OT { get => Blocks.Status.OT; set => Blocks.Status.OT = value; }
    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }

    public override int PlayedHours { get => Blocks.Played.PlayedHours; set => Blocks.Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Blocks.Played.PlayedMinutes; set => Blocks.Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Blocks.Played.PlayedSeconds; set => Blocks.Played.PlayedSeconds = value; }

    public int GameSyncIDSize => MyStatus7b.GameSyncIDSize; // 64 bits
    public string GameSyncID { get => Blocks.Status.GameSyncID; set => Blocks.Status.GameSyncID = value; }
    IMysteryGiftStorage IMysteryGiftStorageProvider.MysteryGiftStorage => Blocks.GiftRecords;
}
