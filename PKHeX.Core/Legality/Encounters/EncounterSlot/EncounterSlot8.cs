using System.Collections.Generic;
using static PKHeX.Core.AreaWeather8;
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

        /// <summary>
        /// Location IDs matched with possible weather types. Unlisted locations may only have Normal weather.
        /// </summary>
        internal static readonly Dictionary<int, AreaWeather8> WeatherbyArea = new()
        {
            { 68, Intense_Sun }, // Route 6
            { 88, Snowing }, // Route 8 (Steamdrift Way)
            { 90, Snowing }, // Route 9
            { 92, Snowing }, // Route 9 (Circhester Bay)
            { 94, Overcast }, // Route 9 (Outer Spikemuth)
            { 106, Snowstorm }, // Route 10
            { 122, All }, // Rolling Fields
            { 124, All }, // Dappled Grove
            { 126, All }, // Watchtower Ruins
            { 128, All }, // East Lake Axewell
            { 130, All }, // West Lake Axewell
            { 132, All }, // Axew's Eye
            { 134, All }, // South Lake Miloch
            { 136, All }, // Giant's Seat
            { 138, All }, // North Lake Miloch
            { 140, All }, // Motostoke Riverbank
            { 142, All }, // Bridge Field
            { 144, All }, // Stony Wilderness
            { 146, All }, // Dusty Bowl
            { 148, All }, // Giant's Mirror
            { 150, All }, // Hammerlocke Hills
            { 152, All }, // Giant's Cap
            { 154, All }, // Lake of Outrage
            { 164, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Fields of Honor
            { 166, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Soothing Wetlands
            { 168, All_IoA }, // Forest of Focus
            { 170, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Challenge Beach
            { 174, All_IoA }, // Challenge Road
            { 178, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Loop Lagoon
            { 180, All_IoA }, // Training Lowlands
            { 184, Normal | Overcast | Raining | Sandstorm | Intense_Sun | Heavy_Fog }, // Potbottom Desert
            { 186, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Workout Sea
            { 188, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Stepping-Stone Sea
            { 190, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Insular Sea
            { 192, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Honeycalm Sea
            { 194, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Honeycalm Island
            { 204, Normal | Overcast | Icy | Intense_Sun | Heavy_Fog }, // Slippery Slope
            { 208, Normal | Overcast | Icy | Intense_Sun | Heavy_Fog }, // Frostpoint Field
            { 210, All_CT }, // Giant's Bed
            { 212, All_CT }, // Old Cemetery
            { 214, Normal | Overcast | Icy | Intense_Sun | Heavy_Fog }, // Snowslide Slope
            { 216, Overcast }, // Tunnel to the Top
            { 218, Normal | Overcast | Icy | Intense_Sun | Heavy_Fog }, // Path to the Peak
            { 222, All_CT }, // Giant's Foot
            { 224, Overcast }, // Roaring-Sea Caves
            { 226, No_Sun_Sand }, // Frigid Sea
            { 228, All_CT }, // Three-Point Pass
            { 230, All_Ballimere }, // Ballimere Lake
            { 232, Overcast }, // Lakeside Cave
        };

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            bool symbol = ((EncounterArea8)Area).PermitCrossover;
            var c = symbol ? EncounterCriteria.Unrestricted : criteria;
            if (!symbol && Location is 30 or 54 && (Weather & AreaWeather8.Fishing) == 0)
                ((PK8)pk).RibbonMarkCurry = true;

            base.ApplyDetails(sav, c, pk);
            var req = GetRequirement(pk);
            if (req != MustHave)
            {
                pk.SetRandomEC();
                return;
            }
            if (symbol)
                Overworld8RNG.ApplyDetails(pk, c, c.Shiny);
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
