using System;
using System.Collections.Generic;
using static PKHeX.Core.OverworldCorrelation8Requirement;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public record EncounterStatic8 : EncounterStatic, IDynamaxLevel, IGigantamax, IRelearn, IOverworldCorrelation8
    {
        public sealed override int Generation => 8;
        public bool ScriptedNoMarks { get; init; }
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }
        public IReadOnlyList<int> Relearn { get; init; } = Array.Empty<int>();

        public AreaWeather8 Weather {get; init; } = AreaWeather8.Normal;

        public EncounterStatic8(GameVersion game) : base(game) { }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            var met = pkm.Met_Level;
            var lvl = Level;
            if (met == lvl)
                return true;
            if (lvl < 60 && EncounterArea8.IsBoostedArea60(Location))
                return met == 60;
            return false;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;
            return base.IsMatchExact(pkm, evo);
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
            var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
            Overworld8RNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
        }

        public bool IsOverworldCorrelation
        {
            get
            {
                if (Gift)
                    return false; // gifts can have any 128bit seed from overworld
                if (ScriptedNoMarks)
                    return false;  // scripted encounters don't act as saved spawned overworld encounters
                return true;
            }
        }

        public OverworldCorrelation8Requirement GetRequirement(PKM pk) => IsOverworldCorrelation
            ? MustHave
            : MustNotHave;

        public bool IsOverworldCorrelationCorrect(PKM pk)
        {
            return Overworld8RNG.ValidateOverworldEncounter(pk, Shiny == Shiny.Random ? Shiny.FixedValue : Shiny, FlawlessIVCount);
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            var rating = base.GetMatchRating(pkm);
            if (rating != EncounterMatchRating.Match)
                return rating;

            var req = GetRequirement(pkm);
            bool correlation = IsOverworldCorrelationCorrect(pkm);
            if ((req == MustHave) != correlation)
                return EncounterMatchRating.Deferred;

            // Only encounter slots can have these marks; defer for collisions.
            if (pkm.Species == (int) Core.Species.Shedinja)
            {
                // Loses Mark on evolution to Shedinja, but not affixed ribbon value.
                return pkm switch
                {
                    IRibbonSetMark8 {RibbonMarkCurry: true} => EncounterMatchRating.Deferred,
                    PK8 {AffixedRibbon: (int) RibbonIndex.MarkCurry} => EncounterMatchRating.Deferred,
                    _ => EncounterMatchRating.Match
                };
            }

            if (pkm is IRibbonSetMark8 m && (m.RibbonMarkCurry || m.RibbonMarkFishing))
                return EncounterMatchRating.Deferred;

            return EncounterMatchRating.Match;
        }
    }

    public interface IOverworldCorrelation8
    {
        OverworldCorrelation8Requirement GetRequirement(PKM pk);
        bool IsOverworldCorrelationCorrect(PKM pk);
    }

    public enum OverworldCorrelation8Requirement
    {
        CanBeEither,
        MustHave,
        MustNotHave,
    }
}
