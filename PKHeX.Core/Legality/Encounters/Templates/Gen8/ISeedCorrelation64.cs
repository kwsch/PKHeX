namespace PKHeX.Core;

/// <summary>
/// Allows for the retrieval of a seed value from a <see cref="T"/> entity.
/// </summary>
public interface ISeedCorrelation64<in T>
{
    SeedCorrelationResult TryGetSeed(T pk, out ulong seed);
}

public enum SeedCorrelationResult
{
    Invalid,
    Ignore,
    Success,
}
