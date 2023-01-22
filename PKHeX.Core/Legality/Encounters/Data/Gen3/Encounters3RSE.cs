using System;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Encounters
/// </summary>
internal static class Encounters3RSE
{
    private static readonly EncounterArea3[] SlotsSwarmRSE = GetSwarm("rse_swarm", "rs", RSE);
    internal static readonly EncounterArea3[] SlotsR = ArrayUtil.ConcatAll(GetRegular("r", "ru", R), SlotsSwarmRSE);
    internal static readonly EncounterArea3[] SlotsS = ArrayUtil.ConcatAll(GetRegular("s", "sa", S), SlotsSwarmRSE);
    internal static readonly EncounterArea3[] SlotsE = ArrayUtil.ConcatAll(GetRegular("e", "em", E), SlotsSwarmRSE);

    private static EncounterArea3[] GetRegular(string resource, string ident, GameVersion game) => EncounterArea3.GetAreas(Get(resource, ident), game);
    private static EncounterArea3[] GetSwarm(string resource, string ident, GameVersion game) => EncounterArea3.GetAreasSwarm(Get(resource, ident), game);

    static Encounters3RSE() => MarkEncounterTradeStrings(TradeGift_RSE, TradeRSE);

    private static readonly EncounterStatic3[] Encounter_RSE_Roam =
    {
        new(380, 40, S) { Roaming = true, Location = 016 }, // Latias
        new(380, 40, E) { Roaming = true, Location = 016 }, // Latias
        new(381, 40, R) { Roaming = true, Location = 016 }, // Latios
        new(381, 40, E) { Roaming = true, Location = 016 }, // Latios
    };

    private static readonly EncounterStatic3[] Encounter_RSE_Regular =
    {
        // Starters
        new(152, 05, E  ) { Gift = true, Location = 000 }, // Chikorita @ Littleroot Town
        new(155, 05, E  ) { Gift = true, Location = 000 }, // Cyndaquil
        new(158, 05, E  ) { Gift = true, Location = 000 }, // Totodile
        new(252, 05, RSE) { Gift = true, Location = 016 }, // Treecko @ Route 101
        new(255, 05, RSE) { Gift = true, Location = 016 }, // Torchic
        new(258, 05, RSE) { Gift = true, Location = 016 }, // Mudkip

        // Fossil @ Rustboro City
        new(345, 20, RSE) { Gift = true, Location = 010 }, // Lileep
        new(347, 20, RSE) { Gift = true, Location = 010 }, // Anorith

        // Gift
        new(351, 25, RSE) { Gift = true, Location = 034    }, // Castform @ Weather Institute
        new(374, 05, RSE) { Gift = true, Location = 013    }, // Beldum @ Mossdeep City
        new(360, 05, RSE) { Gift = true, EggLocation = 253 }, // Wynaut Egg

        // Stationary
        new(352, 30, RSE) { Location = 034 }, // Kecleon @ Route 119
        new(352, 30, RSE) { Location = 035 }, // Kecleon @ Route 120
        new(101, 30, RS ) { Location = 066 }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
        new(101, 30, E  ) { Location = 197 }, // Electrode @ Aqua Hideout
        new(185, 40, E  ) { Location = 058 }, // Sudowoodo @ Battle Frontier

        // Stationary Lengendary
        new(377, 40, RSE) { Location = 082 }, // Regirock @ Desert Ruins
        new(378, 40, RSE) { Location = 081 }, // Regice @ Island Cave
        new(379, 40, RSE) { Location = 083 }, // Registeel @ Ancient Tomb
        new(380, 50, R  ) { Location = 073 }, // Latias @ Southern Island
        new(380, 50,   E) { Location = 073, FatefulEncounter = true }, // Latias @ Southern Island
        new(381, 50,  S ) { Location = 073 }, // Latios @ Southern Island
        new(381, 50,   E) { Location = 073, FatefulEncounter = true }, // Latios @ Southern Island
        new(382, 45,  S ) { Location = 072 }, // Kyogre @ Cave of Origin
        new(382, 70,   E) { Location = 203 }, // Kyogre @ Marine Cave
        new(383, 45, R  ) { Location = 072 }, // Groudon @ Cave of Origin
        new(383, 70,   E) { Location = 205 }, // Groudon @ Terra Cave
        new(384, 70, RSE) { Location = 085 }, // Rayquaza @ Sky Pillar

        // Event
        new(151, 30, E) { Location = 201, FatefulEncounter = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
        new(249, 70, E) { Location = 211, FatefulEncounter = true }, // Lugia @ Navel Rock
        new(250, 70, E) { Location = 211, FatefulEncounter = true }, // Ho-Oh @ Navel Rock
        new(386, 30, E) { Location = 200, FatefulEncounter = true, Form = 3 }, // Deoxys @ Birth Island
    };

    private static readonly EncounterStatic3[] Encounter_RSE = ArrayUtil.ConcatAll(Encounter_RSE_Roam, Encounter_RSE_Regular);

    private static ReadOnlySpan<byte> TradeContest_Cool   => new byte[] { 30, 05, 05, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Beauty => new byte[] { 05, 30, 05, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Cute   => new byte[] { 05, 05, 30, 05, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Clever => new byte[] { 05, 05, 05, 30, 05, 10 };
    private static ReadOnlySpan<byte> TradeContest_Tough  => new byte[] { 05, 05, 05, 05, 30, 10 };

    internal static readonly EncounterTrade3[] TradeGift_RSE =
    {
        new(RS, 0x00009C40, 296, 05) { Ability = OnlySecond, TID16 = 49562, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,5,4,4,4,4), Contest = TradeContest_Tough }, // Slakoth (Level 5 Breeding) -> Makuhita
        new(RS, 0x498A2E17, 300, 03) { Ability = OnlyFirst,  TID16 = 02259, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(5,4,4,5,4,4), Contest = TradeContest_Cute }, // Pikachu (Level 3 Viridian Forest) -> Skitty
        new(RS, 0x4C970B7F, 222, 21) { Ability = OnlySecond, TID16 = 50183, SID16 = 00000, OTGender = 1, Gender = 1, IVs = new(4,4,5,4,4,5), Contest = TradeContest_Beauty }, // Bellossom (Level 21 Oddish -> Gloom -> Bellossom) -> Corsola
        new(E , 0x00000084, 273, 04) { Ability = OnlySecond, TID16 = 38726, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,4,5,4,4,4), Contest = TradeContest_Cool }, // Ralts (Level 4 Route 102) -> Seedot
        new(E , 0x0000006F, 311, 05) { Ability = OnlyFirst,  TID16 = 08460, SID16 = 00001, OTGender = 0, Gender = 1, IVs = new(4,4,4,5,5,4), Contest = TradeContest_Cute }, // Volbeat (Level 5 Breeding) -> Plusle
        new(E , 0x0000007F, 116, 05) { Ability = OnlyFirst,  TID16 = 46285, SID16 = 00000, OTGender = 0, Gender = 0, IVs = new(5,4,4,4,5,4), Contest = TradeContest_Tough }, // Bagon (Level 5 Breeding) -> Horsea*
        new(E , 0x0000008B, 052, 03) { Ability = OnlyFirst,  TID16 = 25945, SID16 = 00001, OTGender = 1, Gender = 0, IVs = new(4,5,4,5,4,4), Contest = TradeContest_Clever }, // Skitty (Level 3 Trade)-> Meowth*
        //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
    };

    private const string tradeRSE = "traderse";
    private static readonly string[][] TradeRSE = Util.GetLanguageStrings7(tradeRSE);

    internal static readonly EncounterStatic3[] StaticR = GetEncounters(Encounter_RSE, R);
    internal static readonly EncounterStatic3[] StaticS = GetEncounters(Encounter_RSE, S);
    internal static readonly EncounterStatic3[] StaticE = GetEncounters(Encounter_RSE, E);
}
