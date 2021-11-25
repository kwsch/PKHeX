namespace PKHeX.Core
{
    public static class EggStateLegality
    {
        public static bool GetIsEggHatchCyclesValid(PKM pk, IEncounterTemplate enc)
        {
            var hatchCounter = pk.OT_Friendship;
            var max = GetMaximumEggHatchCycles(pk, enc);
            if (hatchCounter > max)
                return false;
            var min = GetMinimumEggHatchCycles(pk);
            if (hatchCounter < min)
                return false;

            return true;
        }

        public static int GetMinimumEggHatchCycles(PKM pk) => pk switch
        {
            PK7 => 0, // pelago can decrement to 0
            _ => 1, // whenever it hits 0, it hatches, so anything above that is fine.
        };

        public static int GetMaximumEggHatchCycles(PKM pk)
        {
            var la = new LegalityAnalysis(pk);
            var enc = la.EncounterMatch;
            return GetMaximumEggHatchCycles(pk, enc);
        }

        public static int GetMaximumEggHatchCycles(PKM pk, IEncounterTemplate enc)
        {
            if (enc is EncounterStatic { EggCycles: not 0 } s)
                return s.EggCycles;
            return pk.PersonalInfo.HatchCycles;
        }

        public static bool IsValidHTEgg(PKM pk) => pk switch
        {
            PB8 { Met_Location: Locations.LinkTrade6NPC } pb8 when pb8.HT_Friendship == PersonalTable.BDSP[pb8.Species].BaseFriendship => true,
            _ => false,
        };

        public static bool IsNicknameFlagSet(IEncounterTemplate enc) => enc switch
        {
            EncounterStatic7 => false,
            WB8 or EncounterStatic8b => false,
            { Generation: 4 } => false,
            _ => true,
        };

        public static bool IsNicknameFlagSet(PKM pk) => IsNicknameFlagSet(new LegalityAnalysis(pk).EncounterMatch);
    }
}
