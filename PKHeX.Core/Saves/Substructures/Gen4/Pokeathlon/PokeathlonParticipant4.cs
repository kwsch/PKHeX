using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct PokeathlonParticipant4(Memory<byte> Raw) : ISpeciesForm, ITrainerID32, IFixedGender, IShiny
{
    public const int SIZE = 0xC;

    private Span<byte> Data => Raw.Span;

    private uint Packed { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort Species { get => (ushort)(Packed & 0x1FF); set => Packed = (Packed & ~0x1FFu) | ((uint)value & 0x1FF); }
    public byte Form { get => (byte)((Packed >> 9) & 0x1F); set => Packed = (Packed & ~(0x1Fu << 9)) | (((uint)value & 0x1F) << 9); }
    public byte Gender { get => (byte)((Packed >> 14) & 0x3); set => Packed = (Packed & ~(0x3u << 14)) | (((uint)value & 0x3) << 14); }
    public bool IsShiny { get => ((Packed >> 16) & 0x1) != 0; set => Packed = (Packed & ~(0x1u << 16)) | ((value ? 1u : 0u) << 16); }
    // remainder of bits unused

    /// <summary> <see cref="PKM.EncryptionConstant"/> </summary>
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }

    /// <summary> <see cref="PKM.ID32"/> </summary>
    public uint ID32 { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data[8..]); set => WriteUInt16LittleEndian(Data[8..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[10..]); set => WriteUInt16LittleEndian(Data[10..], value); }
    public TrainerIDFormat TrainerIDDisplayFormat => TrainerIDFormat.SixteenBit;
}
