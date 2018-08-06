using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class Records
    {
        private const int LargeRecordCount = 100;
        private const int SmallRecordCount = 100;
        private const int Count = LargeRecordCount + SmallRecordCount;

        /// <summary>
        /// Gets the maximum value for the specified record using the provided maximum list.
        /// </summary>
        /// <param name="recordID">Record ID to retrieve the maximum for</param>
        /// <param name="maxes">Maximum enum values for each record</param>
        /// <returns>Maximum the record can be</returns>
        public static int GetMax(int recordID, IReadOnlyList<int> maxes)
        {
            if (recordID >= Count)
                return 0;
            return MaxByType[maxes[recordID]];
        }

        public static int GetOffset(int baseOfs, int recordID)
        {
            if (recordID < LargeRecordCount)
                return baseOfs + (recordID * 4);
            if (recordID < Count)
                return baseOfs + (recordID * 2) + 200; // first 100 are 4bytes, so bias the difference
            return -1;
        }

        private static readonly int[] MaxByType = {999999999, 9999999, 999999, 99999, 65535, 9999, 999, 7};

        public static int[] SpecialIndexes_6 = {29, 30, 110, 111, 112, 113, 114, 115, 116, 117};
        public static int[] SpecialIndexes_7 = {22, 23, 110, 111, 112, 113, 114, 115, 116, 117};

        /// <summary>
        /// Festa pairs; if updating the lower index record, update the Festa Mission record if currently active?
        /// </summary>
        public static int[] FestaPairs_USUM =
        {
            175, 6,
            176, 33,
            177, 8,
            179, 38,
            181, 74,
            182, 73,
            183, 7,
            184, 159,
            185, 9,
        };

        public static readonly IReadOnlyList<int> MaxType_XY = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
            3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 6, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        };

        public static readonly IReadOnlyList<int> MaxType_AO = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
            3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 6, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            7, 7, 7, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        };

        public static readonly IReadOnlyList<int> MaxType_SM = new[]
        {
            0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        };

        public static readonly IReadOnlyList<int> MaxType_USUM = new[]
        {
            0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
            0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 4, 4, 4, 5, 5, 4, 5, 5
        };

        public static readonly Dictionary<int, string> RecordList_7 = new Dictionary<int, string>
        {
            {000, "Steps Taken"},
            {001, "Times Saved"},
            {002, "Storyline Completed Time"},
            {003, "Total Battles"},
            {004, "Wild Pokémon Battles"},
            {005, "Trainer Battles"},
            {006, "Pokemon Caught"},
            {007, "Pokemon Caught Fishing"},
            {008, "Eggs Hatched"},
            {009, "Pokémon Evolved"},
            {010, "Pokémon Healed at Pokémon Centers"},
            {011, "Link Trades"},
            {012, "Link Battles"},
            {013, "Link Battle Wins"},
            {014, "Link Battle Losses"},
            {015, "Battle Spot Battles"},
            {016, "Battle Spot Wins"},
            {017, "Battle Spot Losses"},
            {018, "Mart Stack Purchases"},
            {019, "Money Spent"},
            {020, "Pokémon deposited at Nursery"},
            {021, "Pokémon Defeated"},
            {022, "Exp. Points Collected (Highest)"},
            {023, "Exp. Points Collected (Today)"},
            {024, "Deposited in the GTS"},
            {025, "Nicknames Given"},
            {026, "Bonus Premier Balls Received"},
            {027, "Battle Points Earned"},
            {028, "Battle Points Spent"},
            {029, "Super Effective Moves Used"},
            {030, "Clothing Count"},
            {031, "Salon Uses"},
            {032, "Berry Harvests"},
            {033, "Trades at the GTS"},
            {034, "Wonder Trades"},
            {035, "Quick Links"},
            {036, "Pokemon Rides"},
            {037, "Beans Given"},
            {038, "Festival Coins Spent"},
            {039, "Poke Beans Collected"},
            {040, "Battle Tree Challenges"},
            {041, "Z-Moves Used"},
            {042, "Balls Used"},
            {043, "Items Thieved"},
            {044, "Moves Used"},
            {045, "Levels Raised"},
            {046, "Ran From Battles"},
            {047, "Rock Smash Items"},
            {048, "Medicine Used"},
            {050, "Total Thumbs-Ups"},
            {051, "Times Twirled (Pirouette)"},
            {052, "Record Thumbs-ups"},
            {053, "Pokemon Petted"},
            {054, "Poké Pelago Visits"},
            {055, "Poké Bean Trades"},
            {056, "Poké Pelago Tapped Pokémon"},
            {057, "Poké Pelago Bean Stacks put in Crate"},
            {058, "Poké Pelago Levels Gained"},
            {062, "Battle Video QR Teams Scanned"},
            {063, "Battle Videos Watched"},
            {064, "Battle Videos Rebattled"},
            {065, "RotomDex Interactions"},
            {066, "Guests Interacted With"},
            {067, "Berry Piles (not full) Collected"},
            {068, "Berry Piles (full) Collected"},
            {069, "Items Reeled In"},
            // USUM
            {070, "Roto Lotos"},
            {072, "Stickers Collected"},
            {073, "Mantine Surf BP Earned"},
            {074, "Battle Agency Wins"},

            {100, "Champion Title Defense"},
            {104, "Moves used with No Effect"},
            {105, "Own Fainted Pokémon"},
            {107, "Failed Run Attempts"},
            {109, "Failed Fishing Attempts"},
            {110, "Pokemon Defeated (Highest)"},
            {111, "Pokemon Defeated (Today)"},
            {112, "Pokemon Caught (Highest)"},
            {113, "Pokemon Caught (Today)"},
            {114, "Trainers Battled (Highest)"},
            {115, "Trainers Battled (Today)"},
            {116, "Pokemon Evolved (Highest)"},
            {117, "Pokemon Evolved (Today)"},
            {118, "Fossils Restored"},
            {119, "Photos Rated"},
            {120, "Best (Super) Singles Streak"},
            {121, "Best (Super) Doubles Streak"},
            {122, "Best (Super) Multi Streak"},
            {123, "Loto-ID Wins"},
            {124, "PP Raised"},
            {125, "Amie Used"},
            {126, "Fishing Chains"},
            {127, "Shiny Pokemon Encountered"},
            {128, "Missions Participated In"},
            {129, "Facilities Hosted"},
            {130, "QR Code Scans"},
            {131, "Moves learned with TMs"},
            {132, "Café Drinks Bought"},
            {133, "Trainer Card Photos Taken"},
            {134, "Evolutions Cancelled"},
            {135, "SOS Battle Allies Called"},
            {136, "Friendship Raised"},
            {137, "Battle Royal Dome Battles"},
            {138, "Items Picked Up after Battle"},
            {139, "Ate in Malasadas Shop"},
            {140, "Hyper Trainings Recieved"},
            {141, "Dishes eaten in Battle Buffet"},
            {142, "Pokémon Refresh Accessed"},
            {143, "Pokémon Storage System Log-outs"},
            {144, "Lomi Lomi Massages"},
            {145, "Times laid down in Ilima's Bed"},
            {146, "Times laid down in Guzma's Bed"},
            {147, "Times laid down in Kiawe's Bed"},
            {148, "Times laid down in Lana's Bed"},
            {149, "Times laid down in Mallow's Bed"},
            {150, "Times laid down in Olivia's Bed"},
            {151, "Times laid down in Hapu's Bed"},
            {152, "Times laid down in Lusamine's Bed"},
            {153, "Ambush/Smash post-battle items received"},
            {154, "Rustling Tree Encounters"},
            {155, "Ledges Jumped Down"},
            {156, "Water Splash Encounters"},
            {157, "Sand Cloud Encounters"},
            {158, "Outfit Changes"},
            {159, "Battle Royal Dome Wins"},
            {160, "Pelago Treasure Hunts"},
            {161, "Pelago Training Sessions"},
            {162, "Pelago Hot Spring Sessions"},
            {163, "Special QR 1"},
            {164, "Special QR 2"},
            {165, "Special QR Code Scans"},
            {166, "Island Scans"},
            {167, "Rustling Bush Encounters"},
            {168, "Fly Shadow Encounters"},
            {169, "Rustling Grass Encounters"},
            {170, "Dirt Cloud Encounters"},
            {171, "Wimpod Chases"},
            {172, "Berry Tree Battles won"},
            {173, "Bubbling Spot Encounters/Items"},
            {174, "Times laid down in Own Bed"},

            {175, "Catch a lot of Pokémon!"},
            {176, "Trade Pokémon at the GTS!"},
            {177, "Hatch a lot of Eggs!"},
            {178, "Harvest Poké Beans!"},
            {179, "Get high scores with your Poké Finder!"},
            {180, "Find Pokémon using Island Scan!"},
            {181, "Catch Crabrawler!"},
            {182, "Defend your Champion title!"},
            {183, "Fish Pokémon at rare spots!"},
            {185, "Try your luck!"},
            {186, "Get BP at the Battle Tree!"},
            {187, "Catch a lot of Pokémon!"},

            // USUM
            {188, "Ultra Wormhole Travels"},
            {189, "Mantine Surf Plays"},
            {190, "Photo Club Photos saved"},
            {191, "Battle Agency Battles"},
            {195, "Photo Club Sticker usage"},
            {196, "Photo Club Photo Shoots"},
            {197, "Highest Wormhole Travel Distance"},
            {198, "Highest Mantine Surf BP Earned"},
        };
    }
}
