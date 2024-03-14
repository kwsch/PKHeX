using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    public int Badges
    {
        get => Data[0x00];
        set => Data[0x00] = (byte)value;
    }

    public uint Money
    {
        get => ReadUInt32LittleEndian(Data[0x04..]);
        set
        {
            if (value > 9999999)
                value = 9999999;
            WriteUInt32LittleEndian(Data[0x04..], value);
        }
    }

    public int BP
    {
        get => ReadUInt16LittleEndian(Data[0x11C..]);
        set => WriteUInt16LittleEndian(Data[0x11C..], (ushort)value);
    }
}
