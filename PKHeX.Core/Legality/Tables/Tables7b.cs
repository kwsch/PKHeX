using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_7b = 809; // Melmetal
        internal const int MaxMoveID_7b = 742; // Double Iron Bash
        internal const int MaxItemID_7b = 1057; // Magmar Candy
        internal const int MaxBallID_7b = (int)Ball.Beast;
        internal const int MaxGameID_7b = (int)GameVersion.GE;
        internal const int MaxAbilityID_7b = MaxAbilityID_7_USUM;
        internal static readonly ushort[] HeldItems_GG = new ushort[1];
        public const int AwakeningMax = 200;

        internal static readonly HashSet<int> WildPokeballs7b = new HashSet<int>
        {
            (int)Ball.Master, (int)Ball.Ultra, (int)Ball.Great, (int)Ball.Poke, (int)Ball.Premier,
        };

        #region Met Locations

        internal static readonly int[] Met_GG_0 =
        {
            002, // Invalid
            003, 004, 005, 006, 007, 008, 009,
            010, 011, 012, 012, 013, 014, 015, 016, 017, 018, 019,
            020, 021, 022, 022, 023, 024, 025, 026, 027, 028, 029,
            030, 031, 032, 032, 033, 034, 035, 036, 037, 038, 039,
            040, 041, 042, 042, 043, 044, 045, 046, 047, 048, 049,
            050, 051, 052, 053,
        };

        internal static readonly int[] Met_GG_3 =
        {
            30001, 30003, 30004, 30005, 30006, 30007, 30008, 30009, 30010, 30011, 30012, 30013, 30014, 30015, 30016, 30017
        };

        internal static readonly int[] Met_GG_4 =
        {
            40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009,
            40010, 40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019,
            40020, 40021, 40022, 40023, 40024, 40025, 40026, 40027, 40028, 40029,
            40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039,
            40040, 40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049,
            40050, 40051, 40052, 40053, 40054, 40055, 40056, 40057, 40058, 40059,
            40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069,
            40070, 40071, 40072, 40073, 40074, 40075, 40076, 40077,
        };

        internal static readonly int[] Met_GG_6 = {/* XY */ 60001, 60003, /* ORAS */ 60004, };

        #endregion

        #region Items

        internal static readonly ushort[] Pouch_Candy_GG_Regular =
        {
            050, // Rare Candy
            960, 961, 962, 963, 964, 965, // S
            966, 967, 968, 969, 970, 971, // L
            972, 973, 974, 975, 976, 977, // XL
        };

        internal static readonly ushort[] Pouch_Candy_GG_Species =
        {
            978, 979,
            980, 981, 982, 983, 984, 985, 986, 987, 988, 989,
            990, 991, 992, 993, 994, 995, 996, 997, 998, 999,
            1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009,
            1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019,
            1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029,
            1030, 1031, 1032, 1033, 1034, 1035, 1036, 1037, 1038, 1039,
            1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049,
            1050, 1051, 1052, 1053, 1054, 1055, 1056,
            1057,
        };

        internal static readonly ushort[] Pouch_Candy_GG = Pouch_Candy_GG_Regular.Concat(Pouch_Candy_GG_Species).ToArray();

        internal static readonly ushort[] Pouch_Medicine_GG =
        {
            017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 038, 039, 040, 041, 709, 903,
        };

        internal static readonly ushort[] Pouch_TM_GG =
        {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
            338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
            348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
            358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
            368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
            378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        };

        internal static readonly ushort[] Pouch_PowerUp_GG =
        {
            051, 053, 081, 082, 083, 084, 085,
            849,
        };

        internal static readonly ushort[] Pouch_Catching_GG =
            {
            001, 002, 003, 004, 012, 164, 166, 168,
            861, 862, 863, 864, 865, 866,
        };

        internal static readonly ushort[] Pouch_Battle_GG =
            {
            055, 056, 057, 058, 059, 060, 061, 062,
            656, 659, 660, 661, 662, 663, 671, 672, 675, 676, 678, 679,
            760, 762, 770, 773,
        };

        internal static readonly ushort[] Pouch_Regular_GG =
        {
            076, 077, 078, 079, 086, 087, 088, 089,
            090, 091, 092, 093, 101, 102, 103, 113, 115,
            121, 122, 123, 124, 125, 126, 127, 128,
            442,
            571,
            632, 651,
            795, 796,
            872, 873, 874, 875, 876, 877, 878, 885, 886, 887, 888, 889, 890, 891, 892, 893, 894, 895, 896, 900, 901, 902,
        };

        internal static readonly ushort[] Pouch_Regular_GG_Item =
        {
            076, // Super Repel
            077, // Max Repel
            078, // Escape Rope
            079, // Repel
            086, // Tiny Mushroom
            087, // Big Mushroom
            088, // Pearl
            089, // Big Pearl
            090, // Stardust
            091, // Star Piece
            092, // Nugget
            093, // Heart Scale

            571, // Pretty Wing
            795, // Bottle Cap
            796, // Gold Bottle Cap

            900, // Lure
            901, // Super Lure
            902, // Max Lure
        };

        internal static readonly ushort[] Pouch_Regular_GG_Key =
        {
            113, // Tea
            115, // Autograph
            121, // Pokémon Box
            122, // Medicine Pocket
            123, // TM Case
            124, // Candy Jar
            125, // Power-Up Pocket
            126, // Clothing Trunk
            127, // Catching Pocket
            128, // Battle Pocket
            442, // Town Map
            632, // Shiny Charm
            651, // Poké Flute

            872, // Secret Key
            873, // S.S. Ticket
            874, // Silph Scope
            875, // Parcel
            876, // Card Key
            877, // Gold Teeth
            878, // Lift Key
            885, // Stretchy Spring
            886, // Chalky Stone
            887, // Marble
            888, // Lone Earring
            889, // Beach Glass
            890, // Gold Leaf
            891, // Silver Leaf
            892, // Polished Mud Ball
            893, // Tropical Shell
            894, // Leaf Letter (P)
            895, // Leaf Letter (E)
            896, // Small Bouquet
        };

        #endregion

        #region Moves

        internal static readonly byte[] MovePP_GG =
        {
            // Absorb: 25 -> 15 (damage buffed from 20->40)
            // Mega Drain: 15 -> 10 (damage buffed from 40->75)

            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 20, 30, 35, 35, 20, 15, 20, 20, 25, 20, 30, 05, 10, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 10, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 20, 20, 20, 20, 15, 15, 10, 10, 20, 25, 10, 35, 30, 15, 10, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 10, 30, 10, 20, 10, 40, 40, 20, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 15, 20, 10, 15, 35, 20, 15, 10, 10, 30, 15, 40, 20, 10, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            20, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 25, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 10,
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 10, 15, 15,
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15,
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 15, 05, 40, 15, 20, 20, 05, 15, 20, 20, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 25, 15, 20, 15, 20, 15, 20, 10,
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 15, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15,
            10, 15, 15, 15, 10, 10, 10, 20, 10, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20,
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05, 15, 10, 10, 10, 10, 10, 10, 15, 20, 15, 10, 15, 10, 15, 10, 20, 10, 10, 10, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 15, 20, 15, 10,
            15, 15, 15, 15, 10, 10, 10, 10, 10, 15, 15, 15, 15, 05, 05, 15, 05, 10, 10, 10, 20, 20, 20, 10, 10, 30, 15, 15, 10, 15, 25, 10, 15, 10, 10, 10, 20, 10, 10, 10, 10, 10, 15, 15, 05, 05, 10, 10, 10, 05,
            05, 10, 05, 05, 15, 10, 05, 05, 05, 10, 10, 10, 10, 20, 25, 10, 20, 30, 25, 20, 20, 15, 20, 15, 20, 20, 10, 10, 10, 10, 10, 20, 10, 30, 15, 10, 10, 10, 20, 20, 05, 05, 05, 20, 10, 10, 20, 15, 20, 20,
            10, 20, 30, 10, 10, 40, 40, 30, 20, 40, 20, 20, 10, 10, 10, 10, 05, 10, 10, 05, 05, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01, 01,
            01, 01, 01, 01, 01, 01, 01, 01, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 40, 15, 20, 30, 20, 15, 15, 20, 10, 15, 15, 10, 05, 10, 10, 20, 15, 10, 15, 15, 15, 05, 15, 20, 20, 01, 01, 01, 01, 01, 01,
            01, 01, 01, 05, 05, 10, 10, 10, 20, 10, 10, 10, 05, 05, 20, 10, 10, 10, 01, 05, 15, 05, 01, 01, 01, 01, 01, 01, 15, 15, 15, 20, 15, 15, 15, 15, 15, 15, 15, 15, 20, 05,
        };

        internal static readonly int[] TMHM_GG =
        {
            029, 269, 270, 100, 156, 113, 182, 164, 115, 091,
            261, 263, 280, 019, 069, 086, 525, 369, 231, 399,
            492, 157, 009, 404, 127, 398, 092, 161, 503, 339,
            007, 605, 347, 406, 008, 085, 053, 087, 200, 094,
            089, 120, 247, 583, 076, 126, 057, 063, 276, 355,
            059, 188, 072, 430, 058, 446, 006, 529, 138, 224,
            // rest are same as SM, unused

            // No HMs
        };

        internal static readonly int[] Tutor_StarterPikachu =
        {
            729, // Zippy Zap
            730, // Splishy Splash
            731, // Floaty Fall
            // 732, // Pika Papow -- Joycon Shake
        };

        internal static readonly int[] Tutor_StarterEevee =
        {
            733, // Bouncy Bubble
            734, // Buzzy Buzz
            735, // Sizzly Slide
            736, // Glitzy Glow
            737, // Baddy Bad
            738, // Sappy Seed
            739, // Freezy Frost
            740, // Sparkly Swirl
            // 741, // Veevee Volley -- Joycon Shake
        };

        #endregion

        internal static readonly HashSet<int> GoTransferSpeciesShinyBan = new HashSet<int>
        {
            013, // Weedle
            014, // Kakuna
            015, // Beedrill
            021, // Spearow
            022, // Fearow
            023, // Ekans
            024, // Arbok
            032, // Nidoran-M
            033, // Nidorino
            034, // Nidoking
            037, // Vulpix
            038, // Ninetales
            041, // Zubat
            042, // Golbat
            043, // Oddish
            044, // Gloom
            045, // Vileplume
            046, // Paras
            047, // Parasect
            048, // Venonat
            049, // Venomoth
            052, // Meowth
            053, // Persian
            060, // Poliwag
            061, // Poliwhirl
            062, // Poliwrath
            063, // Abra
            064, // Kadabra
            065, // Alakazam
            069, // Bellsprout
            070, // Weepinbell
            071, // Victreebel
            072, // Tentacool
            073, // Tentacruel
            079, // Slowpoke
            080, // Slowbro
            083, // Farfetch'd
            084, // Doduo
            085, // Dodrio
            095, // Onix
            100, // Voltorb
            101, // Electrode
            102, // Exeggcute
            103, // Exeggutor
            106, // Hitmonlee
            107, // Hitmonchan
            108, // Lickitung
            109, // Koffing
            110, // Weezing
            111, // Rhyhorn
            112, // Rhydon
            113, // Chansey
            114, // Tangela
            115, // Kangaskhan
            116, // Horsea
            117, // Seadra
            118, // Goldeen
            119, // Seaking
            120, // Staryu
            121, // Starmie
            122, // Mr. Mime
            128, // Tauros
            132, // Ditto
            137, // Porygon
            143, // Snorlax
            150, // Mewtwo
            151, // Mew
        };

        internal static readonly HashSet<int> GoTransferSpeciesShinyBanAlola = new HashSet<int>
        {
            019, // Rattata
            020, // Raticate
            027, // Sandshrew
            028, // Sandslash
            037, // Vulpix
            038, // Ninetales
            050, // Diglett
            051, // Dugtrio
            052, // Meowth
            053, // Persian
            074, // Geodude
            075, // Graveler
            076, // Golem
            088, // Grimer
            089, // Muk
            103, // Exeggutor
        };
    }
}
