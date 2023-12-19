using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4/5 Chatter Recording
/// </summary>
public interface IChatter
{
    public const int SIZE_PCM = 1000; // 0x3E8
    public bool Initialized { get; set; }
    public Span<byte> Recording { get; }
    public int ConfusionChance { get; }
}
