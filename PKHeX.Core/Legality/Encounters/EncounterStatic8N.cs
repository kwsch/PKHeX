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

        public EncounterStatic8N(int loc, uint minRank, uint maxRank, byte val)
        {
            Location = loc;
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

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (Ability != Encounters8Nest.A4 && pkm.AbilityNumber != 4)
                return false;
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            if (pkm.GetFlawlessIVCount() < DynamaxLevel)
                return false;

            return base.IsMatch(pkm, lvl);
        }
    }
}