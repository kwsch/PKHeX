using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class SkinInfo5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    public abstract ushort CGearSkinChecksum { get; set; }
    public abstract bool HasCGearSkin { get; set; }
}

public sealed class SkinInfo5BW(SAV5BW SAV, Memory<byte> raw) : SkinInfo5(SAV, raw)
{
    public override ushort CGearSkinChecksum
    {
        get => ReadUInt16LittleEndian(Data[0x24..]);
        set => WriteUInt16LittleEndian(Data[0x24..], value);
    }

    public override bool HasCGearSkin
    {
        get => Data[0x26] == 1;
        set => Data[0x26] = value ? (byte)1 : (byte)0;
    }
}

public sealed class SkinInfo5B2W2(SAV5B2W2 sav, Memory<byte> raw) : SkinInfo5(sav, raw)
{
    public override ushort CGearSkinChecksum
    {
        get => ReadUInt16LittleEndian(Data[0x34..]);
        set => WriteUInt16LittleEndian(Data[0x34..], value);
    }

    public override bool HasCGearSkin
    {
        get => Data[0x36] == 1;
        set => Data[0x36] = value ? (byte)1 : (byte)0;
    }

    public Span<byte> SayingTrash => Data.Slice(0x80, 0x12);

    public string Saying
    {
        get => sav.GetString(SayingTrash);
        set => sav.SetString(SayingTrash, value, 8, StringConverterOption.ClearZeroSafeTerminate);
    }
}
