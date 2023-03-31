using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Mystery Event data structure specific to <see cref="GameVersion.RS"/>
    /// </summary>
    public sealed class MysteryEvent3RS : MysteryEvent3
    {
        public MysteryEvent3RS(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        protected override ushort ComputeChecksum() => Checksums.CheckSum16(Data.AsSpan(4));
    }
}
