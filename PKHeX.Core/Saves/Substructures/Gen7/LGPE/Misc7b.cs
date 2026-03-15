using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    public uint Money
    {
        get => ReadUInt32LittleEndian(Data[4..]);
        set => WriteUInt32LittleEndian(Data[4..], value);
    }

    public Span<byte> RivalNameTrash => Data.Slice(0x200, 0x1A);

    public string RivalName
    {
        get => SAV.GetString(RivalNameTrash);
        set => SAV.SetString(RivalNameTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }
}
