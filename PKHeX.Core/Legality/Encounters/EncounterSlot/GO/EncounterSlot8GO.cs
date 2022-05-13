namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot representing data transferred to <see cref="GameVersion.Gen8"/> (HOME).
    /// <inheritdoc cref="EncounterSlotGO" />
    /// </summary>
    public sealed record EncounterSlot8GO : EncounterSlotGO, IFixedOTFriendship
    {
        public override int Generation => 8;

        /// <summary>
        /// Encounters need a Parent Game to determine the original moves when transferred to HOME.
        /// </summary>
        /// <remarks>
        /// Future game releases might change this value.
        /// With respect to date legality, new dates might be incompatible with initial <seealso cref="OriginGroup"/> values.
        /// </remarks>
        public GameVersion OriginGroup { get; }

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, int start, int end, Shiny shiny, Gender gender, PogoType type, GameVersion originGroup)
            : base(area, species, form, start, end, shiny, gender, type)
        {
            OriginGroup = originGroup;
        }

        /// <summary>
        /// Checks if the <seealso cref="Ball"/> is compatible with the <seealso cref="PogoType"/>.
        /// </summary>
        public bool IsBallValid(Ball ball, int currentSpecies)
        {
            // GO does not natively produce Shedinja when evolving Nincada, and thus must be evolved in future games.
            if (currentSpecies == (int)Core.Species.Shedinja && currentSpecies != Species)
                return ball == Ball.Poke;
            return Type.IsBallValid(ball);
        }

        protected override PKM GetBlank()
        {
            if (((PersonalInfoSWSH)PersonalTable.SWSH.GetFormEntry(Species, Form)).IsPresentInGame)
                return new PK8();
            if (((PersonalInfoBDSP)PersonalTable.BDSP.GetFormEntry(Species, Form)).IsPresentInGame)
                return new PB8();
            if (((PersonalInfoLA)PersonalTable.LA.GetFormEntry(Species, Form)).IsPresentInGame)
                return new PA8();
            return new PK8(); // fallback, still illegal
        }

        private PersonalInfo GetPersonal()
        {
            var entry = PersonalTable.SWSH.GetFormEntry(Species, Form);
            if (((PersonalInfoSWSH)entry).IsPresentInGame)
                return entry;
            entry = PersonalTable.BDSP.GetFormEntry(Species, Form);
            if (((PersonalInfoBDSP)PersonalTable.BDSP.GetFormEntry(Species, Form)).IsPresentInGame)
                return entry;
            entry = PersonalTable.LA.GetFormEntry(Species, Form);
            if (((PersonalInfoLA)PersonalTable.LA.GetFormEntry(Species, Form)).IsPresentInGame)
                return entry;
            return entry; // fallback, still illegal
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            pk.HT_Name = "PKHeX";
            pk.CurrentHandler = 1;
            ((IHandlerLanguage)pk).HT_Language = 2;

            base.ApplyDetails(sav, criteria, pk);
            var ball = Type.GetValidBall();
            if (ball != Ball.None)
                pk.Ball = (int)ball;

            if (pk is IScaledSize s)
            {
                s.HeightScalar = PokeSizeUtil.GetRandomScalar();
                s.WeightScalar = PokeSizeUtil.GetRandomScalar();
            }

            pk.SetRandomEC();
        }

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, OriginGroup);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsMatchPartial(pkm))
                return EncounterMatchRating.PartialMatch;
            return base.GetMatchRating(pkm) == EncounterMatchRating.PartialMatch ? EncounterMatchRating.PartialMatch : EncounterMatchRating.Match;
        }

        public byte OT_Friendship => (byte)GetPersonal().BaseFriendship;

        private bool IsMatchPartial(PKM pk)
        {
            var stamp = GetTimeStamp(pk.Met_Year + 2000, pk.Met_Month, pk.Met_Day);
            if (!IsWithinStartEnd(stamp))
                return true;
            if (!GetIVsAboveMinimum(pk))
                return true;
            if (!GetIVsBelowMaximum(pk))
                return true;

            // Eevee & Glaceon have different base friendships. Make sure if it is invalid that we yield the other encounter before.
            if (pk.OT_Friendship != OT_Friendship)
                return true;

            return Species switch
            {
                (int)Core.Species.Yamask when pk.Species != Species && Form == 1 => pk is IFormArgument { FormArgument: 0 },
                (int)Core.Species.Milcery when pk.Species != Species => pk is IFormArgument { FormArgument: 0 },

                (int)Core.Species.Runerigus => pk is IFormArgument { FormArgument: not 0 },
                (int)Core.Species.Alcremie => pk is IFormArgument { FormArgument: not 0 },

                _ => false,
            };
        }
    }
}
