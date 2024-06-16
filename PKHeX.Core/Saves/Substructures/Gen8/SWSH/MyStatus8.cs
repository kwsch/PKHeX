using System;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    public const uint MaxWatt = 9999999;

    public string Number
    {
        get => Encoding.ASCII.GetString(Data.Slice(1, 3));
        set
        {
            for (int i = 0; i < 3; i++)
                Data[0x01 + i] = (byte)(value.Length > i ? value[i] : '\0');
            SAV.State.Edited = true;
        }
    }

    public ulong Skin // aka the base model
    {
        get => ReadUInt64LittleEndian(Data[0x08..]);
        set => WriteUInt64LittleEndian(Data[0x08..], value);
    }

    public ulong Hair
    {
        get => ReadUInt64LittleEndian(Data[0x10..]);
        set => WriteUInt64LittleEndian(Data[0x10..], value);
    }

    public ulong Brow
    {
        get => ReadUInt64LittleEndian(Data[0x18..]);
        set => WriteUInt64LittleEndian(Data[0x18..], value);
    }

    public ulong Lashes
    {
        get => ReadUInt64LittleEndian(Data[0x20..]);
        set => WriteUInt64LittleEndian(Data[0x20..], value);
    }

    public ulong Contacts
    {
        get => ReadUInt64LittleEndian(Data[0x28..]);
        set => WriteUInt64LittleEndian(Data[0x28..], value);
    }

    public ulong Lips
    {
        get => ReadUInt64LittleEndian(Data[0x30..]);
        set => WriteUInt64LittleEndian(Data[0x30..], value);
    }

    public ulong Glasses
    {
        get => ReadUInt64LittleEndian(Data[0x38..]);
        set => WriteUInt64LittleEndian(Data[0x38..], value);
    }

    public ulong Hat
    {
        get => ReadUInt64LittleEndian(Data[0x40..]);
        set => WriteUInt64LittleEndian(Data[0x40..], value);
    }

    public ulong Jacket
    {
        get => ReadUInt64LittleEndian(Data[0x48..]);
        set => WriteUInt64LittleEndian(Data[0x48..], value);
    }

    public ulong Top
    {
        get => ReadUInt64LittleEndian(Data[0x50..]);
        set => WriteUInt64LittleEndian(Data[0x50..], value);
    }

    public ulong Bag
    {
        get => ReadUInt64LittleEndian(Data[0x58..]);
        set => WriteUInt64LittleEndian(Data[0x58..], value);
    }

    public ulong Gloves
    {
        get => ReadUInt64LittleEndian(Data[0x60..]);
        set => WriteUInt64LittleEndian(Data[0x60..], value);
    }

    public ulong BottomOrDress
    {
        get => ReadUInt64LittleEndian(Data[0x68..]);
        set => WriteUInt64LittleEndian(Data[0x68..], value);
    }

    public ulong Sock
    {
        get => ReadUInt64LittleEndian(Data[0x70..]);
        set => WriteUInt64LittleEndian(Data[0x70..], value);
    }

    public ulong Shoe
    {
        get => ReadUInt64LittleEndian(Data[0x78..]);
        set => WriteUInt64LittleEndian(Data[0x78..], value);
    }

    // 80 - 87

    public ulong MomSkin // aka the base model
    {
        get => ReadUInt64LittleEndian(Data[0x88..]);
        set => WriteUInt64LittleEndian(Data[0x88..], value);
    }

    // 8C - 9F
    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data[0xA0..]);
        set => WriteUInt32LittleEndian(Data[0xA0..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[0xA0..]);
        set => WriteUInt16LittleEndian(Data[0xA0..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[0xA2..]);
        set => WriteUInt16LittleEndian(Data[0xA2..], value);
    }

    public byte Game
    {
        get => Data[0xA4];
        set => Data[0xA4] = value;
    }

    public byte Gender
    {
        get => Data[0xA5];
        set => Data[0xA5] = value;
    }

    // A6
    public int Language
    {
        get => Data[0xA7];
        set
        {
            if (value == Language)
                return;
            Data[0xA7] = (byte) value;

            // For runtime language, the game shifts all languages above Language 6 (unused) down one.
            if (value >= 6)
                value--;
            SAV.SetValue(SaveBlockAccessor8SWSH.KGameLanguage, (uint)value);
        }
    }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0xB0, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    // D0
    public uint Watt
    {
        get => ReadUInt32LittleEndian(Data[0xD0..]);
        set
        {
            if (value > MaxWatt)
                value = MaxWatt;
            WriteUInt32LittleEndian(Data[0xD0..], value);
        }
    }
}
