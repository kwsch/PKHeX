using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Local region (within Hisui) <see cref="GameVersion.PLA"/> Pokédex structure.
/// </summary>
public sealed class PokedexSaveLocalData(Memory<byte> raw)
{
    public const int SIZE = 0x10;
    private Span<byte> Data => raw.Span;

    public ushort Field_00 { get => ReadUInt16LittleEndian(Data);  set => WriteUInt16LittleEndian(Data, value); }
    public byte Field_02 { get => Data[2]; set => Data[2] = value; }
    public byte Field_03 { get => Data[3]; set => Data[3] = value; }
    public uint Field_04 { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    public uint Field_08 { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public ushort Field_0C { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], value); }
}
