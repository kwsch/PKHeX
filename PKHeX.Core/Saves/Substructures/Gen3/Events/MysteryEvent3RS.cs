using System;

namespace PKHeX.Core
{
    public sealed class MysteryEvent3RS : MysteryEvent3
    {
        public MysteryEvent3RS(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        public override bool IsChecksumValid() => Checksum == Checksums.CheckSum16(Data);
        public override void FixChecksum() => Checksum = Checksums.CheckSum16(Data);
    }
}