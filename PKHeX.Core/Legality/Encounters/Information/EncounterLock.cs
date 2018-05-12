namespace PKHeX.Core
{
    /// <summary>
    /// Encounter lock values restricting certain properties to a fixed value.
    /// </summary>
    /// <remarks>Used in Colosseum/XD to ensure that non-shadow <see cref="PKM"/> are of a certain Nature/etc.</remarks>
    public sealed class EncounterLock
    {
        public int Species { get; set; }
        public int Nature { get; set; } = -1;
        public int Gender { get; set; } = -1;

        public EncounterLock Clone() => (EncounterLock)MemberwiseClone();
    }
}
