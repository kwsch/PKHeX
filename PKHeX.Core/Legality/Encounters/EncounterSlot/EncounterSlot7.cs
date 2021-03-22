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

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            pk.PID = Util.Rand32();
            pk.Nature = (int)criteria.GetNature(Nature.Random); 
            pk.Gender = criteria.GetGender(-1, pi);
            criteria.SetRandomIVs(pk);

            int num = Ability;
            if (Area.Type == SlotType.SOS && pk.FlawlessIVCount < 2)
                num = 0; // let's fake it as an insufficient chain, no HA possible.
            var ability = criteria.GetAbilityFromNumber(num);
            pk.RefreshAbility(ability);
            pk.SetRandomEC();
        }

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => Area.Type == SlotType.SOS ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
    }
}
