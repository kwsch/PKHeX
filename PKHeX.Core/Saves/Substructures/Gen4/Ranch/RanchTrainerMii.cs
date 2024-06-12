using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchTrainerMii(byte[] Data)
{
    public const int SIZE = 0x2C;
    public readonly byte[] Data = Data;

    public uint MiiId    { get => ReadUInt32BigEndian(Data.AsSpan(0x00)); set => WriteUInt32BigEndian(Data.AsSpan(0x00), value); }
    public uint SystemId { get => ReadUInt32BigEndian(Data.AsSpan(0x04)); set => WriteUInt32BigEndian(Data.AsSpan(0x04), value); }

    public ushort TrainerId { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), value); }
    public ushort SecretId  { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }

    private Span<byte> OriginalTrainerTrash => Data.AsSpan(0x10, 0x10);

    // 0x20-23: ??
    // 0x24:    ??
    // 0x25:    ??
    // 0x26-27: ??
    // 0x28-29: ??
    // 0x2A-2B: ??

    private static byte Language => 0;

    public string TrainerName
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, 7, Language);
    }
}
