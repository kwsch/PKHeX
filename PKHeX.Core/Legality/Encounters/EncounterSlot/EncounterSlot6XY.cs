namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.XY"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot6XY : EncounterSlot
    {
        public override int Generation => 6;
        public bool Pressure { get; init; }
        public bool IsFriendSafari => Area.Type == SlotType.FriendSafari;
        public bool IsHorde => Area.Type == SlotType.Horde;

        public EncounterSlot6XY(EncounterArea6XY area, int species, int form, int min, int max) : base(area, species, form, min, max)
        {
        }

        protected override void SetFormatSpecificData(PKM pk)
        {
            var pk6 = (PK6)pk;
            pk6.SetRandomMemory6();
            pk6.SetRandomEC();
        }

        public override string GetConditionString(out bool valid)
        {
            valid = true;
            return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
        }

        public EncounterSlot6XY CreatePressureFormCopy(int evoForm) => new((EncounterArea6XY) Area, Species, evoForm, LevelMin, LevelMax) {Pressure = true};

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => IsHorde ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
    }
}
