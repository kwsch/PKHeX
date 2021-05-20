namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen7"/> (GO Park, <seealso cref="GameVersion.GG"/>).
    /// <inheritdoc cref="EncounterSlotGO" />
    /// </summary>
    public sealed record EncounterSlot7GO : EncounterSlotGO
    {
        public override int Generation => 7;

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, int start, int end, Shiny shiny, Gender gender, PogoType type)
            : base(area, species, form, start, end, shiny, gender, type)
        {
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pb = (PB7) pk;
            pb.AwakeningSetAllTo(2);
            pk.SetRandomEC();
            pb.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pb.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, GameVersion.GG);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}
