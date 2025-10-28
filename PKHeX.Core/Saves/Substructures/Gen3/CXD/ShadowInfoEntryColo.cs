using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ShadowInfoEntryColo
{
    public readonly Memory<byte> Raw;
    public Span<byte> Data => Raw.Span;
    private const int SIZE_ENTRY = 12;

    public ShadowInfoEntryColo() => Raw = new byte[SIZE_ENTRY];
    public ShadowInfoEntryColo(Memory<byte> raw) => Raw = raw;

    public uint PID { get => ReadUInt32BigEndian(Data); set => WriteUInt32BigEndian(Data, value); }
    public ushort MetLocation { get => ReadUInt16BigEndian(Data[0x06..]); set => WriteUInt16BigEndian(Data[0x06..], value); }
    public uint Unk08 { get => ReadUInt32BigEndian(Data[0x08..]); set => WriteUInt32BigEndian(Data[0x08..], value); }
}
