namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen7"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot7 : EncounterSlot
    {
        public override int Generation => 7;

        public EncounterSlot7(EncounterArea7 area, int species, int form, int min, int max) : base(area, species, form, min, max)
        {
        }

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => Area.Type == SlotType.SOS ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
    }
}
