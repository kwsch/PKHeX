using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc7b : SaveBlock<SAV7b>
{
    public Misc7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

    public uint Money
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 4));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 4), value);
    }

    private Span<byte> Rival_Trash => Data.AsSpan(Offset + 0x200, 0x1A);

    public string Rival
    {
        get => SAV.GetString(Rival_Trash);
        set => SAV.SetString(Rival_Trash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }
}
