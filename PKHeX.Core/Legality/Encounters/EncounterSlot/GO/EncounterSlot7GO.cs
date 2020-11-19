namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Wild Encounter Slot data (GO Park)
    /// </summary>
    public sealed class EncounterSlot7GO : EncounterSlotGO
    {
        public override int Generation => 7;

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, int start, int end, Shiny shiny, PogoType type)
            : base(area, start, end, shiny, type)
        {
            Species = species;
            Form = form;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pb = (PB7) pk;
            pb.AwakeningSetAllTo(2);
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
