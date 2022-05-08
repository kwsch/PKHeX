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

        public EncounterStatic8U(ushort species, byte form, byte level) : base(GameVersion.SWSH) // no difference in met location for hosted raids
        {
            Species = species;
            Form = form;
            Level = level;
            DynamaxLevel = 8;
            FlawlessIVCount = 4;
        }

        public override bool IsMatchExact(PKM pkm, EvoCriteria evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatchExact(pkm, evo);
        }

        // no downleveling, unlike all other raids
        protected override bool IsMatchLevel(PKM pkm, EvoCriteria evo) => pkm.Met_Level == Level;
    }
}
