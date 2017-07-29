namespace PKHeX.Core
{
    public interface IEncounterable
    {
        int Species { get; }
        string Name { get; }
        bool EggEncounter { get; }
        int LevelMin { get; }
        int LevelMax { get; }
    }

    public static partial class Extensions
    {
        private static bool IsWithinRange(this IEncounterable encounter, int lvl)
        {
            return encounter.LevelMin <= lvl && lvl <= encounter.LevelMax;
        }
        public static bool IsWithinRange(this IEncounterable encounter, PKM pkm)
        {
            if (pkm.HasOriginalMetLocation)
            {
                if (encounter.EggEncounter)
                    return pkm.CurrentLevel == Legal.GetEggHatchLevel(pkm);
                if (encounter is MysteryGift g)
                    return pkm.CurrentLevel == g.Level;
                return pkm.CurrentLevel == pkm.Met_Level;
            }
            return encounter.IsWithinRange(pkm.CurrentLevel);
        }
        internal static string GetEncounterTypeName(this IEncounterable Encounter) => Encounter?.Name ?? "Unknown";
    }
}
