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
        public static bool IsWithinRange(this IEncounterable encounter, int lvl)
        {
            return encounter.LevelMin <= lvl && lvl <= encounter.LevelMax;
        }
    }
}
