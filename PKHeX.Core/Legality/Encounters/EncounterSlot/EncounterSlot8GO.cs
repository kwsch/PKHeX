namespace PKHeX.Core
{
    public sealed class EncounterSlot8GO : EncounterSlot
    {
        public override int Generation => 8;
        public GameVersion OriginGroup { get; }

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, GameVersion gameVersion, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            OriginGroup = gameVersion;
        }
    }
}
