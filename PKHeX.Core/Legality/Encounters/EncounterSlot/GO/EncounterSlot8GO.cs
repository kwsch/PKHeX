namespace PKHeX.Core
{
    public sealed class EncounterSlot8GO : EncounterSlotGO
    {
        public override int Generation => 8;
        public GameVersion OriginGroup { get; }

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, GameVersion gameVersion, PogoType type, Shiny shiny, int start, int end)
            : base(area, start, end, type, shiny)
        {
            Species = species;
            Form = form;

            OriginGroup = gameVersion;
        }

        public int GetMinIV() => Type.GetMinIV();
        public bool IsBallValid(Ball ball) => Type.IsBallValid(ball);

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            var pk8 = (PK8)pk;
            pk8.HT_Name = "PKHeX";
            pk8.HT_Language = 2;
            pk8.CurrentHandler = 1;

            base.ApplyDetails(sav, criteria, pk);
            pk.Ball = (int)Type.GetValidBall();

            pk8.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pk8.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }
    }
}
