namespace PKHeX.Core;

/// <summary>
/// Tuple of matched encounter info.
/// </summary>
public readonly record struct MatchedEncounter<T>(T Encounter, EncounterMatchRating Rating);
