namespace PKHeX.Core
{
    public sealed class EncounterStatic1 : EncounterStatic
    {
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
    }
}
