using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Encounters
    /// </summary>
    internal static class Encounters7
    {
        internal static readonly EncounterArea[] SlotsSN, SlotsMN, SlotsUS, SlotsUM;
        internal static readonly EncounterStatic[] StaticSN, StaticMN, StaticUS, StaticUM;

        static Encounters7()
        {
            StaticSN = GetStaticEncounters(Encounter_SM, GameVersion.SN);
            StaticMN = GetStaticEncounters(Encounter_SM, GameVersion.MN);
            StaticUS = GetStaticEncounters(Encounter_USUM, GameVersion.US);
            StaticUM = GetStaticEncounters(Encounter_USUM, GameVersion.UM);

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

            var REG_US = GetEncounterTables(GameVersion.US);
            var REG_UM = GetEncounterTables(GameVersion.UM);
            var SOS_US = GetEncounterTables("uu", "us_sos");
            var SOS_UM = GetEncounterTables("uu", "um_sos");
            MarkG7REGSlots(ref REG_US);
            MarkG7REGSlots(ref REG_UM);
            SlotsUS = AddExtraTableSlots(REG_US, SOS_US, Encounter_Pelago_SM, Encounter_Pelago_SN);
            SlotsUM = AddExtraTableSlots(REG_UM, SOS_UM, Encounter_Pelago_SM, Encounter_Pelago_MN);
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

        private static readonly EncounterStatic[] Encounter_SM = // @ a\1\5\5
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
        private static readonly EncounterStatic[] Encounter_USUM =
        {
            new EncounterStatic { Gift = true, Species = 722, Level = 05, Location = 8, },  // Rowlet
            new EncounterStatic { Gift = true, Species = 725, Level = 05, Location = 8, },  // Litten
            new EncounterStatic { Gift = true, Species = 728, Level = 05, Location = 8, },  // Popplio
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
            new EncounterStatic { Gift = true, Species = 133, Level = 01, EggLocation = 60002, }, // Eevee @ Nursery helpers
            new EncounterStatic { Gift = true, Species = 137, Level = 30, Location = -01, }, // Porygon @ ???
            new EncounterStatic { Gift = true, Species = 772, Level = 60, Location = -01, IV3 = true, }, // Type: Null @ ???
            new EncounterStatic { Gift = true, Species = 801, Level = 50, Location = -01, IV3 = true,Shiny = false, Ability = 2, HeldItem = 795, }, // Magearna @ ???
            new EncounterStatic { Gift = true, Species = 789, Level = 05, Location = -01, IV3 = true,Shiny = false, Ability = 2, }, // Cosmog @ ???
            new EncounterStatic { Gift = true, Species = 142, Level = 40, Location = 172, }, // Aerodactyl @ Seafolk Village
            new EncounterStatic { Gift = true, Species = 025, Level = 40, Location = -01, IV3 = true,HeldItem = 796, }, // Pikachu @ ???
            new EncounterStatic { Gift = true, Species = 803, Level = 40, Location = -01, IV3 = true,}, // Poipole @ ???

            // Totem-Sized Gifts @ Heahea Beach
            new EncounterStatic { Gift = true, Species = 735, Level = 20, Ability = 4, Location = 202, Form = 1, Shiny = false, IV3 = true, Version = GameVersion.US }, // Gumshoos
            new EncounterStatic { Gift = true, Species = 020, Level = 20, Ability = 4, Location = 202, Form = 2, Shiny = false, IV3 = true, Version = GameVersion.UM }, // Raticate
            new EncounterStatic { Gift = true, Species = 105, Level = 25, Ability = 4, Location = 202, Form = 2, Shiny = false, IV3 = true, Version = GameVersion.US }, // Marowak
            new EncounterStatic { Gift = true, Species = 752, Level = 25, Ability = 1, Location = 202, Form = 1, Shiny = false, IV3 = true, Version = GameVersion.UM }, // Araquanid
            new EncounterStatic { Gift = true, Species = 754, Level = 30, Ability = 2, Location = 202, Form = 1, Shiny = false, IV3 = true, Version = GameVersion.US }, // Lurantis
            new EncounterStatic { Gift = true, Species = 758, Level = 30, Ability = 1, Location = 202, Form = 1, Shiny = false, IV3 = true, Version = GameVersion.UM }, // Salazzle
            new EncounterStatic { Gift = true, Species = 738, Level = 35, Ability = 1, Location = 202, Form = 1, Shiny = false, IV3 = true, },                          // Vikavolt
            new EncounterStatic { Gift = true, Species = 777, Level = 35, Ability = 4, Location = 202, Form = 1, Shiny = false, IV3 = true, },                          // Togedemaru
            new EncounterStatic { Gift = true, Species = 778, Level = 40, Ability = 1, Location = 202, Form = 2, Shiny = false, IV3 = true, },                          // Mimikyu
            new EncounterStatic { Gift = true, Species = 743, Level = 50, Ability = 4, Location = 202, Form = 1, Shiny = false, IV3 = true, },                          // Ribombee
            new EncounterStatic { Gift = true, Species = 784, Level = 50, Ability = 4, Location = 202, Form = 1, Shiny = false, IV3 = true, },                          // Kommo-o

            new EncounterStatic { Gift = true, Species = 718, Level = 63, Ability = 1, Location = -01, Form = 1, Shiny = false, IV3 = true, }, // Zygarde @ ???
            new EncounterStatic { Gift = true, Species = 025, Level = 21, Ability = 1, Location = -01, Form = 7, Shiny = false, HeldItem = 571, Nature = Nature.Hardy }, // Pikachu @ ???

            new EncounterStatic { Species = 731, Level = 03, Location = -01, Shiny = false, }, // Pikipek @ ???
            new EncounterStatic { Species = 793, Level = 27, Location = -01, Shiny = false, }, // Nihilego @ ???
            new EncounterStatic { Species = 791, Level = 60, Location = -01, Shiny = false, }, // Solgaleo @ ???
            new EncounterStatic { Species = 792, Level = 60, Location = -01, Shiny = false, }, // Lunala @ ???
            new EncounterStatic { Species = 735, Level = 12, Location = -01, Form = 1, Shiny = false, HeldItem = 151, }, // Gumshoos @ ???
            new EncounterStatic { Species = 734, Level = 11, Location = -01, Shiny = false, }, // Yungoos @ ???
            new EncounterStatic { Species = 735, Level = 11, Location = -01, Shiny = false, }, // Gumshoos @ ???
            new EncounterStatic { Species = 734, Level = 11, Location = -01, Shiny = false, }, // Yungoos @ ???
            new EncounterStatic { Species = 734, Level = 10, Location = -01, Shiny = false, }, // Yungoos @ ???
            new EncounterStatic { Species = 020, Level = 12, Location = -01, Form = 2, Shiny = false, HeldItem = 151, }, // Raticate @ ???
            new EncounterStatic { Species = 019, Level = 11, Location = -01, Form = 1, Shiny = false, }, // Rattata @ ???
            new EncounterStatic { Species = 020, Level = 11, Location = -01, Form = 1, Shiny = false, }, // Raticate @ ???
            new EncounterStatic { Species = 019, Level = 11, Location = -01, Form = 1, Shiny = false, }, // Rattata @ ???
            new EncounterStatic { Species = 019, Level = 10, Location = -01, Form = 1, Shiny = false, }, // Rattata @ ???
            new EncounterStatic { Species = 746, Level = 20, Location = -01, Form = 1, Shiny = false, HeldItem = 158, }, // Wishiwashi @ ???
            new EncounterStatic { Species = 746, Level = 17, Location = -01, Shiny = false, }, // Wishiwashi @ ???
            new EncounterStatic { Species = 746, Level = 18, Location = -01, Shiny = false, }, // Wishiwashi @ ???
            new EncounterStatic { Species = 594, Level = 18, Location = -01, Shiny = false, }, // Alomomola @ ???
            new EncounterStatic { Species = 746, Level = 18, Location = -01, Shiny = false, }, // Wishiwashi @ ???
            new EncounterStatic { Species = 758, Level = 22, Location = -01, Form = 1, Shiny = false, HeldItem = 204, }, // Salazzle @ ???
            new EncounterStatic { Species = 105, Level = 18, Location = -01, Form = 1, Shiny = false, }, // Marowak @ ???
            new EncounterStatic { Species = 105, Level = 18, Location = -01, Form = 1, Shiny = false, }, // Marowak @ ???
            new EncounterStatic { Species = 025, Level = 20, Location = -01, Shiny = false, }, // Pikachu @ ???
            new EncounterStatic { Species = 757, Level = 20, Location = -01, Shiny = false, }, // Salandit @ ???
            new EncounterStatic { Species = 754, Level = 24, Location = -01, Form = 1, Shiny = false, HeldItem = 271, }, // Lurantis @ ???
            new EncounterStatic { Species = 047, Level = 22, Location = -01, Shiny = false, }, // Parasect @ ???
            new EncounterStatic { Species = 756, Level = 22, Location = -01, Shiny = false, }, // Shiinotic @ ???
            new EncounterStatic { Species = 753, Level = 23, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 753, Level = 23, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 753, Level = 23, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 753, Level = 23, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 764, Level = 22, Location = -01, Shiny = false, }, // Comfey @ ???
            new EncounterStatic { Species = 352, Level = 22, Location = -01, Shiny = false, }, // Kecleon @ ???
            new EncounterStatic { Species = 738, Level = 29, Location = -01, Form = 1, Shiny = false, HeldItem = 184, }, // Vikavolt @ ???
            new EncounterStatic { Species = 736, Level = 27, Location = -01, Shiny = false, }, // Grubbin @ ???
            new EncounterStatic { Species = 737, Level = 27, Location = -01, Shiny = false, }, // Charjabug @ ???
            new EncounterStatic { Species = 737, Level = 27, Location = -01, Shiny = false, }, // Charjabug @ ???
            new EncounterStatic { Species = 737, Level = 27, Location = -01, Shiny = false, }, // Charjabug @ ???
            new EncounterStatic { Species = 737, Level = 28, Location = -01, Shiny = false, }, // Charjabug @ ???
            new EncounterStatic { Species = 778, Level = 35, Location = -01, Form = 2, Shiny = false, HeldItem = 157, }, // Mimikyu @ ???
            new EncounterStatic { Species = 092, Level = 30, Location = -01, Shiny = false, }, // Gastly @ ???
            new EncounterStatic { Species = 093, Level = 30, Location = -01, Shiny = false, }, // Haunter @ ???
            new EncounterStatic { Species = 094, Level = 30, Location = -01, Shiny = false, }, // Gengar @ ???
            new EncounterStatic { Species = 354, Level = 32, Location = -01, Shiny = false, }, // Banette @ ???
            new EncounterStatic { Species = 593, Level = 33, Location = -01, Shiny = false, }, // Jellicent @ ???
            new EncounterStatic { Species = 784, Level = 49, Location = -01, Form = 1, Shiny = false, HeldItem = 686, }, // Kommo-o @ ???
            new EncounterStatic { Species = 782, Level = 42, Location = -01, Shiny = false, }, // Jangmo-o @ ???
            new EncounterStatic { Species = 783, Level = 44, Location = -01, Shiny = false, }, // Hakamo-o @ ???
            new EncounterStatic { Species = 715, Level = 48, Location = -01, Shiny = false, }, // Noivern @ ???
            new EncounterStatic { Species = 212, Level = 46, Location = -01, Shiny = false, }, // Scizor @ ???
            new EncounterStatic { Species = 793, Level = 55, Location = -01, Shiny = false, }, // Nihilego @ ???
            new EncounterStatic { Species = 793, Level = 55, Location = -01, Shiny = false, }, // Nihilego @ ???
            new EncounterStatic { Species = 794, Level = 65, Location = -01, Shiny = false, }, // Buzzwole @ ???
            new EncounterStatic { Species = 794, Level = 65, Location = -01, Shiny = false, }, // Buzzwole @ ???
            new EncounterStatic { Species = 795, Level = 60, Location = -01, Shiny = false, }, // Pheromosa @ ???
            new EncounterStatic { Species = 795, Level = 60, Location = -01, Shiny = false, }, // Pheromosa @ ???
            new EncounterStatic { Species = 795, Level = 60, Location = -01, Shiny = false, }, // Pheromosa @ ???
            new EncounterStatic { Species = 795, Level = 60, Location = -01, Shiny = false, }, // Pheromosa @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 796, Level = 65, Location = -01, Shiny = false, }, // Xurkitree @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, Shiny = false, }, // Kartana @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 797, Level = 65, Location = -01, Shiny = false, }, // Celesteela @ ???
            new EncounterStatic { Species = 799, Level = 70, Location = -01, Shiny = false, }, // Guzzlord @ ???
            new EncounterStatic { Species = 800, Level = 75, Location = -01, Shiny = false, }, // Necrozma @ ???

            // QR Scan: Su/M/Tu/W/Th/F/Sa
            // Melemele Island
            new EncounterStatic { Species = 004, Level = 12, Location = 010, }, // Charmander @ Route 3
            new EncounterStatic { Species = 007, Level = 12, Location = 042, }, // Squirtle @ Seaward Cave
            new EncounterStatic { Species = 095, Level = 14, Location = 034, }, // Onix @ Ten Carat Hill
            new EncounterStatic { Species = 116, Level = 18, Location = 014, }, // Horsea @ Kala'e Bay
            new EncounterStatic { Species = 664, Level = 09, Location = 020, Form = 30, }, // Scatterbug @ Hau'oli City
            new EncounterStatic { Species = 001, Level = 10, Location = 012, }, // Bulbasaur @ Route 2
            new EncounterStatic { Species = 607, Level = 09, Location = 038, }, // Litwick @ Hau'oli Cemetery
			
            // Akala Island
            new EncounterStatic { Species = 280, Level = 17, Location = 054, }, // Ralts @ Route 6
            new EncounterStatic { Species = 363, Level = 19, Location = 056, }, // Spheal @ Route 7
            new EncounterStatic { Species = 256, Level = 20, Location = 058, }, // Combusken @ Route 8
            new EncounterStatic { Species = 679, Level = 24, Location = 094, }, // Honedge @ Akala Outskirts
            new EncounterStatic { Species = 015, Level = 14, Location = 050, }, // Beedrill @ Route 4
            new EncounterStatic { Species = 253, Level = 16, Location = 052, }, // Grovyle @ Route 5
            new EncounterStatic { Species = 259, Level = 17, Location = 086, }, // Marshtomp @ Brooklet Hill
			
            // Ula'ula Island
            new EncounterStatic { Species = 111, Level = 32, Location = 138, }, // Rhyhorn @ Blush Mountain
            new EncounterStatic { Species = 220, Level = 33, Location = 114, }, // Swinub @ Tapu Village
            new EncounterStatic { Species = 394, Level = 35, Location = 118, }, // Prinplup @ Route 16
            new EncounterStatic { Species = 388, Level = 36, Location = 128, }, // Grotle @ Ula'ula Meadow
            new EncounterStatic { Species = 018, Level = 29, Location = 106, }, // Pidgeot @ Route 10
            new EncounterStatic { Species = 391, Level = 29, Location = 108, }, // Monferno @ Route 11
            new EncounterStatic { Species = 610, Level = 30, Location = 136, }, // Axew @ Mount Hokulani
			
            // Poni Island
            new EncounterStatic { Species = 604, Level = 55, Location = 164, }, // Eelektross @ Poni Grove
            new EncounterStatic { Species = 306, Level = 57, Location = 166, }, // Aggron @ Poni Plains
            new EncounterStatic { Species = 479, Level = 61, Location = 170, }, // Rotom @ Poni Gauntlet
            new EncounterStatic { Species = 542, Level = 57, Location = 156, }, // Leavanny @ Poni Meadow
            new EncounterStatic { Species = 652, Level = 45, Location = 184, }, // Chesnaught @ Exeggutor Island
            new EncounterStatic { Species = 658, Level = 44, Location = 158, }, // Greninja @ Poni Wilds
            new EncounterStatic { Species = 655, Level = 44, Location = 160, }, // Delphox @ Ancient Poni Path
            
            new EncounterStatic { Species = 785, Level = 60, Location = -01, Shiny = false, }, // Tapu Koko @ ???
            new EncounterStatic { Species = 786, Level = 60, Location = -01, Shiny = false, }, // Tapu Lele @ ???
            new EncounterStatic { Species = 787, Level = 60, Location = -01, Shiny = false, }, // Tapu Bulu @ ???
            new EncounterStatic { Species = 788, Level = 60, Location = -01, Shiny = false, }, // Tapu Fini @ ???
            new EncounterStatic { Species = 023, Level = 10, Location = 012, }, // Ekans @ Route 2
            new EncounterStatic { Species = 103, Level = 40, Location = -01, Form = 1, }, // Exeggutor @ ???
            new EncounterStatic { Species = 785, Level = 60, Location = -01, Shiny = false, }, // Tapu Koko @ ???

            new EncounterStatic { Species = 542, Level = 57, Location = -01, }, // Leavanny @ ???
            new EncounterStatic { Species = 752, Level = 20, Location = -01, Form = 1, Shiny = false, HeldItem = 186, }, // Araquanid @ ???
            new EncounterStatic { Species = 751, Level = 17, Location = -01, Shiny = false, }, // Dewpider @ ???
            new EncounterStatic { Species = 751, Level = 18, Location = -01, Shiny = false, }, // Dewpider @ ???
            new EncounterStatic { Species = 284, Level = 18, Location = -01, Shiny = false, }, // Masquerain @ ???
            new EncounterStatic { Species = 751, Level = 18, Location = -01, Shiny = false, }, // Dewpider @ ???
            new EncounterStatic { Species = 753, Level = 17, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 764, Level = 21, Location = -01, Shiny = false, }, // Comfey @ ???
            new EncounterStatic { Species = 753, Level = 21, Location = -01, Shiny = false, }, // Fomantis @ ???
            new EncounterStatic { Species = 185, Level = 24, Location = -01, Shiny = false, }, // Sudowoodo @ ???
            new EncounterStatic { Species = 777, Level = 33, Location = -01, Form = 1, Shiny = false, HeldItem = 158, }, // Togedemaru @ ???
            new EncounterStatic { Species = 227, Level = 32, Location = -01, Shiny = false, }, // Skarmory @ ???
            new EncounterStatic { Species = 702, Level = 31, Location = -01, Shiny = false, }, // Dedenne @ ???
            new EncounterStatic { Species = 239, Level = 29, Location = -01, Shiny = false, }, // Elekid @ ???
            new EncounterStatic { Species = 125, Level = 30, Location = -01, Shiny = false, }, // Electabuzz @ ???
            new EncounterStatic { Species = 734, Level = 03, Location = -01, Shiny = false, }, // Yungoos @ ???
            new EncounterStatic { Species = 019, Level = 03, Location = -01, Form = 1, Shiny = false, }, // Rattata @ ???
            new EncounterStatic { Species = 736, Level = 04, Location = -01, Shiny = false, }, // Grubbin @ ???
            new EncounterStatic { Species = 021, Level = 04, Location = -01, Shiny = false, }, // Spearow @ ???
            new EncounterStatic { Species = 127, Level = 42, Location = -01, Shiny = false, }, // Pinsir @ ???
            new EncounterStatic { Species = 127, Level = 43, Location = -01, Shiny = false, }, // Pinsir @ ???
            new EncounterStatic { Species = 127, Level = 42, Location = -01, Shiny = false, }, // Pinsir @ ???
            new EncounterStatic { Species = 800, Level = 50, Location = -01, Form = 1, Shiny = false, }, // Necrozma @ ???
            new EncounterStatic { Species = 800, Level = 50, Location = -01, Form = 2, Shiny = false, }, // Necrozma @ ???
            new EncounterStatic { Species = 800, Level = 60, Location = -01, Form = 3, Shiny = false, }, // Necrozma @ ???
            new EncounterStatic { Species = 800, Level = 65, Location = -01, Shiny = false, HeldItem = 923, }, // Necrozma @ ???
            new EncounterStatic { Species = 743, Level = 55, Location = -01, Form = 1, Shiny = false, HeldItem = 184, }, // Ribombee @ ???
            new EncounterStatic { Species = 279, Level = 52, Location = -01, Shiny = false, }, // Pelipper @ ???
            new EncounterStatic { Species = 242, Level = 53, Location = -01, Shiny = false, }, // Blissey @ ???
            new EncounterStatic { Species = 144, Level = 60, Location = -01, }, // Articuno @ ???
            new EncounterStatic { Species = 145, Level = 60, Location = -01, }, // Zapdos @ ???
            new EncounterStatic { Species = 146, Level = 60, Location = -01, }, // Moltres @ ???
            new EncounterStatic { Species = 250, Level = 60, Location = -01, HeldItem = 044, }, // Ho-Oh @ ???
            new EncounterStatic { Species = 384, Level = 60, Location = -01, }, // Rayquaza @ ???
            new EncounterStatic { Species = 488, Level = 60, Location = -01, }, // Cresselia @ ???
            new EncounterStatic { Species = 641, Level = 60, Location = -01, }, // Tornadus @ ???
            new EncounterStatic { Species = 642, Level = 60, Location = -01, }, // Thundurus @ ???
            new EncounterStatic { Species = 645, Level = 60, Location = -01, }, // Landorus @ ???
            new EncounterStatic { Species = 717, Level = 60, Location = -01, }, // Yveltal @ ???
            new EncounterStatic { Species = 150, Level = 60, Location = -01, }, // Mewtwo @ ???
            new EncounterStatic { Species = 243, Level = 60, Location = -01, }, // Raikou @ ???
            new EncounterStatic { Species = 244, Level = 60, Location = -01, }, // Entei @ ???
            new EncounterStatic { Species = 483, Level = 60, Location = -01, }, // Dialga @ ???
            new EncounterStatic { Species = 638, Level = 60, Location = -01, }, // Cobalion @ ???
            new EncounterStatic { Species = 639, Level = 60, Location = -01, }, // Terrakion @ ???
            new EncounterStatic { Species = 640, Level = 60, Location = -01, }, // Virizion @ ???
            new EncounterStatic { Species = 643, Level = 60, Location = -01, }, // Reshiram @ ???
            new EncounterStatic { Species = 644, Level = 60, Location = -01, }, // Zekrom @ ???
            new EncounterStatic { Species = 716, Level = 60, Location = -01, }, // Xerneas @ ???
            new EncounterStatic { Species = 377, Level = 60, Location = -01, }, // Regirock @ ???
            new EncounterStatic { Species = 378, Level = 60, Location = -01, }, // Regice @ ???
            new EncounterStatic { Species = 379, Level = 60, Location = -01, }, // Registeel @ ???
            new EncounterStatic { Species = 383, Level = 60, Location = -01, }, // Groudon @ ???
            new EncounterStatic { Species = 485, Level = 60, Location = -01, }, // Heatran @ ???
            new EncounterStatic { Species = 486, Level = 60, Location = -01, }, // Regigigas @ ???
            new EncounterStatic { Species = 484, Level = 60, Location = -01, }, // Palkia @ ???
            new EncounterStatic { Species = 487, Level = 60, Location = -01, }, // Giratina @ ???
            new EncounterStatic { Species = 245, Level = 60, Location = -01, }, // Suicune @ ???
            new EncounterStatic { Species = 249, Level = 60, Location = -01, }, // Lugia @ ???
            new EncounterStatic { Species = 380, Level = 60, Location = -01, }, // Latias @ ???
            new EncounterStatic { Species = 381, Level = 60, Location = -01, }, // Latios @ ???
            new EncounterStatic { Species = 382, Level = 60, Location = -01, }, // Kyogre @ ???
            new EncounterStatic { Species = 480, Level = 60, Location = -01, }, // Uxie @ ???
            new EncounterStatic { Species = 481, Level = 60, Location = -01, }, // Mesprit @ ???
            new EncounterStatic { Species = 482, Level = 60, Location = -01, }, // Azelf @ ???
            new EncounterStatic { Species = 646, Level = 60, Location = -01, }, // Kyurem @ ???
            new EncounterStatic { Species = 334, Level = 60, Location = -01, }, // Altaria @ ???
            new EncounterStatic { Species = 469, Level = 60, Location = -01, }, // Yanmega @ ???
            new EncounterStatic { Species = 561, Level = 60, Location = -01, }, // Sigilyph @ ???
            new EncounterStatic { Species = 581, Level = 60, Location = -01, }, // Swanna @ ???
            new EncounterStatic { Species = 277, Level = 60, Location = -01, }, // Swellow @ ???
            new EncounterStatic { Species = 452, Level = 60, Location = -01, }, // Drapion @ ???
            new EncounterStatic { Species = 531, Level = 60, Location = -01, }, // Audino @ ???
            new EncounterStatic { Species = 695, Level = 60, Location = -01, }, // Heliolisk @ ???
            new EncounterStatic { Species = 274, Level = 60, Location = -01, }, // Nuzleaf @ ???
            new EncounterStatic { Species = 326, Level = 60, Location = -01, }, // Grumpig @ ???
            new EncounterStatic { Species = 460, Level = 60, Location = -01, }, // Abomasnow @ ???
            new EncounterStatic { Species = 308, Level = 60, Location = -01, }, // Medicham @ ???
            new EncounterStatic { Species = 450, Level = 60, Location = -01, }, // Hippowdon @ ???
            new EncounterStatic { Species = 558, Level = 60, Location = -01, }, // Crustle @ ???
            new EncounterStatic { Species = 219, Level = 60, Location = -01, }, // Magcargo @ ???
            new EncounterStatic { Species = 689, Level = 60, Location = -01, }, // Barbaracle @ ???
            new EncounterStatic { Species = 271, Level = 60, Location = -01, }, // Lombre @ ???
            new EncounterStatic { Species = 618, Level = 60, Location = -01, }, // Stunfisk @ ???
            new EncounterStatic { Species = 419, Level = 60, Location = -01, }, // Floatzel @ ???
            new EncounterStatic { Species = 195, Level = 60, Location = -01, }, // Quagsire @ ???
            new EncounterStatic { Species = 793, Level = 60, Location = -01, }, // Nihilego @ ???
            new EncounterStatic { Species = 794, Level = 60, Location = -01, }, // Buzzwole @ ???
            new EncounterStatic { Species = 795, Level = 60, Location = -01, }, // Pheromosa @ ???
            new EncounterStatic { Species = 796, Level = 60, Location = -01, }, // Xurkitree @ ???
            new EncounterStatic { Species = 797, Level = 60, Location = -01, }, // Celesteela @ ???
            new EncounterStatic { Species = 798, Level = 60, Location = -01, }, // Kartana @ ???
            new EncounterStatic { Species = 799, Level = 60, Location = -01, }, // Guzzlord @ ???
            new EncounterStatic { Species = 735, Level = 60, Location = -01, Form = 1, Shiny = false, HeldItem = 189, }, // Gumshoos @ ???
            new EncounterStatic { Species = 734, Level = 58, Location = -01, Shiny = false, }, // Yungoos @ ???
            new EncounterStatic { Species = 020, Level = 60, Location = -01, Form = 2, Shiny = false, HeldItem = 189, }, // Raticate @ ???
            new EncounterStatic { Species = 019, Level = 58, Location = -01, Form = 1, Shiny = false, }, // Rattata @ ???
            new EncounterStatic { Species = 760, Level = 28, Location = -01, Shiny = false, }, // Bewear @ ???
            new EncounterStatic { Species = 097, Level = 29, Location = -01, Shiny = false, }, // Hypno @ ???
            new EncounterStatic { Species = 097, Level = 29, Location = -01, Shiny = false, }, // Hypno @ ???
            new EncounterStatic { Species = 097, Level = 29, Location = -01, Shiny = false, }, // Hypno @ ???
            new EncounterStatic { Species = 092, Level = 19, Location = -01, Shiny = false, }, // Gastly @ ???
            new EncounterStatic { Species = 425, Level = 19, Location = -01, Shiny = false, }, // Drifloon @ ???
            new EncounterStatic { Species = 769, Level = 30, Location = -01, Shiny = false, }, // Sandygast @ ???
            new EncounterStatic { Species = 592, Level = 34, Location = 126, Shiny = false, }, // Frillish @ ???
            new EncounterStatic { Species = 132, Level = 29, Location = -01, }, // Ditto @ ???
            new EncounterStatic { Species = 132, Level = 29, Location = -01, }, // Ditto @ ???
            new EncounterStatic { Species = 132, Level = 29, Location = -01, }, // Ditto @ ???
            new EncounterStatic { Species = 132, Level = 29, Location = -01, }, // Ditto @ ???
            new EncounterStatic { Species = 132, Level = 29, Location = -01, }, // Ditto @ ???
            new EncounterStatic { Species = 718, Level = 60, Location = -01, Shiny = false, }, // Zygarde @ ???
            new EncounterStatic { Species = 805, Level = 60, Location = -01, }, // Stakataka @ ???
            new EncounterStatic { Species = 806, Level = 60, Location = -01, }, // Blacephalon @ ???
            new EncounterStatic { Species = 105, Level = 22, Location = -01, Form = 2, Shiny = false, HeldItem = 258, }, // Marowak @ ???
            new EncounterStatic { Species = 758, Level = 20, Location = -01, Shiny = false, }, // Salazzle @ ???
            new EncounterStatic { Species = 101, Level = 60, Location = -01, Shiny = false, }, // Electrode @ ???
        };
        internal static readonly EncounterTrade[] TradeGift_USUM =
        {
            // Trades - 4.bin
            new EncounterTrade { Species = 701, Form = 0, Level = 08, Ability = 2, TID = 00410, SID = 00000, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 1, Gender = 0, Nature = Nature.Brave, }, // Hawlucha
            new EncounterTrade { Species = 714, Form = 0, Level = 19, Ability = 1, TID = 20683, SID = 00009, IVs = new[] {-1,-1,-1,-1,31,-1}, OTGender = 0, Gender = 1, Nature = Nature.Modest, }, // Noibat
            new EncounterTrade { Species = 339, Form = 0, Level = 21, Ability = 2, TID = 01092, SID = 00009, IVs = new[] {31,-1,-1,-1,-1,-1}, OTGender = 0, Gender = 1, Nature = Nature.Naughty, }, // Barboach
            new EncounterTrade { Species = 024, Form = 0, Level = 22, Ability = 1, TID = 10913, SID = 00000, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Impish, }, // Arbok
            new EncounterTrade { Species = 708, Form = 0, Level = 33, Ability = 1, TID = 20778, SID = 00009, IVs = new[] {-1,-1,-1,-1,-1,31}, OTGender = 0, Gender = 0, Nature = Nature.Calm, EvolveOnTrade = true }, // Phantump
            new EncounterTrade { Species = 422, Form = 0, Level = 44, Ability = 2, TID = 20679, SID = 00009, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Quiet, }, // Shellos
            new EncounterTrade { Species = 128, Form = 0, Level = 59, Ability = 1, TID = 56734, SID = 00008, IVs = new[] {-1,-1,-1,31,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Jolly, }, // Tauros
        };
        internal static readonly string[][] TradeSM =
        {
            new string[0],                       // 0 - None
            Util.GetStringList("tradesm", "ja"), // 1
            Util.GetStringList("tradesm", "en"), // 2
            Util.GetStringList("tradesm", "fr"), // 3
            Util.GetStringList("tradesm", "it"), // 4
            Util.GetStringList("tradesm", "de"), // 5
            new string[0],                       // 6 - None
            Util.GetStringList("tradesm", "es"), // 7
            Util.GetStringList("tradesm", "ko"), // 8
            Util.GetStringList("tradesm", "zh"), // 9
            Util.GetStringList("tradesm", "zh"), // 10
        };
        internal static readonly string[][] TradeUSUM =
        {
            new string[0],                         // 0 - None
            Util.GetStringList("tradeusum", "ja"), // 1
            Util.GetStringList("tradeusum", "en"), // 2
            Util.GetStringList("tradeusum", "fr"), // 3
            Util.GetStringList("tradeusum", "it"), // 4
            Util.GetStringList("tradeusum", "de"), // 5
            new string[0],                         // 6 - None
            Util.GetStringList("tradeusum", "es"), // 7
            Util.GetStringList("tradeusum", "ko"), // 8
            Util.GetStringList("tradeusum", "zh"), // 9
            Util.GetStringList("tradeusum", "zh"), // 10
        };

        private static readonly EncounterArea[] Encounter_Pelago_SM =
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
        private static readonly EncounterArea[] Encounter_Pelago_SN = { new EncounterArea { Location = 30016, Slots = new[] { new EncounterSlot { Species = 627, LevelMin = 1, LevelMax = 55 }, /* Rufflet SUN  */ } } };
        private static readonly EncounterArea[] Encounter_Pelago_MN = { new EncounterArea { Location = 30016, Slots = new[] { new EncounterSlot { Species = 629, LevelMin = 1, LevelMax = 55 }, /* Vullaby MOON */ } } };
    }
}
