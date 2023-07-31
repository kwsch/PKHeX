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
    internal static readonly EncounterArea9[] Slots = EncounterArea9.GetAreas(Get("wild_paldea", "sv"), SV);

    internal static readonly EncounterStatic9[] Encounter_SV =
    {
        // Starters
        new(SV) { FixedBall = Ball.Poke, Species = 906, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Sprigatito
        new(SV) { FixedBall = Ball.Poke, Species = 909, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Fuecoco
        new(SV) { FixedBall = Ball.Poke, Species = 912, Shiny = Never, Level = 05, Location = 080, Ability = OnlyFirst, Size = 128 }, // Quaxly

        // Scripted
        new (SV) { FixedBall = Ball.Poke, Species = 734, Level = 02, Location = 064 }, // Yungoos level 2, no Marks in Inlet Grotto. Only Poké Ball available in early game.

        // Treasures of Ruin
        new(SV) { Species = 1001, Shiny = Never, Level = 60, Location = 006, Size = 128, FlawlessIVCount = 3 }, // Wo-Chien
        new(SV) { Species = 1002, Shiny = Never, Level = 60, Location = 022, Size = 128, FlawlessIVCount = 3 }, // Chien-Pao
        new(SV) { Species = 1003, Shiny = Never, Level = 60, Location = 109, Size = 128, FlawlessIVCount = 3 }, // Ting-Lu
        new(SV) { Species = 1004, Shiny = Never, Level = 60, Location = 048, Size = 128, FlawlessIVCount = 3 }, // Chi-Yu

        // Former Titans
        new(SV) { Species = 950, Shiny = Never, Level = 16, Location = 020, Size = 255, Ability = OnlyFirst,  Gender = 1, Nature = Nature.Gentle,  IVs = new(30,30,30,30,30,30), Moves = new(011,249,335,317), IsTitan = true }, // Klawf
        new(SV) { Species = 962, Shiny = Never, Level = 20, Location = 022, Size = 255, Ability = OnlyHidden, Gender = 1, Nature = Nature.Jolly,   IVs = new(30,30,30,30,30,30), Moves = new(088,017,365,259), IsTitan = true }, // Bombirdier
        new(SV) { Species = 968, Shiny = Never, Level = 29, Location = 032, Size = 255, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quirky,  IVs = new(30,30,30,30,30,30), Moves = new(231,029,035,201), IsTitan = true }, // Orthworm
        new(SL) { Species = 984, Shiny = Never, Level = 45, Location = 024, Size = 255, Ability = OnlyFirst,              Nature = Nature.Naughty, IVs = new(30,30,30,30,30,30), Moves = new(229,280,282,707), IsTitan = true }, // Great Tusk
        new(VL) { Species = 990, Shiny = Never, Level = 45, Location = 024, Size = 255, Ability = OnlyFirst,              Nature = Nature.Naughty, IVs = new(30,30,30,30,30,30), Moves = new(229,442,282,707), IsTitan = true }, // Iron Treads
        new(SV) { Species = 978, Shiny = Never, Level = 57, Location = 040, Size = 255, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quiet,   IVs = new(30,30,30,30,30,30), Moves = new(330,196,269,406), IsTitan = true }, // Tatsugiri

        // Dummy Tatsugiri
        new(SV) { Species = 978, Shiny = Never, Level = 45, Location = 040, Size = 128           }, // Tatsugiri
        new(SV) { Species = 978, Shiny = Never, Level = 45, Location = 040, Size = 128, Form = 1 }, // Tatsugiri-1
        new(SV) { Species = 978, Shiny = Never, Level = 45, Location = 040, Size = 128, Form = 2 }, // Tatsugiri-2

        #region Chest Form Gimmighoul
        // Gimmighoul has 13 separate static encounter entries, all differing by Level. These can be placed in numerous parts of the map, so for now, manually define them all here.
        new(SV) { Species = 999, Shiny = Never, Level = 05, Location = 006 }, // Gimmighoul - South Province (Area One)
        new(SV) { Species = 999, Shiny = Never, Level = 10, Location = 012 }, // Gimmighoul - South Province (Area Two)
        new(SV) { Species = 999, Shiny = Never, Level = 10, Location = 020 }, // Gimmighoul - South Province (Area Three)
        new(SV) { Species = 999, Shiny = Never, Level = 15, Location = 014 }, // Gimmighoul - South Province (Area Four)
        new(SV) { Species = 999, Shiny = Never, Level = 15, Location = 018 }, // Gimmighoul - South Province (Area Five)
        new(SV) { Species = 999, Shiny = Never, Level = 15, Location = 022 }, // Gimmighoul - West Province (Area One)
        new(SV) { Species = 999, Shiny = Never, Level = 15, Location = 034 }, // Gimmighoul - East Province (Area One)
        new(SV) { Species = 999, Shiny = Never, Level = 20, Location = 024 }, // Gimmighoul - Asado Desert
        new(SV) { Species = 999, Shiny = Never, Level = 25, Location = 032 }, // Gimmighoul - East Province (Area Three)
        new(SV) { Species = 999, Shiny = Never, Level = 25, Location = 058 }, // Gimmighoul - West Paldean Sea
        new(SV) { Species = 999, Shiny = Never, Level = 35, Location = 038 }, // Gimmighoul - Glaseado Mountain
        new(SV) { Species = 999, Shiny = Never, Level = 40, Location = 016 }, // Gimmighoul - South Province (Area Six)
        new(SV) { Species = 999, Shiny = Never, Level = 50, Location = 026 }, // Gimmighoul - West Province (Area Two)
        new(SV) { Species = 999, Shiny = Never, Level = 50, Location = 040 }, // Gimmighoul - Casseroya Lake
        new(SV) { Species = 999, Shiny = Never, Level = 50, Location = 046 }, // Gimmighoul - North Province (Area One)
        new(SV) { Species = 999, Shiny = Never, Level = 50, Location = 048 }, // Gimmighoul - North Province (Area Two)
        new(SV) { Species = 999, Shiny = Never, Level = 50, Location = 109 }, // Gimmighoul - Socarrat Trail
        #endregion
    };

    internal static readonly EncounterStatic9[] StaticSL =
    {
        // Galarian Meowth from Salvatore (Specific Met Location depending on game, inside Academy's Staff Quarters)
        new(SL) { FixedBall = Ball.Poke, Species = 052, Shiny = Never, Level = 05, Location = 130, Form = 2, FlawlessIVCount = 3 }, // Meowth-2
        new(VL) { FixedBall = Ball.Poke, Species = 052, Shiny = Never, Level = 05, Location = 131, Form = 2, FlawlessIVCount = 3 }, // Meowth-2

        // Box Legendary (Battle Form)
        new(SL) { Species = 1007, Shiny = Never, Level = 72, Location = 124, Size = 128, Ability = OnlyFirst, Nature = Nature.Adamant, TeraType = GemType.Fighting, IVs = new(25,31,25,31,31,25), Moves = new(416,339,878,053) }, // Koraidon
        new(VL) { Species = 1008, Shiny = Never, Level = 72, Location = 124, Size = 128, Ability = OnlyFirst, Nature = Nature.Modest,  TeraType = GemType.Electric, IVs = new(25,31,25,31,31,25), Moves = new(063,268,879,408) }, // Miraidon

        // Box Legendary (Ride Form)
        new(SL) { FixedBall = Ball.Poke, Species = 1007, Shiny = Never, Level = 68, Location = 070, Ability = OnlyFirst, Size = 128, Nature = Nature.Quirky, TeraType = GemType.Dragon, IVs = new(31,31,28,31,31,28), Moves = new(053,878,203,851) }, // Koraidon
        new(VL) { FixedBall = Ball.Poke, Species = 1008, Shiny = Never, Level = 68, Location = 070, Ability = OnlyFirst, Size = 128, Nature = Nature.Quirky, TeraType = GemType.Dragon, IVs = new(31,31,28,31,31,28), Moves = new(408,879,203,851) }, // Miraidon

        // Former Titans
        new(SV) { Species = 950, Shiny = Never, Level = 16, Location = 020, Size = 255, Ability = OnlyFirst,  Gender = 1, Nature = Nature.Gentle,  IVs = new(30,30,30,30,30,30), Moves = new(011,249,335,317), IsTitan = true }, // Klawf
        new(SV) { Species = 962, Shiny = Never, Level = 20, Location = 022, Size = 255, Ability = OnlyHidden, Gender = 1, Nature = Nature.Jolly,   IVs = new(30,30,30,30,30,30), Moves = new(088,017,365,259), IsTitan = true }, // Bombirdier
        new(SV) { Species = 968, Shiny = Never, Level = 29, Location = 032, Size = 255, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quirky,  IVs = new(30,30,30,30,30,30), Moves = new(231,029,035,201), IsTitan = true }, // Orthworm
        new(SL) { Species = 984, Shiny = Never, Level = 45, Location = 024, Size = 255, Ability = OnlyFirst,              Nature = Nature.Naughty, IVs = new(30,30,30,30,30,30), Moves = new(229,280,282,707), IsTitan = true }, // Great Tusk
        new(VL) { Species = 990, Shiny = Never, Level = 45, Location = 024, Size = 255, Ability = OnlyFirst,              Nature = Nature.Naughty, IVs = new(30,30,30,30,30,30), Moves = new(229,442,282,707), IsTitan = true }, // Iron Treads
        new(SV) { Species = 978, Shiny = Never, Level = 57, Location = 040, Size = 255, Ability = OnlyFirst,  Gender = 0, Nature = Nature.Quiet,   IVs = new(30,30,30,30,30,30), Moves = new(330,196,269,406), IsTitan = true }, // Tatsugiri
    };

    internal static readonly EncounterStatic9[] StaticVL =
    {
        // Galarian Meowth from Salvatore (Specific Met Location depending on game, inside Academy's Staff Quarters)
        new(VL) { FixedBall = Ball.Poke, Species = 052, Shiny = Never, Level = 05, Location = 131, Form = 2, FlawlessIVCount = 3 }, // Meowth-2

        // Box Legendary (Battle Form)
        new(VL) { Species = 1008, Shiny = Never, Level = 72, Location = 124, Size = 128, Ability = OnlyFirst, Nature = Nature.Modest,  TeraType = GemType.Electric, IVs = new(25,31,25,31,31,25), Moves = new(063,268,879,408) }, // Miraidon

        // Box Legendary (Ride Form)
        new(VL) { FixedBall = Ball.Poke, Species = 1008, Shiny = Never, Level = 68, Location = 070, Ability = OnlyFirst, Size = 128, Nature = Nature.Quirky, TeraType = GemType.Dragon, IVs = new(31,31,28,31,31,28), Moves = new(408,879,203,851) }, // Miraidon

        // Former Titans
        new(VL) { Species = 990, Shiny = Never, Level = 45, Location = 024, Size = 255, Ability = OnlyFirst,              Nature = Nature.Naughty, IVs = new(30,30,30,30,30,30), Moves = new(229,442,282,707), IsTitan = true }, // Iron Treads
    };

    private const string tradeSV = "tradesv";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings10(tradeSV, "zh2");

    internal static readonly EncounterTrade9[] TradeGift_SV =
    {
        new(TradeNames, 00, SV, 872,10) { Ability = Any12,      ID32 = 050724, IVs = new(31,18,13,20,28,26), OTGender = 0, Gender = 1, Nature = Nature.Bashful }, // Snom
        new(TradeNames, 01, SV, 194,18) { Ability = OnlySecond, ID32 = 033081, IVs = new(27,18,25,13,16,31), OTGender = 1, Gender = 0, Nature = Nature.Relaxed }, // Wooper
        new(TradeNames, 02, SV, 093,25) { Ability = OnlyFirst,  ID32 = 016519, IVs = new(14,20,25,31,28,16), OTGender = 1, Gender = 1, Nature = Nature.Lonely, EvolveOnTrade = true }, // Haunter
    };

    internal static readonly EncounterTera9[] Tera = EncounterTera9.GetArray(Get("gem_paldea"));
    internal static readonly EncounterDist9[] Dist = EncounterDist9.GetArray(Get("dist_paldea"));
    internal static readonly EncounterMight9[] Might = EncounterMight9.GetArray(Get("might_paldea"));
    internal static readonly EncounterFixed9[] Fixed = EncounterFixed9.GetArray(Get("fixed_paldea"));
}
