using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMenu7 : SaveBlock<SAV7>
{
    public FieldMenu7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
    public FieldMenu7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

    // US/UM ONLY
    public ushort RotomAffection
    {
        get => ReadUInt16LittleEndian(SAV.Data.AsSpan(Offset + 0x1A));
        set => WriteUInt16LittleEndian(SAV.Data.AsSpan(Offset + 0x1A), Math.Min((ushort)1000, value));
    }

    public bool RotomLoto1 { get => (SAV.Data[Offset + 0x2A] & 1) == 1; set => SAV.Data[Offset + 0x2A] = (byte)((SAV.Data[Offset + 0x2A] & ~1) | (value ? 1 : 0)); }
    public bool RotomLoto2 { get => (SAV.Data[Offset + 0x2A] & 2) == 2; set => SAV.Data[Offset + 0x2A] = (byte)((SAV.Data[Offset + 0x2A] & ~2) | (value ? 2 : 0)); }

    public string RotomOT
    {
        get => SAV.GetString(RotomNameSpan);
        set => SAV.SetString(RotomNameSpan, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    private Span<byte> RotomNameSpan => Data.AsSpan(Offset + 0x30, 0x1A);
}
