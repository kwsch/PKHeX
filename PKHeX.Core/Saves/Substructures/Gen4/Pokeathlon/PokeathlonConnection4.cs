using System;

namespace PKHeX.Core;

public struct PokeathlonConnection4(Memory<byte> Raw)
{
    public const int SIZE = 0xA4;

    public const uint MaxTrainer = PokeathlonEventData4.MaxRecord;

    public Span<byte> Data => Raw.Span;

    public PokeathlonEventData4 Inner => new(Raw.Slice(0 * PokeathlonEventData4.SIZE, PokeathlonEventData4.SIZE));

    /// <summary>
    /// Correlated to the <see cref="Inner"/> indexed records.
    /// </summary>
    public PokeathlonEventTrainer4 GetTrainer(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, MaxTrainer);
        return new(Raw.Slice(0x2C + (index * PokeathlonEventTrainer4.SIZE), PokeathlonEventTrainer4.SIZE));
    }
}
