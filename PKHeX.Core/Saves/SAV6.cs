using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 <see cref="SaveFile"/> object.
/// </summary>
public abstract class SAV6 : SAV_BEEF, ITrainerStatRecord, ISaveBlock6Core, IRegionOrigin, IGameSync, IEventFlag37
{
    // Save Data Attributes
    protected internal override string ShortSummary => $"{OT} ({Version}) - {Played.LastSavedTime}";
    public override string Extension => string.Empty;

    protected SAV6(byte[] data, [ConstantExpected] int biOffset) : base(data, biOffset) { }
    protected SAV6([ConstantExpected] int size, [ConstantExpected] int biOffset) : base(size, biOffset) { }

    // Configuration
    protected override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
    public override PK6 BlankPKM => new();
    public override Type PKMType => typeof(PK6);

    public override int BoxCount => 31;
    public override int MaxEV => EffortValues.Max252;
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    protected override int GiftCountMax => 24;
    protected override int GiftFlagMax => 0x100 * 8;
    public int EventFlagCount => 8 * 0x1A0;
    public int EventWorkCount => (EventFlag - EventWork) / sizeof(ushort);
    protected abstract int EventFlag { get; }
    protected abstract int EventWork { get; }
    public override int MaxStringLengthOT => 12;
    public override int MaxStringLengthNickname => 12;

    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_6;
    public override int MaxBallID => Legal.MaxBallID_6;
    public override int MaxGameID => Legal.MaxGameID_6; // OR

    protected override PK6 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);

    protected int WondercardFlags { get; set; } = int.MinValue;
    protected int JPEG { get; set; } = int.MinValue;
    public int PSS { get; protected set; } = int.MinValue;
    public int BerryField { get; protected set; } = int.MinValue;
    public int HoF { get; protected set; } = int.MinValue;
    protected int PCLayout { private get; set; } = int.MinValue;
    protected int BattleBoxOffset { get; set; } = int.MinValue;
    public int GetBattleBoxSlot(int slot) => BattleBoxOffset + (slot * SIZE_STORED);

    public virtual string JPEGTitle => string.Empty;
    public virtual byte[] GetJPEGData() => [];

    protected internal const int LongStringLength = 0x22; // bytes, not characters
    protected internal const int ShortStringLength = 0x1A; // bytes, not characters

    // Player Information
    public override uint ID32 { get => Status.ID32; set => Status.ID32 = value; }
    public override ushort TID16 { get => Status.TID16; set => Status.TID16 = value; }
    public override ushort SID16 { get => Status.SID16; set => Status.SID16 = value; }
    public override int Game { get => Status.Game; set => Status.Game = value; }
    public override int Gender { get => Status.Gender; set => Status.Gender = value; }
    public override int Language { get => Status.Language; set => Status.Language = value; }
    public override string OT { get => Status.OT; set => Status.OT = value; }
    public byte Region { get => Status.Region; set => Status.Region = value; }
    public byte Country { get => Status.Country; set => Status.Country = value; }
    public byte ConsoleRegion { get => Status.ConsoleRegion; set => Status.ConsoleRegion = value; }
    public int GameSyncIDSize => MyStatus6.GameSyncIDSize; // 64 bits
    public string GameSyncID { get => Status.GameSyncID; set => Status.GameSyncID = value; }
    public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
    public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
    public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

    public abstract int Badges { get; set; }
    public abstract int Vivillon { get; set; }
    public abstract int BP { get; set; }
    // Money

    public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
    public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    // Daycare
    public override int DaycareSeedSize => 16;

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);

    public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

    private int GetBoxNameOffset(int box) => PCLayout + (LongStringLength * box);

    public override string GetBoxName(int box)
    {
        if (PCLayout < 0)
            return $"B{box + 1}";
        return GetString(Data.AsSpan(GetBoxNameOffset(box), LongStringLength));
    }

    public override void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = Data.AsSpan(GetBoxNameOffset(box), LongStringLength);
        SetString(span, value, LongStringLength / 2, StringConverterOption.ClearZero);
    }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        PK6 pk6 = (PK6)pk;
        // Apply to this Save File
        int CT = pk6.CurrentHandler;
        var now = EncounterDate.GetDate3DS();
        pk6.Trade(this, now.Day, now.Month, now.Year);
        if (CT != pk6.CurrentHandler) // Logic updated Friendship
        {
            // Copy over the Friendship Value only under certain circumstances
            if (pk6.HasMove(216)) // Return
                pk6.CurrentFriendship = pk6.OppositeFriendship;
            else if (pk6.HasMove(218)) // Frustration
                pk.CurrentFriendship = pk6.OppositeFriendship;
        }

        pk6.FormArgumentElapsed = pk6.FormArgumentMaximum = 0;
        pk6.FormArgumentRemain = (byte)GetFormArgument(pk, isParty);
        if (!isParty && pk.Form != 0)
        {
            switch (pk.Species)
            {
                case (int) Species.Furfrou:
                    pk.Form = 0;
                    break;
                case (int) Species.Hoopa:
                {
                    pk.Form = 0;
                    var hsf = pk.GetMoveIndex((int) Move.HyperspaceFury);
                    if (hsf != -1)
                        pk.SetMove(hsf, (int) Move.HyperspaceHole);
                    break;
                }
            }
        }

        pk.RefreshChecksum();
        AddCountAcquired(pk);
    }

    private void AddCountAcquired(PKM pk)
    {
        Records.AddRecord(pk.WasEgg ? 009 : 007); // egg, capture
        if (pk.CurrentHandler == 1)
            Records.AddRecord(012); // trade
        if (!pk.WasEgg)
        {
            Records.AddRecord(004); // total battles
            Records.AddRecord(005); // wild encounters
        }
    }

    private static uint GetFormArgument(PKM pk, bool isParty)
    {
        if (!isParty || pk.Form == 0)
            return 0;
        return pk.Species switch
        {
            (int)Species.Furfrou => 5u, // Furfrou
            (int)Species.Hoopa => 3u, // Hoopa
            _ => 0u,
        };
    }

    public override int PartyCount
    {
        get => Data[Party + (6 * SIZE_PARTY)];
        protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
    }

    public sealed override string GetString(ReadOnlySpan<byte> data) => StringConverter6.GetString(data);

    public sealed override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter6.SetString(destBuffer, value, maxLength, option);
    }

    public int GetRecord(int recordID) => Records.GetRecord(recordID);
    public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
    public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
    public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
    public int RecordCount => RecordBlock6.RecordCount;
    public abstract MyItem Items { get; }
    public abstract ItemInfo6 ItemInfo { get; }
    public abstract GameTime6 GameTime { get; }
    public abstract Situation6 Situation { get; }
    public abstract PlayTime6 Played { get; }
    public abstract MyStatus6 Status { get; }
    public abstract RecordBlock6 Records { get; }

    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return GetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        SetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    public ushort GetWork(int index) => ReadUInt16LittleEndian(Data.AsSpan(EventWork + (index * 2)));
    public void SetWork(int index, ushort value) => WriteUInt16LittleEndian(Data.AsSpan(EventWork)[(index * 2)..], value);
}
