namespace PKHeX.Core
{
    public sealed class EncounterSlot7 : EncounterSlot
    {
        public EncounterSlot7(EncounterArea7 area, int species, int form, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
        }

        public override int Generation => 7;
        public bool Pressure { get; set; }
    }
}