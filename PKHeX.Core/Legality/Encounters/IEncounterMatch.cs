using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public interface IEncounterMatch
    {
        bool IsMatchExact(PKM pkm, DexLevel dl);
        EncounterMatchRating GetMatchRating(PKM pkm);
    }

    internal static class EncounterMatchExtensions
    {
        /// <summary>
        /// Some species do not have a Hidden Ability, but can be altered to have the HA slot via pre-evolution.
        /// </summary>
        /// <param name="_">Match object</param>
        /// <param name="species">Species ID</param>
        /// <returns>True if it should not originate as this species.</returns>
        private static bool IsPartialMatchHidden(this IEncounterMatch _, int species)
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
        public static bool IsPartialMatchHidden(this IEncounterMatch _, int current, int original)
        {
            if (current == original)
                return false;
            if (!_.IsPartialMatchHidden(original))
                return false;
            return _.IsPartialMatchHidden(current);
        }
    }
}
