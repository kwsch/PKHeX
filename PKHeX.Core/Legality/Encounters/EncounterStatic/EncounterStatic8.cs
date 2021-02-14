using System;
using System.Collections.Generic;

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
            if (!HasOverworldCorrelation)
                pk.SetRandomEC();
        }

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            // be lazy and just do the regular, and overwrite with correlation if required
            base.SetPINGA(pk, criteria);
            if (!HasOverworldCorrelation)
                return;
            var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
            Overworld8RNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
        }

        public bool HasOverworldCorrelation
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

        public bool IsOverworldCorrelationCorrect(PKM pk)
        {
            return Overworld8RNG.ValidateOverworldEncounter(pk, Shiny == Shiny.Random ? Shiny.FixedValue : Shiny, FlawlessIVCount);
        }
    }

    public interface IOverworldCorrelation8
    {
        bool HasOverworldCorrelation { get; }
        bool IsOverworldCorrelationCorrect(PKM pk);
    }
}
