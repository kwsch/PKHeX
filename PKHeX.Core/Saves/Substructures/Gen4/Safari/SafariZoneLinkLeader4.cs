using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SafariZoneLinkLeader4(Memory<byte> Raw)
{
    private Span<byte> Data => Raw.Span;

    public const int SIZE = 0x28;

    public long Timestamp { get => ReadInt64LittleEndian(Data); set => WriteInt64LittleEndian(Data, value); }
    public long Offset { get => ReadInt64LittleEndian(Data[8..]); set => WriteInt64LittleEndian(Data[8..], value); }
    public bool IsLinked { get => Data[0x10] != 0; set => Data[0x10] = (byte)(value ? 1 : 0); }
    public byte Gender { get => Data[0x11]; set => Data[0x11] = value; }
    public byte Language { get => Data[0x12]; set => Data[0x12] = value; }
    public GameVersion Version { get => (GameVersion)Data[0x13]; set => Data[0x13] = (byte)value; }
    public uint ID32 { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], value); }
    private Span<byte> OriginalTrainerTrash => Data.Slice(0x18, 0x10);

    public string OriginalTrainerName
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, maxLength: 7, language: Language);
    }
}
