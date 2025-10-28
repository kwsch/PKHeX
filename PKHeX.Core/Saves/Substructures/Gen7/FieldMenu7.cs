using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FieldMenu7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    // US/UM ONLY
    public ushort RotomAffection
    {
        get => ReadUInt16LittleEndian(Data[0x1A..]);
        set => WriteUInt16LittleEndian(Data[0x1A..], Math.Min((ushort)1000, value));
    }

    public bool RotomLoto1 { get => (Data[0x2A] & 1) == 1; set => Data[0x2A] = (byte)((Data[0x2A] & ~1) | (value ? 1 : 0)); }
    public bool RotomLoto2 { get => (Data[0x2A] & 2) == 2; set => Data[0x2A] = (byte)((Data[0x2A] & ~2) | (value ? 2 : 0)); }

    public string RotomOT
    {
        get => SAV.GetString(RotomNameSpan);
        set => SAV.SetString(RotomNameSpan, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    private Span<byte> RotomNameSpan => Data.Slice(0x30, 0x1A);
}
