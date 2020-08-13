using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class EncounterStatic7 : EncounterStatic, IRelearn
    {
        public IReadOnlyList<int> Relearn { get; set; } = Array.Empty<int>();
        public bool RibbonWishing { get; set; }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (EggLocation == Locations.Daycare5 && Relearn.Count == 0 && pkm.RelearnMove1 != 0) // Gift Eevee edge case
                return false;
            return base.IsMatchLocation(pkm);
        }

        protected override bool IsMatchForm(PKM pkm, DexLevel evo)
        {
            if (SkipFormCheck)
                return true;

            if (FormConverter.IsTotemForm(Species, Form, Generation))
            {
                var expectForm = pkm.Format == 7 ? Form : FormConverter.GetTotemBaseForm(Species, Form);
                return expectForm == evo.Form;
            }

            return Form == evo.Form || Legal.IsFormChangeable(Species, Form, pkm.Format);
        }

    }
}