namespace PKHeX.Core
{
    public sealed class EncounterSlot5 : EncounterSlot
    {
        public override int Generation => 5;

        public EncounterSlot5(EncounterArea5 area, int species, int form, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
        }
    }
}
