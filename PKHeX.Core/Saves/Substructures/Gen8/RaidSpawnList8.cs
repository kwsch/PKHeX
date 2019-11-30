using System;

namespace PKHeX.Core
{
    public sealed class RaidSpawnList8 : SaveBlock
    {
        public RaidSpawnList8(SaveFile sav, SCBlock block) : base(sav, block.Data) { }

        private const int RaidCount = 111;

        public RaidSpawnDetail GetRaid(int entry) => new RaidSpawnDetail(Data, entry * RaidSpawnDetail.SIZE);

        public RaidSpawnDetail[] GetAllRaids()
        {
            var result = new RaidSpawnDetail[RaidCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetRaid(i);
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

        public ulong Hash => BitConverter.ToUInt64(Data, Offset + 0);
        public ulong Seed => BitConverter.ToUInt64(Data, Offset + 8);

        public byte Stars => Data[Offset + 0x10];
        public byte RandRoll => Data[Offset + 0x11];
        public byte DenType => Data[Offset + 0x12];
        public byte Flags => Data[Offset + 0x13];

        // The games use a xoroshiro RNG to create the PKM from the stored seed.
    }
}