using static PKHeX.Core.OverworldCorrelation8Requirement;

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
            var req = GetRequirement(pk);
            if (req != MustHave)
            {
                pk.SetRandomEC();
                return;
            }
            Overworld8RNG.ApplyDetails(pk, criteria);
        }

        public OverworldCorrelation8Requirement GetRequirement(PKM pk)
        {
            if (((EncounterArea8)Area).PermitCrossover)
                return MustHave; // symbol walking overworld

            bool curry = pk is IRibbonSetMark8 {RibbonMarkCurry: true};
            if (curry)
                return MustNotHave;

            // Tree encounters are generated via the global seed, not the u32
            if ((Weather & AreaWeather8.Shaking_Trees) != 0)
            {
                // Some tree encounters are present in the regular encounters.
                return Weather == AreaWeather8.Shaking_Trees
                    ? MustNotHave
                    : CanBeEither;
            }

            return MustHave;
        }

        public bool IsOverworldCorrelationCorrect(PKM pk)
        {
            return Overworld8RNG.ValidateOverworldEncounter(pk);
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            var rating = base.GetMatchRating(pkm);
            if (rating != EncounterMatchRating.Match)
                return rating;

            if (pkm is IRibbonSetMark8 m)
            {
                if (m.RibbonMarkCurry && (Weather & AreaWeather8.All) == 0)
                    return EncounterMatchRating.Deferred;
                if (m.RibbonMarkFishing && (Weather & AreaWeather8.Fishing) == 0)
                    return EncounterMatchRating.Deferred;
            }

            var req = GetRequirement(pkm);
            return req switch
            {
                MustHave when !IsOverworldCorrelationCorrect(pkm) => EncounterMatchRating.Deferred,
                MustNotHave when IsOverworldCorrelationCorrect(pkm) => EncounterMatchRating.Deferred,
                _ => EncounterMatchRating.Match,
            };
        }
    }
}
