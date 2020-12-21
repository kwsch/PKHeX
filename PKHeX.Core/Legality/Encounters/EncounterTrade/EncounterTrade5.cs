namespace PKHeX.Core
{
    public sealed class EncounterTrade5 : EncounterTrade
    {
        public override int Generation => 5;
    }

    public sealed class EncounterTrade5PID : EncounterTrade
    {
        public override int Generation => 5;

        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade5PID(uint pid)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pi);
            int nature = (int)criteria.GetNature(Nature);
            int ability = criteria.GetAbilityFromNumber(Ability, pi);

            pk.PID = PID;
            pk.Nature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(ability);

            SetIVs(pk);
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            if (PID != pkm.EncryptionConstant)
                return false;
            if (Nature != Nature.Random && (int)Nature != pkm.Nature) // gen5 BW only
                return false;
            return true;
        }
    }
}
