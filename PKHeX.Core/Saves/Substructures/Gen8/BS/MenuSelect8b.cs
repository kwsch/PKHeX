using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Tracks the main menu items. Size: 0x44
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MenuSelect8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    // (TopMenuItemTypeInt32, bool IsNew)[8], TopMenuItemTypeInt32 LastSelected
    private const int COUNT_ITEMS = 8;
    private const int SIZE_TUPLE = 4 + 4; // int,bool32

    public int GetMenuItem(int index)
    {
        int ofs = GetOffset(index);
        return ReadInt32LittleEndian(Data[ofs..]);
    }

    public void SetMenuItem(int index, int value)
    {
        int ofs = GetOffset(index);
        WriteInt32LittleEndian(Data[ofs..], value);
    }

    public bool GetMenuItemIsNew(int index)
    {
        int ofs = GetOffset(index);
        return ReadInt32LittleEndian(Data[(ofs + 4)..]) == 1;
    }

    public void SetMenuItemIsNew(int index, bool value)
    {
        int ofs = GetOffset(index);
        WriteInt32LittleEndian(Data[(ofs + 4)..], value ? 1 : 0);
    }

    private static int GetOffset(int index)
    {
        if ((uint)index >= COUNT_ITEMS)
            throw new ArgumentOutOfRangeException(nameof(index));
        return index * SIZE_TUPLE;
    }

    public int LastSelectedMenu { get => ReadInt32LittleEndian(Data[0x40..]); set => WriteInt32LittleEndian(Data[0x40..], value); }
}
