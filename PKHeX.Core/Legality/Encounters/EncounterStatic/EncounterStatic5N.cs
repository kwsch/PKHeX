namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Static Encounter from N
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    internal sealed record EncounterStatic5N : EncounterStatic5
    {
        public readonly uint PID;
        public const bool NSparkle = true;

        internal EncounterStatic5N(uint pid) : base(GameVersion.B2W2)
        {
            Shiny = Shiny.FixedValue;
            PID = pid;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pk.PersonalInfo);
            int nature = (int)Nature;
            int ability = Ability;

            pk.PID = PID;
            pk.Gender = gender;
            SetIVs(pk);

            pk.Nature = nature;
            pk.RefreshAbility(ability >> 1);
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (PID != pkm.PID)
                return false;
            return base.IsMatchExact(pkm, evo);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            SetNPokemonData((PK5)pk, pk.Language);
        }

        private static void SetNPokemonData(PK5 pk5, int lang)
        {
            pk5.IV_HP = pk5.IV_ATK = pk5.IV_DEF = pk5.IV_SPA = pk5.IV_SPD = pk5.IV_SPE = 30;
            pk5.NPokémon = NSparkle;
            pk5.OT_Name = GetOT(lang);
            pk5.TID = 00002;
            pk5.SID = 00000;
        }

        public static string GetOT(int lang) => lang == (int)LanguageID.Japanese ? "Ｎ" : "N";
    }
}
