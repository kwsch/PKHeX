using System.Buffers.Binary;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Core;

namespace PKHeX.Infrastructure.GiftRecords;

/// <summary>Which trimmed-wondercard game owns the block (determines the card format).</summary>
public enum TrimmedCardGame
{
    PLA,
    SV,
}

/// <summary>
/// PLA/SV received-gift records: both games share block key 0x99E1625E holding 50 slots of
/// 0x278 bytes plus a constant 0x340 tail. Each slot is a trimmed wondercard (WA8/WC9):
/// bytes 0x00-0x03 = unix receive time, 0x08-0x0B = card id (wc 0x08..0x0C),
/// 0x0C = card title index, 0x0E = gift type, 0x10.. = wc bytes 0x18..0x280.
/// Format per community research (Project Pokémon SV mystery-gift research thread;
/// WangPluginSav PLA parser, GPL-3.0).
/// </summary>
public sealed class TrimmedCardGiftRecordStore(SaveFile sav, SCBlock block, TrimmedCardGame game) : IGiftRecordStore
{
    private const int SlotSize = 0x278;
    private const int PhysicalSlotCount = 50;
    private const int SvHistoryCount = 32;
    internal const int RequiredSize = SlotSize * PhysicalSlotCount;

    private const int CardBodyStart = 0x18;  // wondercard offset the slot body resumes at
    private const int CardBodyEnd = 0x280;   // wondercard offset the record stops retaining at
    private const int SlotBodyStart = 0x10;

    public int Count => game == TrimmedCardGame.SV ? SvHistoryCount : PhysicalSlotCount;
    public bool SupportsImport => true;
    public bool CanImport(int index) => (uint)index < Count;
    public IReadOnlyList<string> ImportExtensions { get; } = game == TrimmedCardGame.PLA ? ["*.wa8"] : ["*.wc9"];
    public bool SupportsExport => true;
    public bool SupportsReceivedFlags => false;
    public bool SupportsSerialLock => false;
    public DateTime? SerialLockTimestamp => null;

    private Span<byte> Slot(int index) => block.Data.Slice(index * SlotSize, SlotSize);

    public IReadOnlyList<GiftRecordEntry> ReadAll()
    {
        var result = new GiftRecordEntry[Count];
        for (int i = 0; i < Count; i++)
            result[i] = Parse(i);
        return result;
    }

    private GiftRecordEntry Parse(int index)
    {
        var data = Slot(index);
        if (!data.ContainsAnyExcept((byte)0))
            return new GiftRecordEntry { Index = index, IsEmpty = true };

        var card = ReconstructCard(data);
        var kind = GetKind(data[0x0E]);
        var seconds = BinaryPrimitives.ReadUInt32LittleEndian(data);
        return new GiftRecordEntry
        {
            Index = index,
            ReceivedAt = seconds == 0 ? null : DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime,
            CardId = card.CardID,
            Kind = kind,
            Species = kind == GiftRecordKind.Pokemon ? card.Species : (ushort)0,
            Form = kind == GiftRecordKind.Pokemon ? card.Form : (byte)0,
            IsEgg = kind == GiftRecordKind.Pokemon && card.IsEgg,
            OriginalTrainerName = kind == GiftRecordKind.Pokemon ? card.OriginalTrainerName : string.Empty,
            Items = kind == GiftRecordKind.Item ? ReadItems(card) : [],
            Card = card,
        };
    }

    private GiftRecordKind GetKind(byte type) => game switch
    {
        // WA8.GiftType: 1=Pokémon, 2=Item, 3=Clothing
        TrimmedCardGame.PLA => type switch
        {
            1 => GiftRecordKind.Pokemon,
            2 => GiftRecordKind.Item,
            3 => GiftRecordKind.Clothing,
            _ => GiftRecordKind.Unknown,
        },
        // WC9.GiftType: 1=Pokémon, 2=Item, 3=BP, 4=Clothing
        _ => type switch
        {
            1 => GiftRecordKind.Pokemon,
            2 => GiftRecordKind.Item,
            3 => GiftRecordKind.BattlePoints,
            4 => GiftRecordKind.Clothing,
            _ => GiftRecordKind.Unknown,
        },
    };

    private static List<GiftRecordItem> ReadItems(DataMysteryGift card)
    {
        var items = new List<GiftRecordItem>(6);
        for (int i = 0; i < 6; i++)
        {
            (int id, int qty) = card switch
            {
                WA8 wa => (wa.GetItem(i), wa.GetQuantity(i)),
                WC9 wc => (wc.GetItem(i), wc.GetQuantity(i)),
                _ => (0, 0),
            };
            if (id != 0)
                items.Add(new GiftRecordItem(id, qty));
        }
        return items;
    }

    /// <summary>Rebuilds the wondercard the record was trimmed from. Fields the record dropped stay zero.</summary>
    private DataMysteryGift ReconstructCard(ReadOnlySpan<byte> data)
    {
        if (game == TrimmedCardGame.PLA)
        {
            var raw = new byte[WA8.Size];
            data[0x08..0x0C].CopyTo(raw.AsSpan(0x08));
            raw[0x0F] = data[0x0E]; // WA8.CardType
            raw[0x13] = data[0x0C]; // WA8.CardTitleIndex
            data[SlotBodyStart..SlotSize].CopyTo(raw.AsSpan(CardBodyStart));
            return new WA8(raw);
        }
        else
        {
            var raw = new byte[WC9.Size];
            data[0x08..0x0C].CopyTo(raw.AsSpan(0x08));
            raw[0x11] = data[0x0E]; // WC9.CardType
            raw[0x15] = data[0x0C]; // WC9.CardTitleIndex
            data[SlotBodyStart..SlotSize].CopyTo(raw.AsSpan(CardBodyStart));
            return new WC9(raw);
        }
    }

    public void ClearEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
        Slot(index).Clear();
        sav.State.Edited = true;
    }

    public bool TryImport(int index, byte[] data, string extension, DateTime receivedAt, out GiftRecordImportError error)
    {
        if (!CanImport(index))
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
        bool matches = game == TrimmedCardGame.PLA ? gift is WA8 : gift is WC9;
        if (!matches)
        {
            error = GiftRecordImportError.WrongGame;
            return false;
        }
        var card = gift;
        if (game == TrimmedCardGame.SV && card is WC9 { CardType: not WC9.GiftType.Pokemon })
        {
            error = GiftRecordImportError.UnsupportedGiftType;
            return false;
        }
        if (card is WC9 wc9 && !CanReceive(wc9, sav.Version))
        {
            error = GiftRecordImportError.WrongGame;
            return false;
        }
        long unixSeconds = new DateTimeOffset(receivedAt).ToUnixTimeSeconds();
        if (unixSeconds is < 0 or > uint.MaxValue)
        {
            error = GiftRecordImportError.InvalidTimestamp;
            return false;
        }

        Span<byte> raw = card.Data;
        var slot = Slot(index);
        slot.Clear();
        BinaryPrimitives.WriteUInt32LittleEndian(slot, (uint)unixSeconds);
        raw[0x08..0x0C].CopyTo(slot[0x08..]);
        slot[0x0C] = (byte)card.CardTitleIndex;
        slot[0x0E] = game == TrimmedCardGame.PLA ? raw[0x0F] : raw[0x11];
        raw[CardBodyStart..CardBodyEnd].CopyTo(slot[SlotBodyStart..]);

        sav.State.Edited = true;
        error = GiftRecordImportError.None;
        return true;
    }

    private static bool CanReceive(WC9 wc, GameVersion version) => wc.RestrictVersion switch
    {
        0 or 3 => version is GameVersion.SL or GameVersion.VL,
        1 => version is GameVersion.SL,
        2 => version is GameVersion.VL,
        _ => false,
    };

    public DataMysteryGift? ExportCard(int index)
    {
        if ((uint)index >= Count)
            return null;
        var data = Slot(index);
        if (!data.ContainsAnyExcept((byte)0))
            return null;
        return ReconstructCard(data);
    }

    public IReadOnlyList<int> GetReceivedFlagIndexes() => [];
    public void SetReceivedFlag(int flag, bool value) { }
    public void ResetSerialLock() { }
}
