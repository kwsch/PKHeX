using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Frame List used to cache <see cref="XDRNG"/> results, lazily reversing backwards and keeping the previous results.
/// </summary>
public sealed class FrameCache
{
    private const int DefaultSize = 32;
    private readonly List<uint> Seeds = new(DefaultSize);
    private readonly List<uint> Values = new(DefaultSize);
    private uint Last;

    /// <summary>
    /// Creates a new instance of a <see cref="FrameCache"/>.
    /// </summary>
    /// <param name="origin">Seed at frame 0.</param>
    public FrameCache(uint origin) => Add(origin);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Add(uint seed)
    {
        Seeds.Add(Last = seed);
        Values.Add(seed >> 16);
    }

    /// <summary>
    /// Gets the 16 bit value from <see cref="Values"/> at a given <see cref="index"/>.
    /// </summary>
    /// <param name="index">Index to grab the value from</param>
    public uint this[int index]
    {
        get
        {
            while (index >= Seeds.Count)
                Add(XDRNG.Prev(Last));
            return Values[index];
        }
    }

    /// <summary>
    /// Gets the Seed at a specified frame index.
    /// </summary>
    /// <param name="index">Frame number</param>
    /// <returns>Seed at index</returns>
    public uint GetSeed(int index)
    {
        while (index >= Seeds.Count)
            Add(XDRNG.Prev(Last));
        return Seeds[index];
    }
}
