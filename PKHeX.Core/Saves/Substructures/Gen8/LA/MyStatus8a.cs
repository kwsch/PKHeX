using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores data about the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MyStatus8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data)
{
    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x10));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x10));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x10), value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x12));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x12), value);
    }

    public int Game
    {
        get => Data[0x14];
        set => Data[0x14] = (byte)value;
    }

    public int Gender
    {
        get => Data[0x15];
        set => Data[0x15] = (byte)value;
    }

    // A6
    public int Language
    {
        get => Data[Offset + 0x17];
        set
        {
            if (value == Language)
                return;
            Data[Offset + 0x17] = (byte)value;

            // For runtime language, the game shifts all languages above Language 6 (unused) down one.
            if (value >= 6)
                value--;
            SAV.SetValue(SaveBlockAccessor8LA.KGameLanguage, (uint)value);
        }
    }

    private Span<byte> OT_Trash => Data.AsSpan(0x20, 0x1A);

    public string OT
    {
        get => SAV.GetString(OT_Trash);
        set => SAV.SetString(OT_Trash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public byte Unk_0x50
    {
        get => Data[Offset + 0x50];
        set => Data[Offset + 0x50] = value;
    }
}
