using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Raid)
    /// </summary>
    public sealed class EncounterStatic8N : EncounterStatic8Nest<EncounterStatic8N>
    {
        private readonly uint MinRank;
        private readonly uint MaxRank;
        private readonly byte NestID;

        private IReadOnlyList<byte> NestLocations => Encounters8Nest.NestLocations[NestID];

        public override int Location { get => SharedNest; set { } }
        public override int Level { get => LevelMin; set { } }
        public override int LevelMin => LevelCaps[MinRank * 2];
        public override int LevelMax => LevelCaps[(MaxRank * 2) + 1];

        public EncounterStatic8N(byte nestID, uint minRank, uint maxRank, byte val)
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

        public static bool IsHighestLevelTier(int lvl) => ArrayUtil.WithinRange(lvl, 55, 60);

        protected override int GetMinimalLevel() => LevelCaps[MinRank * 2];

        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            var metLevel = pkm.Met_Level - 15;
            var rank = ((uint)metLevel) / 10;
            if (rank > 4)
                return false;
            if (rank > MaxRank)
                return false;
            if (rank < MinRank) // downleveled
            {
                if (metLevel % 5 != 0)
                    return false;

                // shared nests can be downleveled to any; native downlevels: only allow 1 rank down (?) 
                if (pkm.Met_Location != SharedNest && MinRank - rank > 1)
                    return false;
            }

            return metLevel % 10 <= 5;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == SharedNest || (loc <= 255 && NestLocations.Contains((byte)loc));
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm.FlawlessIVCount < FlawlessIVCount)
                return false;

            return base.IsMatch(pkm, lvl);
        }
    }
}
