namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic6 : EncounterStatic, IContestStats
    {
        public override int Generation => 6;

        public byte CNT_Cool   { get; init; }
        public byte CNT_Beauty { get; init; }
        public byte CNT_Cute   { get; init; }
        public byte CNT_Smart  { get; init; }
        public byte CNT_Tough  { get; init; }
        public byte CNT_Sheen  { get; init; }

        public EncounterStatic6(GameVersion game) : base(game) { }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (base.IsMatchLocation(pkm))
                return true;

            if (Species != (int) Core.Species.Pikachu)
                return false;

            // Cosplay Pikachu is given from multiple locations
            var loc = pkm.Met_Location;
            return loc is 180 or 186 or 194;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk6 = (PK6)pk;
            this.CopyContestStatsTo(pk6);
            pk6.SetRandomMemory6();
        }
    }
}
