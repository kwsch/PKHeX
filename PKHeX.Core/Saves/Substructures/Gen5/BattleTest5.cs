using System;

namespace PKHeX.Core;

public class BattleTest5(Memory<byte> Raw)
{
    public const int SIZE = 0x5C8;
    public const string Extension = "bt5";

    private Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);
}
