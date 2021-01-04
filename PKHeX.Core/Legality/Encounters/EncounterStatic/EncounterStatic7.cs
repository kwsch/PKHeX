using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic7 : EncounterStatic, IRelearn
    {
        public override int Generation => 7;
        public IReadOnlyList<int> Relearn { get; init; } = Array.Empty<int>();

        public EncounterStatic7(GameVersion game) : base(game) { }

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

            if (FormInfo.IsTotemForm(Species, Form, Generation))
            {
                var expectForm = pkm.Format == 7 ? Form : FormInfo.GetTotemBaseForm(Species, Form);
                return expectForm == evo.Form;
            }

            return Form == evo.Form || FormInfo.IsFormChangeable(Species, Form, pkm.Form, pkm.Format);
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            if (Species == (int)Core.Species.Magearna && pk is IRibbonSetEvent4 e4)
                e4.RibbonWishing = true;
        }

        internal static EncounterStatic7 GetVC1(int species, int metLevel)
        {
            bool mew = species == (int)Core.Species.Mew;
            return new EncounterStatic7(GameVersion.RBY)
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = Legal.TransferSpeciesDefaultAbilityGen1(species) ? 1 : 4, // Hidden by default, else first
                Shiny = mew ? Shiny.Never : Shiny.Random,
                Fateful = mew,
                Location = Locations.Transfer1,
                Level = metLevel,
                FlawlessIVCount = mew ? 5 : 3,
            };
        }

        internal static EncounterStatic7 GetVC2(int species, int metLevel)
        {
            bool mew = species == (int)Core.Species.Mew;
            bool fateful = mew || species == (int)Core.Species.Celebi;
            return new EncounterStatic7(GameVersion.GSC)
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = Legal.TransferSpeciesDefaultAbilityGen2(species) ? 1 : 4, // Hidden by default, else first
                Shiny = mew ? Shiny.Never : Shiny.Random,
                Fateful = fateful,
                Location = Locations.Transfer2,
                Level = metLevel,
                FlawlessIVCount = fateful ? 5 : 3
            };
        }
    }
}
