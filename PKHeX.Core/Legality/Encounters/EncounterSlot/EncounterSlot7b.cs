namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.GG"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed class EncounterSlot7b : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7b(EncounterArea7b area, int species, int min, int max) : base(area)
        {
            Species = species;
            LevelMin = min;
            LevelMax = max;
        }
    }
}
