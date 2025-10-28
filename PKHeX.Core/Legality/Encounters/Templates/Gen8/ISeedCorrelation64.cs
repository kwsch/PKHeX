namespace PKHeX.Core;

/// <summary>
/// Allows for the retrieval of a seed value from a <see cref="T"/> entity.
/// </summary>
public interface ISeedCorrelation64<in T>
{
    bool TryGetSeed(T pk, out ulong seed);
}
