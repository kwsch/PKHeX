using System.Collections.Generic;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Max Raid) Underground
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed record EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>
    {
        public override int Location { get => MaxLair; init { } }

        public EncounterStatic8U(int species, int form, int level) : base(GameVersion.SWSH) // no difference in met location for hosted raids
        {
            Species = species;
            Form = form;
            Level = level;
            DynamaxLevel = 8;
            FlawlessIVCount = 4;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatchExact(pkm, evo);
        }

        // no downleveling, unlike all other raids
        protected override bool IsMatchLevel(PKM pkm, DexLevel evo) => pkm.Met_Level == Level;

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            ApplyScientistTrash(pk);
        }

        public void ApplyScientistTrash(PKM pk)
        {
            if (!ShouldHaveScientistTrash)
                return;
            if (!ScientistName.TryGetValue(pk.Language, out var name))
                return;
            var ot = pk.OT_Name;
            pk.OT_Name = name!;
            pk.OT_Name = ot;
        }

        public bool ShouldHaveScientistTrash => !(Legal.Legends.Contains(Species) || Legal.SubLegends.Contains(Species));

        public static bool? HasScientistTrash(PKM pk)
        {
            if (!ScientistName.TryGetValue(pk.Language, out var name))
                return false;

            var ot = pk.OT_Name;
            if (ot.Length + 1 >= name.Length)
                return null;

            return TrashBytes16.HasUnderlayer(pk.OT_Trash, name, ot);
        }

        private static readonly Dictionary<int, string> ScientistName = new()
        {
            {(int) LanguageID.Japanese, "けんきゅういん"},
            {(int) LanguageID.English, "Scientist"},
            {(int) LanguageID.French, "Scientifique"},
            {(int) LanguageID.Italian, "Scienziata"},
            {(int) LanguageID.German, "Forscherin"},
            {(int) LanguageID.Spanish, "Científica"},
            {(int) LanguageID.Korean, "연구원"},
            {(int) LanguageID.ChineseS, "研究员"},
            {(int) LanguageID.ChineseT, "研究員"},
        };
    }
}
