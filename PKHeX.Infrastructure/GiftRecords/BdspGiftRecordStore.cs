using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Core;

namespace PKHeX.Infrastructure.GiftRecords;

/// <summary>
/// BDSP received-gift records via Core's typed <see cref="MysteryBlock8b"/>:
/// 50 received-gift entries, 10 one-day serial entries (appended after the received list),
/// a received-flag bitfield, and the serial-code redemption lock.
/// </summary>
public sealed class BdspGiftRecordStore(SAV8BS sav) : IGiftRecordStore
{
    private const int MysteryRecordsOffset = 0xE9BE8;
    private const int ReceivedCount = 50;
    private const int OneDayCount = 10;
    internal const int RequiredSaveSize = MysteryRecordsOffset + MysteryBlock8b.MinSize;

    private MysteryBlock8b Block => sav.MysteryRecords;
    private Span<byte> ReceivedFlagData => sav.Data.Slice(
        MysteryRecordsOffset + MysteryBlock8b.OFS_RECVFLAG, MysteryBlock8b.FlagSize);

    internal static bool IsSupported(SAV8BS sav) => sav.Data.Length >= RequiredSaveSize;

    public int Count => ReceivedCount + OneDayCount;
    public bool SupportsImport => true;
    public bool CanImport(int index) => (uint)index < ReceivedCount;
    public IReadOnlyList<string> ImportExtensions { get; } = ["*.wb8"];
    public bool SupportsExport => false;
    public bool SupportsReceivedFlags => true;
    public bool SupportsSerialLock => true;

    public DateTime? SerialLockTimestamp => Block.TicksSerialLock == 0 ? null : Block.LocalTimestampSerialLock;

    public IReadOnlyList<GiftRecordEntry> ReadAll()
    {
        var result = new GiftRecordEntry[Count];
        for (int i = 0; i < ReceivedCount; i++)
            result[i] = ParseReceived(i);
        for (int i = 0; i < OneDayCount; i++)
            result[ReceivedCount + i] = ParseOneDay(i);
        return result;
    }

    private GiftRecordEntry ParseReceived(int index)
    {
        var r = Block.GetReceived(index);
        if (r.Ticks == 0 && r.DeliveryID == 0 && r.Species == 0)
            return new GiftRecordEntry { Index = index, IsEmpty = true };

        var items = new List<GiftRecordItem>(7);
        (ushort Id, ushort Qty)[] raw =
        [
            (r.Item1, r.Item1Count), (r.Item2, r.Item2Count), (r.Item3, r.Item3Count),
            (r.Item4, r.Item4Count), (r.Item5, r.Item5Count), (r.Item6, r.Item6Count),
            (r.Item7, r.Item7Count),
        ];
        foreach (var (id, qty) in raw)
        {
            if (id != 0)
                items.Add(new GiftRecordItem(id, qty));
        }

        bool hasDress = r.DressID1 != 0 || r.DressID2 != 0 || r.DressID3 != 0 || r.DressID4 != 0
                        || r.DressID5 != 0 || r.DressID6 != 0 || r.DressID7 != 0;
        var kind = (WB8.GiftType)r.DataType switch
        {
            WB8.GiftType.Pokemon => GiftRecordKind.Pokemon,
            WB8.GiftType.Item => GiftRecordKind.Item,
            WB8.GiftType.UnderGroundItem => GiftRecordKind.UndergroundItem,
            WB8.GiftType.BP => GiftRecordKind.BattlePoints,
            WB8.GiftType.Clothing => GiftRecordKind.Clothing,
            WB8.GiftType.Money => GiftRecordKind.Money,
            _ when r.Species != 0 => GiftRecordKind.Pokemon,
            _ when items.Count > 0 => GiftRecordKind.Item,
            _ when r.MoneyData != 0 => GiftRecordKind.Money,
            _ when hasDress => GiftRecordKind.Clothing,
            _ => GiftRecordKind.Unknown,
        };

        return new GiftRecordEntry
        {
            Index = index,
            ReceivedAt = r.Ticks == 0 ? null : r.LocalTimestamp,
            CardId = r.DeliveryID,
            Kind = kind,
            Species = r.Species,
            Form = (byte)r.Form,
            IsEgg = r.IsEgg == 1,
            OriginalTrainerName = r.OT,
            Items = items,
            Amount = r.MoneyData,
        };
    }

    private GiftRecordEntry ParseOneDay(int index)
    {
        var o = Block.GetOneDay(index);
        if (o.Ticks == 0 && o.DeliveryID == 0)
            return new GiftRecordEntry { Index = ReceivedCount + index, IsEmpty = true };
        return new GiftRecordEntry
        {
            Index = ReceivedCount + index,
            ReceivedAt = o.Ticks == 0 ? null : o.LocalTimestamp,
            CardId = o.DeliveryID,
            Kind = GiftRecordKind.OneDaySerial,
        };
    }

    public void ClearEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
        if (index < ReceivedCount)
            Block.SetReceived(index, new RecvData8b(new byte[RecvData8b.SIZE]));
        else
            Block.SetOneDay(index - ReceivedCount, new OneDay8b(new byte[OneDay8b.SIZE]));
        sav.State.Edited = true;
    }

    public bool TryImport(int index, byte[] data, string extension, DateTime receivedAt, out GiftRecordImportError error)
    {
        if ((uint)index >= ReceivedCount)
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
        if (gift is not WB8 wb)
        {
            error = GiftRecordImportError.WrongGame;
            return false;
        }
        if (wb.CardType is not (WB8.GiftType.Pokemon or WB8.GiftType.Item))
        {
            error = GiftRecordImportError.UnsupportedGiftType;
            return false;
        }
        try
        {
            _ = receivedAt.ToUniversalTime().ToFileTimeUtc();
        }
        catch (ArgumentOutOfRangeException)
        {
            error = GiftRecordImportError.InvalidTimestamp;
            return false;
        }

        var r = new RecvData8b(new byte[RecvData8b.SIZE])
        {
            LocalTimestamp = receivedAt,
            DeliveryID = (ushort)wb.CardID,
            TextID = (ushort)wb.CardTitleIndex,
            DataType = (byte)wb.CardType,
        };
        if (wb.IsEntity)
        {
            r.Species = wb.Species;
            r.Form = wb.Form;
            r.HeldItem = (ushort)Math.Max(0, wb.HeldItem);
            r.Move1 = wb.Move1;
            r.Move2 = wb.Move2;
            r.Move3 = wb.Move3;
            r.Move4 = wb.Move4;
            var redeemLanguage = sav.Language is >= 1 and <= 10 ? sav.Language : (int)LanguageID.English;
            var recordLanguage = wb.GetLanguage(redeemLanguage);
            if (recordLanguage == 0)
                recordLanguage = redeemLanguage;
            bool hasOT = wb.GetHasOT(redeemLanguage);
            r.OT = hasOT ? wb.GetOT(redeemLanguage) : sav.OT;
            r.OriginalTrainerGender = hasOT && wb.OTGender < 2 ? wb.OTGender : sav.Gender;
            r.Gender = wb.Gender;
            r.IsEgg = wb.IsEgg ? (byte)1 : (byte)0;
            r.Language = (byte)recordLanguage;
        }
        else if (wb.IsItem)
        {
            Action<ushort, ushort>[] setters =
            [
                (id, qty) => { r.Item1 = id; r.Item1Count = qty; },
                (id, qty) => { r.Item2 = id; r.Item2Count = qty; },
                (id, qty) => { r.Item3 = id; r.Item3Count = qty; },
                (id, qty) => { r.Item4 = id; r.Item4Count = qty; },
                (id, qty) => { r.Item5 = id; r.Item5Count = qty; },
                (id, qty) => { r.Item6 = id; r.Item6Count = qty; },
                (id, qty) => { r.Item7 = id; r.Item7Count = qty; },
            ];
            int n = 0;
            for (int i = 0; i < 7 && n < setters.Length; i++)
            {
                int id = wb.GetItem(i);
                if (id == 0)
                    continue;
                setters[n]((ushort)id, (ushort)wb.GetQuantity(i));
                n++;
            }
        }

        Block.SetReceived(index, r);
        sav.State.Edited = true;
        error = GiftRecordImportError.None;
        return true;
    }

    public DataMysteryGift? ExportCard(int index) => null;

    public IReadOnlyList<int> GetReceivedFlagIndexes()
    {
        var result = new List<int>();
        var flags = ReceivedFlagData;
        for (int flag = 0; flag < MysteryBlock8b.FlagMax; flag++)
        {
            if ((flags[flag >> 3] & (1 << (flag & 7))) != 0)
                result.Add(flag);
        }
        return result;
    }

    public void SetReceivedFlag(int flag, bool value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(flag);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(flag, MysteryBlock8b.FlagMax);
        var flags = ReceivedFlagData;
        int offset = flag >> 3;
        byte mask = (byte)(1 << (flag & 7));
        flags[offset] = value ? (byte)(flags[offset] | mask) : (byte)(flags[offset] & ~mask);
        sav.State.Edited = true;
    }

    public void ResetSerialLock()
    {
        if (Block.TicksSerialLock == 0)
            return;
        Block.ResetLock();
        sav.State.Edited = true;
    }
}
