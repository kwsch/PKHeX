namespace PKHeX.Core
{
    public interface IEncounterTemplate : ISpeciesForm, IVersion, IGeneration, IShiny
    {
        /// <summary>
        /// Indicates if the encounter originated as an egg.
        /// </summary>
        bool EggEncounter { get; }

        /// <summary>
        /// Minimum level for the encounter.
        /// </summary>
        int LevelMin { get; }
        int LevelMax { get; }
    }

    public static partial class Extensions
    {
        private static bool IsWithinEncounterRange(this IEncounterTemplate encounter, int lvl)
        {
            return encounter.LevelMin <= lvl && lvl <= encounter.LevelMax;
        }

        public static bool IsWithinEncounterRange(this IEncounterTemplate encounter, PKM pkm)
        {
            if (!pkm.HasOriginalMetLocation)
                return encounter.IsWithinEncounterRange(pkm.CurrentLevel);
            if (encounter.EggEncounter)
                return pkm.CurrentLevel == encounter.LevelMin;
            if (encounter is MysteryGift g)
                return pkm.CurrentLevel == g.Level;
            return pkm.CurrentLevel == pkm.Met_Level;
        }
    }
}
