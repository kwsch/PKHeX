namespace PKHeX.Core
{
    /// <summary>
    /// Decoration and logic for Met Location IDs
    /// </summary>
    public static class Locations
    {
        public const int LinkTrade4 = 2002;
        public const int LinkTrade5 = 30003;
        public const int LinkTrade6 = 30002;
        public const int LinkGift6 = 30011;

        public const int Daycare4 = 2000;
        public const int Daycare5 = 60002;

        public const int LinkTrade2NPC = 126;
        public const int LinkTrade3NPC = 254;
        public const int LinkTrade4NPC = 2001;
        public const int LinkTrade5NPC = 30002;
        public const int LinkTrade6NPC = 30001;

        public const int PokeWalker4 = 233;
        public const int Ranger4 = 3001;
        public const int Faraway4 = 3002;

        /// <summary>
        /// Generation 3 -> Generation 4 Transfer Location (Pal Park)
        /// </summary>
        public const int Transfer3 = 0x37;

        /// <summary> Generation 4 -> Generation 5 Transfer Location (Poké Transporter) </summary>
        public const int Transfer4 = 30001;

        /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Celebi - Event not activated in Gen 5) </summary>
        public const int Transfer4_CelebiUnused = 30010;

        /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Celebi - Event activated in Gen 5) </summary>
        public const int Transfer4_CelebiUsed = 30011;

        /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Beast - Event not activated in Gen 5) </summary>
        public const int Transfer4_CrownUnused = 30012;

        /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Beast - Event activated in Gen 5) </summary>
        public const int Transfer4_CrownUsed = 30013;
        
        public static int TradedEggLocationNPC(int gen)
        {
            switch (gen)
            {
                case 1:
                case 2: return LinkTrade2NPC;
                case 3: return LinkTrade3NPC;
                case 4: return LinkTrade4NPC;
                case 5: return LinkTrade5NPC;
                default: return LinkTrade6NPC;
            }
        }

        public static int TradedEggLocation(int gen)
        {
            switch (gen)
            {
                case 4: return LinkTrade4;
                case 5: return LinkTrade5;
                default: return LinkTrade6;
            }
        }

        public static bool IsPtHGSSLocation(int location) => 111 < location && location < 2000;
        public static bool IsPtHGSSLocationEgg(int location) => 2010 < location && location < 3000;
        public static bool IsEventLocation5(int location) => 40000 < location && location < 50000;
    }
}
