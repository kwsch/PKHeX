using System;

namespace PKHeX.Core;

/// <summary>
/// Overall block data for Massive Mass Outbreaks, containing all areas and their spawner objects.
/// </summary>
public readonly ref struct MassiveOutbreakSet8a
{
    public const int SIZE = 0x3980;
    public const int AreaCount = 5;

    private readonly Span<byte> Data;

    public MassiveOutbreakSet8a(SCBlock block) : this(block.Data) { }
    // ReSharper disable once ConvertToPrimaryConstructor
    public MassiveOutbreakSet8a(Span<byte> data) => Data = data;

    public MassiveOutbreakArea8a this[int index] => new(Data.Slice(MassiveOutbreakArea8a.SIZE * index, MassiveOutbreakArea8a.SIZE));
}
