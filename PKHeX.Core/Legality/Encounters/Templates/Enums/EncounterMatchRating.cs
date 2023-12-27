namespace PKHeX.Core;

/// <summary>
/// Enumerates encounter match quality.
/// </summary>
public enum EncounterMatchRating : ushort
{
    /// <summary> Matches all data, no other matches will be better. </summary>
    Match,

    /// <summary> Matches most data, might have a better match later. </summary>
    Deferred,

    /// <summary> Matches most data, might have a better match later. Less preferred than <see cref="Deferred"/> due to small errors in secondary data. </summary>
    DeferredErrors,

    /// <summary> Matches some data, but will likely have a better match later. </summary>
    PartialMatch,

    /// <summary> Unused -- only used as an initial "max" value that anything else will be more suitable of a match. </summary>
    MaxNotMatch,
}
