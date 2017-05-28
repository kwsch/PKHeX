namespace PKHeX.Core
{
    public class EncounterInvalid : IEncounterable
    {
        public int Species { get; }
        public string Name => "Invalid";
        public bool EggEncounter => false;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public readonly int Level;

        public EncounterInvalid(PKM pkm)
        {
            Species = pkm.Species;
            Level = pkm.CurrentLevel;
        }
    }
}
