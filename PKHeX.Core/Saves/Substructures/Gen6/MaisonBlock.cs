using System;

namespace PKHeX.Core
{
    public sealed class MaisonBlock : SaveBlock
    {
        public MaisonBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public MaisonBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        public ushort GetMaisonStat(int index) { return BitConverter.ToUInt16(Data, Offset + 0x1C0 + (2 * index)); }
        public void SetMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C0 + (2 * index)); }
    }
}
