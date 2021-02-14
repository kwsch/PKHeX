namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot8 : EncounterSlot, IOverworldCorrelation8
    {
        public readonly AreaWeather8 Weather;
        public override string LongName => Weather == AreaWeather8.All ? wild : $"{wild} [{(((EncounterArea8)Area).PermitCrossover ? "Symbol" : "Hidden")}] - {Weather.ToString().Replace("_", string.Empty)}";
        public override int Generation => 8;

        public EncounterSlot8(EncounterArea8 area, int species, int form, int min, int max, AreaWeather8 weather) : base(area, species, form, min, max)
        {
            Weather = weather;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (!HasOverworldCorrelation(pk))
                pk.SetRandomEC();
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            // be lazy and just do the regular, and overwrite with correlation if required
            base.SetPINGA(pk, criteria);
            if (!HasOverworldCorrelation(pk))
                return;
            Overworld8RNG.ApplyDetails(pk, criteria);
        }

        // Tree encounters are generated via the global seed, not the u32
        public bool IsOverworldCorrelation => (Weather & AreaWeather8.Shaking_Trees) == 0;

        public bool HasOverworldCorrelation(PKM pk)
        {
            if (!IsOverworldCorrelation)
                return true;
            if (((EncounterArea8)Area).PermitCrossover)
                return true; // symbol walking overworld
            return pk is not IRibbonSetMark8 {RibbonMarkCurry: true};
        }

        public bool IsOverworldCorrelationCorrect(PKM pk)
        {
            return Overworld8RNG.ValidateOverworldEncounter(pk);
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (HasOverworldCorrelation(pkm) && !IsOverworldCorrelationCorrect(pkm))
                return EncounterMatchRating.Deferred;
            return base.GetMatchRating(pkm);
        }
    }
}
