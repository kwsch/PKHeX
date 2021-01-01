namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot8 : EncounterSlot
    {
        public readonly AreaWeather8 Weather;
        public override string LongName => Weather == AreaWeather8.All ? wild : $"{wild} - {Weather.ToString().Replace("_", string.Empty)}";
        public override int Generation => 8;

        public EncounterSlot8(EncounterArea8 area, int species, int form, int min, int max, AreaWeather8 weather) : base(area, species, form, min, max)
        {
            Weather = weather;
        }
    }
}
