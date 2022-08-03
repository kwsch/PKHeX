using System.Collections.Generic;
using static PKHeX.Core.Species;
using static PKHeX.Core.Move;

namespace PKHeX.Core;

public static partial class Legal
{
    /// <summary>
    /// Generation 3 &amp; 4 Battle Frontier Species banlist. When referencing this in context to generation 4, be sure to disallow <see cref="Pichu"/> with Form 1 (Spiky).
    /// </summary>
    public static readonly HashSet<int> BattleFrontierBanlist = new()
    {
        (int)Mewtwo, (int)Mew,
        (int)Lugia, (int)HoOh, (int)Celebi,
        (int)Kyogre, (int)Groudon, (int)Rayquaza, (int)Jirachi, (int)Deoxys,
        (int)Dialga, (int)Palkia, (int)Giratina, (int)Phione, (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,
        (int)Victini, (int)Reshiram, (int)Zekrom, (int)Kyurem, (int)Keldeo, (int)Meloetta, (int)Genesect,
        (int)Xerneas, (int)Yveltal, (int)Zygarde, (int)Diancie, (int)Hoopa, (int)Volcanion,
        (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala, (int)Necrozma, (int)Magearna, (int)Marshadow, (int)Zeraora,
        (int)Meltan, (int)Melmetal,
    };

    /// <summary>
    /// Checks if Sketch can obtain the <see cref="move"/> in the requested <see cref="generation"/>
    /// </summary>
    /// <remarks>Doesn't bounds check the <see cref="generation"/> for max move ID.</remarks>
    /// <param name="move">Move ID</param>
    /// <param name="generation">Generation to check</param>
    /// <returns>True if can be sketched, false if not available.</returns>
    public static bool IsValidSketch(int move, int generation)
    {
        if (MoveInfo.InvalidSketch.Contains(move))
            return false;
        if (generation is 6 && move is ((int)ThousandArrows or (int)ThousandWaves))
            return false;
        if (generation is 8) // can't Sketch unusable moves in BDSP, no Sketch in PLA
        {
            if (DummiedMoves_BDSP.Contains(move))
                return false;
            if (move > MaxMoveID_8)
                return false;
        }

        return move <= GetMaxMoveID(generation);
    }

    /// <summary>
    /// Species that are from Mythical Distributions (disallowed species for competitive rulesets)
    /// </summary>
    public static readonly HashSet<int> Mythicals = new()
    {
        (int)Mew,
        (int)Celebi,
        (int)Jirachi, (int)Deoxys,
        (int)Phione, (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,
        (int)Victini, (int)Keldeo, (int)Meloetta, (int)Genesect,
        (int)Diancie, (int)Hoopa, (int)Volcanion,
        (int)Magearna, (int)Marshadow,
        (int)Zeraora, (int)Meltan, (int)Melmetal,
        (int)Zarude,
    };

    /// <summary>
    /// Species classified as "Legend" by the game code.
    /// </summary>
    public static readonly HashSet<int> Legends = new()
    {
        (int)Mewtwo, (int)Mew,
        (int)Lugia, (int)HoOh, (int)Celebi,
        (int)Kyogre, (int)Groudon, (int)Rayquaza, (int)Jirachi, (int)Deoxys,
        (int)Dialga, (int)Palkia, (int)Giratina, (int)Phione, (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,
        (int)Victini, (int)Reshiram, (int)Zekrom, (int)Kyurem, (int)Keldeo, (int)Meloetta, (int)Genesect,
        (int)Xerneas, (int)Yveltal, (int)Zygarde, (int)Diancie, (int)Hoopa, (int)Volcanion,
        (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala, (int)Necrozma, (int)Magearna, (int)Marshadow, (int)Zeraora,
        (int)Meltan, (int)Melmetal,
        (int)Zacian, (int)Zamazenta, (int)Eternatus, (int)Zarude, (int)Calyrex,
    };

    /// <summary>
    /// Species classified as "SubLegend" by the game code.
    /// </summary>
    public static readonly HashSet<int> SubLegends = new()
    {
        (int)Articuno, (int)Zapdos, (int)Moltres,
        (int)Raikou, (int)Entei, (int)Suicune,
        (int)Regirock, (int)Regice, (int)Registeel, (int)Latias, (int)Latios,
        (int)Uxie, (int)Mesprit, (int)Azelf, (int)Heatran, (int)Regigigas, (int)Cresselia,
        (int)Cobalion, (int)Terrakion, (int)Virizion, (int)Tornadus, (int)Thundurus, (int)Landorus,
        (int)TypeNull, (int)Silvally, (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini,
        (int)Nihilego, (int)Buzzwole, (int)Pheromosa, (int)Xurkitree, (int)Celesteela, (int)Kartana, (int)Guzzlord,
        (int)Poipole, (int)Naganadel, (int)Stakataka, (int)Blacephalon,
        (int)Kubfu, (int)Urshifu, (int)Regieleki, (int)Regidrago, (int)Glastrier, (int)Spectrier, (int)Enamorus,
    };

    /// <summary>
    /// Species that evolve from a Bi-Gendered species into a Single-Gender.
    /// </summary>
    public static readonly HashSet<int> FixedGenderFromBiGender = new()
    {
        (int)Nincada,
        (int)Shedinja, // (G)

        (int)Burmy,
        (int)Wormadam, //(F)
        (int)Mothim, // (M)

        (int)Ralts,
        (int)Gallade, // (M)

        (int)Snorunt,
        (int)Froslass, // (F)

        (int)Espurr,
        (int)Meowstic, // (M/F) form specific
    };

    private static bool[] GetPermitList(int max, IEnumerable<ushort> held)
    {
        var result = new bool[max + 1];
        foreach (var item in held)
            result[item] = true;
        return result;
    }

    private static bool[] GetPermitList(int max, IEnumerable<ushort> held, IEnumerable<ushort> unreleased)
    {
        var result = GetPermitList(max, held);
        foreach (var u in unreleased)
            result[u] = false;
        return result;
    }
}
