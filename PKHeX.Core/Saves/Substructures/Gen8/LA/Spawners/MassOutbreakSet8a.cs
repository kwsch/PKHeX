using System;

namespace PKHeX.Core;

/// <summary>
/// Overall block data for Mass Outbreaks, containing all areas and their spawner objects.
/// </summary>
public readonly ref struct MassOutbreakSet8a
{
    public const int SIZE = 0x190;
    public const int AreaCount = 5;

    private readonly Span<byte> Data;

    public MassOutbreakSet8a(SCBlock block) : this(block.Data) { }
    // ReSharper disable once ConvertToPrimaryConstructor
    public MassOutbreakSet8a(Span<byte> data) => Data = data;

    public MassOutbreakSpawner8a this[int index] => new(Data.Slice(MassOutbreakSpawner8a.SIZE * index, MassOutbreakSpawner8a.SIZE));
}
