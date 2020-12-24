namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot representing data transferred to <see cref="GameVersion.Gen8"/> (HOME).
    /// <inheritdoc cref="EncounterSlotGO" />
    /// </summary>
    public sealed record EncounterSlot8GO : EncounterSlotGO
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

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, int start, int end, Shiny shiny, PogoType type, GameVersion originGroup)
            : base(area, start, end, shiny, type)
        {
            Species = species;
            Form = form;

            OriginGroup = originGroup;
        }

        /// <summary>
        /// Gets the minimum IV (in GO) the encounter must have.
        /// </summary>
        public int GetMinIV() => Type.GetMinIV();

        /// <summary>
        /// Checks if the <seealso cref="Ball"/> is compatible with the <seealso cref="PogoType"/>.
        /// </summary>
        public bool IsBallValid(Ball ball) => Type.IsBallValid(ball);

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            var pk8 = (PK8)pk;
            pk8.HT_Name = "PKHeX";
            pk8.HT_Language = 2;
            pk8.CurrentHandler = 1;

            base.ApplyDetails(sav, criteria, pk);
            var ball = Type.GetValidBall();
            if (ball != Ball.None)
                pk.Ball = (int)ball;

            pk8.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pk8.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, OriginGroup);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}
