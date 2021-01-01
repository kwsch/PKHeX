namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen7"/> (GO Park, <seealso cref="GameVersion.GG"/>).
    /// <inheritdoc cref="EncounterSlotGO" />
    /// </summary>
    public sealed record EncounterSlot7GO : EncounterSlotGO
    {
        public override int Generation => 7;

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, int start, int end, Shiny shiny, PogoType type)
            : base(area, species, form, start, end, shiny, type)
        {
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pb = (PB7) pk;
            pb.AwakeningSetAllTo(2);
            pb.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pb.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }

        public override bool GetIVsValid(PKM pkm)
        {
            // Stamina*2 | 1 -> HP
            // ATK * 2 | 1 -> ATK&SPA
            // DEF * 2 | 1 -> DEF&SPD
            // Speed is random.

            // All IVs must be odd (except speed) and equal to their counterpart.
            if ((pkm.GetIV(1) & 1) != 1 || pkm.GetIV(1) != pkm.GetIV(4)) // ATK=SPA
                return false;
            if ((pkm.GetIV(2) & 1) != 1 || pkm.GetIV(2) != pkm.GetIV(5)) // DEF=SPD
                return false;
            return (pkm.GetIV(0) & 1) == 1; // HP
        }

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, GameVersion.GG);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}
