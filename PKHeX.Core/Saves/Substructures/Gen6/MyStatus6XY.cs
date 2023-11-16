using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// XY specific features for <see cref="MyStatus6"/>
/// </summary>
/// <remarks>These properties are technically included in OR/AS, but they are unused; assumed backwards compatibility for communications with XY</remarks>
public sealed class MyStatus6XY(SAV6XY sav, int offset) : MyStatus6(sav, offset)
{
    public TrainerFashion6 Fashion
    {
        get => TrainerFashion6.GetFashion(SAV.Data, Offset + 0x30, SAV.Gender);
        set => value.Write(Data, Offset + 0x30);
    }

    private Span<byte> Nickname_Trash => Data.AsSpan(Offset + 0x62, SAV6.ShortStringLength);

    public string OT_Nick
    {
        get => SAV.GetString(Nickname_Trash);
        set => SAV.SetString(Nickname_Trash, value, 12, StringConverterOption.ClearZero);
    }

    public short EyeColor
    {
        get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0x148));
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0x148), value);
    }
}
