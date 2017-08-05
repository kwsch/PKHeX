namespace PKHeX.Core
{
    public enum SlotType
    {
        Any,
        Grass,
        Rough_Terrain,
        Yellow_Flowers,
        Purple_Flowers,
        Red_Flowers,
        Surf,
        Old_Rod,
        Good_Rod,
        Super_Rod,
        Rock_Smash,
        Horde,
        FriendSafari,
        Special,
        SOS,
        Swarm,
        Headbutt,
        Headbutt_Special,
        Pokeradar,
        HoneyTree,
        HiddenGrotto,
        BugContest,
        Grass_Safari,
        Surf_Safari,
        Old_Rod_Safari,
        Good_Rod_Safari,
        Super_Rod_Safari,
        Rock_Smash_Safari,
        Pokeradar_Safari
    }

    public static partial class Extensions
    {
        internal static SlotType GetSafariSlotType3(this SlotType t)
        {
            switch (t)
            {
                case SlotType.Grass: return SlotType.Grass_Safari;
                case SlotType.Surf: return SlotType.Surf_Safari;
                case SlotType.Old_Rod: return SlotType.Old_Rod_Safari;
                case SlotType.Good_Rod: return SlotType.Good_Rod_Safari;
                case SlotType.Super_Rod: return SlotType.Super_Rod_Safari;
                case SlotType.Rock_Smash: return SlotType.Rock_Smash_Safari;
                default: return t;
            }
        }
        internal static SlotType GetSafariSlotType4(this SlotType t)
        {
            switch (t)
            {
                case SlotType.Grass: return SlotType.Grass_Safari;
                case SlotType.Surf: return SlotType.Surf_Safari;
                case SlotType.Old_Rod: return SlotType.Old_Rod_Safari;
                case SlotType.Good_Rod: return SlotType.Good_Rod_Safari;
                case SlotType.Super_Rod: return SlotType.Super_Rod_Safari;
                case SlotType.Pokeradar: return SlotType.Pokeradar_Safari;
                default: return t;
            }
        }
        internal static bool IsFishingRodType(this SlotType t)
        {
            switch (t)
            {
                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari:
                    return true;
                default: return false;
            }
        }
    }
}
