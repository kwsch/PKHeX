namespace PKHeX.Core
{
    public class EncounterInvalid : IEncounterable
    {
        public int Species { get; }
        public int LevelMin { get; }
        public int LevelMax { get; }
        public bool EggEncounter { get; }

        public string Name => "Invalid";

        public EncounterInvalid(PKM pkm)
        {
            Species = pkm.Species;
            LevelMin = pkm.Met_Level;
            LevelMax = pkm.CurrentLevel;
            EggEncounter = pkm.WasEgg;
        }
    }
}
