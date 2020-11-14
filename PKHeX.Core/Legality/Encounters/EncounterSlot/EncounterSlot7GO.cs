namespace PKHeX.Core
{
    public sealed class EncounterSlot7GO : EncounterSlot, IPogoSlot
    {
        public override int Generation => 7;
        public PogoType Type { get; }
        public Shiny Shiny { get; }

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, Shiny shiny, PogoType type) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = type.GetMinLevel();
            LevelMax = EncountersGO.MAX_LEVEL;

            Shiny = shiny;
            Type = type;
        }
    }
}
