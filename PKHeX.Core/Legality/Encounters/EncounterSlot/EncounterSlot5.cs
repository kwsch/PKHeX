namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen5"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot5 : EncounterSlot
    {
        public override int Generation => 5;

        public EncounterSlot5(EncounterArea5 area, int species, int form, int min, int max) : base(area, species, form, min, max)
        {
        }

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => Area.Type == SlotType.HiddenGrotto ? HiddenAbilityPermission.Always : HiddenAbilityPermission.Never;
    }
}
