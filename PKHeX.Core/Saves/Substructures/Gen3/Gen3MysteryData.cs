using System;

namespace PKHeX.Core
{
    public abstract class Gen3MysteryData
    {
        public readonly byte[] Data;

        protected Gen3MysteryData(byte[] data) => Data = data;

        public uint Checksum
        {
            get => BitConverter.ToUInt32(Data, 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0);
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
