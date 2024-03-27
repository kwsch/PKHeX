using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Encounters
/// </summary>
internal static class Encounters3RSE
{
    internal static readonly EncounterArea3[] SlotsSwarmRSE = GetSwarm("rse_swarm", "rs"u8, RSE);
    internal static readonly EncounterArea3[] SlotsR = GetRegular("r", "ru"u8, R);
    internal static readonly EncounterArea3[] SlotsS = GetRegular("s", "sa"u8, S);
    internal static readonly EncounterArea3[] SlotsE = GetRegular("e", "em"u8, E);

    private static EncounterArea3[] GetRegular([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident, [ConstantExpected] GameVersion game)
        => EncounterArea3.GetAreas(Get(resource, ident), game);
    private static EncounterArea3[] GetSwarm([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident, [ConstantExpected] GameVersion game)
        => EncounterArea3.GetAreasSwarm(Get(resource, ident), game);

    private static readonly string[] TrainersPikachu = [string.Empty, "コロシアム", "COLOS", "COLOSSEUM", "ARENA", "COLOSSEUM", string.Empty, "CLAUDIO"];
    private static readonly string[] TrainersCelebi = [string.Empty, "アゲト", "AGATE", "SAMARAGD", "SOFO", "EMERITAE", string.Empty, "ÁGATA"];
    private static readonly string[] TrainersMattle = [string.Empty, "バトルやま", "MATTLE", "MT BATAILL", "MONTE LOTT", "DUELLBERG", string.Empty, "ERNESTO"]; // truncated on ck3->pk3 transfer

    internal static readonly EncounterGift3Colo[] ColoGiftsR =
    [
        // In-Game Bonus Disk (Japan only)
        new(025, 10, TrainersPikachu, R) { Location = 255, TID16 = 31121, OriginalTrainerGender = 0 }, // Colosseum Pikachu bonus gift
        new(251, 10, TrainersCelebi, R)  { Location = 255, TID16 = 31121, OriginalTrainerGender = 1 }, // Ageto Celebi bonus gift
    ];

    internal static readonly EncounterGift3Colo[] ColoGiftsS =
    [
        // In-Game without Bonus Disk
        new(250, 70, TrainersMattle, S)  { Location = 255, TID16 = 10048, OriginalTrainerGender = 0, Moves = new(105, 126, 241, 129) }, // Ho-oh @ Mt. Battle
    ];

    private const string tradeRSE = "traderse";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings7(tradeRSE);

    public static readonly EncounterStatic3[] StaticRSE =
    [
        // Starters
        new(252, 05, RSE) { FixedBall = Ball.Poke, Location = 016 }, // Treecko @ Route 101
        new(255, 05, RSE) { FixedBall = Ball.Poke, Location = 016 }, // Torchic
        new(258, 05, RSE) { FixedBall = Ball.Poke, Location = 016 }, // Mudkip

        // Fossil @ Rustboro City
        new(345, 20, RSE) { FixedBall = Ball.Poke, Location = 010 }, // Lileep
        new(347, 20, RSE) { FixedBall = Ball.Poke, Location = 010 }, // Anorith

        // Gift
        new(351, 25, RSE) { FixedBall = Ball.Poke, Location = 034    }, // Castform @ Weather Institute
        new(374, 05, RSE) { FixedBall = Ball.Poke, Location = 013    }, // Beldum @ Mossdeep City
        new(360, 05, RSE) { FixedBall = Ball.Poke, Location = 253, IsEgg = true }, // Wynaut Egg

        // Stationary
        new(352, 30, RSE) { Location = 034 }, // Kecleon @ Route 119
        new(352, 30, RSE) { Location = 035 }, // Kecleon @ Route 120

        // Stationary Lengendary
        new(377, 40, RSE) { Location = 082 }, // Regirock @ Desert Ruins
        new(378, 40, RSE) { Location = 081 }, // Regice @ Island Cave
        new(379, 40, RSE) { Location = 083 }, // Registeel @ Ancient Tomb
        new(384, 70, RSE) { Location = 085 }, // Rayquaza @ Sky Pillar
    ];

    public static readonly EncounterStatic3[] StaticR =
    [
        new(381, 40, R) { Roaming = true, Location = 016 }, // Latios
        new(380, 50, R) { Location = 073 }, // Latias @ Southern Island
        new(383, 45, R) { Location = 072 }, // Groudon @ Cave of Origin

        new(101, 30, R) { Location = 066 }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
    ];

    public static readonly EncounterStatic3[] StaticS =
    [
        new(380, 40, S) { Roaming = true, Location = 016 }, // Latias
        new(381, 50, S) { Location = 073 }, // Latios @ Southern Island
        new(382, 45, S) { Location = 072 }, // Kyogre @ Cave of Origin

        new(101, 30, S) { Location = 066 }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
    ];

    public static readonly EncounterStatic3[] StaticE =
    [
        new(380, 40, E) { Roaming = true, Location = 016 }, // Latias
        new(381, 40, E) { Roaming = true, Location = 016 }, // Latios
        new(382, 70, E) { Location = 203 }, // Kyogre @ Marine Cave
        new(383, 70, E) { Location = 205 }, // Groudon @ Terra Cave

        new(101, 30, E) { Location = 197 }, // Electrode @ Aqua Hideout

        // Starters
        new(152, 05, E) { FixedBall = Ball.Poke, Location = 000 }, // Chikorita @ Littleroot Town
        new(155, 05, E) { FixedBall = Ball.Poke, Location = 000 }, // Cyndaquil
        new(158, 05, E) { FixedBall = Ball.Poke, Location = 000 }, // Totodile
        new(185, 40, E) { Location = 058 }, // Sudowoodo @ Battle Frontier
        new(380, 50, E) { Location = 073, FatefulEncounter = true }, // Latias @ Southern Island
        new(381, 50, E) { Location = 073, FatefulEncounter = true }, // Latios @ Southern Island

        // Event
        new(151, 30, E) { Location = 201, FatefulEncounter = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
        new(249, 70, E) { Location = 211, FatefulEncounter = true }, // Lugia @ Navel Rock
        new(250, 70, E) { Location = 211, FatefulEncounter = true }, // Ho-Oh @ Navel Rock
        new(386, 30, E) { Location = 200, FatefulEncounter = true, Form = 3 }, // Deoxys @ Birth Island
    ];

    private static ReadOnlySpan<byte> TradeContest_Cool   => [ 30, 05, 05, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Beauty => [ 05, 30, 05, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Cute   => [ 05, 05, 30, 05, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Clever => [ 05, 05, 05, 30, 05, 10 ];
    private static ReadOnlySpan<byte> TradeContest_Tough  => [ 05, 05, 05, 05, 30, 10 ];

    internal static readonly EncounterTrade3[] TradeGift_RS =
    [
        new(TradeNames, 00, RS, 0x00009C40, 296, 05) { Ability = OnlySecond, TID16 = 49562, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,5,4,4,4,4), Contest = TradeContest_Tough }, // Slakoth (Level 5 Breeding) -> Makuhita
        new(TradeNames, 01, RS, 0x498A2E17, 300, 03) { Ability = OnlyFirst,  TID16 = 02259, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(5,4,4,5,4,4), Contest = TradeContest_Cute }, // Pikachu (Level 3 Viridian Forest) -> Skitty
        new(TradeNames, 02, RS, 0x4C970B7F, 222, 21) { Ability = OnlySecond, TID16 = 50183, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(4,4,5,4,4,5), Contest = TradeContest_Beauty }, // Bellossom (Level 21 Oddish -> Gloom -> Bellossom) -> Corsola
    ];

    internal static readonly EncounterTrade3[] TradeGift_E =
    [
        new(TradeNames, 03, E , 0x00000084, 273, 04) { Ability = OnlySecond, TID16 = 38726, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,4,5,4,4,4), Contest = TradeContest_Cool }, // Ralts (Level 4 Route 102) -> Seedot
        new(TradeNames, 04, E , 0x0000006F, 311, 05) { Ability = OnlyFirst,  TID16 = 08460, SID16 = 00001, OTGender = 0, Gender = 1, IVs = new(4,4,4,5,5,4), Contest = TradeContest_Cute }, // Volbeat (Level 5 Breeding) -> Plusle
        new(TradeNames, 05, E , 0x0000007F, 116, 05) { Ability = OnlyFirst,  TID16 = 46285, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,4,4,4,5,4), Contest = TradeContest_Tough }, // Bagon (Level 5 Breeding) -> Horsea*
        new(TradeNames, 06, E , 0x0000008B, 052, 03) { Ability = OnlyFirst,  TID16 = 25945, SID16 = 00001, OTGender = 1, Gender = 0, IVs = new(4,5,4,5,4,4), Contest = TradeContest_Clever }, // Skitty (Level 3 Trade)-> Meowth*
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    ];
}
