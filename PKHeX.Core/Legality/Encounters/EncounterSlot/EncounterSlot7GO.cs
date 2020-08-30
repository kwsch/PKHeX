namespace PKHeX.Core
{
    public sealed class EncounterSlot7GO : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, int min, int max, GameVersion game) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            Version = game;
        }
    }
}