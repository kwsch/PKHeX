using System.Buffers.Binary;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Core;

namespace PKHeX.Infrastructure.GiftRecords;

/// <summary>
/// SWSH received-gift records ("Wonder Card Records", WR8): 50 compact 0x68-byte summaries of
/// redeemed gifts inside the 0x112D5141 SCBlock. Format per community research
/// (Project Pokémon WR8 documentation; WangPluginSav reference parser, GPL-3.0).
/// </summary>
public sealed class SwshGiftRecordStore(SAV8SWSH sav, SCBlock block) : IGiftRecordStore
{
    private const int SlotSize = 0x68;
    private const int SlotCount = 50;
    internal const int RequiredSize = SlotSize * SlotCount;

    // WR8 gift types (0x0C): 1=Pokémon, 2=Item, 3=BP, 4=Clothing, 5=Money.
    private const byte TypePokemon = 1;
    private const byte TypeItem = 2;
    private const byte TypeBP = 3;
    private const byte TypeClothing = 4;
    private const byte TypeMoney = 5;

    public int Count => SlotCount;
    public bool SupportsImport => true;
    public bool CanImport(int index) => (uint)index < SlotCount;
    public IReadOnlyList<string> ImportExtensions { get; } = ["*.wc8"];
    public bool SupportsExport => false;
    public bool SupportsReceivedFlags => false;
    public bool SupportsSerialLock => false;
    public DateTime? SerialLockTimestamp => null;

    private Span<byte> Slot(int index) => block.Data.Slice(index * SlotSize, SlotSize);

    public IReadOnlyList<GiftRecordEntry> ReadAll()
    {
        var result = new GiftRecordEntry[SlotCount];
        for (int i = 0; i < SlotCount; i++)
            result[i] = Parse(i);
        return result;
    }

    private GiftRecordEntry Parse(int index)
    {
        var data = Slot(index);
        if (IsSlotEmpty(data))
            return new GiftRecordEntry { Index = index, IsEmpty = true };

        var type = data[0x0C];
        var kind = type switch
        {
            TypePokemon => GiftRecordKind.Pokemon,
            TypeItem => GiftRecordKind.Item,
            TypeBP => GiftRecordKind.BattlePoints,
            TypeClothing => GiftRecordKind.Clothing,
            TypeMoney => GiftRecordKind.Money,
            _ => GiftRecordKind.Unknown,
        };

        var entry = new GiftRecordEntry
        {
            Index = index,
            ReceivedAt = DecodeTimestamp(BinaryPrimitives.ReadUInt64LittleEndian(data)),
            CardId = BinaryPrimitives.ReadUInt16LittleEndian(data[0x08..]),
            Kind = kind,
            Species = kind == GiftRecordKind.Pokemon ? BinaryPrimitives.ReadUInt16LittleEndian(data[0x30..]) : (ushort)0,
            Form = kind == GiftRecordKind.Pokemon ? (byte)BinaryPrimitives.ReadUInt16LittleEndian(data[0x32..]) : (byte)0,
            IsEgg = kind == GiftRecordKind.Pokemon && data[0x12] == 1,
            OriginalTrainerName = kind == GiftRecordKind.Pokemon ? StringConverter8.GetString(data.Slice(0x48, 0x18)) : string.Empty,
            Items = kind == GiftRecordKind.Item ? ReadItems(data) : [],
            Amount = kind is GiftRecordKind.BattlePoints or GiftRecordKind.Money
                ? BinaryPrimitives.ReadUInt16LittleEndian(data[0x46..])
                : 0u,
            Card = ReconstructCard(data, kind),
        };
        return entry;
    }

    private static bool IsSlotEmpty(ReadOnlySpan<byte> data)
    {
        // Unused slots are either zero-filled (blank saves) or carry an FF-filled timestamp word.
        if (BinaryPrimitives.ReadUInt32LittleEndian(data[0x04..]) == uint.MaxValue)
            return true;
        return !data.ContainsAnyExcept((byte)0);
    }

    private static List<GiftRecordItem> ReadItems(ReadOnlySpan<byte> data)
    {
        var items = new List<GiftRecordItem>(6);
        int count = Math.Min(data[0x0D], (byte)6);
        for (int i = 0; i < count; i++)
        {
            int ofs = 0x30 + (i * 4);
            int id = BinaryPrimitives.ReadUInt16LittleEndian(data[ofs..]);
            int qty = BinaryPrimitives.ReadUInt16LittleEndian(data[(ofs + 2)..]);
            if (id != 0)
                items.Add(new GiftRecordItem(id, qty));
        }
        return items;
    }

    /// <summary>Builds a skeleton WC8 so localized card-title lookup and sprite-friendly fields work; not exportable.</summary>
    private static WC8? ReconstructCard(ReadOnlySpan<byte> data, GiftRecordKind kind)
    {
        var type = data[0x0C];
        if (type is 0 or > TypeClothing) // Money has no WC8 GiftType equivalent
            return null;

        var wc = new WC8
        {
            CardID = BinaryPrimitives.ReadUInt16LittleEndian(data[0x08..]),
            CardTitleIndex = data[0x0A],
            CardType = (WC8.GiftType)type,
        };
        if (kind == GiftRecordKind.Pokemon)
        {
            wc.Species = BinaryPrimitives.ReadUInt16LittleEndian(data[0x30..]);
            wc.Form = (byte)BinaryPrimitives.ReadUInt16LittleEndian(data[0x32..]);
            wc.IsEgg = data[0x12] == 1;
            wc.HeldItem = BinaryPrimitives.ReadUInt16LittleEndian(data[0x10..]);
            wc.Move1 = BinaryPrimitives.ReadUInt16LittleEndian(data[0x38..]);
            wc.Move2 = BinaryPrimitives.ReadUInt16LittleEndian(data[0x3C..]);
            wc.Move3 = BinaryPrimitives.ReadUInt16LittleEndian(data[0x40..]);
            wc.Move4 = BinaryPrimitives.ReadUInt16LittleEndian(data[0x44..]);
            var language = data[0x62];
            var ot = StringConverter8.GetString(data.Slice(0x48, 0x18));
            wc.SetOT(language is >= 1 and <= 10 ? language : (byte)(int)LanguageID.English, ot);
        }
        return wc;
    }

    public void ClearEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, SlotCount);
        Slot(index).Clear();
        sav.State.Edited = true;
    }

    public bool TryImport(int index, byte[] data, string extension, DateTime receivedAt, out GiftRecordImportError error)
    {
        if ((uint)index >= SlotCount)
        {
            error = GiftRecordImportError.InvalidSlot;
            return false;
        }
        var gift = MysteryGift.GetMysteryGift(data, extension);
        if (gift is null)
        {
            error = GiftRecordImportError.UnreadableFile;
            return false;
        }
        if (gift is not WC8 wc)
        {
            error = GiftRecordImportError.WrongGame;
            return false;
        }
        if (wc.CardType is not (WC8.GiftType.Pokemon or WC8.GiftType.Item or WC8.GiftType.BP))
        {
            error = GiftRecordImportError.UnsupportedGiftType;
            return false;
        }
        if (!CanReceive(wc, sav.Version))
        {
            error = GiftRecordImportError.WrongGame;
            return false;
        }
        if (receivedAt.Year is < 2000 or > 4095)
        {
            error = GiftRecordImportError.InvalidTimestamp;
            return false;
        }

        var slot = Slot(index);
        slot.Clear();
        BinaryPrimitives.WriteUInt64LittleEndian(slot, EncodeTimestamp(receivedAt));
        BinaryPrimitives.WriteUInt16LittleEndian(slot[0x08..], (ushort)wc.CardID);
        slot[0x0A] = (byte)wc.CardTitleIndex;
        slot[0x0C] = (byte)wc.CardType;
        slot[0x0E] = 1;
        slot[0x0F] = wc.Data[0x1C] == byte.MaxValue ? (byte)0 : (byte)(wc.Data[0x1C] + 1);
        if (wc.IsEntity)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x10..], (ushort)Math.Max(0, wc.HeldItem));
            slot[0x12] = wc.IsEgg ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x30..], wc.Species);
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x32..], wc.Form);
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x38..], wc.Move1);
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x3C..], wc.Move2);
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x40..], wc.Move3);
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x44..], wc.Move4);
            var redeemLanguage = sav.Language is >= 1 and <= 10 ? sav.Language : (int)LanguageID.English;
            var recordLanguage = wc.GetLanguage(redeemLanguage);
            if (recordLanguage == 0)
                recordLanguage = redeemLanguage;
            bool hasOT = wc.GetHasOT(redeemLanguage);
            var ot = hasOT ? wc.GetOT(redeemLanguage) : sav.OT;
            StringConverter8.SetString(slot.Slice(0x48, 0x18), ot, 12, StringConverterOption.ClearZero);
            slot[0x34] = hasOT && wc.OTGender < 2 ? wc.OTGender : sav.Gender;
            slot[0x62] = (byte)recordLanguage;
            byte ribbon = wc.GetRibbonAtIndex(0);
            slot[0x63] = ribbon == byte.MaxValue ? (byte)98 : ribbon;
            slot[0x64] = wc.Gender;
        }
        else if (wc.IsItem)
        {
            int n = 0;
            for (int i = 0; i < 6; i++)
            {
                int id = wc.GetItem(i);
                if (id == 0)
                    continue;
                int ofs = 0x30 + (n * 4);
                BinaryPrimitives.WriteUInt16LittleEndian(slot[ofs..], (ushort)id);
                BinaryPrimitives.WriteUInt16LittleEndian(slot[(ofs + 2)..], (ushort)wc.GetQuantity(i));
                n++;
            }
            slot[0x0D] = (byte)Math.Max(1, n);
        }
        else if (wc.CardType == WC8.GiftType.BP)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(slot[0x46..],
                BinaryPrimitives.ReadUInt16LittleEndian(wc.Data[0x20..]));
        }

        sav.State.Edited = true;
        error = GiftRecordImportError.None;
        return true;
    }

    private static bool CanReceive(WC8 wc, GameVersion version)
    {
        try
        {
            return wc.CanBeReceivedByVersion(version);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    public DataMysteryGift? ExportCard(int index) => null;
    public IReadOnlyList<int> GetReceivedFlagIndexes() => [];
    public void SetReceivedFlag(int flag, bool value) { }
    public void ResetSerialLock() { }

    // 8-byte bit-packed local timestamp: [Year:12 @26][Month:4 @22][Day:5 @17][Hour:5 @12][Min:6 @6][Sec:6 @0].
    private static DateTime? DecodeTimestamp(ulong value)
    {
        if (value == 0)
            return null;
        int year = (int)((value >> 26) & 0xFFF);
        int month = (int)((value >> 22) & 0xF);
        int day = (int)((value >> 17) & 0x1F);
        int hour = (int)((value >> 12) & 0x1F);
        int minute = (int)((value >> 6) & 0x3F);
        int second = (int)(value & 0x3F);
        if (year is < 2000 or > 4095 || month is < 1 or > 12 || day is < 1 or > 31 || hour > 23 || minute > 59 || second > 59)
            return null;
        try
        {
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private static ulong EncodeTimestamp(DateTime value) =>
        ((ulong)(uint)value.Year << 26) | ((ulong)(uint)value.Month << 22) | ((ulong)(uint)value.Day << 17) |
        ((ulong)(uint)value.Hour << 12) | ((ulong)(uint)value.Minute << 6) | (uint)value.Second;
}
