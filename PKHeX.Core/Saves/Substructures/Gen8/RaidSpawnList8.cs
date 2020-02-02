using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    public sealed class RaidSpawnList8 : SaveBlock
    {
        public RaidSpawnList8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        public const int RaidCount = 111;

        public RaidSpawnDetail GetRaid(int entry) => new RaidSpawnDetail(Data, entry * RaidSpawnDetail.SIZE);

        public RaidSpawnDetail[] GetAllRaids()
        {
            var result = new RaidSpawnDetail[RaidCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetRaid(i);
            return result;
        }

        public void ActivateAllRaids(bool rare, bool isEvent)
        {
            var rnd = Util.Rand;
            for (int i = 0; i < RaidCount; i++)
            {
                var star = (byte)rnd.Next(0, 5);
                var rand = (byte)rnd.Next(0, 100);
                GetRaid(i).Activate(star, rand, rare, isEvent);
            }
        }

        public string[] DumpAll()
        {
            var raids = GetAllRaids();
            var result = new string[RaidCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = raids[i].Dump();
            return result;
        }
    }

    public class RaidSpawnDetail
    {
        public const int SIZE = 0x18;

        private readonly byte[] Data;
        private readonly int Offset;

        public RaidSpawnDetail(byte[] data, int ofs)
        {
            Data = data;
            Offset = ofs;
        }

        private const string General = nameof(General);
        private const string Derived = nameof(Derived);

        [Category(General), Description("FNV Hash for fetching the Raid data table (64bit)."), TypeConverter(typeof(TypeConverterU64))]
        public ulong Hash
        {
            get => BitConverter.ToUInt64(Data, Offset + 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0);
        }

        [Category(General), Description("RNG Seed for generating the Raid's content (64bit)."), TypeConverter(typeof(TypeConverterU64))]
        public ulong Seed
        {
            get => BitConverter.ToUInt64(Data, Offset + 8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 8);
        }

        [Category(General), Description("Star Count for the Raid's content (0-4).")]
        public byte Stars
        {
            get => Data[Offset + 0x10];
            set => Data[Offset + 0x10] = value;
        }

        [Category(General), Description("Random value which picks out the encounter from the Raid data table (0-99).")]
        public byte RandRoll
        {
            get => Data[Offset + 0x11];
            set => Data[Offset + 0x11] = value;
        }

        [Category(General), Description("First set of Den Flags.")]
        public byte DenType
        {
            get => Data[Offset + 0x12];
            set => Data[Offset + 0x12] = value;
        }

        [Category(General), Description("Second set of Den Flags.")]
        public byte Flags
        {
            get => Data[Offset + 0x13];
            set => Data[Offset + 0x13] = value;
        }

        [Category(Derived), Description("Active Nest")]
        public bool IsActive => DenType > 0;

        [Category(Derived), Description("Rare encounter details used instead of Common details.")]
        public bool IsRare
        {
            get => IsActive && (DenType & 1) == 0;
            set => DenType = 2; // set the 1th bit; the 2th bit has a similar-unknown function (?)
        }

        [Category(Derived), Description("Wishing Piece was used for Raid encounter.")]
        public bool IsWishingPiece
        {
            get => IsActive && ((Flags >> 0) & 1) == 1;
            set => Flags = (byte)((Flags & ~1) | (value ? 1 : 0));
        }

        [Category(Derived), Description("Distribution (event) details used for Raid encounter.")]
        public bool IsEvent
        {
            get => IsActive && ((Flags >> 1) & 1) == 1;
            set => Flags = (byte)((Flags & ~2) | (value ? 2 : 0));
        }

        public void Activate(byte star, byte rand, bool rare = false, bool isEvent = false)
        {
            Stars = star;
            RandRoll = rand;
            IsRare = rare;
            IsEvent = isEvent;
        }

        public string Dump() => $"{Hash:X16}\t{Seed:X16}\t{Stars}\t{RandRoll:00}\t{DenType:X2}\t{Flags:X2}";

        // The games use a xoroshiro RNG to create the PKM from the stored seed.
    }

    public enum RaidType : byte
    {
        None = 0,
        Common = 1,
        Rare = 2,
        CommonWish = 3,
        RareWish = 4,
        Event = 5,
    }

    public class TypeConverterU64 : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ulong)
                return $"{value:X16}"; // no 0x prefix
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (!(value is string input))
                return base.ConvertFrom(context, culture, value);
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2);
            return ulong.TryParse(input, System.Globalization.NumberStyles.HexNumber, culture, out var result) ? result : 0ul;
        }
    }
}