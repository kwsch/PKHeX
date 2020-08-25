namespace PKHeX.Core
{
    public sealed class EncounterSlot7b : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7b(EncounterArea7b area, int species, int min, int max, GameVersion game) : base(area)
        {
            Species = species;
            LevelMin = min;
            LevelMax = max;
            Version = game;
        }
    }
}