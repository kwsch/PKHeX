using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ShadowInfoEntryColo
{
    public readonly byte[] Data;
    private const int SIZE_ENTRY = 12;

    public ShadowInfoEntryColo() => Data = new byte[SIZE_ENTRY];
    public ShadowInfoEntryColo(byte[] data) => Data = data;

    public uint PID { get => ReadUInt32BigEndian(Data.AsSpan(0x00)); set => WriteUInt32BigEndian(Data.AsSpan(0x00), value); }
    public ushort MetLocation { get => ReadUInt16BigEndian(Data.AsSpan(0x06)); set => WriteUInt16BigEndian(Data.AsSpan(0x06), value); }
    public uint Unk08 { get => ReadUInt32BigEndian(Data.AsSpan(0x08)); set => WriteUInt32BigEndian(Data.AsSpan(0x08), value); }
}
