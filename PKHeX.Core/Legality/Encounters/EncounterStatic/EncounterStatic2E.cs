using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Event data for Generation 2
    /// </summary>
    /// <inheritdoc cref="EncounterStatic2"/>
    public sealed record EncounterStatic2E : EncounterStatic2, IFixedGBLanguage
    {
        public EncounterGBLanguage Language { get; init; } = EncounterGBLanguage.Japanese;

        /// <summary> Trainer name for the event. </summary>
        public string OT_Name { get; init; } = string.Empty;

        public IReadOnlyList<string> OT_Names { get; init; } = Array.Empty<string>();

        /// <summary> Trainer ID for the event. </summary>
        public int TID { get; init; } = -1;

        public int CurrentLevel { get; init; } = -1;

        public EncounterStatic2E(int species, int level, GameVersion ver) : base(species, level, ver)
        {
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatchExact(pkm, evo))
                return false;

            if (Language != EncounterGBLanguage.Any && pkm.Japanese != (Language == EncounterGBLanguage.Japanese))
                return false;

            if (CurrentLevel != -1 && CurrentLevel > pkm.CurrentLevel)
                return false;

            // EC/PID check doesn't exist for these, so check Shiny state here.
            if (!IsShinyValid(pkm))
                return false;

            if (EggEncounter && !pkm.IsEgg)
                return true;

            // Check OT Details
            if (TID != -1 && pkm.TID != TID)
                return false;

            if (OT_Name.Length != 0)
            {
                if (pkm.OT_Name != OT_Name)
                    return false;
            }
            else if (OT_Names.Count != 0)
            {
                if (!OT_Names.Contains(pkm.OT_Name))
                    return false;
            }

            return true;
        }

        private bool IsShinyValid(PKM pkm) => Shiny switch
        {
            Shiny.Never => !pkm.IsShiny,
            Shiny.Always => pkm.IsShiny,
            _ => true
        };

        protected override int GetMinimalLevel() => CurrentLevel == -1 ? base.GetMinimalLevel() : CurrentLevel;

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (CurrentLevel != -1) // Restore met level
                pk.Met_Level = LevelMin;

            if (TID != -1)
                pk.TID = TID;

            if (OT_Name.Length != 0)
                pk.OT_Name = OT_Name;
            else if (OT_Names.Count != 0)
                pk.OT_Name = OT_Names[Util.Rand.Next(OT_Names.Count)];
        }
    }
}
