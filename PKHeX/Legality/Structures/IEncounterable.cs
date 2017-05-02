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
}
