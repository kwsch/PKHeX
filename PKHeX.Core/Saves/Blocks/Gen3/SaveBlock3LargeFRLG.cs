using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3LargeFRLG(Memory<byte> Raw) : ISaveBlock3LargeExpansion, IRecordStatStorage<RecID3FRLG, uint>
{
    public Span<byte> Data => Raw.Span;
    public ushort X { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Y { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }

    public byte PartyCount { get => Data[0x034]; set => Data[0x034] = value; }
    public Span<byte> PartyBuffer => Data.Slice(0x038, 6 * PokeCrypto.SIZE_3PARTY);

    public uint Money { get => ReadUInt32LittleEndian(Data[0x0290..]); set => WriteUInt32LittleEndian(Data[0x0290..], value); }
    public ushort Coin { get => ReadUInt16LittleEndian(Data[0x0294..]); set => WriteUInt16LittleEndian(Data[0x0294..], value); }
    public ushort RegisteredItem { get => ReadUInt16LittleEndian(Data[0x0296..]); set => WriteUInt16LittleEndian(Data[0x0296..], value); }
    public Span<byte> Inventory => Data.Slice(0x0298, 0x360);
    public int SeenOffset2 => 0x5F8;

    private const int EventFlag = 0xEE0;
    private const int EventWork = 0x1000;

    public int EventFlagCount => 8 * 288;
    public int EventWorkCount => 0x100;
    public int EggEventFlag => 0x266;
    public int BadgeFlagStart => 0x820;

    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return FlagUtil.GetFlag(Data, EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        FlagUtil.SetFlag(Data, EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    public ushort GetWork(int index) => ReadUInt16LittleEndian(Data[(EventWork + (index * 2))..]);
    public void SetWork(int index, ushort value) => WriteUInt16LittleEndian(Data[EventWork..][(index * 2)..], value);

    private const int RecordOffset = 0x1200;
    private static int GetRecordOffset(RecID3FRLG record)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)record, (uint)RecID3FRLG.NUM_GAME_STATS);
        return RecordOffset + (sizeof(uint) * (int)record);
    }

    public uint GetRecord(int record) => GetRecord((RecID3FRLG)record);
    public void SetRecord(int record, uint value) => SetRecord((RecID3FRLG)record, value);
    public uint GetRecord(RecID3FRLG record) => ReadUInt32LittleEndian(Data[GetRecordOffset(record)..]);
    public void SetRecord(RecID3FRLG record, uint value) => WriteUInt32LittleEndian(Data[GetRecordOffset(record)..], value);
    public void AddRecord(RecID3FRLG record, uint value) => SetRecord(record, GetRecord(record) + value);


    private const int MailOffset = 0x2CD0;
    private static int GetMailOffset(int index) => (index * Mail3.SIZE) + MailOffset;
    private Span<byte> GetMailSpan(int ofs) => Data.Slice(ofs, Mail3.SIZE);

    public Mail3 GetMail(int mailIndex)
    {
        var ofs = GetMailOffset(mailIndex);
        var span = Data.Slice(ofs, Mail3.SIZE);
        return new Mail3(span.ToArray(), ofs);
    }

    public void SetMail(int mailIndex, Mail3 value)
    {
        var ofs = GetMailOffset(mailIndex);
        value.CopyTo(GetMailSpan(ofs));
    }

    public int DaycareOffset => 0x2F80;
    public int DaycareSlotSize => PokeCrypto.SIZE_3STORED + 0x3C; // 0x38 mail + 4 exp

    public ushort DaycareSeed
    {
        get => ReadUInt16LittleEndian(Data[0x3098..]);
        set => WriteUInt16LittleEndian(Data[0x3098..], value);
    }

    public Span<byte> GiftRibbons => Data.Slice(0x309C, 11);
    public int ExternalEventData => 0x30A7;
    public Memory<byte> RoamerData => Raw.Slice(0x30D0, Roamer3.SIZE);

    private const int OFFSET_EBERRY = 0x30EC;
    private const int SIZE_EBERRY = 0x34;
    public Span<byte> EReaderBerry => Data.Slice(OFFSET_EBERRY, SIZE_EBERRY);

    public const int WonderNewsOffset = 0x3120;

    private static int WonderCardOffset(bool isJapanese) => WonderNewsOffset + (isJapanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE);
    private static int WonderCardExtraOffset(bool isJapanese) => WonderCardOffset(isJapanese) + (isJapanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE);

    private Span<byte> WonderNewsData(bool isJapanese) => Data.Slice(WonderNewsOffset, isJapanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE);
    private Span<byte> WonderCardData(bool isJapanese) => Data.Slice(WonderCardOffset(isJapanese), isJapanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE);
    private Span<byte> WonderCardExtraData(bool isJapanese) => Data.Slice(WonderCardExtraOffset(isJapanese), WonderCard3Extra.SIZE);

    public WonderNews3 GetWonderNews(bool isJapanese) => new(WonderNewsData(isJapanese).ToArray());
    public void SetWonderNews(bool isJapanese, ReadOnlySpan<byte> data) => data.CopyTo(WonderNewsData(isJapanese));
    public WonderCard3 GetWonderCard(bool isJapanese) => new(WonderCardData(isJapanese).ToArray());
    public void SetWonderCard(bool isJapanese, ReadOnlySpan<byte> data) => data.CopyTo(WonderCardData(isJapanese));
    public WonderCard3Extra GetWonderCardExtra(bool isJapanese) => new(WonderCardExtraData(isJapanese).ToArray());
    public void SetWonderCardExtra(bool isJapanese, ReadOnlySpan<byte> data) => data.CopyTo(WonderCardExtraData(isJapanese));

    public Span<byte> RivalNameTrash => Data.Slice(0x3A4C, 8);

    private Span<byte> MysterySpan => Data.Slice(0x361C, MysteryEvent3.SIZE);
    public Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3(MysterySpan.ToArray());
        set => value.Data.CopyTo(MysterySpan);
    }

    public int SeenOffset3 => 0x3A18;
    public Memory<byte> SingleDaycareRoute5 => Raw.Slice(0x3C98, PokeCrypto.SIZE_3STORED); // 0x38 mail + 4 exp
}
