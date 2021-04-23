using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Regular Raid Dens)
    /// </summary>
    /// <inheritdoc cref="EncounterStatic8Nest{T}"/>
    public sealed record EncounterStatic8N : EncounterStatic8Nest<EncounterStatic8N>
    {
        private readonly uint MinRank;
        private readonly uint MaxRank;
        private readonly byte NestID;

        private IReadOnlyList<byte> NestLocations => Encounters8Nest.NestLocations[NestID];

        public override int Level { get => LevelMin; init { } }
        public override int LevelMin => LevelCaps[MinRank * 2];
        public override int LevelMax => LevelCaps[(MaxRank * 2) + 1];

        public EncounterStatic8N(byte nestID, uint minRank, uint maxRank, byte val, GameVersion game) : base(game)
        {
            NestID = nestID;
            MinRank = minRank;
            MaxRank = maxRank;
            DynamaxLevel = (byte)(MinRank + 1u);
            FlawlessIVCount = val;
        }

        private static readonly int[] LevelCaps =
        {
            15, 20, // 0
            25, 30, // 1
            35, 40, // 2
            45, 50, // 3
            55, 60, // 4
        };

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            var met = pkm.Met_Level;
            var metLevel = met - 15;
            var rank = ((uint)metLevel) / 10;
            if (rank > 4)
                return false;
            if (rank > MaxRank)
                return false;

            if (rank <= 1)
            {
                if (InaccessibleRank12Nests.TryGetValue(pkm.Met_Location, out var nests) && nests.Contains(NestID))
                    return false;
            }

            if (rank < MinRank) // down-leveled
                return IsDownLeveled(pkm, metLevel, met);

            return metLevel % 10 <= 5;
        }

        public bool IsDownLeveled(PKM pkm)
        {
            var met = pkm.Met_Level;
            var metLevel = met - 15;
            return met != LevelMax && IsDownLeveled(pkm, metLevel, met);
        }

        private bool IsDownLeveled(PKM pkm, int metLevel, int met)
        {
            if (metLevel % 5 != 0)
                return false;

            // shared nests can be down-leveled to any
            if (pkm.Met_Location == SharedNest)
                return met >= 20;

            // native down-levels: only allow 1 rank down (1 badge 2star -> 25), (3badge 3star -> 35)
            return ((MinRank <= 1 && 1 <= MaxRank && met == 25)
                 || (MinRank <= 2 && 2 <= MaxRank && met == 35)) && !pkm.IsShiny;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == SharedNest || (loc <= 255 && NestLocations.Contains((byte)loc));
        }

        public override bool IsMatchExact(PKM pkm, DexLevel evo)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatchExact(pkm, evo);
        }
    }
}
