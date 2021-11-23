using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Details about the Poketch corner app.
    /// </summary>
    /// <remarks>size: ???</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SystemData8b : SaveBlock
    {
        // Structure:
        // (u32 count, u64 FILETIME) Start Time
        // (u32 count, u64 FILETIME) Latest Save Time
        // (u32 count, u64 FILETIME) Penalty Timeout Time
        // (u32 count, u64 FILETIME) Last Daily Event Time
        // byte[???] Snapshot
        // u32 "fd_bgmEvnet"
        // s64[6] reserved
        private const int SIZE_GMTIME = 12;

        public SystemData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public uint CountStart   { get => BitConverter.ToUInt32(Data, Offset + 0 + (0 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0 + (0 * SIZE_GMTIME)); }
        public long TicksStart   { get => BitConverter. ToInt64(Data, Offset + 4 + (0 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4 + (0 * SIZE_GMTIME)); }
        public uint CountLatest  { get => BitConverter.ToUInt32(Data, Offset + 0 + (1 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0 + (1 * SIZE_GMTIME)); }
        public long TicksLatest  { get => BitConverter. ToInt64(Data, Offset + 4 + (1 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4 + (1 * SIZE_GMTIME)); }
        public uint CountPenalty { get => BitConverter.ToUInt32(Data, Offset + 0 + (2 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0 + (2 * SIZE_GMTIME)); }
        public long TicksPenalty { get => BitConverter. ToInt64(Data, Offset + 4 + (2 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4 + (2 * SIZE_GMTIME)); }
        public uint CountDaily   { get => BitConverter.ToUInt32(Data, Offset + 0 + (3 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0 + (3 * SIZE_GMTIME)); }
        public long TicksDaily   { get => BitConverter. ToInt64(Data, Offset + 4 + (3 * SIZE_GMTIME)); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4 + (3 * SIZE_GMTIME)); }

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
}
