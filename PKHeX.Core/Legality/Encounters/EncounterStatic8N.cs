using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Nest Encounter (Raid)
    /// </summary>
    public sealed class EncounterStatic8N : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }

        private readonly uint MinRank;
        private readonly uint MaxRank;
        private readonly byte NestID;

        private const int OnlineNest = 162;

        private IReadOnlyList<byte> NestLocations => Encounters8Nest.NestLocations[NestID];

        public override int Location { get => OnlineNest; set { } }

        public EncounterStatic8N(byte nestID, uint minRank, uint maxRank, byte val)
        {
            NestID = nestID;
            MinRank = minRank;
            MaxRank = maxRank;
            DynamaxLevel = val;
        }

        private readonly int[] LevelCaps =
        {
            15, 20, // 0
            25, 30, // 1
            35, 40, // 2
            45, 50, // 3
            55, 60, // 4
        };

        protected override int GetMinimalLevel() => LevelCaps[MinRank * 2];

        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            var metLevel = pkm.Met_Level - 15;
            var rank = (uint)(metLevel / 10);
            if (rank > 4)
                return false;
            if (rank < MinRank || MaxRank < rank)
                return false;

            return metLevel % 10 <= 5;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            var loc = pkm.Met_Location;
            return loc == OnlineNest || loc <= 255 && NestLocations.Contains((byte)loc);
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            if (pkm.FlawlessIVCount < MinRank + 1)
                return false;

            return base.IsMatch(pkm, lvl);
        }

        public override bool IsMatchDeferred(PKM pkm)
        {
            if (base.IsMatchDeferred(pkm))
                return true;
            if (Ability == Encounters8Nest.A3 && pkm.AbilityNumber == 4)
                return false;
            if (pkm is IGigantamax g && g.CanGigantamax != CanGigantamax)
                return true;

            return false;
        }
    }
}
