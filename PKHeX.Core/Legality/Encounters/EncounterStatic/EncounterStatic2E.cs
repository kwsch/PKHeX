using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Event data for Generation 2
    /// </summary>
    public sealed class EncounterStatic2E : EncounterStatic2
    {
        public EncounterGBLanguage Language { get; set; } = EncounterGBLanguage.Japanese;

        /// <summary> Trainer name for the event. </summary>
        public string OT_Name { get; set; } = string.Empty;

        public IReadOnlyList<string> OT_Names { get; set; } = Array.Empty<string>();

        /// <summary> Trainer ID for the event. </summary>
        public int TID { get; set; } = -1;

        public EncounterStatic2E(int species, int level, GameVersion ver) : base(species, level, ver)
        {
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatch(pkm, evo))
                return false;

            if (Language != EncounterGBLanguage.Any && pkm.Japanese != (Language == EncounterGBLanguage.Japanese))
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

            if (TID != -1 && pkm.TID != TID)
                return false;

            return true;
        }

        public override bool IsMatchDeferred(PKM pkm)
        {
            if (base.IsMatchDeferred(pkm))
                return true;
            return !ParseSettings.AllowGBCartEra;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            if (TID != -1)
                pk.TID = TID;

            if (OT_Name.Length != 0)
                pk.OT_Name = OT_Name;
            else if (OT_Names.Count != 0)
                pk.OT_Name = OT_Names[Util.Rand.Next(OT_Names.Count)];
        }
    }
}
