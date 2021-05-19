namespace PKHeX.Core
{
    public sealed record EncounterTrade5 : EncounterTrade
    {
        public override int Generation => 5;
        public override int Location => Locations.LinkTrade5NPC;

        public EncounterTrade5(GameVersion game) : base(game)
        {
        }
    }

    public sealed record EncounterTrade5PID : EncounterTrade
    {
        public override int Generation => 5;
        public override int Location => Locations.LinkTrade5NPC;

        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade5PID(GameVersion game, uint pid) : base(game)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            // Trades for JPN games have language ID of 0, not 1.
            if (pk.Language == (int) LanguageID.Japanese)
                pk.Language = 0;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(PKX.GetGenderFromPID(Species, PID), pi);
            int nature = (int)criteria.GetNature(Nature);
            int ability = criteria.GetAbilityFromNumber(Ability);

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

        public static bool IsValidMissingLanguage(PKM pkm)
        {
            return pkm.Format == 5 && pkm.BW;
        }
    }
}
