using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.GroundTileAllowed;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Encounters
/// </summary>
internal static class Encounters4HGSS
{
    internal static readonly EncounterArea4[] SlotsHG = EncounterArea4.GetAreas(Get("hg", "hg"u8), HG);
    internal static readonly EncounterArea4[] SlotsSS = EncounterArea4.GetAreas(Get("ss", "ss"u8), SS);

    internal static readonly EncounterStatic4Pokewalker[] Encounter_PokeWalker = EncounterStatic4Pokewalker.GetAll(Util.GetBinaryResource("encounter_walker4.pkl"));

    private const string tradeHGSS = "tradehgss";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings8(tradeHGSS);

    #region Static Encounter/Gift Tables

    internal static readonly EncounterStatic4[] Encounter_HGSS =
    [
        // Starters
        new(HGSS) { FixedBall = Ball.Poke, Species = 001, Level = 05, Location = 138, GroundTile = Max_Pt }, // Bulbasaur @ Pallet Town
        new(HGSS) { FixedBall = Ball.Poke, Species = 004, Level = 05, Location = 138, GroundTile = Max_Pt }, // Charmander
        new(HGSS) { FixedBall = Ball.Poke, Species = 007, Level = 05, Location = 138, GroundTile = Max_Pt }, // Squirtle
        new(HGSS) { FixedBall = Ball.Poke, Species = 152, Level = 05, Location = 126, GroundTile = Max_DP }, // Chikorita @ New Bark Town
        new(HGSS) { FixedBall = Ball.Poke, Species = 155, Level = 05, Location = 126, GroundTile = Max_DP }, // Cyndaquil
        new(HGSS) { FixedBall = Ball.Poke, Species = 158, Level = 05, Location = 126, GroundTile = Max_DP }, // Totodile
        new(HGSS) { FixedBall = Ball.Poke, Species = 252, Level = 05, Location = 148, GroundTile = Max_Pt }, // Treecko @ Saffron City
        new(HGSS) { FixedBall = Ball.Poke, Species = 255, Level = 05, Location = 148, GroundTile = Max_Pt }, // Torchic
        new(HGSS) { FixedBall = Ball.Poke, Species = 258, Level = 05, Location = 148, GroundTile = Max_Pt }, // Mudkip

        // Fossils @ Pewter City
        new(HGSS) { FixedBall = Ball.Poke, Species = 138, Level = 20, Location = 140, GroundTile = Max_Pt }, // Omanyte
        new(HGSS) { FixedBall = Ball.Poke, Species = 140, Level = 20, Location = 140, GroundTile = Max_Pt }, // Kabuto
        new(HGSS) { FixedBall = Ball.Poke, Species = 142, Level = 20, Location = 140, GroundTile = Max_Pt }, // Aerodactyl
        new(HGSS) { FixedBall = Ball.Poke, Species = 345, Level = 20, Location = 140, GroundTile = Max_Pt }, // Lileep
        new(HGSS) { FixedBall = Ball.Poke, Species = 347, Level = 20, Location = 140, GroundTile = Max_Pt }, // Anorith
        new(HGSS) { FixedBall = Ball.Poke, Species = 408, Level = 20, Location = 140, GroundTile = Max_Pt }, // Cranidos
        new(HGSS) { FixedBall = Ball.Poke, Species = 410, Level = 20, Location = 140, GroundTile = Max_Pt }, // Shieldon

        // Gift
        new(HGSS) { FixedBall = Ball.Poke, Species = 072, Level = 15, Location = 130, GroundTile = Max_Pt }, // Tentacool @ Cianwood City
        new(HGSS) { FixedBall = Ball.Poke, Species = 133, Level = 05, Location = 131, GroundTile = Max_Pt }, // Eevee @ Goldenrod City
        new(HGSS) { FixedBall = Ball.Poke, Species = 147, Level = 15, Location = 222, GroundTile = Max_Pt, Moves = new(245) }, // Dratini @ Dragon's Den (ExtremeSpeed)
        new(HGSS) { FixedBall = Ball.Poke, Species = 236, Level = 10, Location = 216, GroundTile = Max_Pt }, // Tyrogue @ Mt. Mortar
        new(HGSS) { FixedBall = Ball.Poke, Species = 175, Level = 01, Location = 000, EggLocation = 2013, Moves = new((int)Move.Growl, (int)Move.Charm, (int)Move.Extrasensory) }, // Togepi Egg from Mr. Pokémon (Extrasensory as Egg move)
        new(HGSS) { FixedBall = Ball.Poke, Species = 179, Level = 01, Location = 000, EggLocation = 2014 }, // Mareep Egg from Primo
        new(HGSS) { FixedBall = Ball.Poke, Species = 194, Level = 01, Location = 000, EggLocation = 2014 }, // Wooper Egg from Primo
        new(HGSS) { FixedBall = Ball.Poke, Species = 218, Level = 01, Location = 000, EggLocation = 2014 }, // Slugma Egg from Primo

        // Celadon City Game Corner
        new(HGSS) { FixedBall = Ball.Poke, Species = 122, Level = 15, Location = 144, GroundTile = Max_Pt }, // Mr. Mime
        new(HGSS) { FixedBall = Ball.Poke, Species = 133, Level = 15, Location = 144, GroundTile = Max_Pt }, // Eevee
        new(HGSS) { FixedBall = Ball.Poke, Species = 137, Level = 15, Location = 144, GroundTile = Max_Pt }, // Porygon

        // Goldenrod City Game Corner
        new(HGSS) { FixedBall = Ball.Poke, Species = 063, Level = 15, Location = 131, GroundTile = Max_Pt }, // Abra
        new(HGSS) { FixedBall = Ball.Poke, Species = 147, Level = 15, Location = 131, GroundTile = Max_Pt }, // Dratini

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
        new(HGSS) { Species = 384, Level = 50, Location = 232, GroundTile = Cave }, // Rayquaza @ Embedded Tower
        new(HGSS) { Species = 483, Level = 01, Location = 231, FixedBall = Ball.Poke, GroundTile = Max_Pt }, // Dialga @ Sinjoh Ruins
        new(HGSS) { Species = 484, Level = 01, Location = 231, FixedBall = Ball.Poke, GroundTile = Max_Pt }, // Palkia @ Sinjoh Ruins
        new(HGSS) { Species = 487, Level = 01, Location = 231, FixedBall = Ball.Poke, GroundTile = Max_Pt, Form = 1, HeldItem = 112 }, // Giratina @ Sinjoh Ruins

        // Johto Roamers
        new(HGSS) { Roaming = true, Species = 243, Location = 177, Level = 40, GroundTile = Grass | Water }, // Raikou
        new(HGSS) { Roaming = true, Species = 244, Location = 177, Level = 40, GroundTile = Grass | Water }, // Entei
    ];

    internal static readonly EncounterStatic4[] StaticHG =
    [
        new(HG  ) { FixedBall = Ball.Poke, Species = 023, Level = 15, Location = 131, GroundTile = Max_Pt }, // Ekans
        new(HG  ) { Species = 249, Level = 70, Location = 218, GroundTile = Water }, // Lugia @ Whirl Islands
        new(HG  ) { Species = 250, Level = 45, Location = 205, GroundTile = Building }, // Ho-Oh @ Bell Tower
        new(HG  ) { Species = 381, Level = 40, Location = 140, GroundTile = Building }, // Latios @ Pewter City
        new(HG  ) { Species = 382, Level = 50, Location = 232, GroundTile = Cave }, // Kyogre @ Embedded Tower
        new(HG  ) { Roaming = true, Species = 380, Location = 149, Level = 35, GroundTile = Grass | Water }, // Latias
    ];

    internal static readonly EncounterStatic4[] StaticSS =
    [
        new(  SS) { FixedBall = Ball.Poke, Species = 027, Level = 15, Location = 131, GroundTile = Max_Pt }, // Sandshrew
        new(  SS) { Species = 249, Level = 45, Location = 218, GroundTile = Water }, // Lugia @ Whirl Islands
        new(  SS) { Species = 250, Level = 70, Location = 205, GroundTile = Building }, // Ho-Oh @ Bell Tower
        new(  SS) { Species = 380, Level = 40, Location = 140, GroundTile = Building }, // Latias @ Pewter City
        new(  SS) { Species = 383, Level = 50, Location = 232, GroundTile = Cave }, // Groudon @ Embedded Tower
        new(  SS) { Roaming = true, Species = 381, Location = 149, Level = 35, GroundTile = Grass | Water }, // Latios
    ];

    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade4PID[] TradeGift_HGSS =
    [
        new(TradeNames, 00, HGSS, 0x000025EF, 095, 01) { Ability = OnlySecond, TID16 = 48926, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(25,20,25,15,15,15) }, // Bellsprout -> Onix
        new(TradeNames, 01, HGSS, 0x00002310, 066, 01) { Ability = OnlyFirst,  TID16 = 37460, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(15,25,20,20,15,15) }, // Drowzee -> Machop
        new(TradeNames, 02, HGSS, 0x000001DB, 100, 01) { Ability = OnlySecond, TID16 = 29189, SID16 = 00000, OTGender = 0, Gender = 2, IVs = new(15,20,15,25,25,15) }, // Krabby -> Voltorb
        new(TradeNames, 03, HGSS, 0x0001FC0A, 085, 15) { Ability = OnlyFirst,  TID16 = 00283, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(20,20,20,15,15,15) }, // Dragonair (15 from DPPt) -> Dodrio
        new(TradeNames, 04, HGSS, 0x0000D136, 082, 19) { Ability = OnlyFirst,  TID16 = 50082, SID16 = 00000, OTGender = 0, Gender = 2, IVs = new(15,20,15,20,20,20) }, // Dugtrio (19 from Diglett's Cave) -> Magneton
        new(TradeNames, 05, HGSS, 0x000034E4, 178, 16) { Ability = OnlyFirst,  TID16 = 15616, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(15,20,15,20,20,20) }, // Haunter (16 from Old Château) -> Xatu
        new(TradeNames, 06, HGSS, 0x00485876, 025, 02) { Ability = OnlyFirst,  TID16 = 33038, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(20,25,18,31,25,13) }, // Pikachu
        new(TradeNames, 07, HGSS, 0x0012B6D4, 374, 31) { Ability = OnlyFirst,  TID16 = 23478, SID16 = 00000, OTGender = 0, Gender = 2, IVs = new(28,29,24,23,24,25) }, // Forretress -> Beldum
        new(TradeNames, 08, HGSS, 0x0012971C, 111, 01) { Ability = OnlyFirst,  TID16 = 06845, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(22,31,13,00,22,09), Moves = new(422) }, // Bonsly -> Rhyhorn w/ Thunder Fang
        new(TradeNames, 09, HGSS, 0x00101596, 208, 01) { Ability = OnlyFirst,  TID16 = 26491, SID16 = 00000, OTGender = 1, Gender = 0, IVs = new(08,30,28,06,18,20) }, // Any -> Steelix

        //Gift
        new(TradeNames, 10, HGSS, 0x00006B5E, 021, 20) { Ability = OnlyFirst,  TID16 = 01001, SID16 = 00000, OTGender = 0, Gender = 1, IVs = new(15,20,15,20,20,20), MetLocation = 183, Moves = new(043,031,228,332) },// Webster's Spearow
        new(TradeNames, 11, HGSS, 0x000214D7, 213, 20) { Ability = OnlySecond, TID16 = 04336, SID16 = 00001, OTGender = 0, Gender = 0, IVs = new(15,20,15,20,20,20), MetLocation = 130, Moves = new(132,117,227,219) },// Kirk's Shuckle
    ];
    #endregion
}
