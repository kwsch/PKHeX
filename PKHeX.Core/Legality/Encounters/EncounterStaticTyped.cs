namespace PKHeX.Core
{
    public class EncounterStaticTyped : EncounterStatic
    {
        public EncounterType TypeEncounter { get; internal set; } = EncounterType.None;
    }
}