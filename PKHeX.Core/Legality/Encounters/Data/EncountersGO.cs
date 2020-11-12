using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
    /// </summary>
    internal static class EncountersGO
    {
        /// <summary> Clamp for generating encounters; no species allowed above this value except for those in <see cref="ExtraSpecies"/>. </summary>
        private const int MaxSpeciesID_GO_HOME = Legal.MaxSpeciesID_6;

        /// <summary> When generating encounters, these species will be skipped. </summary>
        private static readonly HashSet<int> DisallowedSpecies = new HashSet<int>
        {
            (int)Spinda,
        };

        /// <summary> Species beyond <see cref="MaxSpeciesID_GO_HOME"/> </summary>
        private static readonly int[] ExtraSpecies =
        {
            (int)Meltan,
            (int)Melmetal,

            (int)Obstagoon,
            (int)Perrserker,
            (int)Runerigus,
        };

        internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea();
        internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(SlotsGO_GG[0], MaxSpeciesID_GO_HOME, DisallowedSpecies, ExtraSpecies);
    }
}
