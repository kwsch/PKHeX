using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Data)
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed record EncounterStatic8ND : EncounterStatic8Nest<EncounterStatic8ND>
    {
        public EncounterStatic8ND(byte lvl, byte dyna, byte flawless)
        {
            Level = lvl;
            DynamaxLevel = dyna;
            FlawlessIVCount = flawless;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == SharedNest || EncounterArea8.IsWildArea(loc);
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatch(pkm, evo);
        }
    }
}
