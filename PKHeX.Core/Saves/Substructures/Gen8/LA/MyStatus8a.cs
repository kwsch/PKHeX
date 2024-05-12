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
        get => ReadUInt32LittleEndian(Data[0x10..]);
        set => WriteUInt32LittleEndian(Data[0x10..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[0x10..]);
        set => WriteUInt16LittleEndian(Data[0x10..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[0x12..]);
        set => WriteUInt16LittleEndian(Data[0x12..], value);
    }

    public byte Game
    {
        get => Data[0x14];
        set => Data[0x14] = value;
    }

    public byte Gender
    {
        get => Data[0x15];
        set => Data[0x15] = value;
    }

    // A6
    public int Language
    {
        get => Data[0x17];
        set
        {
            if (value == Language)
                return;
            Data[0x17] = (byte)value;

            // For runtime language, the game shifts all languages above Language 6 (unused) down one.
            if (value >= 6)
                value--;
            SAV.SetValue(SaveBlockAccessor8LA.KGameLanguage, (uint)value);
        }
    }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0x20, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public byte Unk_0x50
    {
        get => Data[0x50];
        set => Data[0x50] = value;
    }
}
