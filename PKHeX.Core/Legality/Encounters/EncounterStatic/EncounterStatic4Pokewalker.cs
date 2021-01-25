namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Pokéwalker  Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic4Pokewalker : EncounterStatic
    {
        public override int Generation => 4;

        public EncounterStatic4Pokewalker(int species, int gender, int level) : base(GameVersion.HGSS)
        {
            Species = species;
            Gender = gender;
            Level = level;
            Gift = true;
            Location = Locations.PokeWalker4;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (pkm.Format == 4)
                return Location == pkm.Met_Location;
            return true; // transfer location verified later
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (Gift && pkm.Ball != Ball)
                return true;
            return base.IsMatchPartial(pkm);
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(Gender, pi);
            int nature = (int)criteria.GetNature(Nature.Random);

            // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
            // int ability = criteria.GetAbilityFromNumber(Ability, pi);

            PIDGenerator.SetRandomPIDPokewalker(pk, nature, gender);
            criteria.SetRandomIVs(pk);
        }
    }
}
