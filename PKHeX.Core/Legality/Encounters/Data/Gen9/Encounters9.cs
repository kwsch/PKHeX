using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Encounters
/// </summary>
internal static class Encounters9
{
    internal static readonly EncounterArea9[] Slots = EncounterArea9.GetAreas(Get("wild_paldea", "sv"u8), SV);

    internal static readonly EncounterStatic9[] Encounter_SV =
    [
        // Starters
        new(SV) { FixedBall = Ball.Poke, Species = 0906, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Sprigatito
        new(SV) { FixedBall = Ball.Poke, Species = 0909, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Fuecoco
        new(SV) { FixedBall = Ball.Poke, Species = 0912, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Quaxly

        new(SV) { FixedBall = Ball.Poke, Species = 0734, Level = 02, Location = 064 }, // Yungoos level 2, no Marks in Inlet Grotto. Only Poké Ball available in early game.

        // Treasures of Ruin
        new(SV) { Species = 1001, Shiny = Never, Level = 60, Location = 006, Size = 128, FlawlessIVCount = 3 }, // Wo-Chien
        new(SV) { Species = 1002, Shiny = Never, Level = 60, Location = 022, Size = 128, FlawlessIVCount = 3 }, // Chien-Pao
        new(SV) { Species = 1003, Shiny = Never, Level = 60, Location = 109, Size = 128, FlawlessIVCount = 3 }, // Ting-Lu
        new(SV) { Species = 1004, Shiny = Never, Level = 60, Location = 048, Size = 128, FlawlessIVCount = 3 }, // Chi-Yu

        // Former Titans
        new(SV) { Species = 0950, Shiny = Never, Level = 16, Location = 020, Ability = OnlyFirst,  Gender = 1, Nature = Nature.Gentle, Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(011,249,335,317), IsTitan = true }, // Klawf
        new(SV) { Species = 0962, Shiny = Never, Level = 20, Location = 022, Ability = OnlyHidden, Gender = 1, Nature = Nature.Jolly,  Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(088,017,365,259), IsTitan = true }, // Bombirdier
        new(SV) { Species = 0968, Shiny = Never, Level = 29, Location = 032, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quirky, Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(231,029,035,201), IsTitan = true }, // Orthworm
        new(SV) { Species = 0978, Shiny = Never, Level = 57, Location = 040, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quiet,  Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(330,196,269,406), IsTitan = true }, // Tatsugiri

        // Decoy Tatsugiri
        new(SV) { Species = 0978, Shiny = Never, Level = 45, Location = 040, Size = 128           }, // Tatsugiri
        new(SV) { Species = 0978, Shiny = Never, Level = 45, Location = 040, Size = 128, Form = 1 }, // Tatsugiri-1
        new(SV) { Species = 0978, Shiny = Never, Level = 45, Location = 040, Size = 128, Form = 2 }, // Tatsugiri-2

        #region Chest Form Gimmighoul
        // Gimmighoul has 13 separate static encounter entries, all differing by Level. These can be placed in numerous parts of the map, so for now, manually define them all here.
        new(SV) { Species = 0999, Shiny = Never, Level = 05, Location = 006 }, // Gimmighoul - South Province (Area One)
        new(SV) { Species = 0999, Shiny = Never, Level = 10, Location = 012 }, // Gimmighoul - South Province (Area Two)
        new(SV) { Species = 0999, Shiny = Never, Level = 10, Location = 020 }, // Gimmighoul - South Province (Area Three)
        new(SV) { Species = 0999, Shiny = Never, Level = 15, Location = 014 }, // Gimmighoul - South Province (Area Four)
        new(SV) { Species = 0999, Shiny = Never, Level = 15, Location = 018 }, // Gimmighoul - South Province (Area Five)
        new(SV) { Species = 0999, Shiny = Never, Level = 15, Location = 022 }, // Gimmighoul - West Province (Area One)
        new(SV) { Species = 0999, Shiny = Never, Level = 15, Location = 034 }, // Gimmighoul - East Province (Area One)
        new(SV) { Species = 0999, Shiny = Never, Level = 20, Location = 024 }, // Gimmighoul - Asado Desert
        new(SV) { Species = 0999, Shiny = Never, Level = 25, Location = 032 }, // Gimmighoul - East Province (Area Three)
        new(SV) { Species = 0999, Shiny = Never, Level = 25, Location = 058 }, // Gimmighoul - West Paldean Sea
        new(SV) { Species = 0999, Shiny = Never, Level = 35, Location = 038 }, // Gimmighoul - Glaseado Mountain
        new(SV) { Species = 0999, Shiny = Never, Level = 40, Location = 016 }, // Gimmighoul - South Province (Area Six)
        new(SV) { Species = 0999, Shiny = Never, Level = 50, Location = 026 }, // Gimmighoul - West Province (Area Two)
        new(SV) { Species = 0999, Shiny = Never, Level = 50, Location = 040 }, // Gimmighoul - Casseroya Lake
        new(SV) { Species = 0999, Shiny = Never, Level = 50, Location = 046 }, // Gimmighoul - North Province (Area One)
        new(SV) { Species = 0999, Shiny = Never, Level = 50, Location = 048 }, // Gimmighoul - North Province (Area Two)
        new(SV) { Species = 0999, Shiny = Never, Level = 50, Location = 109 }, // Gimmighoul - Socarrat Trail
        #endregion

        #region Sudachi 1
        new(SV) { Species = 0168, Shiny = Never, Level = 65, Location = 166, Ability = OnlyFirst, Gender = 0, Nature = Nature.Hardy, TeraType = GemType.Bug, Size = 128, IVs = new(20,20,20,20,20,20) }, // Ariados
        new(SV) { Species = 0901, Shiny = Never, Level = 70, Location = 166, Ability = OnlyFirst, Gender = 0, Nature = Nature.Hardy, TeraType = GemType.Normal, Size = 128, FlawlessIVCount = 3, Moves = new(901,414,163,347), Form = 1 }, // Ursaluna-1
        new(SV) { Species = 1014, Shiny = Never, Level = 70, Location = 162, Ability = OnlyFirst, Gender = 0, TeraType = GemType.Poison, Size = 128, FlawlessIVCount = 3 }, // Okidogi
        new(SV) { Species = 1015, Shiny = Never, Level = 70, Location = 156, Ability = OnlyFirst, Gender = 0, TeraType = GemType.Poison, Size = 128, FlawlessIVCount = 3 }, // Munkidori
        new(SV) { Species = 1016, Shiny = Never, Level = 70, Location = 144, Ability = OnlyFirst, Gender = 0, TeraType = GemType.Poison, Size = 128, FlawlessIVCount = 3 }, // Fezandipiti
        new(SV) { Species = 1017, Shiny = Never, Level = 70, Location = 146, Ability = OnlyFirst, Gender = 1, Nature = Nature.Lonely, TeraType = GemType.Grass, Size = 128, IVs = new(31,31,20,31,20,20), Moves = new(904,067,021,580) }, // Ogerpon
        new(SV) { Species = 1017, Shiny = Never, Level = 20, Location = 146, Ability = OnlyFirst, Gender = 1, Nature = Nature.Lonely, TeraType = GemType.Grass, Size = 128, IVs = new(31,31,20,31,20,20), Moves = new(904,021,074,022) }, // Ogerpon

        new(SV) { FixedBall = Ball.Poke, Species = 0446, Shiny = Always, Level = 01, Location = 142, Ability = OnlyHidden, Gender = 0, Nature = Nature.Impish, Size = 255 }, // Munchlax
        new(SV) { FixedBall = Ball.Poke, Species = 0058, Shiny = Never,  Level = 15, Location = 134, Ability = OnlyHidden, Gender = 0, Nature = Nature.Jolly, TeraType = GemType.Rock, Size = 254, IVs = new(31,31,20,31,20,20), Form = 1 }, // Growlithe-1
        new(SV) { FixedBall = Ball.Poke, Species = 0387, Shiny = Never,  Level = 01, Location = 000, Ability = OnlyFirst, Size = 128, EggLocation = 60005 }, // Turtwig
        new(SV) { FixedBall = Ball.Poke, Species = 0390, Shiny = Never,  Level = 01, Location = 000, Ability = OnlyFirst, Size = 128, EggLocation = 60005 }, // Chimchar
        new(SV) { FixedBall = Ball.Poke, Species = 0393, Shiny = Never,  Level = 01, Location = 000, Ability = OnlyFirst, Size = 128, EggLocation = 60005 }, // Piplup
        #endregion

        #region Sudachi 2
        // Legendary Pokémon
        new(SV) { Species = 0144, Shiny = Never, Level = 70, Location = 038, Ability = OnlyFirst,             TeraType = GemType.Ice,      Size = 128                               }, // Articuno
        new(SV) { Species = 0145, Shiny = Never, Level = 70, Location = 006, Ability = OnlyFirst,             TeraType = GemType.Electric, Size = 128                               }, // Zapdos
        new(SV) { Species = 0146, Shiny = Never, Level = 70, Location = 024, Ability = OnlyFirst,             TeraType = GemType.Fire,     Size = 128                               }, // Moltres
        new(SV) { Species = 0243, Shiny = Never, Level = 70, Location = 022, Ability = OnlyFirst,             TeraType = GemType.Electric, Size = 128                               }, // Raikou
        new(SV) { Species = 0244, Shiny = Never, Level = 70, Location = 032, Ability = OnlyFirst,             TeraType = GemType.Fire,     Size = 128                               }, // Entei
        new(SV) { Species = 0245, Shiny = Never, Level = 70, Location = 040, Ability = OnlyFirst,             TeraType = GemType.Water,    Size = 128                               }, // Suicune
        new(SV) { Species = 0249, Shiny = Never, Level = 70, Location = 062, Ability = OnlyFirst,             TeraType = GemType.Water,    Size = 128                               }, // Lugia
        new(SV) { Species = 0250, Shiny = Never, Level = 70, Location = 014, Ability = OnlyFirst,             TeraType = GemType.Fire,     Size = 128                               }, // Ho-Oh
        new(SV) { Species = 0380, Shiny = Never, Level = 70, Location = 014, Ability = OnlyFirst, Gender = 1, TeraType = GemType.Psychic,  Size = 128                               }, // Latias
        new(SV) { Species = 0381, Shiny = Never, Level = 70, Location = 048, Ability = OnlyFirst, Gender = 0, TeraType = GemType.Psychic,  Size = 128, Moves = new(295,406,428,225) }, // Latios
        new(SV) { Species = 0382, Shiny = Never, Level = 70, Location = 040, Ability = OnlyFirst,             TeraType = GemType.Water,    Size = 128                               }, // Kyogre
        new(SV) { Species = 0383, Shiny = Never, Level = 70, Location = 067, Ability = OnlyFirst,             TeraType = GemType.Fire,     Size = 128                               }, // Groudon
        new(SV) { Species = 0384, Shiny = Never, Level = 70, Location = 050, Ability = OnlyFirst,             TeraType = GemType.Flying,   Size = 128                               }, // Rayquaza
        new(SV) { Species = 0638, Shiny = Never, Level = 70, Location = 048, Ability = OnlyFirst,             TeraType = GemType.Fighting, Size = 128                               }, // Cobalion
        new(SV) { Species = 0639, Shiny = Never, Level = 70, Location = 022, Ability = OnlyFirst,             TeraType = GemType.Fighting, Size = 128                               }, // Terrakion
        new(SV) { Species = 0640, Shiny = Never, Level = 70, Location = 030, Ability = OnlyFirst,             TeraType = GemType.Fighting, Size = 128                               }, // Virizion
        new(SV) { Species = 0643, Shiny = Never, Level = 70, Location = 034, Ability = OnlyFirst,             TeraType = GemType.Dragon,   Size = 128                               }, // Reshiram
        new(SV) { Species = 0644, Shiny = Never, Level = 70, Location = 018, Ability = OnlyFirst,             TeraType = GemType.Dragon,   Size = 128                               }, // Zekrom
        new(SV) { Species = 0646, Shiny = Never, Level = 70, Location = 069, Ability = OnlyFirst,             TeraType = GemType.Dragon,   Size = 128                               }, // Kyurem
        new(SV) { Species = 0791, Shiny = Never, Level = 70, Location = 010, Ability = OnlyFirst,             TeraType = GemType.Psychic,  Size = 128                               }, // Solgaleo
        new(SV) { Species = 0792, Shiny = Never, Level = 70, Location = 040, Ability = OnlyFirst,             TeraType = GemType.Psychic,  Size = 128                               }, // Lunala
        new(SV) { Species = 0800, Shiny = Never, Level = 70, Location = 109, Ability = OnlyFirst,             TeraType = GemType.Psychic,  Size = 128                               }, // Necrozma
        new(SV) { Species = 0891, Shiny = Never, Level = 30, Location = 048, Ability = OnlyFirst,             TeraType = GemType.Fighting, Size = 128                               }, // Kubfu
        new(SV) { Species = 0896, Shiny = Never, Level = 70, Location = 038, Ability = OnlyFirst,             TeraType = GemType.Ice,      Size = 128                               }, // Glastrier
        new(SV) { Species = 0897, Shiny = Never, Level = 70, Location = 038, Ability = OnlyFirst,             TeraType = GemType.Ghost,    Size = 128                               }, // Spectrier

        // Terapagos (Captured in Stellar Form)
        new(SV) { Species = 1024, Shiny = Never, Level = 85, Location = 198, Ability = OnlyFirst, Gender = 0, Nature = Nature.Hardy, TeraType = GemType.Stellar, Size = 128, IVs = new(31,15,31,31,31,31), Moves = new(906,428,414,352) }, // Terapagos

        // Mythical Pokémon
        new(SV) { Species = 0648, Shiny = Never, Level = 70, Location = 176, Ability = OnlyFirst, TeraType = GemType.Fighting, Size = 128, FlawlessIVCount = 3, Moves = new(547,304,047,094) }, // Meloetta
        new(SV) { Species = 1025, Shiny = Never, Level = 88, Location = 138, Ability = OnlyFirst, Nature = Nature.Timid, TeraType = GemType.Poison, Size = 128, FlawlessIVCount = 3, Moves = new(417,092,919,247) }, // Pecharunt
        #endregion
    ];

    internal static readonly EncounterStatic9[] StaticSL =
    [
        // Box Legendary (Ride Form)
        new(SL) { FixedBall = Ball.Poke, Species = 1007, Shiny = Never, Level = 68, Location = 070, Ability = OnlyFirst, Nature = Nature.Quirky, TeraType = GemType.Dragon, Size = 128, IVs = new(31,31,28,31,31,28), Moves = new(053,878,203,851) }, // Koraidon

        // Galarian Meowth from Salvatore (Specific Met Location depending on game, inside Academy's Staff Quarters)
        new(SL) { FixedBall = Ball.Poke, Species = 0052, Shiny = Never, Level = 05, Location = 130, FlawlessIVCount = 3, Form = 2 }, // Meowth-2

        // Box Legendary (Battle Form)
        new(SL) { Species = 1007, Shiny = Never, Level = 72, Location = 124, Ability = OnlyFirst, Nature = Nature.Adamant, TeraType = GemType.Fighting, Size = 128, IVs = new(25,31,25,31,31,25), Moves = new(416,339,878,053) }, // Koraidon

        // Former Quaking Earth Titan
        new(SL) { Species = 0984, Shiny = Never, Level = 45, Location = 024, Ability = OnlyFirst, Nature = Nature.Naughty, Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(229,280,282,707), IsTitan = true }, // Great Tusk

        // Paradox Pokémon
        new(SL) { Species = 1020, Shiny = Never, Level = 75, Location = 124, Ability = OnlyFirst, TeraType = GemType.Fire,     Size = 128, IVs = new(20,20,20,20,20,20) }, // Gouging Fire
        new(SL) { Species = 1021, Shiny = Never, Level = 75, Location = 124, Ability = OnlyFirst, TeraType = GemType.Electric, Size = 128, IVs = new(20,20,20,20,20,20) }, // Raging Bolt
    ];

    internal static readonly EncounterStatic9[] StaticVL =
    [
        // Box Legendary (Ride Form)
        new(VL) { FixedBall = Ball.Poke, Species = 1008, Shiny = Never, Level = 68, Location = 070, Ability = OnlyFirst, Nature = Nature.Quirky, TeraType = GemType.Dragon, Size = 128, IVs = new(31,31,28,31,31,28), Moves = new(408,879,203,851) }, // Miraidon

        // Galarian Meowth from Salvatore (Specific Met Location depending on game, inside Academy's Staff Quarters)
        new(VL) { FixedBall = Ball.Poke, Species = 0052, Shiny = Never, Level = 05, Location = 131, FlawlessIVCount = 3, Form = 2 }, // Meowth-2

        // Box Legendary (Battle Form)
        new(VL) { Species = 1008, Shiny = Never, Level = 72, Location = 124, Ability = OnlyFirst, Nature = Nature.Modest, TeraType = GemType.Electric, Size = 128, IVs = new(25,31,25,31,31,25), Moves = new(063,268,879,408) }, // Miraidon

        // Former Quaking Earth Titan
        new(VL) { Species = 0990, Shiny = Never, Level = 45, Location = 024, Ability = OnlyFirst, Nature = Nature.Naughty, Size = 255, IVs = new(30,30,30,30,30,30), Moves = new(229,442,282,707), IsTitan = true }, // Iron Treads

        // Paradox Pokémon
        new(VL) { Species = 1022, Shiny = Never, Level = 75, Location = 124, Ability = OnlyFirst, TeraType = GemType.Rock,  Size = 128, IVs = new(20,20,20,20,20,20) }, // Iron Boulder
        new(VL) { Species = 1023, Shiny = Never, Level = 75, Location = 124, Ability = OnlyFirst, TeraType = GemType.Steel, Size = 128, IVs = new(20,20,20,20,20,20) }, // Iron Crown

    ];

    private const string tradeSV = "tradesv";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings10(tradeSV, "zh2");

    internal static readonly EncounterTrade9[] TradeGift_SV =
    [
        new(TradeNames, 00, SV, 0194, 18) { FixedBall = Ball.Poke,  ID32 = 033081, Ability = OnlySecond, OTGender = 1, Gender = 0, Nature = Nature.Relaxed, TeraType = GemType.Water,    Weight = SizeType9.M,     Scale = SizeType9.M,     IVs = new(27,18,25,13,16,31) }, // Wooper
        new(TradeNames, 01, SV, 0093, 25) { FixedBall = Ball.Poke,  ID32 = 016519, Ability = OnlyFirst,  OTGender = 1, Gender = 1, Nature = Nature.Lonely,  TeraType = GemType.Ghost,    Weight = SizeType9.S,     Scale = SizeType9.S,     IVs = new(14,20,25,31,28,16), EvolveOnTrade = true }, // Haunter
        new(TradeNames, 02, SV, 0240, 12) { FixedBall = Ball.Quick, ID32 = 418071, Ability = OnlyHidden, OTGender = 0, Gender = 0, Nature = Nature.Docile,  TeraType = GemType.Fire,     Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,12,25,15,31,31), Moves = new(264,052,108,499) }, // Magby
        new(TradeNames, 03, SV, 0982, 35) { FixedBall = Ball.Poke,  ID32 = 766634, Ability = OnlyFirst,  OTGender = 0, Gender = 0, Nature = Nature.Relaxed, TeraType = GemType.Flying,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(15,24,31,26,31,11), Moves = new(263,403,529,887) }, // Dudunsparce
        new(TradeNames, 04, SV, 0133, 01) { FixedBall = Ball.Poke,  ID32 = 376983, Ability = OnlyHidden, OTGender = 1, Gender = 0, Nature = Nature.Modest,  TeraType = GemType.Dark,     Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(08,26,31,31,31,31), Moves = new(033,039,270,174) }, // Eevee
        new(TradeNames, 05, SV, 0194, 08) { FixedBall = Ball.Poke,  ID32 = 591912, Ability = OnlyHidden, OTGender = 1, Gender = 0, Nature = Nature.Naughty, TeraType = GemType.Poison,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(27,31,31,06,16,30), Moves = new(341,068,491,342), Form = 1 }, // Wooper-1
        new(TradeNames, 06, SV, 0922, 24) { FixedBall = Ball.Poke,  ID32 = 209896, Ability = OnlyFirst,  OTGender = 0, Gender = 0, Nature = Nature.Hasty,   TeraType = GemType.Fighting, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(15,22,31,29,14,03) }, // Pawmo
        new(TradeNames, 07, SV, 0974, 37) { FixedBall = Ball.Poke,  ID32 = 209896, Ability = OnlyHidden, OTGender = 0, Gender = 0, Nature = Nature.Bashful, TeraType = GemType.Ice,      Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,23,18,31,15,09) }, // Cetoddle
        new(TradeNames, 08, SV, 0976, 20) { FixedBall = Ball.Poke,  ID32 = 373015, Ability = OnlyHidden, OTGender = 0, Gender = 1, Nature = Nature.Sassy,   TeraType = GemType.Water,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,28,14,26,14,18) }, // Veluza
        new(TradeNames, 09, SV, 0997, 45) { FixedBall = Ball.Poke,  ID32 = 316242, Ability = OnlyFirst,  OTGender = 0, Gender = 0, Nature = Nature.Relaxed, TeraType = GemType.Dragon,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(24,22,21,30,30,31), Moves = new(407,423,242,116) }, // Arctibax
        new(TradeNames, 10, SV, 0415, 15) { FixedBall = Ball.Poke,  ID32 = 993663, Ability = OnlyHidden, OTGender = 1, Gender = 1, Nature = Nature.Calm,    TeraType = GemType.Bug,      Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(24,22,31,16,28,00) }, // Combee
        new(TradeNames, 11, SV, 0884, 36) { FixedBall = Ball.Poke,  ID32 = 217978, Ability = OnlyHidden, OTGender = 0, Gender = 0, Nature = Nature.Quiet,   TeraType = GemType.Electric, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(20,30,25,00,31,13) }, // Duraludon
        new(TradeNames, 12, SV, 0307, 28) { FixedBall = Ball.Poke,  ID32 = 137719, Ability = OnlySecond, OTGender = 1, Gender = 0, Nature = Nature.Quirky,  TeraType = GemType.Fire,     Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(24,30,12,29,00,06), Moves = new(223,007,008,009) }, // Meditite
        new(TradeNames, 13, SV, 0192, 40) { FixedBall = Ball.Poke,  ID32 = 584457, Ability = OnlySecond, OTGender = 0, Gender = 0, Nature = Nature.Brave,   TeraType = GemType.Grass,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(12,19,25,20,16,24), Moves = new(885,241,283,235) }, // Sunflora
        new(TradeNames, 14, SV, 0747, 20) { FixedBall = Ball.Poke,  ID32 = 158604, Ability = OnlyHidden, OTGender = 1, Gender = 1, Nature = Nature.Timid,   TeraType = GemType.Poison,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(30,21,10,16,22,29) }, // Mareanie
        new(TradeNames, 15, SV, 0081, 22) { FixedBall = Ball.Poke,  ID32 = 568659, Ability = OnlyFirst,  OTGender = 1, Gender = 2, Nature = Nature.Rash,    TeraType = GemType.Electric, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(04,29,08,30,31,02), Moves = new(527,048,086,209) }, // Magnemite
        new(TradeNames, 16, SV, 0128, 50) { FixedBall = Ball.Poke,  ID32 = 933665, Ability = OnlyFirst,  OTGender = 1, Gender = 0, Nature = Nature.Jolly,   TeraType = GemType.Fighting, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,31,26,31,31,31), Moves = new(873,528,038,370), Form = 1 }, // Tauros-1
        new(TradeNames, 17, SV, 0227, 40) { FixedBall = Ball.Heavy, ID32 = 745642, Ability = OnlySecond, OTGender = 1, Gender = 1, Nature = Nature.Lonely,  TeraType = GemType.Steel,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(24,15,22,19,14,09) }, // Skarmory
        new(TradeNames, 18, SV, 0969, 05) { FixedBall = Ball.Poke,  ID32 = 661291, Ability = OnlyHidden, OTGender = 1, Gender = 1, Nature = Nature.Modest,  TeraType = GemType.Psychic,  Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,00,22,31,30,31), Moves = new(851,092,088,479) }, // Glimmet
        new(TradeNames, 19, SV, 0819, 07) { FixedBall = Ball.Poke,  ID32 = 105971, Ability = OnlyHidden, OTGender = 0, Gender = 1, Nature = Nature.Gentle,  TeraType = GemType.Fighting, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(14,12,22,10,23,22) }, // Skwovet
        new(TradeNames, 20, SV, 0958, 25) { FixedBall = Ball.Poke,  ID32 = 949475, Ability = OnlyFirst,  OTGender = 1, Gender = 1, Nature = Nature.Careful, TeraType = GemType.Fairy,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(12,29,29,26,28,22) }, // Tinkatuff
        new(TradeNames, 21, SV, 0971, 10) { FixedBall = Ball.Poke,  ID32 = 275703, Ability = OnlyHidden, OTGender = 1, Gender = 0, Nature = Nature.Jolly,   TeraType = GemType.Ghost,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(15,26,14,16,04,31) }, // Greavard
        new(TradeNames, 22, SV, 0999, 30) { FixedBall = Ball.Poke,  ID32 = 361010, Ability = OnlyFirst,  OTGender = 1, Gender = 2, Nature = Nature.Sassy,   TeraType = GemType.Steel,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(05,12,01,22,16,18) }, // Gimmighoul
        new(TradeNames, 23, SV, 0955, 32) { FixedBall = Ball.Poke,  ID32 = 149671, Ability = OnlyHidden, OTGender = 1, Gender = 1, Nature = Nature.Impish,  TeraType = GemType.Psychic,  Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(13,20,12,12,21,21) }, // Flittle
        new(TradeNames, 24, SV, 0857, 33) { FixedBall = Ball.Poke,  ID32 = 654886, Ability = OnlyFirst,  OTGender = 0, Gender = 1, Nature = Nature.Serious, TeraType = GemType.Fairy,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(22,18,18,07,31,31) }, // Hattrem
        new(TradeNames, 25, SV, 0052, 21) { FixedBall = Ball.Poke,  ID32 = 314512, Ability = OnlySecond, OTGender = 0, Gender = 1, Nature = Nature.Relaxed, TeraType = GemType.Dark,     Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(09,31,24,11,12,12), Form = 1 }, // Meowth-1
        new(TradeNames, 26, SV, 0522, 20) { FixedBall = Ball.Poke,  ID32 = 390518, Ability = OnlyFirst,  OTGender = 0, Gender = 0, Nature = Nature.Hardy,   TeraType = GemType.Electric, Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(31,25,23,31,23,31), Shiny = Always }, // Blitzle
        new(TradeNames, 27, SV, 0840, 15) { FixedBall = Ball.Poke,  ID32 = 184745, Ability = OnlyHidden, OTGender = 0, Gender = 0, Nature = Nature.Modest,  TeraType = GemType.Dragon,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(30,31,22,18,31,26), Moves = new(110,310,278,389) }, // Applin
        new(TradeNames, 28, SV, 0209, 18) { FixedBall = Ball.Nest,  ID32 = 816963, Ability = OnlyFirst,  OTGender = 1, Gender = 0, Nature = Nature.Adamant, TeraType = GemType.Fairy,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(18,30,24,31,24,25) }, // Snubbull
        new(TradeNames, 29, SV, 0744, 12) { FixedBall = Ball.Poke,  ID32 = 980975, Ability = OnlyHidden, OTGender = 1, Gender = 1, Nature = Nature.Impish,  TeraType = GemType.Ground,   Weight = SizeType9.M,     Scale = SizeType9.M,     IVs = new(20,30,02,19,15,17), Moves = new(707,088,387,283) }, // Rockruff
        new(TradeNames, 30, SV, 1012, 30) { FixedBall = Ball.Poke,  ID32 = 704310, Ability = OnlyFirst,  OTGender = 1, Gender = 2, Nature = Nature.Sassy,   TeraType = GemType.Water,    Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(12,19,29,22,31,03), Moves = new(506,072,078,503) }, // Poltchageist
        new(TradeNames, 31, SV, 0316, 17) { FixedBall = Ball.Poke,  ID32 = 134745, Ability = OnlyHidden, OTGender = 0, Gender = 1, Nature = Nature.Gentle,  TeraType = GemType.Poison,   Weight = SizeType9.VALUE, Scale = SizeType9.VALUE, IVs = new(12,28,25,19,17,31), Moves = new(491,227,499,281) }, // Gulpin
        new(TradeNames, 32, SV, 0872, 10) { FixedBall = Ball.Poke,  ID32 = 050724, Ability = Any12,      OTGender = 0, Gender = 1, Nature = Nature.Bashful, TeraType = GemType.Ice,      Weight = SizeType9.L,     Scale = SizeType9.L,     IVs = new(31,18,13,20,28,26) }, // Snom
    ];

    internal static readonly EncounterTera9[] TeraBase = EncounterTera9.GetArray(Get("gem_paldea"), TeraRaidMapParent.Paldea);
    internal static readonly EncounterTera9[] TeraDLC1 = EncounterTera9.GetArray(Get("gem_kitakami"), TeraRaidMapParent.Kitakami);
    internal static readonly EncounterTera9[] TeraDLC2 = EncounterTera9.GetArray(Get("gem_blueberry"), TeraRaidMapParent.Blueberry);
    internal static readonly EncounterDist9[] Dist = EncounterDist9.GetArray(Get("dist_paldea"));
    internal static readonly EncounterMight9[] Might = EncounterMight9.GetArray(Get("might_paldea"));
    internal static readonly EncounterFixed9[] Fixed = EncounterFixed9.GetArray(Get("fixed_paldea"));
    internal static readonly EncounterOutbreak9[] Outbreak = EncounterOutbreak9.GetArray(Get("outbreak_paldea"));
}
