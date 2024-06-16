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

    private Span<byte> RivalTrash => Data.Slice(0x200, 0x1A);

    public string Rival
    {
        get => SAV.GetString(RivalTrash);
        set => SAV.SetString(RivalTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }
}
