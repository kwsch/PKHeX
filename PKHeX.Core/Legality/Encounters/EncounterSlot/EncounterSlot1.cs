namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Wild Encounter Slot data
    /// </summary>
    /// <remarks>
    /// Contains Time data which is present in <see cref="GameVersion.C"/> origin data.
    /// Contains <see cref="GameVersion"/> identification, as this Version value is not stored in <see cref="PK1"/> or <see cref="PK2"/> formats.
    /// </remarks>
    public sealed class EncounterSlot1 : EncounterSlot
    {
        public int Rate;
        internal EncounterTime Time;
    }
}