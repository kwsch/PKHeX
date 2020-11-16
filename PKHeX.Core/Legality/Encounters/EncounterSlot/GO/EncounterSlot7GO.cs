namespace PKHeX.Core
{
    public sealed class EncounterSlot7GO : EncounterSlotGO
    {
        public override int Generation => 7;

        public EncounterSlot7GO(EncounterArea7g area, int species, int form, PogoType type, Shiny shiny, int start, int end)
            : base(area, start, end, type, shiny)
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
    }
}
