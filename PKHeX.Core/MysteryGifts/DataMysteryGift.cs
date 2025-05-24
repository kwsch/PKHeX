using System;

namespace PKHeX.Core;

/// <summary>
/// Mystery Gift backed by serialized fields from ROM/SAV data, rather than observed specifications.
/// </summary>
public abstract class DataMysteryGift(Memory<byte> Raw) : MysteryGift
{
    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Returns an array for exporting outside the program (to disk, etc.).
    /// </summary>
    public virtual ReadOnlySpan<byte> Write() => Data;

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var b in Data)
            hash = (hash * 31) + b;
        return hash;
    }

    /// <summary>
    /// Creates a deep copy of the <see cref="MysteryGift"/> object data.
    /// </summary>
    public override MysteryGift Clone()
    {
        var data = Data.ToArray();
        var result = GetMysteryGift(data);
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    public sealed override bool Empty => !Data.ContainsAnyExcept<byte>(0);

    public void Clear() => Data.Clear();
}
