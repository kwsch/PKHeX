using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores mystery gift OneDay information.
/// </summary>
/// <remarks>size ???, struct_name CONTEST_DATA</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MysteryBlock8b : SaveBlock<SAV8BS>
{
    public MysteryBlock8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public const int RecvDataMax = 50;
    public const int OneDayMax = 10;
    public const int SerialDataNoMax = 895;
    public const int ReserveSize = 66;
    public const int FlagSize = 0x100;
    public const int FlagMax = 2048; // 0x100 * 8 bitflags

    public const int OFS_RECV = 0;
    public const int OFS_RECVFLAG = OFS_RECV + (RecvDataMax * RecvData8b.SIZE); // 0x2BC0
    public const int OFS_ONEDAY = OFS_RECVFLAG + FlagSize; // 0x2CC0
    public const int OFS_SERIALLOCK = OFS_ONEDAY + (OneDayMax * OneDay8b.SIZE); // 0x2D60

    // Structure:
    // RecvData[50] recvDatas;
    // byte[0x100] receiveFlag;
    // OneDayData[10] oneDayDatas;
    // long serialLockTimestamp;
    //  Runtime only: bool ngFlag; -- lockout of Serial Gifts
    //  Runtime only: byte ngCounter; -- count of bad Serials entered
    // ushort[66] reserved_ushorts;
    // uint[66] reserve;

    private int GetRecvDataOffset(int index)
    {
        if ((uint)index >= RecvDataMax)
            throw new ArgumentOutOfRangeException(nameof(index));
        return Offset + OFS_RECV + (index * RecvData8b.SIZE);
    }
    private int GetFlagOffset(int flag)
    {
        if ((uint)flag >= FlagMax)
            throw new ArgumentOutOfRangeException(nameof(flag));
        return Offset + OFS_RECVFLAG + (flag >> 8);
    }
    private int GetOneDayOffset(int index)
    {
        if ((uint)index >= OneDayMax)
            throw new ArgumentOutOfRangeException(nameof(index));
        return Offset + OFS_ONEDAY + (index * OneDay8b.SIZE);
    }

    public RecvData8b GetReceived(int index) => new(Data, GetRecvDataOffset(index));
    public OneDay8b GetOneDay(int index) => new(Data, GetOneDayOffset(index));
    public bool GetFlag(int flag) => FlagUtil.GetFlag(Data, GetFlagOffset(flag), flag & 7);
    public void SetFlag(int flag, bool value) => FlagUtil.SetFlag(Data, GetFlagOffset(flag), flag & 7, value);
    public void SetReceived(int index, RecvData8b data) => data.CopyTo(Data, GetRecvDataOffset(index));
    public void SetOneDay(int index, OneDay8b data) => data.CopyTo(Data, GetOneDayOffset(index));

    public long TicksSerialLock { get => ReadInt64LittleEndian(Data.AsSpan(Offset + OFS_SERIALLOCK)); set => WriteInt64LittleEndian(Data.AsSpan(Offset + OFS_SERIALLOCK), value); }
    public DateTime TimestampSerialLock { get => DateTime.FromFileTimeUtc(TicksSerialLock); set => TicksSerialLock = value.ToFileTimeUtc(); }
    public DateTime LocalTimestampSerialLock { get => TimestampSerialLock.ToLocalTime(); set => TimestampSerialLock = value.ToUniversalTime(); }
    public void ResetLock() => TicksSerialLock = 0;

    public List<int> ReceivedFlagIndexes()
    {
        var result = new List<int>();
        for (int i = 0; i < FlagSize; i++)
        {
            if (GetFlag(i))
                result.Add(i);
        }
        return result;
    }

    #region Received Array
    public RecvData8b[] Received
    {
        get => GetReceived();
        set => SetReceived(value);
    }

    private RecvData8b[] GetReceived()
    {
        var result = new RecvData8b[RecvDataMax];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetReceived(i);
        return result;
    }
    private void SetReceived(IReadOnlyList<RecvData8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, RecvDataMax);
        for (int i = 0; i < value.Count; i++)
            SetReceived(i, value[i]);
    }
    #endregion

    #region Flag Array
    public bool[] ReceivedFlags
    {
        get => GetFlags();
        set => SetFlags(value);
    }

    private bool[] GetFlags()
    {
        var result = new bool[FlagSize];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetFlag(i);
        return result;
    }
    private void SetFlags(IReadOnlyList<bool> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, FlagSize);
        for (int i = 0; i < value.Count; i++)
            SetFlag(i, value[i]);
    }
    #endregion

    #region OneDay Array
    public OneDay8b[] OneDay
    {
        get => GetOneDay();
        set => SetOneDay(value);
    }

    private OneDay8b[] GetOneDay()
    {
        var result = new OneDay8b[OneDayMax];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetOneDay(i);
        return result;
    }

    private void SetOneDay(IReadOnlyList<OneDay8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, OneDayMax);
        for (int i = 0; i < value.Count; i++)
            SetOneDay(i, value[i]);
    }
    #endregion
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class RecvData8b(byte[] Data, int Offset = 0)
{
    public const int SIZE = 0xE0;
    // private const int ItemCount = 7;
    // private const int DressIDCount = 7;

    public override string ToString() => $"{DeliveryID:0000} @ {LocalTimestamp:F}";
    public void CopyTo(Span<byte> destination, int offset1) => Data.AsSpan(Offset, SIZE).CopyTo(destination[offset1..]);
    public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

    public long Ticks { get => ReadInt64LittleEndian(Data.AsSpan(Offset)); set => WriteInt64LittleEndian(Data.AsSpan(Offset), value); }
    public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
    public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }

    public ushort DeliveryID { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x8)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x8), value); }
    public ushort TextID { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xA)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xA), value); }
    public byte DataType { get => Data[Offset + 0xC]; set => Data[Offset + 0xC] = value; }
    public byte ReservedByte1 { get => Data[Offset + 0xD]; set => Data[Offset + 0xD] = value; }
    public short ReservedShort1 { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0xE)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0xE), value); }

    #region MonsData: 0x30 Bytes
    public ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x10), value); }
    public ushort Form    { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), value); }
    public ushort HeldItem{ get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), value); }
    public ushort Move1   { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x16), value); }
    public ushort Move2   { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x18), value); }
    public ushort Move3   { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1A), value); }
    public ushort Move4   { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1C)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1C), value); }

    public string OT
    {
        get => StringConverter8.GetString(Data.AsSpan(Offset + 0x1E, 0x1A));
        set => StringConverter8.SetString(Data.AsSpan(Offset + 0x1E, 0x1A), value, 12, StringConverterOption.ClearZero);
    }

    public byte OT_Gender { get => Data[Offset + 0x38]; set => Data[Offset + 0x38] = value; }
    public byte IsEgg     { get => Data[Offset + 0x39]; set => Data[Offset + 0x39] = value; }
    public byte TwoRibbon { get => Data[Offset + 0x3A]; set => Data[Offset + 0x3A] = value; }
    public byte Gender    { get => Data[Offset + 0x3B]; set => Data[Offset + 0x3B] = value; }
    public byte Language  { get => Data[Offset + 0x3C]; set => Data[Offset + 0x3C] = value; }
    // 0x3D 0x3E 0x3F reserved
    #endregion

    public ushort Item1      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x40)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x40), value); }
    public ushort Item2      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x50)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x50), value); }
    public ushort Item3      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x60)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x60), value); }
    public ushort Item4      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x70)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x70), value); }
    public ushort Item5      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x80)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x80), value); }
    public ushort Item6      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x90)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x90), value); }
    public ushort Item7      { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xA0)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xA0), value); }
    public ushort Item1Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x42)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x42), value); }
    public ushort Item2Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x52)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x52), value); }
    public ushort Item3Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x62)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x62), value); }
    public ushort Item4Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x72)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x72), value); }
    public ushort Item5Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x82)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x82), value); }
    public ushort Item6Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x92)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x92), value); }
    public ushort Item7Count { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xA2)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xA2), value); }
    // 0xC reserved for each item index

    public uint DressID1 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xB0)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xB0), value); }
    public uint DressID2 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xB4)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xB4), value); }
    public uint DressID3 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xB8)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xB8), value); }
    public uint DressID4 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xBC)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xBC), value); }
    public uint DressID5 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xC0)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xC0), value); }
    public uint DressID6 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xC4)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xC4), value); }
    public uint DressID7 { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xC8)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xC8), value); }

    public uint MoneyData { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0xCC)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0xCC), value); }
    public int Reserved1  { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0xD0)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0xD0), value); }
    public int Reserved2  { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0xD4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0xD4), value); }
    public int Reserved3  { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0xD8)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0xD8), value); }
    public int Reserved4  { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0xDC)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0xDC), value); }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class OneDay8b(byte[] Data, int Offset = 0)
{
    public const int SIZE = 0x10;

    public override string ToString() => $"{DeliveryID:0000} @ {LocalTimestamp:F}";

    public void CopyTo(Span<byte> destination, int offset1) => Data.AsSpan(Offset, SIZE).CopyTo(destination[offset1..]);
    public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

    public long Ticks { get => ReadInt64LittleEndian(Data.AsSpan(Offset)); set => WriteInt64LittleEndian(Data.AsSpan(Offset), value); }
    public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
    public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }

    public ushort DeliveryID { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x8)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x8), value); }
    public short Reserved1 { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0xA)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0xA), value); }
    public short Reserved2 { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0xC)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0xC), value); }
    public short Reserved3 { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0xE)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0xE), value); }
}
