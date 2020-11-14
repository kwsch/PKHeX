namespace PKHeX.Core
{
    /// <summary>
    /// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
    /// </summary>
    internal static class EncountersGO
    {
        internal const int MAX_LEVEL = 40;

        internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"));
        internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"));

        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);
    }
}
