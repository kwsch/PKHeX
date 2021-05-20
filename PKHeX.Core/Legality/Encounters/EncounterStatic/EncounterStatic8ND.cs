using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Distributed Data)
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed record EncounterStatic8ND : EncounterStatic8Nest<EncounterStatic8ND>
    {
        /// <summary>
        /// Distribution raid index for <see cref="GameVersion.SWSH"/>
        /// </summary>
        public byte Index { get; init; }

        public EncounterStatic8ND(byte lvl, byte dyna, byte flawless, GameVersion game = GameVersion.SWSH) : base(game)
        {
            Level = lvl;
            DynamaxLevel = dyna;
            FlawlessIVCount = flawless;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            var lvl = pkm.Met_Level;

            if (lvl <= 25) // 1 or 2 stars
            {
                if (InaccessibleRank12DistributionLocations.Contains(pkm.Met_Location))
                    return false;
            }

            if (lvl == Level)
                return true;

            // Check downleveled (20-55)
            if (lvl > Level)
                return false;
            if (lvl is < 20 or > 55)
                return false;

            if (lvl % 5 != 0)
                return false;

            // shared nests can be down-leveled to any
            if (pkm.Met_Location == SharedNest)
                return true;

            // native down-levels: only allow 1 rank down (1 badge 2star -> 25), (3badge 3star -> 35)
            var badges = (lvl - 20) / 5;
            return badges is 1 or 3 && !pkm.IsShiny;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc is SharedNest || Index switch
            {
                >= 40 => EncounterArea8.IsWildArea(loc),
                >= 25 => EncounterArea8.IsWildArea8(loc) || EncounterArea8.IsWildArea8Armor(loc),
                _ => EncounterArea8.IsWildArea8(loc),
            };
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatchExact(pkm, evo);
        }
    }
}
