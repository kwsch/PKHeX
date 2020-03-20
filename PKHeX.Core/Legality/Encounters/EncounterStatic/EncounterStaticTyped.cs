namespace PKHeX.Core
{
    public sealed class EncounterStaticTyped : EncounterStatic
    {
        /// <summary>
        /// <see cref="PK4.EncounterType"/> values permitted for the encounter.
        /// </summary>
        public EncounterType TypeEncounter { get; internal set; } = EncounterType.None;
    }
}