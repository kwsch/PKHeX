using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// XY specific features for <see cref="MyStatus6"/>
/// </summary>
/// <remarks>These properties are technically included in OR/AS, but they are unused; assumed backwards compatibility for communications with XY</remarks>
public sealed class MyStatus6XY(SAV6XY sav, Memory<byte> raw) : MyStatus6(sav, raw)
{
    public TrainerFashion6 Fashion
    {
        get => TrainerFashion6.GetFashion(FashionSpan, SAV.Gender);
        set => value.Write(FashionSpan);
    }

    private Span<byte> FashionSpan => Data.Slice(0x30, TrainerFashion6.SIZE);

    private Span<byte> NicknameTrash => Data.Slice(0x62, SAV6.ShortStringLength);

    public string Nickname
    {
        get => SAV.GetString(NicknameTrash);
        set => SAV.SetString(NicknameTrash, value, 12, StringConverterOption.ClearZero);
    }

    public short EyeColor
    {
        get => ReadInt16LittleEndian(Data[0x148..]);
        set => WriteInt16LittleEndian(Data[0x148..], value);
    }
}
