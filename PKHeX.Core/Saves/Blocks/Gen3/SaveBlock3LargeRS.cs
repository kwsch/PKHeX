using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3LargeRS(Memory<byte> Raw) : ISaveBlock3LargeHoenn, IRecordStatStorage<RecID3RuSa, uint>
{
    public Span<byte> Data => Raw.Span;
    public ushort X { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Y { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }

    public byte PartyCount { get => Data[0x234]; set => Data[0x234] = value; }
    public Span<byte> PartyBuffer => Data.Slice(0x238, 6 * PokeCrypto.SIZE_3PARTY);

    public uint Money { get => ReadUInt32LittleEndian(Data[0x0490..]); set => WriteUInt32LittleEndian(Data[0x0490..], value); }
    public ushort Coin { get => ReadUInt16LittleEndian(Data[0x0494..]); set => WriteUInt16LittleEndian(Data[0x0494..], value); }
    public ushort RegisteredItem { get => ReadUInt16LittleEndian(Data[0x0496..]); set => WriteUInt16LittleEndian(Data[0x0496..], value); }
    public Span<byte> Inventory => Data.Slice(0x498, 0x360);
    private Span<byte> PokeBlockData => Data.Slice(0x7F8, PokeBlock3Case.SIZE);
    public PokeBlock3Case PokeBlocks { get => new(PokeBlockData); set => value.Write(PokeBlockData); }
    public int SeenOffset2 => 0x938;

    private const int OFS_BerryBlenderRecord = 0x96C;

    /// <summary>
    /// Max RPM for 2, 3 and 4 players. Each value unit represents 0.01 RPM. Value 0 if no record.
    /// </summary>
    /// <remarks>2 players: index 0, 3 players: index 1, 4 players: index 2</remarks>
    public const int BerryBlenderRPMRecordCount = 3;

    private Span<byte> GetBlenderRPMSpan(int index)
    {
        if ((uint)index >= BerryBlenderRPMRecordCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return Data[(OFS_BerryBlenderRecord + (index * 2))..];
    }

    public ushort GetBerryBlenderRPMRecord(int index) => ReadUInt16LittleEndian(GetBlenderRPMSpan(index));
    public void SetBerryBlenderRPMRecord(int index, ushort value) => WriteUInt16LittleEndian(GetBlenderRPMSpan(index), value);

    private const int EventFlag = 0x1220;
    private const int EventWork = 0x1340;
    public int EventFlagCount => 8 * 288;
    public int EventWorkCount => 0x100;
    public int EggEventFlag => 0x86;
    public int BadgeFlagStart => 0x807;

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

    private const int RecordOffset = 0x1540;
    private static int GetRecordOffset(RecID3RuSa record)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)record, (uint)RecID3RuSa.NUM_GAME_STATS);
        return RecordOffset + (sizeof(uint) * (int)record);
    }

    public uint GetRecord(int record) => GetRecord((RecID3RuSa)record);
    public void SetRecord(int record, uint value) => SetRecord((RecID3RuSa)record, value);
    public uint GetRecord(RecID3RuSa record) => ReadUInt32LittleEndian(Data[GetRecordOffset(record)..]);
    public void SetRecord(RecID3RuSa record, uint value) => WriteUInt32LittleEndian(Data[GetRecordOffset(record)..], value);
    public void AddRecord(RecID3RuSa record, uint value) => SetRecord(record, GetRecord(record) + value);

    private Memory<byte> SecretBaseData => Raw.Slice(0x1A08, SecretBaseManager3.BaseCount * SecretBase3.SIZE);
    public SecretBaseManager3 SecretBases => new(SecretBaseData);

    public DecorationInventory3 Decorations => new(Data.Slice(0x26A0, DecorationInventory3.SIZE));

    private Span<byte> SwarmData => Data.Slice(0x2AFC, Swarm3.SIZE);
    public Swarm3 Swarm { get => new(SwarmData.ToArray()); set => value.Data.CopyTo(SwarmData); }
    private void ClearSwarm() => SwarmData.Clear();
    public IReadOnlyList<Swarm3> DefaultSwarms => Swarm3Details.Swarms_RS;

    public int SwarmIndex
    {
        get
        {
            var map = Swarm.MapNum;
            for (int i = 0; i < DefaultSwarms.Count; i++)
            {
                if (DefaultSwarms[i].MapNum == map)
                    return i;
            }
            return -1;
        }
        set
        {
            var arr = DefaultSwarms;
            if ((uint)value >= arr.Count)
                ClearSwarm();
            else
                Swarm = arr[value];
        }
    }

    private const int MailOffset = 0x2B4C;
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


    private const int Painting = 0x2EFC;
    private const int PaintingCount = 5;
    private Span<byte> GetPaintingSpan(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, PaintingCount);
        return Data.Slice(Painting + (Paintings3.SIZE * index), Paintings3.SIZE * PaintingCount);
    }

    public Paintings3 GetPainting(int index, bool japanese) => new(GetPaintingSpan(index).ToArray(), japanese);
    public void SetPainting(int index, Paintings3 value) => value.Data.CopyTo(GetPaintingSpan(index));

    public int DaycareOffset => 0x2F9C;
    public int DaycareSlotSize => PokeCrypto.SIZE_3STORED; // mail stored separate from box mons

    public ushort DaycareSeed
    {
        get => ReadUInt16LittleEndian(Data[0x30B4..]);
        set => WriteUInt16LittleEndian(Data[0x30B4..], value);
    }

    public Span<byte> GiftRibbons => Data.Slice(ExternalEventData - 11, 11);
    public int ExternalEventData => 0x311B;
    public Memory<byte> RoamerData => Raw.Slice(0x3144, Roamer3.SIZE);

    private const int OFFSET_EBERRY = 0x3160;
    private const int SIZE_EBERRY = 0x530;
    public Span<byte> EReaderBerry => Data.Slice(OFFSET_EBERRY, SIZE_EBERRY);

    private Span<byte> MysterySpan => Data.Slice(0x3690, MysteryEvent3.SIZE);
    public Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3(MysterySpan.ToArray());
        set => value.Data.CopyTo(MysterySpan);
    }

    private Span<byte> RecordSpan => Data.Slice(0x3A7C, RecordMixing3Gift.SIZE);
    public RecordMixing3Gift RecordMixingGift
    {
        get => new(RecordSpan.ToArray());
        set => value.Data.CopyTo(RecordSpan);
    }

    public int SeenOffset3 => 0x3A8C;
}
