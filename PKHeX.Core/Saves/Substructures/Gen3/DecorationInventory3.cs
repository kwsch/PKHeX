using System;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

public readonly ref struct DecorationInventory3
{
    public const int SIZE = 150;
    private readonly Span<byte> Data;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DecorationInventory3(Span<byte> data) => Data = data;

    public Span<Decoration3> Desk     => MemoryMarshal.Cast<byte, Decoration3>(Data[..10]);
    public Span<Decoration3> Chair    => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(10, 10));
    public Span<Decoration3> Plant    => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(20, 10));
    public Span<Decoration3> Ornament => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(30, 30));
    public Span<Decoration3> Mat      => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(60, 30));
    public Span<Decoration3> Poster   => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(90, 10));
    public Span<Decoration3> Doll     => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(100, 40));
    public Span<Decoration3> Cushion  => MemoryMarshal.Cast<byte, Decoration3>(Data.Slice(140, 10));
}
