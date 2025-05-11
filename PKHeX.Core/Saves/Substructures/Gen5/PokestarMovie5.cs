using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PokestarMovie5(Memory<byte> Raw)
{
    public const int SIZE = 0x4B4;
    public const string Extension = "psm5";

    private Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);
    public string Name => IsUninitialized ? "Empty": "Movie";
}
