using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class Misc7b : SaveBlock
    {
        public Misc7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 4));
            set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 4), value);
        }

        public string Rival
        {
            get => SAV.GetString(Offset + 0x200, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x200);
        }
    }
}