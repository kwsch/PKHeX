namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed PID.
    /// </summary>
    public sealed class EncounterTradePID : EncounterTrade
    {
        public uint PID;
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
    }
}