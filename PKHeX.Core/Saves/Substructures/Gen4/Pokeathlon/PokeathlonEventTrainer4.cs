using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct PokeathlonEventTrainer4(Memory<byte> Raw) : ITrainerID32
{
    public const int SIZE = 0x18;
    public Span<byte> Data => Raw.Span;
    public uint ID32 { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public TrainerIDFormat TrainerIDDisplayFormat => TrainerIDFormat.SixteenBit;

    public Span<byte> OriginalTrainerTrash => Data.Slice(4, 8 * sizeof(ushort));

    public byte Language { get => Data[0x14]; set => Data[0x14] = value; }
    // remaining 3 bytes unused

    public string OriginalTrainerName
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, 7, Language);
    }
}
