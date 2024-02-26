using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.GroundTileAllowed;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Encounters
/// </summary>
internal static class Encounters4DPPt
{
    internal static readonly EncounterArea4[] SlotsD = EncounterArea4.GetAreas(Get("d", "da"u8), D);
    internal static readonly EncounterArea4[] SlotsP = EncounterArea4.GetAreas(Get("p", "pe"u8), P);
    internal static readonly EncounterArea4[] SlotsPt = EncounterArea4.GetAreas(Get("pt", "pt"u8), Pt);

    private const string tradeDPPt = "tradedppt";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings8(tradeDPPt);

    #region Static Encounter/Gift Tables

    public static readonly EncounterStatic4[] StaticDPPt =
    [
        // Gift
        new(DPPt) { FixedBall = Ball.Poke, Species = 447, Level = 01, Location = 000, EggLocation = 2010 }, // Riolu Egg from Riley
        new(DPPt) { Species = 442, Level = 25, Location = 24 }, // Spiritomb @ Route 209
        new(DPPt) { Species = 480, Level = 50, Location = 089, GroundTile = Cave }, // Uxie @ Acuity Cavern
        new(DPPt) { Species = 482, Level = 50, Location = 088, GroundTile = Cave }, // Azelf @ Valor Cavern

        // Roamers
        new(DPPt) { Roaming = true, Location = 16, Species = 481, Level = 50, GroundTile = Grass | Water }, // Mesprit
        new(DPPt) { Roaming = true, Location = 16, Species = 488, Level = 50, GroundTile = Grass | Water }, // Cresselia
    ];

    public static readonly EncounterStatic4[] StaticDP =
    [
        new(DP) { FixedBall = Ball.Poke, Species = 133, Level = 05, Location = 010, GroundTile = Max_DP }, // Eevee @ Hearthome City
        new(DP) { FixedBall = Ball.Poke, Species = 440, Level = 01, Location = 000, EggLocation = 2009 }, // Happiny Egg from Traveling Man

        new(DP) { FixedBall = Ball.Poke, Species = 387, Level = 5, Location = 076, GroundTile = Max_DP }, // Turtwig @ Lake Verity
        new(DP) { FixedBall = Ball.Poke, Species = 390, Level = 5, Location = 076, GroundTile = Max_DP }, // Chimchar
        new(DP) { FixedBall = Ball.Poke, Species = 393, Level = 5, Location = 076, GroundTile = Max_DP }, // Piplup

        new(DP) { FixedBall = Ball.Poke, Species = 138, Level = 20, Location = 094, GroundTile = Max_DP }, // Omanyte
        new(DP) { FixedBall = Ball.Poke, Species = 140, Level = 20, Location = 094, GroundTile = Max_DP }, // Kabuto
        new(DP) { FixedBall = Ball.Poke, Species = 142, Level = 20, Location = 094, GroundTile = Max_DP }, // Aerodactyl
        new(DP) { FixedBall = Ball.Poke, Species = 345, Level = 20, Location = 094, GroundTile = Max_DP }, // Lileep
        new(DP) { FixedBall = Ball.Poke, Species = 347, Level = 20, Location = 094, GroundTile = Max_DP }, // Anorith
        new(DP) { FixedBall = Ball.Poke, Species = 408, Level = 20, Location = 094, GroundTile = Max_DP }, // Cranidos
        new(DP) { FixedBall = Ball.Poke, Species = 410, Level = 20, Location = 094, GroundTile = Max_DP }, // Shieldon

        new(DP) { Species = 425, Level = 22, Location = 47 }, // Drifloon @ Valley Windworks
        new(DP) { Species = 479, Level = 15, Location = 70, GroundTile = Building }, // Rotom @ Old Château

        new(DP) { Species = 485, Level = 70, Location = 084, GroundTile = Cave }, // Heatran @ Stark Mountain
        new(DP) { Species = 486, Level = 70, Location = 064, GroundTile = Cave }, // Regigigas @ Snowpoint Temple
        new(DP) { Species = 487, Level = 70, Location = 062, GroundTile = Cave, Form = 0 }, // Giratina @ Turnback Cave
        //new(DP) { Species = 492, Form = 0, Level = 30, Location = 063, Fateful = false }, // Shaymin @ Flower Paradise (Unreleased in Diamond and Pearl)
    ];

    public static readonly EncounterStatic4[] StaticPt =
    [
        new(Pt) { FixedBall = Ball.Poke, Species = 387, Level = 5, Location = 016, GroundTile = Max_Pt }, // Turtwig @ Route 201
        new(Pt) { FixedBall = Ball.Poke, Species = 390, Level = 5, Location = 016, GroundTile = Max_Pt }, // Chimchar
        new(Pt) { FixedBall = Ball.Poke, Species = 393, Level = 5, Location = 016, GroundTile = Max_Pt }, // Piplup

        new(Pt) { FixedBall = Ball.Poke, Species = 138, Level = 20, Location = 094, GroundTile = Max_Pt }, // Omanyte
        new(Pt) { FixedBall = Ball.Poke, Species = 140, Level = 20, Location = 094, GroundTile = Max_Pt }, // Kabuto
        new(Pt) { FixedBall = Ball.Poke, Species = 142, Level = 20, Location = 094, GroundTile = Max_Pt }, // Aerodactyl
        new(Pt) { FixedBall = Ball.Poke, Species = 345, Level = 20, Location = 094, GroundTile = Max_Pt }, // Lileep
        new(Pt) { FixedBall = Ball.Poke, Species = 347, Level = 20, Location = 094, GroundTile = Max_Pt }, // Anorith
        new(Pt) { FixedBall = Ball.Poke, Species = 408, Level = 20, Location = 094, GroundTile = Max_Pt }, // Cranidos
        new(Pt) { FixedBall = Ball.Poke, Species = 410, Level = 20, Location = 094, GroundTile = Max_Pt }, // Shieldon

        new(Pt) { FixedBall = Ball.Poke, Species = 133, Level = 20, Location = 010, GroundTile = Max_Pt }, // Eevee @ Hearthome City
        new(Pt) { FixedBall = Ball.Poke, Species = 137, Level = 25, Location = 012, GroundTile = Max_Pt }, // Porygon @ Veilstone City
        new(Pt) { FixedBall = Ball.Poke, Species = 175, Level = 01, Location = 000, EggLocation = 2011 }, // Togepi Egg from Cynthia

        new(Pt) { Species = 425, Level = 15, Location = 47 }, // Drifloon @ Valley Windworks
        new(Pt) { Species = 479, Level = 20, Location = 70, GroundTile = Building }, // Rotom @ Old Château

        // Stationary Legendary
        new(Pt) { Species = 377, Level = 30, Location = 125, GroundTile = Cave }, // Regirock @ Rock Peak Ruins
        new(Pt) { Species = 378, Level = 30, Location = 124, GroundTile = Cave }, // Regice @ Iceberg Ruins
        new(Pt) { Species = 379, Level = 30, Location = 123, GroundTile = Cave }, // Registeel @ Iron Ruins
        new(Pt) { Species = 483, Level = 70, Location = 051, GroundTile = Rock }, // Dialga @ Spear Pillar
        new(Pt) { Species = 484, Level = 70, Location = 051, GroundTile = Rock }, // Palkia @ Spear Pillar
        new(Pt) { Species = 485, Level = 50, Location = 084, GroundTile = Cave }, // Heatran @ Stark Mountain
        new(Pt) { Species = 486, Level = 01, Location = 064, GroundTile = Cave }, // Regigigas @ Snowpoint Temple
        new(Pt) { Species = 487, Level = 47, Location = 062, GroundTile = Cave, Form = 0 }, // Giratina @ Turnback Cave
        new(Pt) { Species = 487, Level = 47, Location = 117, GroundTile = Distortion, Form = 1, HeldItem = 112 }, // Giratina @ Distortion World

        new(Pt) { Species = 491, Level = 50, Location = 079, GroundTile = Grass }, // Darkrai @ Newmoon Island
        new(Pt) { Species = 492, Form = 0, Level = 30, Location = 063, FatefulEncounter = true }, // Shaymin @ Flower Paradise
        new(Pt)   { Roaming = true, Location = 16, Species = 144, Level = 60, GroundTile = Grass | Water }, // Articuno
        new(Pt)   { Roaming = true, Location = 16, Species = 145, Level = 60, GroundTile = Grass | Water }, // Zapdos
        new(Pt)   { Roaming = true, Location = 16, Species = 146, Level = 60, GroundTile = Grass | Water }, // Moltres
    ];

    public static readonly EncounterStatic4[] StaticD =
    [
        new(D ) { Species = 483, Level = 47, Location = 051, GroundTile = Rock }, // Dialga @ Spear Pillar
    ];

    public static readonly EncounterStatic4[] StaticP =
    [
        new( P) { Species = 484, Level = 47, Location = 051, GroundTile = Rock }, // Palkia @ Spear Pillar
    ];

    #endregion
    #region Trade Tables

    internal static readonly EncounterTrade4RanchGift[] RanchGifts =
    [
        new(323975838, 025, 18) { Moves = new(447,085,148,104), TID16 = 1000, SID16 = 19840, OTGender = 1, Location = 0068, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 20 }, // Pikachu
        new(323977664, 037, 16) { Moves = new(412,109,053,219), TID16 = 1000, SID16 = 21150, OTGender = 1, Location = 3000, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 30 }, // Vulpix
        new(323975579, 077, 13) { Moves = new(036,033,039,052), TID16 = 1000, SID16 = 01123, OTGender = 1, Location = 3000, Gender = 0, Ability = OnlySecond, CurrentLevel = 16 }, // Ponyta
        new(323975564, 108, 34) { Moves = new(076,111,014,205), TID16 = 1000, SID16 = 03050, OTGender = 1, Location = 0077, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 40 }, // Lickitung
        new(323977579, 114, 01) { Moves = new(437,438,079,246), TID16 = 1000, SID16 = 49497, OTGender = 1, Location = 3000, Gender = 1, Ability = OnlySecond }, // Tangela
        new(323977675, 133, 16) { Moves = new(363,270,098,247), TID16 = 1000, SID16 = 47710, OTGender = 1, Location = 0068, Gender = 0, Ability = OnlySecond, CurrentLevel = 30 }, // Eevee
        new(323977588, 142, 20) { Moves = new(363,089,444,332), TID16 = 1000, SID16 = 43066, OTGender = 1, Location = 0094, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 50 }, // Aerodactyl
        new(232975554, 193, 22) { Moves = new(318,095,246,138), TID16 = 1000, SID16 = 42301, OTGender = 1, Location = 0052, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 45, FixedBall = Ball.Safari }, // Yanma
        new(323975570, 241, 16) { Moves = new(208,215,360,359), TID16 = 1000, SID16 = 02707, OTGender = 1, Location = 3000, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 48 }, // Miltank
        new(323975563, 285, 22) { Moves = new(402,147,206,078), TID16 = 1000, SID16 = 02788, OTGender = 1, Location = 3000, Gender = 0, Ability = OnlySecond, CurrentLevel = 45, FixedBall = Ball.Safari }, // Shroomish
        new(323975559, 320, 30) { Moves = new(156,323,133,058), TID16 = 1000, SID16 = 27046, OTGender = 1, Location = 0038, Gender = 0, Ability = OnlySecond, CurrentLevel = 45 }, // Wailmer
        new(323977657, 360, 01) { Moves = new(204,150,227,000), TID16 = 1000, SID16 = 01788, OTGender = 1, Location = 0004, Gender = 0, Ability = OnlySecond, EggLocation = 2000 }, // Wynaut
        new(323975563, 397, 02) { Moves = new(355,017,283,018), TID16 = 1000, SID16 = 59298, OTGender = 1, Location = 0016, Gender = 0, Ability = OnlySecond, CurrentLevel = 23 }, // Staravia
        new(323970584, 415, 05) { Moves = new(230,016,000,000), TID16 = 1000, SID16 = 54140, OTGender = 1, Location = 0020, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 20 }, // Combee
        new(323977539, 417, 09) { Moves = new(447,045,351,098), TID16 = 1000, SID16 = 18830, OTGender = 1, Location = 0020, Gender = 1, Ability = OnlySecond, CurrentLevel = 10 }, // Pachirisu
        new(323974107, 422, 20) { Moves = new(363,352,426,104), TID16 = 1000, SID16 = 39272, OTGender = 1, Location = 0028, Gender = 0, Ability = OnlySecond, CurrentLevel = 25, Form = 1 }, // Shellos
        new(323977566, 427, 10) { Moves = new(204,193,409,098), TID16 = 1000, SID16 = 31045, OTGender = 1, Location = 3000, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 16 }, // Buneary
        new(323975579, 453, 22) { Moves = new(310,207,426,389), TID16 = 1000, SID16 = 41342, OTGender = 1, Location = 0052, Gender = 0, Ability = OnlySecond, CurrentLevel = 31, FixedBall = Ball.Safari }, // Croagunk
        new(323977566, 456, 15) { Moves = new(213,352,219,392), TID16 = 1000, SID16 = 48348, OTGender = 1, Location = 0020, Gender = 1, Ability = OnlyFirst,  CurrentLevel = 35 }, // Finneon
        new(323975582, 459, 32) { Moves = new(452,420,275,059), TID16 = 1000, SID16 = 23360, OTGender = 1, Location = 0031, Gender = 0, Ability = OnlyFirst,  CurrentLevel = 41 }, // Snover
        new(151, 50) { Moves = new(235,216,095,100), TID16 = 1000, SID16 = 59228, OTGender = 1, FixedBall = Ball.Cherish, Gender = 2, Ability = Any12 }, // Mew
        new(489, 01) { Moves = new(447,240,156,057), TID16 = 1000, SID16 = 09248, OTGender = 1, FixedBall = Ball.Cherish, Gender = 2, Ability = Any12, CurrentLevel = 50, EggLocation = 3000 }, // Phione
    ];

    internal static readonly EncounterTrade4PID[] TradeGift_DPPtIngame =
    [
        new(TradeNames, 00, DPPt, 0x0000008E, 063, 01) { Ability = OnlyFirst,  TID16 = 25643, SID16 = 00000, OTGender = 1, Gender = 0, IVs = new(15,15,15,20,25,25) }, // Machop -> Abra
        new(TradeNames, 01, DPPt, 0x00000867, 441, 01) { Ability = OnlySecond, TID16 = 44142, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(15,20,15,25,25,15), Contest = 20 }, // Buizel -> Chatot
        new(TradeNames, 02, DPPt, 0x00000088, 093, 35) { Ability = OnlyFirst,  TID16 = 19248, SID16 = 00000, OTGender = 1, Gender = 0, IVs = new(20,25,15,25,15,15) }, // Medicham (35 from Route 217) -> Haunter
        new(TradeNames, 03, DPPt, 0x0000045C, 129, 01) { Ability = OnlyFirst,  TID16 = 53277, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(15,25,15,20,25,15) }, // Finneon -> Magikarp
    ];

    #endregion
}
