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
        public override string LongName => $"{wild} [{(((EncounterArea8)Area).PermitCrossover ? "Symbol" : "Hidden")}] - {Weather.ToString().Replace("_", string.Empty)}";
        public override int Generation => 8;

        public EncounterSlot8(EncounterArea8 area, int species, int form, int min, int max, AreaWeather8 weather) : base(area, species, form, min, max)
        {
            Weather = weather;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (Location is 30 or 54 && !((EncounterArea8)Area).PermitCrossover && (Weather & AreaWeather8.Fishing) == 0)
                ((PK8)pk).RibbonMarkCurry = true;
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

            bool curry = pk is IRibbonSetMark8 {RibbonMarkCurry: true} || (pk.Species == (int)Core.Species.Shedinja && pk is PK8 {AffixedRibbon:(int)RibbonIndex.MarkCurry});
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
            var flawless = GetFlawlessIVCount();
            return Overworld8RNG.ValidateOverworldEncounter(pk, flawless: flawless);
        }

        private int GetFlawlessIVCount()
        {
            const int none = 0;
            const int any023 = -1;

            var area = (EncounterArea8) Area;
            if (area.PermitCrossover)
                return any023; // Symbol
            if ((Weather & AreaWeather8.Fishing) != 0)
                return any023; // Fishing
            return none; // Hidden
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            // Glimwood Tangle does not spawn Symbol encounters, only Hidden.
            if (Location is 76 && ((EncounterArea8)Area).PermitCrossover)
                return EncounterMatchRating.PartialMatch;

            bool isHidden = pkm.AbilityNumber == 4;
            if (isHidden && this.IsPartialMatchHidden(pkm.Species, Species))
                return EncounterMatchRating.PartialMatch;

            if (pkm is IRibbonSetMark8 m)
            {
                if (m.RibbonMarkCurry && (Weather & AreaWeather8.All) == 0)
                    return EncounterMatchRating.Deferred;
                if (m.RibbonMarkFishing && (Weather & AreaWeather8.Fishing) == 0)
                    return EncounterMatchRating.Deferred;

                // Galar Mine hidden encounters can only be found via Curry.
                if (Location is 30 or 54 && !((EncounterArea8)Area).PermitCrossover && !m.RibbonMarkCurry && (Weather & AreaWeather8.Fishing) == 0)
                    return EncounterMatchRating.PartialMatch;
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
