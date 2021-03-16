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
            base.SetPINGA(pk, criteria);
            pk.SetRandomEC();
            if (Area.Type is not SlotType.SOS)
                return;

            var ivs = new int[] { criteria.IV_HP, criteria.IV_ATK, criteria.IV_DEF,
                                 criteria.IV_SPE, criteria.IV_SPA, criteria.IV_SPD };
            var flawless = 0;
            foreach (var iv in ivs)
                if (iv == 31) flawless++;

            if (flawless >= 2)
                return;

            var abilities = pk.PersonalInfo.Abilities;
            if (abilities[0] == criteria.Ability) pk.RefreshAbility(0);
            if (abilities[1] == criteria.Ability) pk.RefreshAbility(1);
        }

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => Area.Type == SlotType.SOS ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
    }
}
