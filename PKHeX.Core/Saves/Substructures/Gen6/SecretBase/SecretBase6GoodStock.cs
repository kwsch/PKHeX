using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Secret Base Decoration Good Inventory stock for a given good-index.
/// </summary>
public sealed class SecretBase6GoodStock(byte[] Data, int Offset)
{
    public const int SIZE = 4;

    private Span<byte> Span => Data.AsSpan(Offset);

    public ushort Count { get => ReadUInt16LittleEndian(Span); set => WriteUInt16LittleEndian(Span, value); }
    public bool IsNew { get => Span[2] != 0; set => Span[2] = (byte)(value ? 1 : 0); }

    public void Clear() => MemoryMarshal.Write(Span, 0);
}
