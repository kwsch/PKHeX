using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores mystery gift OneDay information.
    /// </summary>
    /// <remarks>size ???, struct_name CONTEST_DATA</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class MysteryBlock8b : SaveBlock
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

        public long TicksSerialLock { get => BitConverter.ToInt64(Data, Offset + OFS_SERIALLOCK); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SERIALLOCK); }
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
            if (value.Count != RecvDataMax)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
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
            if (value.Count != FlagSize)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
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
            if (value.Count != OneDayMax)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            for (int i = 0; i < value.Count; i++)
                SetOneDay(i, value[i]);
        }
        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class RecvData8b
    {
        public const int SIZE = 0xE0;
        // private const int ItemCount = 7;
        // private const int DressIDCount = 7;

        private readonly int Offset;
        private readonly byte[] Data;

        public RecvData8b(byte[] data, int offset = 0)
        {
            Data = data;
            Offset = offset;
        }

        public override string ToString() => $"{DeliveryID:0000} @ {LocalTimestamp:F}";
        public void CopyTo(byte[] data, int offset) => Data.AsSpan(Offset, SIZE).CopyTo(data.AsSpan(offset));
        public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

        public long Ticks { get => BitConverter.ToInt64(Data, Offset); set => BitConverter.GetBytes(value).CopyTo(Data, Offset); }
        public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
        public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }

        public ushort DeliveryID { get => BitConverter.ToUInt16(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); }
        public ushort TextID { get => BitConverter.ToUInt16(Data, Offset + 0xA); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xA); }
        public byte DataType { get => Data[Offset + 0xC]; set => Data[Offset + 0xC] = value; }
        public byte ReservedByte1 { get => Data[Offset + 0xD]; set => Data[Offset + 0xD] = value; }
        public short ReservedShort1 { get => BitConverter.ToInt16(Data, Offset + 0xE); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xE); }

        #region MonsData: 0x30 Bytes
        public ushort Species { get => BitConverter.ToUInt16(Data, Offset + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10); }
        public ushort Form    { get => BitConverter.ToUInt16(Data, Offset + 0x12); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x12); }
        public ushort HeldItem{ get => BitConverter.ToUInt16(Data, Offset + 0x14); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14); }
        public ushort Move1   { get => BitConverter.ToUInt16(Data, Offset + 0x16); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x16); }
        public ushort Move2   { get => BitConverter.ToUInt16(Data, Offset + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18); }
        public ushort Move3   { get => BitConverter.ToUInt16(Data, Offset + 0x1A); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1A); }
        public ushort Move4   { get => BitConverter.ToUInt16(Data, Offset + 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C); }

        public string OT
        {
            get => StringConverter.GetString7b(Data, Offset + 0x1E, 0x1A);
            set => StringConverter.SetString7b(value, 12, 12).CopyTo(Data, Offset + 0x1E);
        }

        public byte OT_Gender { get => Data[Offset + 0x38]; set => Data[Offset + 0x38] = value; }
        public byte IsEgg     { get => Data[Offset + 0x39]; set => Data[Offset + 0x39] = value; }
        public byte TwoRibbon { get => Data[Offset + 0x3A]; set => Data[Offset + 0x3A] = value; }
        public byte Gender    { get => Data[Offset + 0x3B]; set => Data[Offset + 0x3B] = value; }
        public byte Language  { get => Data[Offset + 0x3C]; set => Data[Offset + 0x3C] = value; }
        // 0x3D 0x3E 0x3F reserved
        #endregion

        public ushort Item1 { get => BitConverter.ToUInt16(Data, Offset + 0x40); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x40); }
        public ushort Item2 { get => BitConverter.ToUInt16(Data, Offset + 0x50); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x50); }
        public ushort Item3 { get => BitConverter.ToUInt16(Data, Offset + 0x60); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x60); }
        public ushort Item4 { get => BitConverter.ToUInt16(Data, Offset + 0x70); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x70); }
        public ushort Item5 { get => BitConverter.ToUInt16(Data, Offset + 0x80); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x80); }
        public ushort Item6 { get => BitConverter.ToUInt16(Data, Offset + 0x90); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x90); }
        public ushort Item7 { get => BitConverter.ToUInt16(Data, Offset + 0xA0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xA0); }
        public ushort Item1Count { get => BitConverter.ToUInt16(Data, Offset + 0x42); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x42); }
        public ushort Item2Count { get => BitConverter.ToUInt16(Data, Offset + 0x52); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x52); }
        public ushort Item3Count { get => BitConverter.ToUInt16(Data, Offset + 0x62); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x62); }
        public ushort Item4Count { get => BitConverter.ToUInt16(Data, Offset + 0x72); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x72); }
        public ushort Item5Count { get => BitConverter.ToUInt16(Data, Offset + 0x82); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x82); }
        public ushort Item6Count { get => BitConverter.ToUInt16(Data, Offset + 0x92); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x92); }
        public ushort Item7Count { get => BitConverter.ToUInt16(Data, Offset + 0xA2); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xA2); }
        // 0xC reserved for each item index

        public uint DressID1 { get => BitConverter.ToUInt32(Data, Offset + 0xB0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xB0); }
        public uint DressID2 { get => BitConverter.ToUInt32(Data, Offset + 0xB4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xB4); }
        public uint DressID3 { get => BitConverter.ToUInt32(Data, Offset + 0xB8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xB8); }
        public uint DressID4 { get => BitConverter.ToUInt32(Data, Offset + 0xBC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xBC); }
        public uint DressID5 { get => BitConverter.ToUInt32(Data, Offset + 0xC0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC0); }
        public uint DressID6 { get => BitConverter.ToUInt32(Data, Offset + 0xC4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC4); }
        public uint DressID7 { get => BitConverter.ToUInt32(Data, Offset + 0xC8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC8); }

        public uint MoneyData { get => BitConverter.ToUInt32(Data, Offset + 0xCC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xCC); }
        public int Reserved1 { get => BitConverter.ToInt32(Data, Offset + 0xD0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xD0); }
        public int Reserved2 { get => BitConverter.ToInt32(Data, Offset + 0xD4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xD4); }
        public int Reserved3 { get => BitConverter.ToInt32(Data, Offset + 0xD8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xD8); }
        public int Reserved4 { get => BitConverter.ToInt32(Data, Offset + 0xDC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xDC); }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class OneDay8b
    {
        public const int SIZE = 0x10;

        private readonly int Offset;
        private readonly byte[] Data;

        public OneDay8b(byte[] data, int offset = 0)
        {
            Data = data;
            Offset = offset;
        }

        public override string ToString() => $"{DeliveryID:0000} @ {LocalTimestamp:F}";

        public void CopyTo(byte[] data, int offset) => Data.AsSpan(Offset, SIZE).CopyTo(data.AsSpan(offset));
        public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

        public long Ticks { get => BitConverter.ToInt64(Data, Offset); set => BitConverter.GetBytes(value).CopyTo(Data, Offset); }
        public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
        public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }

        public ushort DeliveryID { get => BitConverter.ToUInt16(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); }
        public short Reserved1 { get => BitConverter.ToInt16(Data, Offset + 0xA); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xA); }
        public short Reserved2 { get => BitConverter.ToInt16(Data, Offset + 0xC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC); }
        public short Reserved3 { get => BitConverter.ToInt16(Data, Offset + 0xE); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xE); }
    }
}
