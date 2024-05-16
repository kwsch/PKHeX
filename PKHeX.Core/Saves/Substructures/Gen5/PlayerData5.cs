using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Combined save block; 0x100 for first, 0x100 for second.
/// </summary>
public sealed class PlayerData5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    private Span<byte> OriginalTrainerTrash => Data.Slice(4, 0x10);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data[0x14..]);
        set => WriteUInt32LittleEndian(Data[0x14..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[(0x14 + 0)..]);
        set => WriteUInt16LittleEndian(Data[(0x14 + 0)..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[(0x14 + 2)..]);
        set => WriteUInt16LittleEndian(Data[(0x14 + 2)..], value);
    }

    public int Country
    {
        get => Data[0x1C];
        set => Data[0x1C] = (byte)value;
    }

    public int Region
    {
        get => Data[0x1D];
        set => Data[0x1D] = (byte)value;
    }

    public int Language
    {
        get => Data[0x1E];
        set => Data[0x1E] = (byte)value;
    }

    public byte Game
    {
        get => Data[0x1F];
        set => Data[0x1F] = value;
    }

    public byte Gender
    {
        get => Data[0x21];
        set => Data[0x21] = value;
    }

    // 22,23 ??

    public int PlayedHours
    {
        get => ReadUInt16LittleEndian(Data[0x24..]);
        set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value);
    }

    public int PlayedMinutes
    {
        get => Data[0x24 + 2];
        set => Data[0x24 + 2] = (byte)value;
    }

    public int PlayedSeconds
    {
        get => Data[0x24 + 3];
        set => Data[0x24 + 3] = (byte)value;
    }
}

public sealed class PlayerPosition5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public int M { get => ReadInt32LittleEndian(Data[0x80..]); set => WriteUInt16LittleEndian(Data[0x80..], (ushort)value); }
    public int X { get => ReadUInt16LittleEndian(Data[0x86..]); set => WriteUInt16LittleEndian(Data[0x86..], (ushort)value); }
    public int Z { get => ReadUInt16LittleEndian(Data[0x8A..]); set => WriteUInt16LittleEndian(Data[0x8A..], (ushort)value); }
    public int Y { get => ReadUInt16LittleEndian(Data[0x8E..]); set => WriteUInt16LittleEndian(Data[0x8E..], (ushort)value); }
}
