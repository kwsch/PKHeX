using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Secret Base Decoration Good Inventory stock for a given good-index.
/// </summary>
public sealed class SecretBase6GoodStock(Memory<byte> raw)
{
    public const int SIZE = 4;

    private Span<byte> Data => raw.Span;

    public ushort Count { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public bool IsNew { get => Data[2] != 0; set => Data[2] = (byte)(value ? 1 : 0); }

    public void Clear() => MemoryMarshal.Write(Data, 0);
}
