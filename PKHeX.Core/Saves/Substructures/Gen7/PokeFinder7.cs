using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PokeFinder7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    public ushort CameraVersion
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public bool GyroFlag
    {
        get => ReadUInt16LittleEndian(Data[0x02..]) == 1;
        set => WriteUInt16LittleEndian(Data[0x02..], (ushort)(value ? 1 : 0));
    }

    public uint SnapCount
    {
        get => ReadUInt32LittleEndian(Data[0x04..]);
        set
        {
            if (value > 9999999) // Top bound is unchecked, check anyway
                value = 9999999;
            WriteUInt32LittleEndian(Data[0x04..], value);
        }
    }

    public uint ThumbsTotalValue
    {
        get => ReadUInt32LittleEndian(Data[0x0C..]);
        set => WriteUInt32LittleEndian(Data[0x0C..], value);
    }

    public uint ThumbsHighValue
    {
        get => ReadUInt32LittleEndian(Data[0x10..]);
        set
        {
            if (value > 9_999_999)
                value = 9_999_999;
            WriteUInt32LittleEndian(Data[0x10..], value);

            if (value > ThumbsTotalValue)
                ThumbsTotalValue = value;
        }
    }

    public ushort TutorialFlags
    {
        get => ReadUInt16LittleEndian(Data[0x14..]);
        set => WriteUInt16LittleEndian(Data[0x14..], value);
    }
}
