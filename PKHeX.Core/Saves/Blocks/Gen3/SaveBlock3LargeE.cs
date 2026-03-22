using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record SaveBlock3LargeE(Memory<byte> Raw) : ISaveBlock3LargeExpansion, ISaveBlock3LargeHoenn, IRecordStatStorage<RecID3Emerald, uint>
{
    public Span<byte> Data => Raw.Span;
    public ushort X { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Y { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }

    public byte PartyCount { get => Data[0x234]; set => Data[0x234] = value; }
    public Span<byte> PartyBuffer => Data.Slice(0x238, 6 * PokeCrypto.SIZE_3PARTY);

    public uint Money { get => ReadUInt32LittleEndian(Data[0x0490..]); set => WriteUInt32LittleEndian(Data[0x0490..], value); }
    public ushort Coin { get => ReadUInt16LittleEndian(Data[0x0494..]); set => WriteUInt16LittleEndian(Data[0x0494..], value); }
    public ushort RegisteredItem { get => ReadUInt16LittleEndian(Data[0x0496..]); set => WriteUInt16LittleEndian(Data[0x0496..], value); }
    public Span<byte> Inventory => Data.Slice(0x0498, 0x3B0);
    private Span<byte> PokeBlockData => Data.Slice(0x848, PokeBlock3Case.SIZE);
    public PokeBlock3Case PokeBlocks { get => new(PokeBlockData); set => value.Write(PokeBlockData); }
    public int SeenOffset2 => 0x988;

    private const int OFS_BerryBlenderRecord = 0x9BC;

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

    private const int EventFlag = 0x1270;
    private const int EventWork = 0x139C;
    public int EventFlagCount => 8 * 300;
    public int EventWorkCount => 0x100;
    public int EggEventFlag => 0x86;
    public int BadgeFlagStart => 0x867;

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

    private const int RecordOffset = 0x159C;
    private static int GetRecordOffset(RecID3Emerald record)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)record, (uint)RecID3Emerald.NUM_GAME_STATS);
        return RecordOffset + (sizeof(uint) * (int)record);
    }

    public uint GetRecord(int record) => GetRecord((RecID3Emerald)record);
    public void SetRecord(int record, uint value) => SetRecord((RecID3Emerald)record, value);
    public uint GetRecord(RecID3Emerald record) => ReadUInt32LittleEndian(Data[GetRecordOffset(record)..]);
    public void SetRecord(RecID3Emerald record, uint value) => WriteUInt32LittleEndian(Data[GetRecordOffset(record)..], value);
    public void AddRecord(RecID3Emerald record, uint value) => SetRecord(record, GetRecord(record) + value);

    private Memory<byte> SecretBaseData => Raw.Slice(0x1A9C, SecretBaseManager3.BaseCount * SecretBase3.SIZE);
    public SecretBaseManager3 SecretBases => new(SecretBaseData);

    public DecorationInventory3 Decorations => new(Data.Slice(0x2734, DecorationInventory3.SIZE));

    private Span<byte> SwarmData => Data.Slice(0x2B90, Swarm3.SIZE);
    public Swarm3 Swarm { get => new(SwarmData.ToArray()); set => value.Data.CopyTo(SwarmData); }
    private void ClearSwarm() => SwarmData.Clear();
    public IReadOnlyList<Swarm3> DefaultSwarms => Swarm3Details.Swarms_E;

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

    private const int MailOffset = 0x2BE0;
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

    private const int OFS_TrendyWord = 0x2E20;
    public bool GetTrendyWordUnlocked(TrendyWord3E word) => FlagUtil.GetFlag(Data, OFS_TrendyWord + ((byte)word >> 3), (byte)word & 7);
    public void SetTrendyWordUnlocked(TrendyWord3E word, bool value) => FlagUtil.SetFlag(Data, OFS_TrendyWord + ((byte)word >> 3), (byte)word & 7, value);

    private const int Painting = 0x2F90;
    private const int PaintingCount = 5;
    private Span<byte> GetPaintingSpan(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, PaintingCount);
        return Data.Slice(Painting + (Paintings3.SIZE * index), Paintings3.SIZE * PaintingCount);
    }

    public Paintings3 GetPainting(int index, bool japanese) => new(GetPaintingSpan(index).ToArray(), japanese);
    public void SetPainting(int index, Paintings3 value) => value.Data.CopyTo(GetPaintingSpan(index));

    public int DaycareOffset => 0x3030;
    public int DaycareSlotSize => PokeCrypto.SIZE_3STORED + 0x3C; // 0x38 mail + 4 exp

    public uint DaycareSeed
    {
        get => ReadUInt32LittleEndian(Data[0x3148..]);
        set => WriteUInt32LittleEndian(Data[0x3148..], value);
    }

    public Span<byte> GiftRibbons => Data.Slice(0x31B3, 11);
    public int ExternalEventData => 0x31B3;
    public Memory<byte> RoamerData => Raw.Slice(0x31DC, Roamer3.SIZE);
    private const int OFFSET_EBERRY = 0x31F8;
    private const int SIZE_EBERRY = 0x34;
    public Span<byte> EReaderBerry => Data.Slice(OFFSET_EBERRY, SIZE_EBERRY);

    public const int WonderNewsOffset = 0x322C;

    // RAM Script
    private Span<byte> MysterySpan => Data.Slice(0x3728, MysteryEvent3.SIZE);
    public Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3(MysterySpan.ToArray());
        set => value.Data.CopyTo(MysterySpan);
    }

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

    private const int OFS_TrainerHillRecord = 0x3718;

    /** Each value unit represents 1/60th of a second. Value 0 if no record. */
    public uint GetTrainerHillRecord(TrainerHillMode3E mode) => ReadUInt32LittleEndian(Data[(OFS_TrainerHillRecord + ((byte)mode * 4))..]);
    public void SetTrainerHillRecord(TrainerHillMode3E mode, uint value) => WriteUInt32LittleEndian(Data[(OFS_TrainerHillRecord + ((byte)mode * 4))..], value);

    private Span<byte> RecordMixingData => Data.Slice(0x3B14, RecordMixing3Gift.SIZE);
    public RecordMixing3Gift RecordMixingGift
    {
        get => new(RecordMixingData.ToArray());
        set => value.Data.CopyTo(RecordMixingData);
    }

    public int SeenOffset3 => 0x3B24;

    private const int Walda = 0x3D70;
    public ushort WaldaBackgroundColor { get => ReadUInt16LittleEndian(Data[(Walda + 0)..]); set => WriteUInt16LittleEndian(Data[(Walda + 0)..], value); }
    public ushort WaldaForegroundColor { get => ReadUInt16LittleEndian(Data[(Walda + 2)..]); set => WriteUInt16LittleEndian(Data[(Walda + 2)..], value); }
    public byte WaldaIconID { get => Data[Walda + 0x14]; set => Data[Walda + 0x14] = value; }
    public byte WaldaPatternID { get => Data[Walda + 0x15]; set => Data[Walda + 0x15] = value; }
    public bool WaldaUnlocked { get => Data[Walda + 0x16] != 0; set => Data[Walda + 0x16] = (byte)(value ? 1 : 0); }
}
