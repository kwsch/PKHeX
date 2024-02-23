using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Combined save block; 0x100 for first, 0x100 for second.
/// </summary>
public sealed class PlayerData5(SAV5 sav, int offset) : SaveBlock<SAV5>(sav, offset)
{
    private Span<byte> OriginalTrainerTrash => Data.AsSpan(Offset + 4, 0x10);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x14));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x14), value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14 + 0));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14 + 0), value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14 + 2));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14 + 2), value);
    }

    public int Country
    {
        get => Data[Offset + 0x1C];
        set => Data[Offset + 0x1C] = (byte)value;
    }

    public int Region
    {
        get => Data[Offset + 0x1D];
        set => Data[Offset + 0x1D] = (byte)value;
    }

    public int Language
    {
        get => Data[Offset + 0x1E];
        set => Data[Offset + 0x1E] = (byte)value;
    }

    public byte Game
    {
        get => Data[Offset + 0x1F];
        set => Data[Offset + 0x1F] = value;
    }

    public byte Gender
    {
        get => Data[Offset + 0x21];
        set => Data[Offset + 0x21] = value;
    }

    // 22,23 ??

    public int PlayedHours
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x24));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x24), (ushort)value);
    }

    public int PlayedMinutes
    {
        get => Data[Offset + 0x24 + 2];
        set => Data[Offset + 0x24 + 2] = (byte)value;
    }

    public int PlayedSeconds
    {
        get => Data[Offset + 0x24 + 3];
        set => Data[Offset + 0x24 + 3] = (byte)value;
    }

    public int M
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x180));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x180), (ushort)value);
    }

    public int X
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x186));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x186), (ushort)value);
    }

    public int Z
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x18A));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x18A), (ushort)value);
    }

    public int Y
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x18E));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x18E), (ushort)value);
    }
}
