using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    internal static class Encounters7
    {
        internal static readonly EncounterArea[] SlotsSN, SlotsMN;
        internal static readonly EncounterStatic[] StaticSN, StaticMN;

        static Encounters7()
        {
            StaticSN = GetStaticEncounters(Encounter_SM, GameVersion.SN);
            StaticMN = GetStaticEncounters(Encounter_SM, GameVersion.MN);

            var REG_SN = GetEncounterTables(GameVersion.SN);
            var REG_MN = GetEncounterTables(GameVersion.MN);
            var SOS_SN = GetEncounterTables("sm", "sn_sos");
            var SOS_MN = GetEncounterTables("sm", "mn_sos");
            MarkG7REGSlots(ref REG_SN);
            MarkG7REGSlots(ref REG_MN);
            MarkG7SMSlots(ref SOS_SN);
            MarkG7SMSlots(ref SOS_MN);
            SlotsSN = AddExtraTableSlots(REG_SN, SOS_SN, Encounter_Pelago_SM, Encounter_Pelago_SN);
            SlotsMN = AddExtraTableSlots(REG_MN, SOS_MN, Encounter_Pelago_SM, Encounter_Pelago_MN);
        }
        private static void MarkG7REGSlots(ref EncounterArea[] Areas)
        {
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG7SMSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas.SelectMany(area => area.Slots))
                s.Type = SlotType.SOS;
            ReduceAreasSize(ref Areas);
        }

        internal static readonly EncounterStatic[] Encounter_SM = // @ a\1\5\5
        {
            // Gifts - 0.bin
            new EncounterStatic { Gift = true, Species = 722, Level = 5,  Location = 24, }, // Rowlet
            new EncounterStatic { Gift = true, Species = 725, Level = 5,  Location = 24, }, // Litten
            new EncounterStatic { Gift = true, Species = 728, Level = 5,  Location = 24, }, // Popplio
            new EncounterStatic { Gift = true, Species = 138, Level = 15, Location = 58, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 15, Location = 58, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 15, Location = 58, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 15, Location = 58, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 15, Location = 58, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 15, Location = 58, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 15, Location = 58, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 15, Location = 58, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 15, Location = 58, }, // Archen
            new EncounterStatic { Gift = true, Species = 696, Level = 15, Location = 58, }, // Tyrunt
            new EncounterStatic { Gift = true, Species = 698, Level = 15, Location = 58, }, // Amaura
            new EncounterStatic { Gift = true, Species = 133, Level = 1,  EggLocation = 60002, }, // Eevee @ Nursery helpers
            new EncounterStatic { Gift = true, Species = 137, Level = 30, Location = 116, }, // Porygon @ Route 15
            new EncounterStatic { Gift = true, Species = 772, Level = 40, Location = 188, IV3 = true, }, // Type: Null
            new EncounterStatic { Gift = true, Species = 789, Level = 5,  Location = 142, Shiny = false, IV3 = true, Version = GameVersion.SN}, // Cosmog                00 FF
            new EncounterStatic { Gift = true, Species = 789, Level = 5,  Location = 144, Shiny = false, IV3 = true, Version = GameVersion.MN}, // Cosmog                00 FF
            new EncounterStatic { Gift = true, Species = 142, Level = 40, Location = 172, }, // Aerodactyl @ Seafolk Village

            new EncounterStatic { Gift = true, Species = 718, Form = 0, Level = 30, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 1, Level = 30, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 2, Level = 30, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 3, Level = 30, Shiny = false, Location = 118, IV3 = true, }, // Zygarde

            new EncounterStatic { Gift = true, Species = 718, Form = 0, Level = 50, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 1, Level = 50, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 2, Level = 50, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            new EncounterStatic { Gift = true, Species = 718, Form = 3, Level = 50, Shiny = false, Location = 118, IV3 = true, }, // Zygarde
            
            new EncounterStatic // Magearna (Bottle Cap) 00 FF
            {
                Gift = true, Species = 801, Level = 50, Location = 40001, Shiny = false, IV3 = true,
                Fateful = true, RibbonWishing = true, Relearn = new [] {705, 430, 381, 270}, Ball = 0x10, // Cherish
            },

            // Static Encounters - 1.bin
            new EncounterStatic { Species = 791, Level = 55, Relearn = new[]{713, 322, 242, 428}, Shiny = false, Ability = 1, Location = 176, IV3 = true, Version = GameVersion.SN }, // Solgaleo
            new EncounterStatic { Species = 792, Level = 55, Relearn = new[]{714, 322, 539, 247}, Shiny = false, Ability = 1, Location = 178, IV3 = true, Version = GameVersion.MN }, // Lunala

            new EncounterStatic { Species = 746, Level = 17, Shiny = false, Ability = 1, Location = 86, }, // Wishiwashi
            new EncounterStatic { Species = 746, Level = 18, Shiny = false, Ability = 1, Location = 86, }, // Wishiwashi

            new EncounterStatic { Species = 793, Level = 55, Shiny = false, Ability = 1, Location = 082, IV3 = true, }, // Nihilego @ Wela Volcano Park
            new EncounterStatic { Species = 793, Level = 55, Shiny = false, Ability = 1, Location = 100, IV3 = true, }, // Nihilego @ Diglett’s Tunnel
            new EncounterStatic { Species = 794, Level = 65, Shiny = false, Ability = 1, Location = 040, IV3 = true, Version = GameVersion.SN }, // Buzzwole @ Melemele Meadow
            new EncounterStatic { Species = 795, Level = 60, Shiny = false, Ability = 1, Location = 046, IV3 = true, Version = GameVersion.MN }, // Pheromosa @ Verdant Cavern (Trial Site)
            new EncounterStatic { Species = 796, Level = 65, Shiny = false, Ability = 1, Location = 090, IV3 = true, }, // Xurkitree @ Lush Jungle
            new EncounterStatic { Species = 796, Level = 65, Shiny = false, Ability = 1, Location = 076, IV3 = true, }, // Xurkitree @ Memorial Hill
            new EncounterStatic { Species = 798, Level = 60, Shiny = false, Ability = 1, Location = 134, IV3 = true, Version = GameVersion.SN }, // Kartana @ Malie Garden
            new EncounterStatic { Species = 798, Level = 60, Shiny = false, Ability = 1, Location = 120, IV3 = true, Version = GameVersion.SN }, // Kartana @ Route 17
            new EncounterStatic { Species = 797, Level = 65, Shiny = false, Ability = 1, Location = 124, IV3 = true, Version = GameVersion.MN }, // Celesteela @ Haina Desert
            new EncounterStatic { Species = 797, Level = 65, Shiny = false, Ability = 1, Location = 134, IV3 = true, Version = GameVersion.MN }, // Celesteela @ Malie Garden
            new EncounterStatic { Species = 799, Level = 70, Shiny = false, Ability = 1, Location = 182, IV3 = true, }, // Guzzlord @ Resolution Cave
            new EncounterStatic { Species = 800, Level = 75, Shiny = false, Ability = 1, Location = 036, IV3 = true, }, // Necrozma @ Ten Carat Hill (Farthest Hollow)
            
            // QR Scan: Su/M/Tu/W/Th/F/Sa
            // Melemele Island
            new EncounterStatic { Species = 155, Level = 12, Relearn = new[]{024, 052, 108, 043}, Location = 010, }, // Cyndaquil @ Route 3
            new EncounterStatic { Species = 158, Level = 12, Relearn = new[]{232, 099, 055, 043}, Location = 042, }, // Totodile @ Seaward Cave
            new EncounterStatic { Species = 633, Level = 13, Relearn = new[]{372, 029, 044, 116}, Location = 034, }, // Deino @ Ten Carat Hill
            new EncounterStatic { Species = 116, Level = 18, Relearn = new[]{225, 239, 055, 043}, Location = 014, }, // Horsea @ Kala'e Bay
            new EncounterStatic { Species = 599, Level = 08, Relearn = new[]{268, 011, 000, 000}, Location = 020, }, // Klink @ Hau'oli City
            new EncounterStatic { Species = 152, Level = 10, Relearn = new[]{073, 077, 075, 045}, Location = 012, }, // Chikorita @ Route 2
            new EncounterStatic { Species = 607, Level = 10, Relearn = new[]{051, 109, 083, 123}, Location = 038, }, // Litwick @ Hau'oli Cemetery
                                                                                                             
            // Akala Island                                                                                  
            new EncounterStatic { Species = 574, Level = 17, Relearn = new[]{399, 060, 003, 313}, Location = 054, }, // Gothita @ Route 6
            new EncounterStatic { Species = 363, Level = 19, Relearn = new[]{392, 362, 301, 227}, Location = 056, }, // Spheal @ Route 7
            new EncounterStatic { Species = 404, Level = 20, Relearn = new[]{598, 044, 209, 268}, Location = 058, }, // Luxio @ Route 8
            new EncounterStatic { Species = 679, Level = 23, Relearn = new[]{194, 332, 425, 475}, Location = 094, }, // Honedge @ Akala Outskirts
            new EncounterStatic { Species = 543, Level = 14, Relearn = new[]{390, 228, 103, 040}, Location = 050, }, // Venipede @ Route 4
            new EncounterStatic { Species = 069, Level = 16, Relearn = new[]{491, 077, 079, 035}, Location = 052, }, // Bellsprout @ Route 5
            new EncounterStatic { Species = 183, Level = 17, Relearn = new[]{453, 270, 061, 205}, Location = 086, }, // Marill @ Brooklet Hill
                                                                                                             
            // Ula'ula Island                                                                                
            new EncounterStatic { Species = 111, Level = 30, Relearn = new[]{130, 350, 498, 523}, Location = 138, }, // Rhyhorn @ Blush Mountain
            new EncounterStatic { Species = 220, Level = 31, Relearn = new[]{573, 036, 420, 196}, Location = 114, }, // Swinub @ Tapu Village
            new EncounterStatic { Species = 578, Level = 33, Relearn = new[]{101, 248, 283, 473}, Location = 118, }, // Duosion @ Route 16
            new EncounterStatic { Species = 315, Level = 34, Relearn = new[]{437, 275, 230, 390}, Location = 128, }, // Roselia @ Ula'ula Meadow
            new EncounterStatic { Species = 397, Level = 27, Relearn = new[]{355, 018, 283, 104}, Location = 106, }, // Staravia @ Route 10
            new EncounterStatic { Species = 288, Level = 27, Relearn = new[]{359, 498, 163, 203}, Location = 108, }, // Vigoroth @ Route 11
            new EncounterStatic { Species = 610, Level = 28, Relearn = new[]{231, 337, 206, 163}, Location = 136, }, // Axew @ Mount Hokulani
                                                                                                             
            // Poni Island                                                                                   
            new EncounterStatic { Species = 604, Level = 55, Relearn = new[]{435, 051, 029, 306}, Location = 164, }, // Eelektross @ Poni Grove
            new EncounterStatic { Species = 534, Level = 57, Relearn = new[]{409, 276, 264, 444}, Location = 166, }, // Conkeldurr @ Poni Plains
            new EncounterStatic { Species = 468, Level = 59, Relearn = new[]{248, 403, 396, 245}, Location = 170, }, // Togekiss @ Poni Gauntlet
            new EncounterStatic { Species = 542, Level = 57, Relearn = new[]{382, 437, 014, 494}, Location = 156, }, // Leavanny @ Poni Meadow
            new EncounterStatic { Species = 497, Level = 43, Relearn = new[]{137, 489, 348, 021}, Location = 184, }, // Serperior @ Exeggutor Island
            new EncounterStatic { Species = 503, Level = 43, Relearn = new[]{362, 227, 453, 279}, Location = 158, }, // Samurott @ Poni Wilds
            new EncounterStatic { Species = 500, Level = 43, Relearn = new[]{276, 053, 372, 535}, Location = 160, }, // Emboar @ Ancient Poni Path
            
            new EncounterStatic { Species = 785, Level = 60, Shiny = false, Ability = 1, Location = 030, IV3 = true, }, // Tapu Koko
            new EncounterStatic { Species = 786, Level = 60, Shiny = false, Ability = 1, Location = 092, IV3 = true, }, // Tapu Lele
            new EncounterStatic { Species = 787, Level = 60, Shiny = false, Ability = 1, Location = 140, IV3 = true, }, // Tapu Bulu
            new EncounterStatic { Species = 788, Level = 60, Shiny = false, Ability = 1, Location = 180, IV3 = true, }, // Tapu Fini

            new EncounterStatic { Species = 103, Form = 1, Level = 40, Ability = 1, Location = 184, }, // Exeggutor-1 @ Exeggutor Island
        };
        internal static readonly EncounterTrade[] TradeGift_SM = // @ a\1\5\5
        {
            // Trades - 4.bin
            new EncounterTrade { Species = 066, Form = 0, Level = 09, Ability = 2, TID = 00410, SID = 00000, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 1, Gender = 0, Nature = Nature.Brave, }, // Machop
            new EncounterTrade { Species = 761, Form = 0, Level = 16, Ability = 1, TID = 20683, SID = 00009, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 0, Gender = 1, Nature = Nature.Adamant, }, // Bounsweet
            new EncounterTrade { Species = 061, Form = 0, Level = 22, Ability = 2, TID = 01092, SID = 00009, IVs = new[] {31,-1,-1,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Naughty, }, // Poliwhirl
            new EncounterTrade { Species = 440, Form = 0, Level = 27, Ability = 2, TID = 10913, SID = 00000, IVs = new[] {-1,-1,-1,-1,31,-1}, OTGender = 1, Gender = 1, Nature = Nature.Calm, }, // Happiny
            new EncounterTrade { Species = 075, Form = 1, Level = 32, Ability = 1, TID = 20778, SID = 00009, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Impish, EvolveOnTrade = true }, // Graveler-1
            new EncounterTrade { Species = 762, Form = 0, Level = 43, Ability = 1, TID = 20679, SID = 00009, IVs = new[] {-1,-1,-1,-1,-1,31}, OTGender = 1, Gender = 1, Nature = Nature.Careful, }, // Steenee
            new EncounterTrade { Species = 663, Form = 0, Level = 59, Ability = 4, TID = 56734, SID = 00008, IVs = new[] {-1,-1,-1,31,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Jolly, }, // Talonflame
        };

        internal static readonly EncounterArea[] Encounter_Pelago_SM =
        {
            new EncounterArea
            {
                Location = 30016, // Poké Pelago
                Slots = new[]
                {
                    new EncounterSlot {Species = 021, LevelMin = 01, LevelMax = 55}, // Spearow
                    new EncounterSlot {Species = 041, LevelMin = 01, LevelMax = 55}, // Zubat
                    new EncounterSlot {Species = 090, LevelMin = 01, LevelMax = 55}, // Shellder
                    new EncounterSlot {Species = 278, LevelMin = 01, LevelMax = 55}, // Wingull
                    new EncounterSlot {Species = 731, LevelMin = 01, LevelMax = 55}, // Pikipek

                    new EncounterSlot {Species = 064, LevelMin = 11, LevelMax = 55}, // Kadabra
                    new EncounterSlot {Species = 081, LevelMin = 11, LevelMax = 55}, // Magnemite
                    new EncounterSlot {Species = 092, LevelMin = 11, LevelMax = 55}, // Gastly
                    new EncounterSlot {Species = 198, LevelMin = 11, LevelMax = 55}, // Murkrow
                    new EncounterSlot {Species = 426, LevelMin = 11, LevelMax = 55}, // Drifblim
                    new EncounterSlot {Species = 703, LevelMin = 11, LevelMax = 55}, // Carbink

                    new EncounterSlot {Species = 060, LevelMin = 21, LevelMax = 55}, // Poliwag
                    new EncounterSlot {Species = 120, LevelMin = 21, LevelMax = 55}, // Staryu
                    new EncounterSlot {Species = 127, LevelMin = 21, LevelMax = 55}, // Pinsir
                    new EncounterSlot {Species = 661, LevelMin = 21, LevelMax = 55}, // Fletchling
                    new EncounterSlot {Species = 709, LevelMin = 21, LevelMax = 55}, // Trevenant
                    new EncounterSlot {Species = 771, LevelMin = 21, LevelMax = 55}, // Pyukumuku

                    new EncounterSlot {Species = 227, LevelMin = 37, LevelMax = 55}, // Skarmory
                    new EncounterSlot {Species = 375, LevelMin = 37, LevelMax = 55}, // Metang
                    new EncounterSlot {Species = 707, LevelMin = 37, LevelMax = 55}, // Klefki

                    new EncounterSlot {Species = 123, LevelMin = 49, LevelMax = 55}, // Scyther
                    new EncounterSlot {Species = 131, LevelMin = 49, LevelMax = 55}, // Lapras
                    new EncounterSlot {Species = 429, LevelMin = 49, LevelMax = 55}, // Mismagius
                    new EncounterSlot {Species = 587, LevelMin = 49, LevelMax = 55}, // Emolga
                },
            }
        };
        internal static readonly EncounterArea[] Encounter_Pelago_SN = { new EncounterArea { Location = 30016, Slots = new[] { new EncounterSlot { Species = 627, LevelMin = 1, LevelMax = 55 }, /* Rufflet SUN  */ } } };
        internal static readonly EncounterArea[] Encounter_Pelago_MN = { new EncounterArea { Location = 30016, Slots = new[] { new EncounterSlot { Species = 629, LevelMin = 1, LevelMax = 55 }, /* Vullaby MOON */ } } };
    }
}
