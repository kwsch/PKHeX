using System;

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
            for (int i = 0; i < RaidCount; i++)
            {
                var star = (byte)Util.Rand.Next(0, 5);
                var rand = (byte)Util.Rand.Next(0, 100);
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

        public ulong Hash
        {
            get => BitConverter.ToUInt64(Data, Offset + 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0);
        }

        public ulong Seed
        {
            get => BitConverter.ToUInt64(Data, Offset + 8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 8);
        }

        public byte Stars
        {
            get => Data[Offset + 0x10];
            set => Data[Offset + 0x10] = value;
        }

        public byte RandRoll
        {
            get => Data[Offset + 0x11];
            set => Data[Offset + 0x11] = value;
        }

        public byte DenType
        {
            get => Data[Offset + 0x12];
            set => Data[Offset + 0x12] = value;
        }

        public byte Flags
        {
            get => Data[Offset + 0x13];
            set => Data[Offset + 0x13] = value;
        }

        public bool IsActive => DenType > 0;

        public bool IsRare
        {
            get => IsActive && (DenType & 1) != 0;
            set => DenType = 2;
        }

        public bool IsEvent
        {
            get => IsActive && ((Flags >> 1) & 1) == 1;
            set => Flags = (byte)((Flags & 2) | (value ? 2 : 0));
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
}