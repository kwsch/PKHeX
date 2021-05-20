using System;

namespace PKHeX.Core
{
    public sealed class FieldMenu7 : SaveBlock
    {
        public FieldMenu7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public FieldMenu7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // USUM ONLY
        public ushort RotomAffection
        {
            get => BitConverter.ToUInt16(SAV.Data, Offset + 0x1A);
            set => SAV.SetData(BitConverter.GetBytes(Math.Min((ushort)1000, value)), Offset + 0x1A);
        }

        public bool RotomLoto1 { get => (SAV.Data[Offset + 0x2A] & 1) == 1; set => SAV.Data[Offset + 0x2A] = (byte)((SAV.Data[Offset + 0x2A] & ~1) | (value ? 1 : 0)); }
        public bool RotomLoto2 { get => (SAV.Data[Offset + 0x2A] & 2) == 2; set => SAV.Data[Offset + 0x2A] = (byte)((SAV.Data[Offset + 0x2A] & ~2) | (value ? 2 : 0)); }

        public string RotomOT
        {
            get => SAV.GetString(Offset + 0x30, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x30);
        }
    }
}