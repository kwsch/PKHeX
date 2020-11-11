using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
    /// </summary>
    internal static class EncountersGO
    {
        private const int MaxSpeciesID_GO_HOME = Legal.MaxSpeciesID_6;

        private static readonly HashSet<int> DisallowedSpecies = new HashSet<int>
        {
            (int)Spinda,
        };

        private static readonly int[] ExtraSpecies =
        {
            (int)Meltan,
            (int)Melmetal,
        };

        internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea();
        internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(SlotsGO_GG[0], MaxSpeciesID_GO_HOME, DisallowedSpecies, ExtraSpecies);
    }
}
