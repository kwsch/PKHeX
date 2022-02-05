using System;
using System.Buffers.Binary;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores data about the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MyStatus8a : SaveBlock
{
    public MyStatus8a(SAV8LA sav, SCBlock block) : base(sav, block.Data) { }

    public int TID
    {
        get => BinaryPrimitives.ReadUInt16LittleEndian(Data.AsSpan(0x10));
        set => BinaryPrimitives.WriteUInt16LittleEndian(Data.AsSpan(0x10), (ushort)value);
    }

    public int SID
    {
        get => BinaryPrimitives.ReadUInt16LittleEndian(Data.AsSpan(0x12));
        set => BinaryPrimitives.WriteUInt16LittleEndian(Data.AsSpan(0x12), (ushort)value);
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
            ((SAV8LA)SAV).SetValue(SaveBlockAccessor8LA.KGameLanguage, (uint)value);
        }
    }

    private Span<byte> OT_Trash => Data.AsSpan(0x20, 0x1A);

    public string OT
    {
        get => SAV.GetString(OT_Trash);
        set => SAV.SetString(OT_Trash, value.AsSpan(), SAV.OTLength, StringConverterOption.ClearZero);
    }

    public byte Unk_0x50
    {
        get => Data[Offset + 0x50];
        set => Data[Offset + 0x50] = value;
    }
}
