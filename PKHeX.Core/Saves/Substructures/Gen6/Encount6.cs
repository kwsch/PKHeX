using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Swarm and other overworld info
    /// </summary>
    public sealed class Encount6 : SaveBlock
    {
        public Encount6(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        public ushort RepelItemUsed { get => BitConverter.ToUInt16(Data, Offset + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00); }
        public byte RepelSteps { get => Data[Offset + 0x02]; set => Data[Offset + 0x02] = value; }

        // 0x04

        public PokeRadar6 Radar
        {
            get => new(Data.Slice(Offset + 0x04, PokeRadar6.SIZE));
            set => value.Data.CopyTo(Data, Offset + 0x04);
        }

        // 0x1C

        public Roamer6 Roamer
        {
            get => new(Data.Slice(Offset + 0x1C, Roamer6.SIZE));
            set => value.Data.CopyTo(Data, Offset + 0x1C);
        }

        // 0x44

        // 4 bytes at end??
    }

    [TypeConverter(typeof(ValueTypeTypeConverter))]
    public sealed class PokeRadar6
    {
        public const int SIZE = 2 + (RecordCount * PokeRadarRecord.SIZE); // 0x18

        private const int MaxCharge = 50;
        private const int RecordCount = 5;

        public readonly byte[] Data;

        public PokeRadar6(byte[] data) => Data = data;
        public override string ToString() => ((Species)PokeRadarSpecies).ToString();

        public ushort PokeRadarSpecies { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        private ushort PokeRadarPacked { get => BitConverter.ToUInt16(Data, 0x02); set => BitConverter.GetBytes(value).CopyTo(Data, 0x02); }

        public int PokeRadarCharge { get => PokeRadarPacked & 0x3FFF; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~0x3FFF) | Math.Min(MaxCharge, value)); }
        public bool PokeRadarFlag1 { get => PokeRadarPacked >> 14 != 0; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~(1 << 14)) | (value ? (1 << 14) : 0)); }
        public bool PokeRadarFlag2 { get => PokeRadarPacked >> 15 != 0; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~(1 << 15)) | (value ? (1 << 15) : 0)); }

        public PokeRadarRecord GetRecord(int index) => PokeRadarRecord.ReadRecord(Data, GetRecordOffset(index));
        public void SetRecord(PokeRadarRecord record, int index) => record.WriteRecord(Data, GetRecordOffset(index));

        private static int GetRecordOffset(int index)
        {
            if ((uint) index >= RecordCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            return 6 + (index * 2);
        }

        public PokeRadarRecord Record1 { get => GetRecord(0); set => SetRecord(value, 0); }
        public PokeRadarRecord Record2 { get => GetRecord(1); set => SetRecord(value, 1); }
        public PokeRadarRecord Record3 { get => GetRecord(2); set => SetRecord(value, 2); }
        public PokeRadarRecord Record4 { get => GetRecord(3); set => SetRecord(value, 3); }
        public PokeRadarRecord Record5 { get => GetRecord(4); set => SetRecord(value, 4); }
    }

    [TypeConverter(typeof(ValueTypeTypeConverter))]
    public sealed class PokeRadarRecord
    {
        public const int SIZE = 4;
        public override string ToString() => ((Species)Species).ToString();

        public ushort Species { get; set; }
        public ushort Count { get; set; }

        private PokeRadarRecord(ushort species, ushort count)
        {
            Species = species;
            Count = count;
        }

        public static PokeRadarRecord ReadRecord(byte[] data, int ofs)
        {
            var species = BitConverter.ToUInt16(data, ofs);
            var count = BitConverter.ToUInt16(data, ofs + 2);
            return new PokeRadarRecord(species, count);
        }

        public void WriteRecord(byte[] data, int ofs)
        {
            BitConverter.GetBytes(Species).CopyTo(data, ofs);
            BitConverter.GetBytes(Count).CopyTo(data, ofs + 2);
        }
    }

    [TypeConverter(typeof(ValueTypeTypeConverter))]
    public sealed class Roamer6
    {
        public const int SIZE = 0x28;

        public readonly byte[] Data;

        public Roamer6(byte[] data) => Data = data;
        public override string ToString() => ((Species)Species).ToString();

        private ushort SpecForm { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public int Species { get => SpecForm & 0x3FF; set => SpecForm = (ushort)((SpecForm & ~0x3FF) | (value & 0x3FF)); }
        public bool Flag1 { get => SpecForm >> 14 != 0; set => SpecForm = (ushort)((SpecForm & 0xBFFF) | (value ? (1 << 14) : 0)); }
        public bool Flag2 { get => SpecForm >> 15 != 0; set => SpecForm = (ushort)((SpecForm & 0x7FFF) | (value ? (1 << 15) : 0)); }

        public int CurrentLevel { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        private int Status { get => Data[0x07]; set => Data[0x07] = (byte)value; }
        public Roamer6State RoamStatus { get => (Roamer6State)((Status >> 4) & 0xF); set => Status = (Status & 0x0F) | (((int)value << 4) & 0xF0); }

        public uint TimesEncountered { get => BitConverter.ToUInt32(Data, 0x24); set => BitConverter.GetBytes(value).CopyTo(Data, 0x24); }
    }

    public enum Roamer6State
    {
        Inactive,
        Roaming,
        Stationary,
        Defeated,
        Captured,
    }
}
