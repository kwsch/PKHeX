using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Record of a received <see cref="WB7"/> file.
/// </summary>
/// <remarks>
/// A full <see cref="WB7"/> is not stored in the <see cref="SAV7b"/> structure, as it is immediately converted to <see cref="PKM"/> upon receiving from server.
/// The save file just stores a summary of the received data for the user to look back at.
/// </remarks>
public sealed class WR7(byte[] Data) : DataMysteryGift(Data)
{
    public WR7() : this(new byte[Size]) { }

    public const int Size = 0x140;
    public override byte Generation => 7;
    public override EntityContext Context => EntityContext.Gen7;
    public override bool FatefulEncounter => true;

    public override GameVersion Version => GameVersion.GG;

    public override AbilityPermission Ability => AbilityPermission.Any12H; // undefined

    public long Epoch
    {
        get => ReadInt64LittleEndian(Data.AsSpan(0x00));
        set => WriteInt64LittleEndian(Data.AsSpan(0x00), value);
    }

    public DateOnly Date => new DateOnly(1970, 1, 1).AddDays((int)(Epoch / 86400));

    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x08));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x08), (ushort)value);
    }

    public ushort CardType
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), value);
    }

    public WR7GiftType GiftType
    {
        get => (WR7GiftType)Data[0x0C];
        set => Data[0x0C] = (byte)value;
    }

    public int ItemCount
    {
        get => Data[0x0D];
        set => Data[0x0D] = (byte)value;
    }

    // unknown: region from 0x10 to 0xFF ?

    public override ushort Species
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x10C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x10C), value);
    }

    public override bool GiftUsed { get; set; }

    public override byte Level // are moves stored? mew has '1' but this could be storing a Move ID...
    {
        get => Data[0x10E];
        set => Data[0x10E] = value;
    }

    public override int ItemID { get => ReadUInt16LittleEndian(Data.AsSpan(0x110)); set => WriteUInt16LittleEndian(Data.AsSpan(0x110), (ushort)value); }
    public ushort ItemIDCount   { get => ReadUInt16LittleEndian(Data.AsSpan(0x112)); set => WriteUInt16LittleEndian(Data.AsSpan(0x112), value); }
    public ushort ItemSet2Item  { get => ReadUInt16LittleEndian(Data.AsSpan(0x114)); set => WriteUInt16LittleEndian(Data.AsSpan(0x114), value); }
    public ushort ItemSet2Count { get => ReadUInt16LittleEndian(Data.AsSpan(0x116)); set => WriteUInt16LittleEndian(Data.AsSpan(0x116), value); }
    public ushort ItemSet3Item  { get => ReadUInt16LittleEndian(Data.AsSpan(0x118)); set => WriteUInt16LittleEndian(Data.AsSpan(0x118), value); }
    public ushort ItemSet3Count { get => ReadUInt16LittleEndian(Data.AsSpan(0x11A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x11A), value); }
    public ushort ItemSet4Item  { get => ReadUInt16LittleEndian(Data.AsSpan(0x11C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x11C), value); }
    public ushort ItemSet4Count { get => ReadUInt16LittleEndian(Data.AsSpan(0x11E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x11E), value); }
    public ushort ItemSet5Item  { get => ReadUInt16LittleEndian(Data.AsSpan(0x120)); set => WriteUInt16LittleEndian(Data.AsSpan(0x120), value); } // struct union overlaps OT Name data, beware!
    public ushort ItemSet5Count { get => ReadUInt16LittleEndian(Data.AsSpan(0x122)); set => WriteUInt16LittleEndian(Data.AsSpan(0x122), value); }
    public ushort ItemSet6Item  { get => ReadUInt16LittleEndian(Data.AsSpan(0x124)); set => WriteUInt16LittleEndian(Data.AsSpan(0x124), value); }
    public ushort ItemSet6Count { get => ReadUInt16LittleEndian(Data.AsSpan(0x126)); set => WriteUInt16LittleEndian(Data.AsSpan(0x126), value); }

    public override byte Gender { get; set; }
    public override byte Form { get; set; }
    public override uint ID32 { get; set; }
    public override ushort TID16 { get; set; }
    public override ushort SID16 { get; set; }

    public override string OriginalTrainerName
    {
        get => StringConverter8.GetString(Data.AsSpan(0x120, 0x1A));
        set => StringConverter8.SetString(Data.AsSpan(0x120, 0x1A), value, 12, StringConverterOption.ClearZero);
    }

    public LanguageID LanguageReceived
    {
        get => (LanguageID)Data[0x13A];
        set => Data[0x13A] = (byte)value;
    }

    // Mystery Gift implementation, unused.
    public override bool IsMatchExact(PKM pk, EvoCriteria evo) => false;
    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false;
    public override Shiny Shiny => Shiny.Never;

    public override ushort Location { get; set; }
    public override ushort EggLocation { get; set; }
    public override byte Ball { get; set; } = 4;

    public override string CardTitle { get => $"{nameof(WB7)} Record ({OriginalTrainerName}) [{LanguageReceived}]"; set { } }

    public override bool IsItem
    {
        get => GiftType == WR7GiftType.Item;
        set
        {
            if (value)
                GiftType = WR7GiftType.Item;
        }
    }

    public override bool IsEntity
    {
        get => GiftType == WR7GiftType.Pokemon;
        set
        {
            if (value)
                GiftType = WR7GiftType.Pokemon;
        }
    }

    public override PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        // this method shouldn't really be called, use the WB7 data not the WR7 data.
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        // we'll just generate something as close as we can, since we must return something!
        var pk = new PB7();
        tr.ApplyTo(pk);
        if (!GameVersion.GG.Contains(tr.Version))
            pk.Version = GameVersion.GP;

        pk.Species = Species;
        pk.MetLevel = pk.CurrentLevel = Level;
        pk.MetDate = Date;

        return pk; // can't really do much more, just return the rough data
    }
}
