namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen7"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed class EncounterSlot7 : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7(EncounterArea7 area, int species, int form, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
        }
    }
}
