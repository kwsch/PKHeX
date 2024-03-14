using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Details about the Console and specific timestamps.
/// </summary>
/// <remarks>size: 0x138</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SystemData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    // Structure:
    // (u32 count, u64 FILETIME) Start Time
    // (u32 count, u64 FILETIME) Latest Save Time
    // (u32 count, u64 FILETIME) Penalty Timeout Time
    // (u32 count, u64 FILETIME) Last Daily Event Time
    // byte[208] ClockSnapshot (char[0x24])
    // u32 "fd_bgmEvnet"
    // s64[6] reserved
    private const int SIZE_GMTIME = 12;
    private const int SIZE_SNAPSHOT = 0xD0;

    private const int OFS_SNAPSHOT = 4 + (3 * SIZE_GMTIME) + SIZE_GMTIME; // 0x34
    private const int OFS_FDBGM = OFS_SNAPSHOT + SIZE_SNAPSHOT;
    private const int OFS_RESERVED = OFS_FDBGM + 4;
    internal const int SIZE = OFS_RESERVED + (6 * 8); // 0x138

    public uint CountStart   { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public long TicksStart   { get =>  ReadInt64LittleEndian(Data[(4 + (0 * SIZE_GMTIME))..]); set =>  WriteInt64LittleEndian(Data[(4 + (0 * SIZE_GMTIME))..], value); }
    public uint CountLatest  { get => ReadUInt32LittleEndian(Data[(0 + (1 * SIZE_GMTIME))..]); set => WriteUInt32LittleEndian(Data[(0 + (1 * SIZE_GMTIME))..], value); }
    public long TicksLatest  { get =>  ReadInt64LittleEndian(Data[(4 + (1 * SIZE_GMTIME))..]); set =>  WriteInt64LittleEndian(Data[(4 + (1 * SIZE_GMTIME))..], value); }
    public uint CountPenalty { get => ReadUInt32LittleEndian(Data[(0 + (2 * SIZE_GMTIME))..]); set => WriteUInt32LittleEndian(Data[(0 + (2 * SIZE_GMTIME))..], value); }
    public long TicksPenalty { get =>  ReadInt64LittleEndian(Data[(4 + (2 * SIZE_GMTIME))..]); set =>  WriteInt64LittleEndian(Data[(4 + (2 * SIZE_GMTIME))..], value); }
    public uint CountDaily   { get => ReadUInt32LittleEndian(Data[(0 + (3 * SIZE_GMTIME))..]); set => WriteUInt32LittleEndian(Data[(0 + (3 * SIZE_GMTIME))..], value); }
    public long TicksDaily   { get =>  ReadInt64LittleEndian(Data[(4 + (3 * SIZE_GMTIME))..]); set =>  WriteInt64LittleEndian(Data[(4 + (3 * SIZE_GMTIME))..], value); }

    // byte[] nxSnapshot
    // u32 fd_bgmEvnet

    // THESE ARE IN UTC
    public DateTime TimestampStart   { get => DateTime.FromFileTimeUtc(TicksStart);   set => TicksStart   = value.ToFileTimeUtc(); }
    public DateTime TimestampLatest  { get => DateTime.FromFileTimeUtc(TicksLatest);  set => TicksLatest  = value.ToFileTimeUtc(); }
    public DateTime TimestampPenalty { get => DateTime.FromFileTimeUtc(TicksPenalty); set => TicksPenalty = value.ToFileTimeUtc(); }
    public DateTime TimestampDaily   { get => DateTime.FromFileTimeUtc(TicksDaily);   set => TicksDaily   = value.ToFileTimeUtc(); }

    public DateTime LocalTimestampStart   { get => TimestampStart  .ToLocalTime(); set => TimestampStart   = value.ToUniversalTime(); }
    public DateTime LocalTimestampLatest  { get => TimestampLatest .ToLocalTime(); set => TimestampLatest  = value.ToUniversalTime(); }
    public DateTime LocalTimestampPenalty { get => TimestampPenalty.ToLocalTime(); set => TimestampPenalty = value.ToUniversalTime(); }
    public DateTime LocalTimestampDaily   { get => TimestampDaily  .ToLocalTime(); set => TimestampDaily   = value.ToUniversalTime(); }

    public string LastSavedTime
    {
        get
        {
            var stamp = LocalTimestampLatest;
            return $"{stamp.Year:0000}-{stamp.Month:00}-{stamp.Day:00} {stamp.Hour:00}ː{stamp.Minute:00}ː{stamp.Second:00}"; // not :
        }
    }
}
