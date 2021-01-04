using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Crystal Data)
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed record EncounterStatic8NC : EncounterStatic8Nest<EncounterStatic8NC>
    {
        public EncounterStatic8NC(GameVersion game) : base(game) { }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc is SharedNest or Watchtower;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            var lvl = pkm.Met_Level;
            if (lvl == Level)
                return true;

            // Check downleveled (20-55)
            if (lvl > Level)
                return false;
            if (lvl is < 20 or > 55)
                return false;
            return lvl % 5 == 0;
        }
    }
}
