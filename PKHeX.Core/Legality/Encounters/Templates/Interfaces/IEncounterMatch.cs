using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Exposes details about the quality of a potential match compared to an input <see cref="PKM"/>.
/// </summary>
public interface IEncounterMatch
{
    /// <summary>
    /// Checks if the implementing object's details might have been the originator of the current <see cref="pk"/> data.
    /// </summary>
    bool IsMatchExact(PKM pk, EvoCriteria evo);

    /// <summary>
    /// Checks if the potential match may not be a perfect match (might be a better match later during iteration).
    /// </summary>
    EncounterMatchRating GetMatchRating(PKM pk);
}

internal static class EncounterMatchExtensions
{
    /// <summary>
    /// Some species do not have a Hidden Ability, but can be altered to have the HA slot via pre-evolution.
    /// </summary>
    /// <param name="_">Match object</param>
    /// <param name="species">Species ID</param>
    /// <returns>True if it should not originate as this species.</returns>
    private static bool IsPartialMatchHidden(this IEncounterMatch _, ushort species)
    {
        return species is (int)Metapod or (int)Kakuna
            or (int)Pupitar
            or (int)Silcoon or (int)Cascoon
            or (int)Vibrava or (int)Flygon;
    }

    /// <summary>
    /// Some species do not have a Hidden Ability, but can be altered to have the HA slot via pre-evolution.
    /// </summary>
    /// <param name="_">Match object</param>
    /// <param name="current">Current Species ID</param>
    /// <param name="original">Original Species ID</param>
    /// <returns>True if it should not originate as this species.</returns>
    public static bool IsPartialMatchHidden(this IEncounterMatch _, ushort current, ushort original)
    {
        if (current == original)
            return false;
        if (!_.IsPartialMatchHidden(original))
            return false;
        return _.IsPartialMatchHidden(current);
    }
}
