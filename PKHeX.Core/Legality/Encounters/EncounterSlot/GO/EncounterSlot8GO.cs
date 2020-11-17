namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Wild Encounter Slot data (GO -> HOME)
    /// </summary>
    public sealed class EncounterSlot8GO : EncounterSlotGO
    {
        public override int Generation => 8;
        public GameVersion OriginGroup { get; }

        public EncounterSlot8GO(EncounterArea8g area, int species, int form, int start, int end, Shiny shiny, PogoType type, GameVersion originGroup)
            : base(area, start, end, shiny, type)
        {
            Species = species;
            Form = form;

            OriginGroup = originGroup;
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
            var ball = Type.GetValidBall();
            if (ball != Ball.None)
                pk.Ball = (int)ball;

            pk8.HeightScalar = PokeSizeUtil.GetRandomScalar();
            pk8.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }
    }
}
