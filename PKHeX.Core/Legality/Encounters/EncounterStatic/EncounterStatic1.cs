namespace PKHeX.Core
{
    public sealed class EncounterStatic1 : EncounterStatic
    {
        public override int Generation => 1;

        public EncounterStatic1(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public EncounterStatic1(int species, int level, GameVersion ver)
        {
            Species = species;
            Level = level;
            Version = ver;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            return true;
        }
    }
}
