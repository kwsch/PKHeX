using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    public sealed class RaidSpawnList8 : SaveBlock
    {
        public readonly int CountAll;
        public readonly int CountUsed;

        public RaidSpawnList8(SaveFile sav, SCBlock block, int legal) : base(sav, block.Data)
        {
            CountAll = block.Data.Length / RaidSpawnDetail.SIZE;
            CountUsed = legal;
        }

        public const int RaidCountLegal_O0 = 100;
        public const int RaidCountLegal_R1 = 90;
        public const int RaidCountLegal_R2 = 86;

        public RaidSpawnDetail GetRaid(int entry) => new(Data, entry * RaidSpawnDetail.SIZE);

        public RaidSpawnDetail[] GetAllRaids()
        {
            var result = new RaidSpawnDetail[CountAll];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetRaid(i);
            return result;
        }

        public void DectivateAllRaids()
        {
            for (int i = 0; i < CountUsed; i++)
            {
                if (i == 16) // Watchtower, special
                    continue;
                GetRaid(i).Deactivate();
            }
        }

        public void ActivateAllRaids(bool rare, bool isEvent)
        {
            var rnd = Util.Rand;
            for (int i = 0; i < CountUsed; i++)
            {
                if (i == 16) // Watchtower, special
                    continue;
                var star = (byte)rnd.Next(0, 5);
                var rand = (byte)rnd.Next(1, 101);
                GetRaid(i).Activate(star, rand, rare, isEvent);
            }
        }

        public string[] DumpAll()
        {
            var raids = GetAllRaids();
            var result = new string[CountAll];
            for (int i = 0; i < result.Length; i++)
                result[i] = raids[i].Dump();
            return result;
        }
    }

    public sealed class RaidSpawnDetail
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

        [Category(General), Description("Random value which picks out the encounter from the Raid data table (1-100).")]
        public byte RandRoll
        {
            get => Data[Offset + 0x11];
            set => Data[Offset + 0x11] = value;
        }

        [Category(General), Description("First set of Den Flags.")]
        public RaidType DenType
        {
            get => (RaidType)Data[Offset + 0x12];
            set
            {
                Data[Offset + 0x12] = (byte)value;
                if (value == RaidType.Event)
                {
                    IsEvent = true;
                }
                else if (value != RaidType.CommonWish)
                {
                    IsEvent = false;
                }
            }
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
            get => DenType is RaidType.Rare or RaidType.RareWish;
            set
            {
                if (value)
                {
                    DenType = IsWishingPiece ? RaidType.RareWish : RaidType.Rare;
                }
                else
                {
                    DenType = IsWishingPiece ? RaidType.CommonWish : RaidType.Common;
                }
            }
        }

        [Category(Derived), Description("Wishing Piece was used for Raid encounter.")]
        public bool IsWishingPiece
        {
            get => DenType is RaidType.CommonWish or RaidType.RareWish;
            set
            {
                if (value)
                {
                    DenType = IsRare ? RaidType.RareWish : RaidType.CommonWish;
                }
                else
                {
                    DenType = IsRare ? RaidType.Rare : RaidType.Common;
                }
            }
        }

        [Category(Derived), Description("Has watts already been harvested.")]
        public bool WattsHarvested
        {
            get => IsActive && (Flags & 1) == 1;
            set => Flags = (byte)((Flags & ~1) | (value ? 1 : 0));
        }

        [Category(Derived), Description("Distribution (event) details used for Raid encounter.")]
        public bool IsEvent
        {
            get => IsActive && (Flags & 2) == 2;
            set
            {
                Flags = (byte)((Flags & ~2) | (value ? 2 : 0));
                if (value)
                {
                    if (DenType != RaidType.CommonWish && DenType != RaidType.Event)
                    {
                        DenType = RaidType.Event;
                    }
                }
                else
                {
                    if (DenType == RaidType.Event)
                    {
                        DenType = RaidType.Common;
                    }
                }
            }
        }

        public void Activate(byte star, byte rand, bool rare = false, bool isEvent = false)
        {
            Stars = star;
            RandRoll = rand;
            IsRare = rare;
            IsEvent = isEvent;
        }

        public void Deactivate()
        {
            DenType = RaidType.None;
            Stars = 0;
            RandRoll = 0;
        }

        public string Dump() => $"{Hash:X16}\t{Seed:X16}\t{Stars}\t{RandRoll:00}\t{DenType}\t{Flags:X2}";

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
        DynamaxCrystal = 6,
    }
}
