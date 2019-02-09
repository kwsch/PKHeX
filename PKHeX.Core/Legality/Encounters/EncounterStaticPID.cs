namespace PKHeX.Core
{
    internal class EncounterStaticPID : EncounterStatic
    {
        public uint PID { get; set; }
        public override Shiny Shiny { get; set; } = Shiny.FixedValue;

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pk.PersonalInfo);
            int nature = (int)(PID % 25);
            int ability = Ability;

            pk.PID = PID;
            pk.Gender = gender;
            SetIVs(pk);

            if (Generation >= 5)
                pk.Nature = nature;
            pk.RefreshAbility(ability >> 1);
        }
    }

    internal sealed class EncounterStaticN : EncounterStaticPID
    {
        public bool NSparkle { get; set; }

        internal void SetNPokemonData(PK5 pk5, int lang)
        {
            pk5.IV_HP = pk5.IV_ATK = pk5.IV_DEF = pk5.IV_SPA = pk5.IV_SPD = pk5.IV_SPE = 30;
            pk5.NPokémon = NSparkle;
            pk5.OT_Name = Legal.GetG5OT_NSparkle(lang);
            pk5.TID = 00002;
            pk5.SID = 00000;
        }
    }
}