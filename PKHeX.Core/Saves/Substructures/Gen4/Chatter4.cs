using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Chatter Recording
/// </summary>
public sealed class Chatter4(Memory<byte> Raw) : IChatter
{
    public const int SIZE = sizeof(uint) + IChatter.SIZE_PCM;

    private Span<byte> Data => Raw.Span;

    public bool Initialized
    {
        get => ReadUInt32LittleEndian(Data) == 1u;
        set => WriteUInt32LittleEndian(Data, value ? 1u : 0u);
    }

    public Span<byte> Recording => Data.Slice(sizeof(uint), IChatter.SIZE_PCM);

    public int ConfusionChance => !Initialized ? 1 : (sbyte)Recording[15] switch
    {
        < -30 => 11,
        >= 30 => 31,
        _ => 1,
    };
}
