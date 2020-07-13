namespace PKHeX.Core
{
    /// <summary>
    /// Common Encounter Properties base interface.
    /// </summary>
    public interface IEncounterable : IVersion
    {
        int Species { get; }
        int Form { get; }
        string Name { get; }
        string LongName { get; }
        bool EggEncounter { get; }
        int LevelMin { get; }
        int LevelMax { get; }
        int Generation { get; }

        PKM ConvertToPKM(ITrainerInfo sav);
        PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria);
    }

    public static partial class Extensions
    {
        private static bool IsWithinRange(this IEncounterable encounter, int lvl)
        {
            return encounter.LevelMin <= lvl && lvl <= encounter.LevelMax;
        }

        public static bool IsWithinRange(this IEncounterable encounter, PKM pkm)
        {
            if (!pkm.HasOriginalMetLocation)
                return encounter.IsWithinRange(pkm.CurrentLevel);
            if (encounter.EggEncounter)
                return pkm.CurrentLevel == encounter.LevelMin;
            if (encounter is MysteryGift g)
                return pkm.CurrentLevel == g.Level;
            return pkm.CurrentLevel == pkm.Met_Level;
        }
    }
}
