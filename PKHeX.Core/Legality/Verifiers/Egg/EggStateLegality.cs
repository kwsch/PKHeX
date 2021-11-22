namespace PKHeX.Core
{
    public static class EggStateLegality
    {
        public static int GetMinimumEggHatchCycles(PKM pkm) => pkm switch
        {
            PK7 => 0, // pelago can decrement to 0
            _ => 1, // whenever it hits 0, it hatches, so anything above that is fine.
        };

        public static bool IsValidHTEgg(PKM pkm) => pkm switch
        {
            PB8 { Met_Location: Locations.LinkTrade6NPC } pb8 when pb8.HT_Friendship == PersonalTable.BDSP[pb8.Species].BaseFriendship => true,
            _ => false,
        };
    }
}
