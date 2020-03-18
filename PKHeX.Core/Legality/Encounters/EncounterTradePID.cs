namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed PID.
    /// </summary>
    public sealed class EncounterTradePID : EncounterTrade
    {
        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTradePID(uint pid) => PID = pid;

        public override Shiny Shiny { get; set; } = Shiny.FixedValue;

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature);
            int ability = Ability >> 1;

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