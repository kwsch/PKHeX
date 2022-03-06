using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Revision details for the <see cref="GameVersion.PLA"/> Pokédex.
/// </summary>
public sealed class PokedexSaveGlobalData
{
    private readonly byte[] _data;
    private readonly int Offset;

    public PokedexSaveGlobalData(byte[] data, int offset) => (_data, Offset) = (data, offset);
    private Span<byte> Data => _data.AsSpan(Offset);

    public uint Flags      { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public byte Field_04   { get => Data[4]; set => Data[4] = value; }
    public byte FormField   { get => Data[5]; set => Data[5] = value; }
    public ushort UpdateCounter { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }
    public ushort ReportCounter { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    public ushort Field_0A { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }
    public int TotalResearchPoints { get => ReadInt32LittleEndian(Data[0x0C..]); set => WriteInt32LittleEndian(Data[0x0C..], value); }
}
