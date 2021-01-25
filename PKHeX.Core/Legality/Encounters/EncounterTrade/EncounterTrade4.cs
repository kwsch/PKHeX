namespace PKHeX.Core
{
    public abstract record EncounterTrade4 : EncounterTrade
    {
        public sealed override int Generation => 4;

        protected static readonly string[] RanchOTNames = { string.Empty, "ユカリ", "Hayley", "EULALIE", "GIULIA", "EUKALIA", string.Empty, "Eulalia" };

        protected EncounterTrade4(GameVersion game) : base(game)
        {
        }
    }

    public sealed record EncounterTrade4PID : EncounterTrade4, IContestStats
    {
        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade4PID(GameVersion game, uint pid, int species, int level) : base(game)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
            Species = species;
            Level = level;
        }

        public byte CNT_Cool { get; init; }
        public byte CNT_Beauty { get; init; }
        public byte CNT_Cute { get; init; }
        public byte CNT_Smart { get; init; }
        public byte CNT_Tough { get; init; }
        public byte CNT_Sheen { get; init; }

        public byte Contest
        {
            init
            {
                CNT_Cool = value;
                CNT_Beauty = value;
                CNT_Cute = value;
                CNT_Smart = value;
                CNT_Tough = value;
              //CNT_Sheen = value;
            }
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatchExact(pkm, evo))
                return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pkm = (PK4) pk;

            if (Version == GameVersion.DPPt)
            {
                // Has German Language ID for all except German origin, which is English
                if (Species == (int)Core.Species.Magikarp)
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.German ? LanguageID.English : LanguageID.German);
                // All other trades received: English games have a Japanese language ID instead of English.
                else if (pkm.Language == (int)LanguageID.English)
                    pkm.Language = (int)LanguageID.Japanese;
            }
            else // HGSS
            {
                // Has English Language ID for all except English origin, which is French
                if (Species == (int)Core.Species.Pikachu)
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.English ? LanguageID.French : LanguageID.English);
            }

            this.CopyContestStatsTo((PK4)pk);
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            pk.PID = PID;
            pk.Nature = (int)(PID % 25);
            pk.Gender = Gender;
            pk.RefreshAbility(Ability >> 1);
            SetIVs(pk);
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            return PID == pkm.EncryptionConstant;
        }
    }

    public sealed record EncounterTrade4RanchGift : EncounterTrade4
    {
        /// <summary>
        /// Fixed <see cref="PKM.PID"/> value the encounter must have.
        /// </summary>
        public readonly uint PID;

        public EncounterTrade4RanchGift(uint pid, int species, int level) : base(GameVersion.D)
        {
            PID = pid;
            Shiny = Shiny.FixedValue;
            Species = species;
            Level = level;
            TrainerNames = RanchOTNames;
        }

        protected override bool IsMatchNatureGenderShiny(PKM pkm)
        {
            return PID == pkm.EncryptionConstant;
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            pk.PID = PID;
            pk.Nature = (int)(PID % 25);
            pk.Gender = Gender;
            pk.RefreshAbility(Ability >> 1);
            SetIVs(pk);
        }
    }

    public sealed record EncounterTrade4RanchSpecial : EncounterTrade4
    {
        public EncounterTrade4RanchSpecial(int species, int level) : base(GameVersion.D)
        {
            Species = species;
            Level = level;
            Ball = 0x10;
            OTGender = 1;
            Location = 3000;
            TrainerNames = RanchOTNames;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            pk.FatefulEncounter = true;
        }
    }
}
