using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.GroundTileAllowed;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Encounters
/// </summary>
internal static class Encounters4
{
    internal static readonly EncounterArea4[] SlotsD = EncounterArea4.GetAreas(Get("d", "da"), D);
    internal static readonly EncounterArea4[] SlotsP = EncounterArea4.GetAreas(Get("p", "pe"), P);
    internal static readonly EncounterArea4[] SlotsPt = EncounterArea4.GetAreas(Get("pt", "pt"), Pt);
    internal static readonly EncounterArea4[] SlotsHG = EncounterArea4.GetAreas(Get("hg", "hg"), HG);
    internal static readonly EncounterArea4[] SlotsSS = EncounterArea4.GetAreas(Get("ss", "ss"), SS);

    private static readonly EncounterStatic4Pokewalker[] Encounter_PokeWalker = EncounterStatic4Pokewalker.GetAll(Util.GetBinaryResource("encounter_walker4.pkl"));

    static Encounters4()
    {
        MarkEncounterTradeStrings(TradeGift_DPPt, TradeDPPt);
        MarkEncounterTradeStrings(TradeGift_HGSS, TradeHGSS);
    }

    #region Static Encounter/Gift Tables
    private static readonly EncounterStatic4[] Encounter_DPPt =
    {
        // Starters
        new(DP) { Gift = true, Species = 387, Level = 5, Location = 076, GroundTile = Max_DP }, // Turtwig @ Lake Verity
        new(DP) { Gift = true, Species = 390, Level = 5, Location = 076, GroundTile = Max_DP }, // Chimchar
        new(DP) { Gift = true, Species = 393, Level = 5, Location = 076, GroundTile = Max_DP }, // Piplup
        new(Pt) { Gift = true, Species = 387, Level = 5, Location = 016, GroundTile = Max_Pt }, // Turtwig @ Route 201
        new(Pt) { Gift = true, Species = 390, Level = 5, Location = 016, GroundTile = Max_Pt }, // Chimchar
        new(Pt) { Gift = true, Species = 393, Level = 5, Location = 016, GroundTile = Max_Pt }, // Piplup

        // Fossil @ Mining Museum
        new(DP) { Gift = true, Species = 138, Level = 20, Location = 094, GroundTile = Max_DP }, // Omanyte
        new(DP) { Gift = true, Species = 140, Level = 20, Location = 094, GroundTile = Max_DP }, // Kabuto
        new(DP) { Gift = true, Species = 142, Level = 20, Location = 094, GroundTile = Max_DP }, // Aerodactyl
        new(DP) { Gift = true, Species = 345, Level = 20, Location = 094, GroundTile = Max_DP }, // Lileep
        new(DP) { Gift = true, Species = 347, Level = 20, Location = 094, GroundTile = Max_DP }, // Anorith
        new(DP) { Gift = true, Species = 408, Level = 20, Location = 094, GroundTile = Max_DP }, // Cranidos
        new(DP) { Gift = true, Species = 410, Level = 20, Location = 094, GroundTile = Max_DP }, // Shieldon
        new(Pt) { Gift = true, Species = 138, Level = 20, Location = 094, GroundTile = Max_Pt }, // Omanyte
        new(Pt) { Gift = true, Species = 140, Level = 20, Location = 094, GroundTile = Max_Pt }, // Kabuto
        new(Pt) { Gift = true, Species = 142, Level = 20, Location = 094, GroundTile = Max_Pt }, // Aerodactyl
        new(Pt) { Gift = true, Species = 345, Level = 20, Location = 094, GroundTile = Max_Pt }, // Lileep
        new(Pt) { Gift = true, Species = 347, Level = 20, Location = 094, GroundTile = Max_Pt }, // Anorith
        new(Pt) { Gift = true, Species = 408, Level = 20, Location = 094, GroundTile = Max_Pt }, // Cranidos
        new(Pt) { Gift = true, Species = 410, Level = 20, Location = 094, GroundTile = Max_Pt }, // Shieldon

        // Gift
        new(DP) { Gift = true, Species = 133, Level = 05, Location = 010, GroundTile = Max_DP }, // Eevee @ Hearthome City
        new(Pt) { Gift = true, Species = 133, Level = 20, Location = 010, GroundTile = Max_Pt }, // Eevee @ Hearthome City
        new(Pt) { Gift = true, Species = 137, Level = 25, Location = 012, GroundTile = Max_Pt }, // Porygon @ Veilstone City
        new(Pt) { Gift = true, Species = 175, Level = 01, EggLocation = 2011 }, // Togepi Egg from Cynthia
        new(DP) { Gift = true, Species = 440, Level = 01, EggLocation = 2009 }, // Happiny Egg from Traveling Man
        new(DPPt) { Gift = true, Species = 447, Level = 01, EggLocation = 2010 }, // Riolu Egg from Riley

        // Stationary
        new(DP) { Species = 425, Level = 22, Location = 47 }, // Drifloon @ Valley Windworks
        new(Pt) { Species = 425, Level = 15, Location = 47 }, // Drifloon @ Valley Windworks
        new(DP) { Species = 479, Level = 15, Location = 70, GroundTile = Building }, // Rotom @ Old Chateau
        new(Pt) { Species = 479, Level = 20, Location = 70, GroundTile = Building }, // Rotom @ Old Chateau
        new(DPPt) { Species = 442, Level = 25, Location = 24 }, // Spiritomb @ Route 209

        // Stationary Legendary
        new(Pt) { Species = 377, Level = 30, Location = 125, GroundTile = Cave }, // Regirock @ Rock Peak Ruins
        new(Pt) { Species = 378, Level = 30, Location = 124, GroundTile = Cave }, // Regice @ Iceberg Ruins
        new(Pt) { Species = 379, Level = 30, Location = 123, GroundTile = Cave }, // Registeel @ Iron Ruins
        new(DPPt) { Species = 480, Level = 50, Location = 089, GroundTile = Cave }, // Uxie @ Acuity Cavern
        new(DPPt) { Species = 482, Level = 50, Location = 088, GroundTile = Cave }, // Azelf @ Valor Cavern
        new(D ) { Species = 483, Level = 47, Location = 051, GroundTile = Rock }, // Dialga @ Spear Pillar
        new( P) { Species = 484, Level = 47, Location = 051, GroundTile = Rock }, // Palkia @ Spear Pillar
        new(Pt) { Species = 483, Level = 70, Location = 051, GroundTile = Rock }, // Dialga @ Spear Pillar
        new(Pt) { Species = 484, Level = 70, Location = 051, GroundTile = Rock }, // Palkia @ Spear Pillar
        new(DP) { Species = 485, Level = 70, Location = 084, GroundTile = Cave }, // Heatran @ Stark Mountain
        new(Pt) { Species = 485, Level = 50, Location = 084, GroundTile = Cave }, // Heatran @ Stark Mountain
        new(DP) { Species = 486, Level = 70, Location = 064, GroundTile = Cave }, // Regigigas @ Snowpoint Temple
        new(Pt) { Species = 486, Level = 01, Location = 064, GroundTile = Cave }, // Regigigas @ Snowpoint Temple
        new(DP) { Species = 487, Level = 70, Location = 062, GroundTile = Cave, Form = 0 }, // Giratina @ Turnback Cave
        new(Pt) { Species = 487, Level = 47, Location = 062, GroundTile = Cave, Form = 0 }, // Giratina @ Turnback Cave
        new(Pt) { Species = 487, Level = 47, Location = 117, GroundTile = Distortion, Form = 1, HeldItem = 112 }, // Giratina @ Distortion World

        // Event
        //new(DP) { Species = 491, Level = 40, Location = 079, GroundTile = Grass }, // Darkrai @ Newmoon Island (Unreleased in Diamond and Pearl)
        new(Pt) { Species = 491, Level = 50, Location = 079, GroundTile = Grass }, // Darkrai @ Newmoon Island
        new(Pt) { Species = 492, Form = 0, Level = 30, Location = 063, Fateful = true }, // Shaymin @ Flower Paradise
        //new(DP) { Species = 492, Form = 0, Level = 30, Location = 063, Fateful = false }, // Shaymin @ Flower Paradise (Unreleased in Diamond and Pearl)
        //new(DPPt) { Species = 493, Form = 0, Level = 80, Location = 086, GroundTile = Cave }, // Arceus @ Hall of Origin (Unreleased)

        // Roamers
        new(DPPt) { Roaming = true, Location = 16, Species = 481, Level = 50, GroundTile = Grass | Water }, // Mesprit
        new(DPPt) { Roaming = true, Location = 16, Species = 488, Level = 50, GroundTile = Grass | Water }, // Cresselia
        new(Pt)   { Roaming = true, Location = 16, Species = 144, Level = 60, GroundTile = Grass | Water }, // Articuno
        new(Pt)   { Roaming = true, Location = 16, Species = 145, Level = 60, GroundTile = Grass | Water }, // Zapdos
        new(Pt)   { Roaming = true, Location = 16, Species = 146, Level = 60, GroundTile = Grass | Water }, // Moltres
    };

    private static readonly EncounterStatic4[] Encounter_HGSS =
    {
        // Starters
        new(HGSS) { Gift = true, Species = 001, Level = 05, Location = 138, GroundTile = Max_Pt }, // Bulbasaur @ Pallet Town
        new(HGSS) { Gift = true, Species = 004, Level = 05, Location = 138, GroundTile = Max_Pt }, // Charmander
        new(HGSS) { Gift = true, Species = 007, Level = 05, Location = 138, GroundTile = Max_Pt }, // Squirtle
        new(HGSS) { Gift = true, Species = 152, Level = 05, Location = 126, GroundTile = Max_DP }, // Chikorita @ New Bark Town
        new(HGSS) { Gift = true, Species = 155, Level = 05, Location = 126, GroundTile = Max_DP }, // Cyndaquil
        new(HGSS) { Gift = true, Species = 158, Level = 05, Location = 126, GroundTile = Max_DP }, // Totodile
        new(HGSS) { Gift = true, Species = 252, Level = 05, Location = 148, GroundTile = Max_Pt }, // Treecko @ Saffron City
        new(HGSS) { Gift = true, Species = 255, Level = 05, Location = 148, GroundTile = Max_Pt }, // Torchic
        new(HGSS) { Gift = true, Species = 258, Level = 05, Location = 148, GroundTile = Max_Pt }, // Mudkip

        // Fossils @ Pewter City
        new(HGSS) { Gift = true, Species = 138, Level = 20, Location = 140, GroundTile = Max_Pt }, // Omanyte
        new(HGSS) { Gift = true, Species = 140, Level = 20, Location = 140, GroundTile = Max_Pt }, // Kabuto
        new(HGSS) { Gift = true, Species = 142, Level = 20, Location = 140, GroundTile = Max_Pt }, // Aerodactyl
        new(HGSS) { Gift = true, Species = 345, Level = 20, Location = 140, GroundTile = Max_Pt }, // Lileep
        new(HGSS) { Gift = true, Species = 347, Level = 20, Location = 140, GroundTile = Max_Pt }, // Anorith
        new(HGSS) { Gift = true, Species = 408, Level = 20, Location = 140, GroundTile = Max_Pt }, // Cranidos
        new(HGSS) { Gift = true, Species = 410, Level = 20, Location = 140, GroundTile = Max_Pt }, // Shieldon

        // Gift
        new(HGSS) { Gift = true, Species = 072, Level = 15, Location = 130, GroundTile = Max_Pt }, // Tentacool @ Cianwood City
        new(HGSS) { Gift = true, Species = 133, Level = 05, Location = 131, GroundTile = Max_Pt }, // Eevee @ Goldenrod City
        new(HGSS) { Gift = true, Species = 147, Level = 15, Location = 222, GroundTile = Max_Pt, Moves = new(245) }, // Dratini @ Dragon's Den (ExtremeSpeed)
        new(HGSS) { Gift = true, Species = 236, Level = 10, Location = 216, GroundTile = Max_Pt }, // Tyrogue @ Mt. Mortar
        new(HGSS) { Gift = true, Species = 175, Level = 01, EggLocation = 2013, Moves = new((int)Move.Growl, (int)Move.Charm, (int)Move.Extrasensory) }, // Togepi Egg from Mr. Pokemon (Extrasensory as Egg move)
        new(HGSS) { Gift = true, Species = 179, Level = 01, EggLocation = 2014 }, // Mareep Egg from Primo
        new(HGSS) { Gift = true, Species = 194, Level = 01, EggLocation = 2014 }, // Wooper Egg from Primo
        new(HGSS) { Gift = true, Species = 218, Level = 01, EggLocation = 2014 }, // Slugma Egg from Primo

        // Celadon City Game Corner
        new(HGSS) { Gift = true, Species = 122, Level = 15, Location = 144, GroundTile = Max_Pt }, // Mr. Mime
        new(HGSS) { Gift = true, Species = 133, Level = 15, Location = 144, GroundTile = Max_Pt }, // Eevee
        new(HGSS) { Gift = true, Species = 137, Level = 15, Location = 144, GroundTile = Max_Pt }, // Porygon

        // Goldenrod City Game Corner
        new(HGSS) { Gift = true, Species = 063, Level = 15, Location = 131, GroundTile = Max_Pt }, // Abra
        new(HG  ) { Gift = true, Species = 023, Level = 15, Location = 131, GroundTile = Max_Pt }, // Ekans
        new(  SS) { Gift = true, Species = 027, Level = 15, Location = 131, GroundTile = Max_Pt }, // Sandshrew
        new(HGSS) { Gift = true, Species = 147, Level = 15, Location = 131, GroundTile = Max_Pt }, // Dratini

        // Team Rocket HQ Trap Floor
        new(HGSS) { Species = 100, Level = 23, Location = 213, GroundTile = Building }, // Voltorb
        new(HGSS) { Species = 074, Level = 21, Location = 213, GroundTile = Building }, // Geodude
        new(HGSS) { Species = 109, Level = 21, Location = 213, GroundTile = Building }, // Koffing

        // Stationary
        new(HGSS) { Species = 130, Level = 30, Location = 135, GroundTile = Water, Shiny = Shiny.Always }, // Gyarados @ Lake of Rage
        new(HGSS) { Species = 131, Level = 20, Location = 210, GroundTile = Water }, // Lapras @ Union Cave Friday Only
        new(HGSS) { Species = 101, Level = 23, Location = 213, GroundTile = Building }, // Electrode @ Team Rocket HQ
        new(HGSS) { Species = 143, Level = 50, Location = 159 }, // Snorlax @ Route 11
        new(HGSS) { Species = 143, Level = 50, Location = 160 }, // Snorlax @ Route 12
        new(HGSS) { Species = 185, Level = 20, Location = 184 }, // Sudowoodo @ Route 36, Encounter does not have type

        new(HGSS) // Spiky-Eared Pichu @ Ilex Forest
        {
            Species = 172,
            Level = 30,
            Gender = 1,
            Form = 1,
            Nature = Nature.Naughty,
            Location = 214,
            Moves = new(344, 270, 207, 220),
            GroundTile = Max_Pt,
            Shiny = Shiny.Never,
        },

        // Stationary Legendary
        new(HGSS) { Species = 144, Level = 50, Location = 203, GroundTile = Cave }, // Articuno @ Seafoam Islands
        new(HGSS) { Species = 145, Level = 50, Location = 158 }, // Zapdos @ Route 10
        new(HGSS) { Species = 146, Level = 50, Location = 219, GroundTile = Cave }, // Moltres @ Mt. Silver Cave
        new(HGSS) { Species = 150, Level = 70, Location = 199, GroundTile = Cave }, // Mewtwo @ Cerulean Cave
        new(HGSS) { Species = 245, Level = 40, Location = 173 }, // Suicune @ Route 25
        new(HGSS) { Species = 245, Level = 40, Location = 206, GroundTile = Cave }, // Suicune @ Burned Tower
        new(  SS) { Species = 249, Level = 45, Location = 218, GroundTile = Water }, // Lugia @ Whirl Islands
        new(HG  ) { Species = 249, Level = 70, Location = 218, GroundTile = Water }, // Lugia @ Whirl Islands
        new(HG  ) { Species = 250, Level = 45, Location = 205, GroundTile = Building }, // Ho-Oh @ Bell Tower
        new(  SS) { Species = 250, Level = 70, Location = 205, GroundTile = Building }, // Ho-Oh @ Bell Tower
        new(  SS) { Species = 380, Level = 40, Location = 140, GroundTile = Building }, // Latias @ Pewter City
        new(HG  ) { Species = 381, Level = 40, Location = 140, GroundTile = Building }, // Latios @ Pewter City
        new(HG  ) { Species = 382, Level = 50, Location = 232, GroundTile = Cave }, // Kyogre @ Embedded Tower
        new(  SS) { Species = 383, Level = 50, Location = 232, GroundTile = Cave }, // Groudon @ Embedded Tower
        new(HGSS) { Species = 384, Level = 50, Location = 232, GroundTile = Cave }, // Rayquaza @ Embedded Tower
        new(HGSS) { Species = 483, Level = 01, Location = 231, Gift = true, GroundTile = Max_Pt }, // Dialga @ Sinjoh Ruins
        new(HGSS) { Species = 484, Level = 01, Location = 231, Gift = true, GroundTile = Max_Pt }, // Palkia @ Sinjoh Ruins
        new(HGSS) { Species = 487, Level = 01, Location = 231, Gift = true, GroundTile = Max_Pt, Form = 1, HeldItem = 112 }, // Giratina @ Sinjoh Ruins

        // Johto Roamers
        new(HGSS) { Roaming = true, Species = 243, Location = 177, Level = 40, GroundTile = Grass | Water }, // Raikou
        new(HGSS) { Roaming = true, Species = 244, Location = 177, Level = 40, GroundTile = Grass | Water }, // Entei

        // Kanto Roamers
        new(HG  ) { Roaming = true, Species = 380, Location = 149, Level = 35, GroundTile = Grass | Water }, // Latias
        new(  SS) { Roaming = true, Species = 381, Location = 149, Level = 35, GroundTile = Grass | Water }, // Latios
    };
    #endregion
    #region Trade Tables

    private static readonly EncounterTrade4[] RanchGifts =
    {
        new EncounterTrade4RanchGift(323975838, 025, 18) { Moves = new(447,085,148,104), TID = 1000, SID = 19840, OTGender = 1, MetLocation = 0068, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 20 }, // Pikachu
        new EncounterTrade4RanchGift(323977664, 037, 16) { Moves = new(412,109,053,219), TID = 1000, SID = 21150, OTGender = 1, MetLocation = 3000, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 30 }, // Vulpix
        new EncounterTrade4RanchGift(323975579, 077, 13) { Moves = new(036,033,039,052), TID = 1000, SID = 01123, OTGender = 1, MetLocation = 3000, Gender = 0, Ability = OnlySecond, CurrentLevel = 16 }, // Ponyta
        new EncounterTrade4RanchGift(323975564, 108, 34) { Moves = new(076,111,014,205), TID = 1000, SID = 03050, OTGender = 1, MetLocation = 0077, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 40 }, // Lickitung
        new EncounterTrade4RanchGift(323977579, 114, 01) { Moves = new(437,438,079,246), TID = 1000, SID = 49497, OTGender = 1, MetLocation = 3000, Gender = 1, Ability = OnlySecond }, // Tangela
        new EncounterTrade4RanchGift(323977675, 133, 16) { Moves = new(363,270,098,247), TID = 1000, SID = 47710, OTGender = 1, MetLocation = 0068, Gender = 0, Ability = OnlySecond, CurrentLevel = 30 }, // Eevee
        new EncounterTrade4RanchGift(323977588, 142, 20) { Moves = new(363,089,444,332), TID = 1000, SID = 43066, OTGender = 1, MetLocation = 0094, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 50 }, // Aerodactyl
        new EncounterTrade4RanchGift(232975554, 193, 22) { Moves = new(318,095,246,138), TID = 1000, SID = 42301, OTGender = 1, MetLocation = 0052, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 45, Ball = 5 }, // Yanma
        new EncounterTrade4RanchGift(323975570, 241, 16) { Moves = new(208,215,360,359), TID = 1000, SID = 02707, OTGender = 1, MetLocation = 3000, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 48 }, // Miltank
        new EncounterTrade4RanchGift(323975563, 285, 22) { Moves = new(402,147,206,078), TID = 1000, SID = 02788, OTGender = 1, MetLocation = 3000, Gender = 0, Ability = OnlySecond, CurrentLevel = 45, Ball = 5 }, // Shroomish
        new EncounterTrade4RanchGift(323975559, 320, 30) { Moves = new(156,323,133,058), TID = 1000, SID = 27046, OTGender = 1, MetLocation = 0038, Gender = 0, Ability = OnlySecond, CurrentLevel = 45 }, // Wailmer
        new EncounterTrade4RanchGift(323977657, 360, 01) { Moves = new(204,150,227,000), TID = 1000, SID = 01788, OTGender = 1, MetLocation = 0004, Gender = 0, Ability = OnlySecond, EggLocation = 2000 }, // Wynaut
        new EncounterTrade4RanchGift(323975563, 397, 02) { Moves = new(355,017,283,018), TID = 1000, SID = 59298, OTGender = 1, MetLocation = 0016, Gender = 0, Ability = OnlySecond, CurrentLevel = 23 }, // Staravia
        new EncounterTrade4RanchGift(323970584, 415, 05) { Moves = new(230,016,000,000), TID = 1000, SID = 54140, OTGender = 1, MetLocation = 0020, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 20 }, // Combee
        new EncounterTrade4RanchGift(323977539, 417, 09) { Moves = new(447,045,351,098), TID = 1000, SID = 18830, OTGender = 1, MetLocation = 0020, Gender = 1, Ability = OnlySecond, CurrentLevel = 10 }, // Pachirisu
        new EncounterTrade4RanchGift(323974107, 422, 20) { Moves = new(363,352,426,104), TID = 1000, SID = 39272, OTGender = 1, MetLocation = 0028, Gender = 0, Ability = OnlySecond, CurrentLevel = 25, Form = 1 }, // Shellos
        new EncounterTrade4RanchGift(323977566, 427, 10) { Moves = new(204,193,409,098), TID = 1000, SID = 31045, OTGender = 1, MetLocation = 3000, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 16 }, // Buneary
        new EncounterTrade4RanchGift(323975579, 453, 22) { Moves = new(310,207,426,389), TID = 1000, SID = 41342, OTGender = 1, MetLocation = 0052, Gender = 0, Ability = OnlySecond, CurrentLevel = 31, Ball = 5 }, // Croagunk
        new EncounterTrade4RanchGift(323977566, 456, 15) { Moves = new(213,352,219,392), TID = 1000, SID = 48348, OTGender = 1, MetLocation = 0020, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 35 }, // Finneon
        new EncounterTrade4RanchGift(323975582, 459, 32) { Moves = new(452,420,275,059), TID = 1000, SID = 23360, OTGender = 1, MetLocation = 0031, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 41 }, // Snover
        new EncounterTrade4RanchSpecial(151, 50) { Moves = new(235,216,095,100), TID = 1000, SID = 59228, OTGender = 1, Ball = 0x10, Gender = 2 }, // Mew
        new EncounterTrade4RanchSpecial(489, 01) { Moves = new(447,240,156,057), TID = 1000, SID = 09248, OTGender = 1, Ball = 0x10, Gender = 2, CurrentLevel = 50, EggLocation = 3000 }, // Phione
    };

    private static readonly EncounterTrade4PID[] TradeGift_DPPtIngame =
    {
        new(DPPt, 0x0000008E, 063, 01) { Ability = OnlyFirst,  TID = 25643, SID = 00000, OTGender = 1, Gender = 0, IVs = new(15,15,15,20,25,25) }, // Machop -> Abra
        new(DPPt, 0x00000867, 441, 01) { Ability = OnlySecond, TID = 44142, SID = 00000, OTGender = 0, Gender = 1, IVs = new(15,20,15,25,25,15), Contest = 20 }, // Buizel -> Chatot
        new(DPPt, 0x00000088, 093, 35) { Ability = OnlyFirst,  TID = 19248, SID = 00000, OTGender = 1, Gender = 0, IVs = new(20,25,15,25,15,15) }, // Medicham (35 from Route 217) -> Haunter
        new(DPPt, 0x0000045C, 129, 01) { Ability = OnlyFirst,  TID = 53277, SID = 00000, OTGender = 0, Gender = 1, IVs = new(15,25,15,20,25,15) }, // Finneon -> Magikarp
    };

    internal static readonly EncounterTrade4[] TradeGift_DPPt = ArrayUtil.ConcatAll(TradeGift_DPPtIngame, RanchGifts);

    internal static readonly EncounterTrade4PID[] TradeGift_HGSS =
    {
        new(HGSS, 0x000025EF, 095, 01) { Ability = OnlySecond, TID = 48926, SID = 00000, OTGender = 0, Gender = 0, IVs = new(25,20,25,15,15,15) }, // Bellsprout -> Onix
        new(HGSS, 0x00002310, 066, 01) { Ability = OnlyFirst,  TID = 37460, SID = 00000, OTGender = 0, Gender = 1, IVs = new(15,25,20,20,15,15) }, // Drowzee -> Machop
        new(HGSS, 0x000001DB, 100, 01) { Ability = OnlySecond, TID = 29189, SID = 00000, OTGender = 0, Gender = 2, IVs = new(15,20,15,25,25,15) }, // Krabby -> Voltorb
        new(HGSS, 0x0001FC0A, 085, 15) { Ability = OnlyFirst,  TID = 00283, SID = 00000, OTGender = 1, Gender = 1, IVs = new(20,20,20,15,15,15) }, // Dragonair (15 from DPPt) -> Dodrio
        new(HGSS, 0x0000D136, 082, 19) { Ability = OnlyFirst,  TID = 50082, SID = 00000, OTGender = 0, Gender = 2, IVs = new(15,20,15,20,20,20) }, // Dugtrio (19 from Diglett's Cave) -> Magneton
        new(HGSS, 0x000034E4, 178, 16) { Ability = OnlyFirst,  TID = 15616, SID = 00000, OTGender = 0, Gender = 0, IVs = new(15,20,15,20,20,20) }, // Haunter (16 from Old Chateau) -> Xatu
        new(HGSS, 0x00485876, 025, 02) { Ability = OnlyFirst,  TID = 33038, SID = 00000, OTGender = 0, Gender = 1, IVs = new(20,25,18,31,25,13) }, // Pikachu
        new(HGSS, 0x0012B6D4, 374, 31) { Ability = OnlyFirst,  TID = 23478, SID = 00000, OTGender = 0, Gender = 2, IVs = new(28,29,24,23,24,25) }, // Forretress -> Beldum
        new(HGSS, 0x0012971C, 111, 01) { Ability = OnlyFirst,  TID = 06845, SID = 00000, OTGender = 0, Gender = 1, IVs = new(22,31,13,00,22,09), Moves = new(422) }, // Bonsly -> Rhyhorn
        new(HGSS, 0x00101596, 208, 01) { Ability = OnlyFirst,  TID = 26491, SID = 00000, OTGender = 1, Gender = 0, IVs = new(08,30,28,06,18,20) }, // Any -> Steelix

        //Gift
        new(HGSS, 0x00006B5E, 021, 20) { Ability = OnlyFirst,  TID = 01001, SID = 00000, OTGender = 0, Gender = 1, IVs = new(15,20,15,20,20,20), MetLocation = 183, Moves = new(043,031,228,332) },// Webster's Spearow
        new(HGSS, 0x000214D7, 213, 20) { Ability = OnlySecond, TID = 04336, SID = 00001, OTGender = 0, Gender = 0, IVs = new(15,20,15,20,20,20), MetLocation = 130, Moves = new(132,117,227,219) },// Kirk's Shuckle
    };

    private const string tradeDPPt = "tradedppt";
    private const string tradeHGSS = "tradehgss";
    private static readonly string[][] TradeDPPt = Util.GetLanguageStrings8(tradeDPPt);
    private static readonly string[][] TradeHGSS = Util.GetLanguageStrings8(tradeHGSS);
    #endregion

    internal static readonly EncounterStatic4[] StaticD = GetEncounters(Encounter_DPPt, D);
    internal static readonly EncounterStatic4[] StaticP = GetEncounters(Encounter_DPPt, P);
    internal static readonly EncounterStatic4[] StaticPt = GetEncounters(Encounter_DPPt, Pt);
    internal static readonly EncounterStatic[] StaticHG = GetEncounters(ArrayUtil.ConcatAll<EncounterStatic>(Encounter_HGSS, Encounter_PokeWalker), HG);
    internal static readonly EncounterStatic[] StaticSS = GetEncounters(ArrayUtil.ConcatAll<EncounterStatic>(Encounter_HGSS, Encounter_PokeWalker), SS);
}
