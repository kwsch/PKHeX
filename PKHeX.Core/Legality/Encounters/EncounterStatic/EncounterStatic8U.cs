using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Max Raid) Underground
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed class EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>
    {
        public override int Location { get => MaxLair; set { } }

        public EncounterStatic8U(int species, int form, int level)
        {
            Species = species;
            Form = form;
            Level = level;
            DynamaxLevel = 8;
            FlawlessIVCount = 4;
            Version = GameVersion.SWSH; // no difference in met location for hosted raids
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatch(pkm, evo);
        }

        // no downleveling, unlike all other raids
        protected override bool IsMatchLevel(PKM pkm, DexLevel evo) => pkm.Met_Level == Level;
    }
}
