namespace PKHeX.Core
{
    public sealed class EncounterStaticTyped : EncounterStatic
    {
        public EncounterType TypeEncounter { get; internal set; } = EncounterType.None;
    }
}