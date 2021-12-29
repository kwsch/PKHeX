using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public abstract class Gen3MysteryData
    {
        public readonly byte[] Data;

        protected Gen3MysteryData(byte[] data) => Data = data;

        public uint Checksum
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(0));
            set => WriteUInt32LittleEndian(Data.AsSpan(0), value);
        }

        public bool IsChecksumValid() => Checksum == GetChecksum(Data);
        public void FixChecksum() => Checksum = GetChecksum(Data);

        private static uint GetChecksum(byte[] data)
        {
            uint sum = 0;
            for (var i = 4; i < data.Length; i++)
            {
                var b = data[i];
                sum += b;
            }

            return sum;
        }
    }
}
