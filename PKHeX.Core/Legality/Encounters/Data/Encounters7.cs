using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Encounters
    /// </summary>
    internal static class Encounters7
    {
        internal static readonly EncounterArea7[] SlotsSN = EncounterArea7.GetAreas(Get("sn", "sm"), SN);
        internal static readonly EncounterArea7[] SlotsMN = EncounterArea7.GetAreas(Get("mn", "sm"), MN);
        internal static readonly EncounterArea7[] SlotsUS = EncounterArea7.GetAreas(Get("us", "uu"), US);
        internal static readonly EncounterArea7[] SlotsUM = EncounterArea7.GetAreas(Get("um", "uu"), UM);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters7()
        {
            MarkEncounterTradeStrings(TradeGift_SM, TradeSM);
            MarkEncounterTradeStrings(TradeGift_USUM, TradeUSUM);
        }

        private static readonly EncounterStatic7[] Encounter_SM = // @ a\1\5\5
        {
            // Gifts - 0.bin
            new(SM) { Gift = true, Species = 722, Level = 5,  Location = 024, }, // Rowlet
            new(SM) { Gift = true, Species = 725, Level = 5,  Location = 024, }, // Litten
            new(SM) { Gift = true, Species = 728, Level = 5,  Location = 024, }, // Popplio
            new(SM) { Gift = true, Species = 138, Level = 15, Location = 058, }, // Omanyte
            new(SM) { Gift = true, Species = 140, Level = 15, Location = 058, }, // Kabuto
         // new(SM) { Gift = true, Species = 142, Level = 15, Location = 058, }, // Aerodactyl
            new(SM) { Gift = true, Species = 345, Level = 15, Location = 058, }, // Lileep
            new(SM) { Gift = true, Species = 347, Level = 15, Location = 058, }, // Anorith
            new(SM) { Gift = true, Species = 408, Level = 15, Location = 058, }, // Cranidos
            new(SM) { Gift = true, Species = 410, Level = 15, Location = 058, }, // Shieldon
            new(SM) { Gift = true, Species = 564, Level = 15, Location = 058, }, // Tirtouga
            new(SM) { Gift = true, Species = 566, Level = 15, Location = 058, }, // Archen
            new(SM) { Gift = true, Species = 696, Level = 15, Location = 058, }, // Tyrunt
            new(SM) { Gift = true, Species = 698, Level = 15, Location = 058, }, // Amaura
            new(SM) { Gift = true, Species = 137, Level = 30, Location = 116, }, // Porygon @ Route 15
            new(SM) { Gift = true, Species = 133, Level = 1,  EggLocation = 60002, }, // Eevee @ Nursery helpers
            new(SM) { Gift = true, Species = 772, Level = 40, Location = 188, FlawlessIVCount = 3, }, // Type: Null
            new(SN) { Gift = true, Species = 789, Level = 5,  Location = 142, Shiny = Shiny.Never, Ability = 2, FlawlessIVCount = 3 }, // Cosmog
            new(MN) { Gift = true, Species = 789, Level = 5,  Location = 144, Shiny = Shiny.Never, Ability = 2, FlawlessIVCount = 3 }, // Cosmog
            new(SM) { Gift = true, Species = 142, Level = 40, Location = 172, }, // Aerodactyl @ Seafolk Village

            new(SM) { Gift = true, Species = 718, Form = 0, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 1, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 2, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 3, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde

            new(SM) { Gift = true, Species = 718, Form = 0, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 1, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 2, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(SM) { Gift = true, Species = 718, Form = 3, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde

            new(SM) // Magearna (Bottle Cap) 00 FF
            {
                Gift = true, Species = 801, Level = 50, Location = 40001, Shiny = Shiny.Never, FlawlessIVCount = 3, HeldItem = 795, Ability = 2,
                Fateful = true, Relearn = new [] {705, 430, 381, 270}, Ball = 0x10, // Cherish
            },

            // Static Encounters - 1.bin
            new(SM) { Species = 746, Level = 17, Location = 086, Shiny = Shiny.Never, Ability = 1, }, // Wishiwashi
            new(SM) { Species = 746, Level = 18, Location = 086, Shiny = Shiny.Never, Ability = 1, }, // Wishiwashi

            new(SN) { Species = 791, Level = 55, Location = 176, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, Relearn = new[]{713, 322, 242, 428} }, // Solgaleo
            new(MN) { Species = 792, Level = 55, Location = 178, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, Relearn = new[]{714, 322, 539, 247} }, // Lunala

            new(SM) { Species = 785, Level = 60, Location = 030, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, }, // Tapu Koko
            new(SM) { Species = 786, Level = 60, Location = 092, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, }, // Tapu Lele
            new(SM) { Species = 787, Level = 60, Location = 140, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, }, // Tapu Bulu
            new(SM) { Species = 788, Level = 60, Location = 180, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3, }, // Tapu Fini

            new(SM) { Species = 793, Level = 55, Location = 082, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Nihilego @ Wela Volcano Park
            new(SM) { Species = 793, Level = 55, Location = 100, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Nihilego @ Diglett’s Tunnel
            new(SN) { Species = 794, Level = 65, Location = 040, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Buzzwole @ Melemele Meadow
            new(MN) { Species = 795, Level = 60, Location = 046, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Pheromosa @ Verdant Cavern (Trial Site)
            new(SM) { Species = 796, Level = 65, Location = 090, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Xurkitree @ Lush Jungle
            new(SM) { Species = 796, Level = 65, Location = 076, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Xurkitree @ Memorial Hill
            new(SN) { Species = 798, Level = 60, Location = 134, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Kartana @ Malie Garden
            new(SN) { Species = 798, Level = 60, Location = 120, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Kartana @ Route 17
            new(MN) { Species = 797, Level = 65, Location = 124, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Celesteela @ Haina Desert
            new(MN) { Species = 797, Level = 65, Location = 134, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Celesteela @ Malie Garden
            new(SM) { Species = 799, Level = 70, Location = 182, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Guzzlord @ Resolution Cave
            new(SM) { Species = 800, Level = 75, Location = 036, Shiny = Shiny.Never, Ability = 1, FlawlessIVCount = 3 }, // Necrozma @ Ten Carat Hill (Farthest Hollow)

            // QR Scan: Su/M/Tu/W/Th/F/Sa
            // Melemele Island
            new(SM) { Species = 155, Level = 12, Location = 010, Relearn = new[]{024, 052, 108, 043} }, // Cyndaquil @ Route 3
            new(SM) { Species = 158, Level = 12, Location = 042, Relearn = new[]{232, 099, 055, 043} }, // Totodile @ Seaward Cave
            new(SM) { Species = 633, Level = 13, Location = 034, Relearn = new[]{372, 029, 044, 116} }, // Deino @ Ten Carat Hill
            new(SM) { Species = 116, Level = 18, Location = 014, Relearn = new[]{225, 239, 055, 043} }, // Horsea @ Kala'e Bay
            new(SM) { Species = 599, Level = 08, Location = 020, Relearn = new[]{268, 011, 000, 000} }, // Klink @ Hau'oli City
            new(SM) { Species = 152, Level = 10, Location = 012, Relearn = new[]{073, 077, 075, 045} }, // Chikorita @ Route 2
            new(SM) { Species = 607, Level = 10, Location = 038, Relearn = new[]{051, 109, 083, 123} }, // Litwick @ Hau'oli Cemetery

            // Akala Island
            new(SM) { Species = 574, Level = 17, Location = 054, Relearn = new[]{399, 060, 003, 313} }, // Gothita @ Route 6
            new(SM) { Species = 363, Level = 19, Location = 056, Relearn = new[]{392, 362, 301, 227} }, // Spheal @ Route 7
            new(SM) { Species = 404, Level = 20, Location = 058, Relearn = new[]{598, 044, 209, 268} }, // Luxio @ Route 8
            new(SM) { Species = 679, Level = 23, Location = 094, Relearn = new[]{194, 332, 425, 475} }, // Honedge @ Akala Outskirts
            new(SM) { Species = 543, Level = 14, Location = 050, Relearn = new[]{390, 228, 103, 040} }, // Venipede @ Route 4
            new(SM) { Species = 069, Level = 16, Location = 052, Relearn = new[]{491, 077, 079, 035} }, // Bellsprout @ Route 5
            new(SM) { Species = 183, Level = 17, Location = 086, Relearn = new[]{453, 270, 061, 205} }, // Marill @ Brooklet Hill

            // Ula'ula Island
            new(SM) { Species = 111, Level = 30, Location = 138, Relearn = new[]{130, 350, 498, 523} }, // Rhyhorn @ Blush Mountain
            new(SM) { Species = 220, Level = 31, Location = 114, Relearn = new[]{573, 036, 420, 196} }, // Swinub @ Tapu Village
            new(SM) { Species = 578, Level = 33, Location = 118, Relearn = new[]{101, 248, 283, 473} }, // Duosion @ Route 16
            new(SM) { Species = 315, Level = 34, Location = 128, Relearn = new[]{437, 275, 230, 390} }, // Roselia @ Ula'ula Meadow
            new(SM) { Species = 397, Level = 27, Location = 106, Relearn = new[]{355, 018, 283, 104} }, // Staravia @ Route 10
            new(SM) { Species = 288, Level = 27, Location = 108, Relearn = new[]{359, 498, 163, 203} }, // Vigoroth @ Route 11
            new(SM) { Species = 610, Level = 28, Location = 136, Relearn = new[]{231, 337, 206, 163} }, // Axew @ Mount Hokulani

            // Poni Island
            new(SM) { Species = 604, Level = 55, Location = 164, Relearn = new[]{435, 051, 029, 306} }, // Eelektross @ Poni Grove
            new(SM) { Species = 534, Level = 57, Location = 166, Relearn = new[]{409, 276, 264, 444} }, // Conkeldurr @ Poni Plains
            new(SM) { Species = 468, Level = 59, Location = 170, Relearn = new[]{248, 403, 396, 245} }, // Togekiss @ Poni Gauntlet
            new(SM) { Species = 542, Level = 57, Location = 156, Relearn = new[]{382, 437, 014, 494} }, // Leavanny @ Poni Meadow
            new(SM) { Species = 497, Level = 43, Location = 184, Relearn = new[]{137, 489, 348, 021} }, // Serperior @ Exeggutor Island
            new(SM) { Species = 503, Level = 43, Location = 158, Relearn = new[]{362, 227, 453, 279} }, // Samurott @ Poni Wilds
            new(SM) { Species = 500, Level = 43, Location = 160, Relearn = new[]{276, 053, 372, 535} }, // Emboar @ Ancient Poni Path
        };

        internal static readonly EncounterTrade7[] TradeGift_SM = // @ a\1\5\5
        {
            // Trades - 4.bin
            new(SM) { Species = 066, Form = 0, Level = 09, Ability = 2, TID = 00410, SID = 00000, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 1, Gender = 0, Nature = Nature.Brave, }, // Machop
            new(SM) { Species = 761, Form = 0, Level = 16, Ability = 1, TID = 20683, SID = 00009, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 0, Gender = 1, Nature = Nature.Adamant, }, // Bounsweet
            new(SM) { Species = 061, Form = 0, Level = 22, Ability = 2, TID = 01092, SID = 00009, IVs = new[] {31,-1,-1,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Naughty, }, // Poliwhirl
            new(SM) { Species = 440, Form = 0, Level = 27, Ability = 2, TID = 10913, SID = 00000, IVs = new[] {-1,-1,-1,-1,31,-1}, OTGender = 1, Gender = 1, Nature = Nature.Calm, }, // Happiny
            new(SM) { Species = 075, Form = 1, Level = 32, Ability = 1, TID = 20778, SID = 00009, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Impish, EvolveOnTrade = true }, // Graveler-1
            new(SM) { Species = 762, Form = 0, Level = 43, Ability = 1, TID = 20679, SID = 00009, IVs = new[] {-1,-1,-1,-1,-1,31}, OTGender = 1, Gender = 1, Nature = Nature.Careful, }, // Steenee
            new(SM) { Species = 663, Form = 0, Level = 59, Ability = 4, TID = 56734, SID = 00008, IVs = new[] {-1,-1,-1,31,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Jolly, }, // Talonflame
        };

        private static readonly EncounterStatic7[] Encounter_USUM =
        {
            new(USUM) { Gift = true, Species = 722, Level = 05, Location = 008, }, // Rowlet
            new(USUM) { Gift = true, Species = 725, Level = 05, Location = 008, }, // Litten
            new(USUM) { Gift = true, Species = 728, Level = 05, Location = 008, }, // Popplio
            new(USUM) { Gift = true, Species = 138, Level = 15, Location = 058, }, // Omanyte
            new(USUM) { Gift = true, Species = 140, Level = 15, Location = 058, }, // Kabuto
         // new(USUM) { Gift = true, Species = 142, Level = 15, Location = 058, }, // Aerodactyl
            new(USUM) { Gift = true, Species = 345, Level = 15, Location = 058, }, // Lileep
            new(USUM) { Gift = true, Species = 347, Level = 15, Location = 058, }, // Anorith
            new(USUM) { Gift = true, Species = 408, Level = 15, Location = 058, }, // Cranidos
            new(USUM) { Gift = true, Species = 410, Level = 15, Location = 058, }, // Shieldon
            new(USUM) { Gift = true, Species = 564, Level = 15, Location = 058, }, // Tirtouga
            new(USUM) { Gift = true, Species = 566, Level = 15, Location = 058, }, // Archen
            new(USUM) { Gift = true, Species = 696, Level = 15, Location = 058, }, // Tyrunt
            new(USUM) { Gift = true, Species = 698, Level = 15, Location = 058, }, // Amaura
            new(USUM) { Gift = true, Species = 133, Level = 01, EggLocation = 60002, }, // Eevee @ Nursery helpers
            new(USUM) { Gift = true, Species = 137, Level = 30, Location = 116, }, // Porygon @ Route 15
            new(USUM) { Gift = true, Species = 772, Level = 60, Location = 188, FlawlessIVCount = 3, }, // Type: Null @ Aether Paradise
            new(USUM) { Gift = true, Species = 772, Level = 60, Location = 160, FlawlessIVCount = 3, }, // Type: Null @ Ancient Poni Path
            new(US  ) { Gift = true, Species = 789, Level = 05, Location = 142, FlawlessIVCount = 3, Shiny = Shiny.Never, Ability = 2 }, // Cosmog @ Lake of the Sunne
            new(  UM) { Gift = true, Species = 789, Level = 05, Location = 144, FlawlessIVCount = 3, Shiny = Shiny.Never, Ability = 2 }, // Cosmog @ Lake of the Moone
            new(USUM) { Gift = true, Species = 142, Level = 40, Location = 172, }, // Aerodactyl @ Seafolk Village
            new(USUM) { Gift = true, Species = 025, Level = 40, Location = 070, FlawlessIVCount = 3, HeldItem = 796, Relearn = new[] {57,0,0,0} }, // Pikachu @ Heahea City
            new(USUM) { Gift = true, Species = 803, Level = 40, Location = 208, FlawlessIVCount = 3, }, // Poipole @ Megalo Tower
            new(USUM) { Gift = true, Species = 803, Level = 40, Location = 206, FlawlessIVCount = 3, }, // Poipole @ Ultra Megalopolis

            // Totem-Sized Gifts @ Heahea Beach
            new(US  ) { Gift = true, Species = 735, Level = 20, Ability = 4, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Gumshoos
            new(  UM) { Gift = true, Species = 020, Level = 20, Ability = 4, Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Raticate
            new(US  ) { Gift = true, Species = 105, Level = 25, Ability = 4, Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Marowak
            new(  UM) { Gift = true, Species = 752, Level = 25, Ability = 1, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Araquanid
            new(US  ) { Gift = true, Species = 754, Level = 30, Ability = 2, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Lurantis
            new(  UM) { Gift = true, Species = 758, Level = 30, Ability = 1, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Salazzle
            new(US  ) { Gift = true, Species = 738, Level = 35, Ability = 1, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Vikavolt
            new(  UM) { Gift = true, Species = 777, Level = 35, Ability = 4, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Togedemaru
            new(USUM) { Gift = true, Species = 778, Level = 40, Ability = 1, Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 },                          // Mimikyu
            new(US  ) { Gift = true, Species = 743, Level = 50, Ability = 4, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Ribombee
            new(  UM) { Gift = true, Species = 784, Level = 50, Ability = 4, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kommo-o

            new(USUM) { Gift = true, Species = 718, Level = 63, Ability = 1, Location = 118, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, }, // Zygarde (10%) @ Route 16

            new(USUM) // Magearna (Bottle Cap)
            {
                Gift = true, Species = 801, Level = 50, Location = 40001, Shiny = Shiny.Never, FlawlessIVCount = 3, HeldItem = 795, Ability = 2,
                Fateful = true, Relearn = new [] {705, 430, 381, 270}, Ball = 0x10, // Cherish
            },

            new(USUM) { Gift = true, Species = 718, Form = 0, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3, }, // Zygarde (50%)
            new(USUM) { Gift = true, Species = 718, Form = 1, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3, }, // Zygarde (10%)
            new(USUM) { Gift = true, Species = 718, Form = 2, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3, }, // Zygarde (10%-C)
            new(USUM) { Gift = true, Species = 718, Form = 3, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3, }, // Zygarde (50%-C)

            new(US  ) { Species = 791, Level = 60, Location = 028, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new[] {713,322,242,428} }, // Solgaleo @ Mahalo Trail (Plank Bridge)
            new(  UM) { Species = 792, Level = 60, Location = 028, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new[] {714,322,539,585} }, // Lunala @ Mahalo Trail (Plank Bridge)

            // QR Scan: Su/M/Tu/W/Th/F/Sa
            // Melemele Island
            new(USUM) { Species = 004, Level = 12, Location = 010, Relearn = new[] {068,108,052,010}, }, // Charmander @ Route 3
            new(USUM) { Species = 007, Level = 12, Location = 042, Relearn = new[] {453,110,055,033}, }, // Squirtle @ Seaward Cave
            new(USUM) { Species = 095, Level = 14, Location = 034, Relearn = new[] {563,099,317,088}, }, // Onix @ Ten Carat Hill
            new(USUM) { Species = 116, Level = 18, Location = 014, Relearn = new[] {352,239,055,043}, }, // Horsea @ Kala'e Bay
            new(USUM) { Species = 664, Level = 09, Location = 020, Relearn = new[] {476,081,078,033}, SkipFormCheck = true, }, // Scatterbug @ Hau'oli City
            new(USUM) { Species = 001, Level = 10, Location = 012, Relearn = new[] {580,022,045,033}, }, // Bulbasaur @ Route 2
            new(USUM) { Species = 607, Level = 09, Location = 038, Relearn = new[] {203,052,083,123}, }, // Litwick @ Hau'oli Cemetery

            // Akala Island
            new(USUM) { Species = 280, Level = 17, Location = 054, Relearn = new[] {581,345,381,574}, }, // Ralts @ Route 6
            new(USUM) { Species = 363, Level = 19, Location = 056, Relearn = new[] {187,362,301,227}, }, // Spheal @ Route 7
            new(USUM) { Species = 256, Level = 20, Location = 058, Relearn = new[] {067,488,064,028}, }, // Combusken @ Route 8
            new(USUM) { Species = 679, Level = 24, Location = 094, Relearn = new[] {469,332,425,475}, }, // Honedge @ Akala Outskirts
            new(USUM) { Species = 015, Level = 14, Location = 050, Relearn = new[] {099,031,041,000}, }, // Beedrill @ Route 4
            new(USUM) { Species = 253, Level = 16, Location = 052, Relearn = new[] {580,072,098,071}, }, // Grovyle @ Route 5
            new(USUM) { Species = 259, Level = 17, Location = 086, Relearn = new[] {068,193,189,055}, }, // Marshtomp @ Brooklet Hill

            // Ula'ula Island
            new(USUM) { Species = 111, Level = 32, Location = 138, Relearn = new[] {470,350,498,523}, }, // Rhyhorn @ Blush Mountain
            new(USUM) { Species = 220, Level = 33, Location = 114, Relearn = new[] {333,036,420,196}, }, // Swinub @ Tapu Village
            new(USUM) { Species = 394, Level = 35, Location = 118, Relearn = new[] {681,362,031,117}, }, // Prinplup @ Route 16
            new(USUM) { Species = 388, Level = 36, Location = 128, Relearn = new[] {484,073,072,044}, }, // Grotle @ Ula'ula Meadow
            new(USUM) { Species = 018, Level = 29, Location = 106, Relearn = new[] {211,297,239,098}, }, // Pidgeot @ Route 10
            new(USUM) { Species = 391, Level = 29, Location = 108, Relearn = new[] {612,172,154,259}, }, // Monferno @ Route 11
            new(USUM) { Species = 610, Level = 30, Location = 136, Relearn = new[] {068,337,206,163}, }, // Axew @ Mount Hokulani

            // Poni Island
            new(USUM) { Species = 604, Level = 55, Location = 164, Relearn = new[] {242,435,029,306}, }, // Eelektross @ Poni Grove
            new(USUM) { Species = 306, Level = 57, Location = 166, Relearn = new[] {179,484,038,334}, }, // Aggron @ Poni Plains
            new(USUM) { Species = 479, Level = 61, Location = 170, Relearn = new[] {268,506,486,164}, }, // Rotom @ Poni Gauntlet
            new(USUM) { Species = 542, Level = 57, Location = 156, Relearn = new[] {580,437,014,494}, }, // Leavanny @ Poni Meadow
            new(USUM) { Species = 652, Level = 45, Location = 184, Relearn = new[] {191,341,402,596}, }, // Chesnaught @ Exeggutor Island
            new(USUM) { Species = 658, Level = 44, Location = 158, Relearn = new[] {516,164,185,594}, }, // Greninja @ Poni Wilds
            new(USUM) { Species = 655, Level = 44, Location = 160, Relearn = new[] {273,473,113,595}, }, // Delphox @ Ancient Poni Path

            new(USUM) { Species = 785, Level = 60, Location = 030, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, }, // Tapu Koko @ Ruins of Conflict
            new(USUM) { Species = 786, Level = 60, Location = 092, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, }, // Tapu Lele @ Ruins of Life
            new(USUM) { Species = 787, Level = 60, Location = 140, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, }, // Tapu Bulu @ Ruins of Abundance
            new(USUM) { Species = 788, Level = 60, Location = 180, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, }, // Tapu Fini @ Ruins of Hope

            new(USUM) { Species = 023, Level = 10, Location = 012, Ability = 1, }, // Ekans @ Route 2

            new(USUM) { Species = 127, Level = 42, Location = 184, Shiny = Shiny.Never, }, // Pinsir @ Exeggutor Island
            new(USUM) { Species = 127, Level = 43, Location = 184, Shiny = Shiny.Never, }, // Pinsir @ Exeggutor Island
            new(USUM) { Species = 800, Level = 65, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new[] {722,334,408,400}, HeldItem = 923, }, // Necrozma @ Mount Lanakila

            // Legendaries
            new(USUM) { Species = 144, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {246,573,115,258}, }, // Articuno
            new(USUM) { Species = 145, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {246,435,365,240}, }, // Zapdos
            new(USUM) { Species = 146, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {246,053,403,241}, }, // Moltres
            new(USUM) { Species = 150, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {094,105,129,427}, }, // Mewtwo
            new(US  ) { Species = 243, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Raikou
            new(  UM) { Species = 244, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {023,044,207,436} }, // Entei
            new(USUM) { Species = 245, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {061,062,054,240} }, // Suicune
            new(  UM) { Species = 249, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {285,177,326,246} }, // Lugia
            new(US  ) { Species = 250, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {682,221,326,246}, HeldItem = 044 }, // Ho-Oh
            new(USUM) { Species = 377, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, }, // Regirock
            new(USUM) { Species = 378, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, }, // Regice
            new(USUM) { Species = 379, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, }, // Registeel
            new(  UM) { Species = 380, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {296,406,375,273}, Gender = 1 }, // Latias
            new(US  ) { Species = 381, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {295,406,375,225}, Gender = 0 }, // Latios
            new(  UM) { Species = 382, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {058,618,347,330} }, // Kyogre
            new(US  ) { Species = 383, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {089,619,339,076} }, // Groudon
            new(USUM) { Species = 384, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, }, // Rayquaza
            new(USUM) { Species = 480, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {326,281,133,129}, }, // Uxie
            new(USUM) { Species = 481, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {326,204,248,129}, }, // Mesprit
            new(USUM) { Species = 482, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {326,417,253,129}, }, // Azelf
            new(US  ) { Species = 483, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Dialga
            new(  UM) { Species = 484, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Palkia
            new(US  ) { Species = 485, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Heatran
            new(  UM) { Species = 486, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {428,279,146,109} }, // Regigigas
            new(USUM) { Species = 487, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {467,396,414,337} }, // Giratina
            new(USUM) { Species = 488, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Gender = 1, }, // Cresselia
            new(USUM) { Species = 638, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {533,014,098,442} }, // Cobalion
            new(USUM) { Species = 639, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {533,014,157,444} }, // Terrakion
            new(USUM) { Species = 640, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {533,014,202,348} }, // Virizion
            new(US  ) { Species = 641, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Gender = 0 }, // Tornadus
            new(  UM) { Species = 642, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Gender = 0 }, // Thundurus
            new(US  ) { Species = 643, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Reshiram
            new(  UM) { Species = 644, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3 }, // Zekrom
            new(USUM) { Species = 645, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Gender = 0, }, // Landorus
            new(USUM) { Species = 646, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, }, // Kyurem
            new(US  ) { Species = 716, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {601,532,400,585} }, // Xerneas
            new(  UM) { Species = 717, Level = 60, Location = 222, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {613,399,566,094} }, // Yveltal
            new(USUM) { Species = 718, Level = 60, Location = 182, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new[] {616,137,219,225} }, // Zygarde @ Resolution Cave

            // Ultra Space Wilds
            new(USUM) { Species = 334, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Altaria
            new(USUM) { Species = 469, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Yanmega
            new(USUM) { Species = 561, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Sigilyph
            new(USUM) { Species = 581, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Swanna
            new(USUM) { Species = 277, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Swellow
            new(USUM) { Species = 452, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Drapion
            new(USUM) { Species = 531, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Audino
            new(USUM) { Species = 695, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Heliolisk
            new(USUM) { Species = 274, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Nuzleaf
            new(USUM) { Species = 326, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Grumpig
            new(USUM) { Species = 460, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Abomasnow
            new(USUM) { Species = 308, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Medicham
            new(USUM) { Species = 450, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Hippowdon
            new(USUM) { Species = 558, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Crustle
            new(USUM) { Species = 219, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Magcargo
            new(USUM) { Species = 689, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Barbaracle
            new(USUM) { Species = 271, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Lombre
            new(USUM) { Species = 618, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Stunfisk
            new(USUM) { Species = 419, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Floatzel
            new(USUM) { Species = 195, Level = 60, Location = 222, FlawlessIVCount = 3, }, // Quagsire

            // Ultra Beasts
            new(USUM) { Species = 793, Level = 60, Location = 190, Ability = 1, FlawlessIVCount = 3, Relearn = new[] {408,491,446,243}, }, // Nihilego @ Ultra Deep Sea
            new(US  ) { Species = 794, Level = 60, Location = 218, Ability = 1, FlawlessIVCount = 3 }, // Buzzwole @ Ultra Jungle
            new(  UM) { Species = 795, Level = 60, Location = 214, Ability = 1, FlawlessIVCount = 3 }, // Pheromosa @ Ultra Desert
            new(USUM) { Species = 796, Level = 60, Location = 210, Ability = 1, FlawlessIVCount = 3 }, // Xurkitree @ Ultra Plant
            new(  UM) { Species = 797, Level = 60, Location = 212, Ability = 1, FlawlessIVCount = 3 }, // Celesteela @ Ultra Crater
            new(US  ) { Species = 798, Level = 60, Location = 216, Ability = 1, FlawlessIVCount = 3 }, // Kartana @ Ultra Forest
            new(USUM) { Species = 799, Level = 60, Location = 220, Ability = 1, FlawlessIVCount = 3 }, // Guzzlord @ Ultra Ruin
            new(  UM) { Species = 805, Level = 60, Location = 164, Ability = 1, FlawlessIVCount = 3 }, // Stakataka @ Poni Grove
            new(US  ) { Species = 806, Level = 60, Location = 164, Ability = 1, FlawlessIVCount = 3 }, // Blacephalon @ Poni Grove

            // Ditto Five
            new(USUM) { Species = 132, Level = 29, Location = 060, IVs = new[] {-1,-1,31,00,30,-1}, Nature = Nature.Bold }, // Ditto @ Route 9
            new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new[] {-1,-1,30,31,30,-1}, Nature = Nature.Jolly }, // Ditto @ Konikoni City
            new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new[] {-1,31,30,30,-1,-1}, Nature = Nature.Adamant }, // Ditto @ Konikoni City
            new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new[] {-1,00,-1,-1,31,30}, Nature = Nature.Modest }, // Ditto @ Konikoni City
            new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new[] {-1,30,-1,31,-1,30}, Nature = Nature.Timid }, // Ditto @ Konikoni City

            // Miscellaneous Static
            new(USUM) { Species = 760, Level = 28, Location = 020, Shiny = Shiny.Never, }, // Bewear @ Hau’oli City (Shopping District)
            new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new[] {095,171,139,029}, }, // Hypno @ Hau'oli City Police Station
            new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new[] {417,060,050,139}, }, // Hypno @ Hau'oli City Police Station
            new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new[] {093,050,001,096}, }, // Hypno @ Hau'oli City Police Station
            new(USUM) { Species = 092, Level = 19, Location = 230, Shiny = Shiny.Never, Relearn = new[] {174,109,122,101}, }, // Gastly @ Route 1 (Trainers’ School)
            new(USUM) { Species = 425, Level = 19, Location = 230, Shiny = Shiny.Never, Relearn = new[] {310,132,016,371}, }, // Drifloon @ Route 1 (Trainers’ School)
            new(  UM) { Species = 769, Level = 30, Location = 116, Shiny = Shiny.Never, Relearn = new[] {310,523,072,328}, }, // Sandygast @ Route 15
            new(USUM) { Species = 592, Level = 34, Location = 126, Shiny = Shiny.Never, Gender = 1, }, // Frillish @ Route 14
            new(USUM) { Species = 101, Level = 60, Location = 224, Ability = 1, Shiny = Shiny.Never, }, // Electrode @ Team Rocket's Castle

            // Crabrawler in Berry Piles
            new(USUM) { Species = 739, Level = 25, Location = 106, }, // Route 10
            new(USUM) { Species = 739, Level = 28, Location = 110, }, // Ula'ula Beach
            new(USUM) { Species = 739, Level = 31, Location = 118, }, // Route 16
            new(USUM) { Species = 739, Level = 32, Location = 120, }, // Route 17
        };

        internal static readonly EncounterTrade7[] TradeGift_USUM =
        {
            // Trades - 4.bin
            new(USUM) { Species = 701, Form = 0, Level = 08, Ability = 2, TID = 00410, SID = 00000, IVs = new[] {-1,31,-1,-1,-1,-1}, OTGender = 1, Gender = 0, Nature = Nature.Brave, }, // Hawlucha
            new(USUM) { Species = 714, Form = 0, Level = 19, Ability = 1, TID = 20683, SID = 00009, IVs = new[] {-1,-1,-1,-1,31,-1}, OTGender = 0, Gender = 0, Nature = Nature.Modest, }, // Noibat
            new(USUM) { Species = 339, Form = 0, Level = 21, Ability = 2, TID = 01092, SID = 00009, IVs = new[] {31,-1,-1,-1,-1,-1}, OTGender = 0, Gender = 1, Nature = Nature.Naughty, }, // Barboach
            new(USUM) { Species = 024, Form = 0, Level = 22, Ability = 1, TID = 10913, SID = 00000, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Impish, }, // Arbok
            new(USUM) { Species = 708, Form = 0, Level = 33, Ability = 1, TID = 20778, SID = 00009, IVs = new[] {-1,-1,-1,-1,-1,31}, OTGender = 0, Gender = 0, Nature = Nature.Calm, EvolveOnTrade = true }, // Phantump
            new(USUM) { Species = 422, Form = 0, Level = 44, Ability = 2, TID = 20679, SID = 00009, IVs = new[] {-1,-1,31,-1,-1,-1}, OTGender = 1, Gender = 1, Nature = Nature.Quiet, }, // Shellos
            new(USUM) { Species = 128, Form = 0, Level = 59, Ability = 1, TID = 56734, SID = 00008, IVs = new[] {-1,-1,-1,31,-1,-1}, OTGender = 0, Gender = 0, Nature = Nature.Jolly, }, // Tauros
        };

        private const string tradeSM = "tradesm";
        private const string tradeUSUM = "tradeusum";
        private static readonly string[][] TradeSM = Util.GetLanguageStrings10(tradeSM);
        private static readonly string[][] TradeUSUM = Util.GetLanguageStrings10(tradeUSUM);

        internal static readonly EncounterStatic7[] StaticSN = GetEncounters(Encounter_SM, SN);
        internal static readonly EncounterStatic7[] StaticMN = GetEncounters(Encounter_SM, MN);
        internal static readonly EncounterStatic7[] StaticUS = GetEncounters(Encounter_USUM, US);
        internal static readonly EncounterStatic7[] StaticUM = GetEncounters(Encounter_USUM, UM);
    }
}
