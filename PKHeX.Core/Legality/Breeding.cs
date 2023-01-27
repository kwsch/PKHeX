using System.Collections.Generic;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic related to breeding.
/// </summary>
public static class Breeding
{
    /// <summary>
    /// Checks if the game has a Daycare, and returns true if it does.
    /// </summary>
    /// <param name="game">Version ID to check for.</param>
    public static bool CanGameGenerateEggs(GameVersion game) => GamesWithEggs.Contains(game);

    private static readonly HashSet<GameVersion> GamesWithEggs = new()
    {
        GD, SI, C,
        R, S, E, FR, LG,
        D, P, Pt, HG, SS,
        B, W, B2, W2,
        X, Y, OR, AS,
        SN, MN, US, UM,
        SW, SH, BD, SP,
        SL, VL,

        GS,
    };

    /// <summary>
    /// Species that have special handling for breeding.
    /// </summary>
    internal static readonly HashSet<ushort> MixedGenderBreeding = new()
    {
        (int)NidoranF,
        (int)NidoranM,

        (int)Volbeat,
        (int)Illumise,

        (int)Indeedee, // male/female
    };

    /// <summary>
    /// Checks if the <see cref="species"/> can be born with inherited moves from the parents.
    /// </summary>
    /// <param name="species">Entity species ID</param>
    /// <returns>True if can inherit moves, false if cannot.</returns>
    internal static bool GetCanInheritMoves(ushort species)
    {
        if (Legal.FixedGenderFromBiGender.Contains(species)) // Nincada -> Shedinja loses gender causing 'false', edge case
            return true;
        var pi = PKX.Personal[species];
        if (pi is { Genderless: false, OnlyMale: false })
            return true;
        if (MixedGenderBreeding.Contains(species))
            return true;
        return false;
    }

    private static readonly HashSet<ushort> SplitBreed_3 = new()
    {
        // Incense
        (int)Marill,
        (int)Wobbuffet,
    };

    /// <summary>
    /// Species that can yield a different baby species when bred.
    /// </summary>
    private static readonly HashSet<ushort> SplitBreed = new(SplitBreed_3)
    {
        // Incense
        (int)Chansey,
        (int)MrMime,
        (int)Snorlax,
        (int)Sudowoodo,
        (int)Mantine,
        (int)Roselia,
        (int)Chimecho,
    };

    internal static IReadOnlySet<ushort>? GetSplitBreedGeneration(int generation) => generation switch
    {
        3 => SplitBreed_3,
        4 or 5 or 6 or 7 or 8 => SplitBreed,
        // Gen9 does not have split-breed egg generation.
        _ => null,
    };

    /// <summary>
    /// Checks if the <see cref="species"/> can be obtained from a daycare egg.
    /// </summary>
    /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
    /// <param name="species">Current species</param>
    public static bool CanHatchAsEgg(ushort species) => !NoHatchFromEgg.Contains(species);

    /// <summary>
    /// Checks if the <see cref="species"/>-<see cref="form"/> can exist as a hatched egg in the requested <see cref="context"/>.
    /// </summary>
    /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
    /// <param name="species">Current species</param>
    /// <param name="form">Current form</param>
    /// <param name="context">Generation of origin</param>
    public static bool CanHatchAsEgg(ushort species, byte form, EntityContext context)
    {
        if (form == 0)
            return true;

        if (FormInfo.IsTotemForm(species, form, context))
            return false;

        if (FormInfo.IsLordForm(species, form, context))
            return false;

        return IsBreedableForm(species, form);
    }

    /// <summary>
    /// Some species can have forms that cannot exist as egg (event/special forms). Same idea as <see cref="FormInfo.IsTotemForm(ushort,byte,EntityContext)"/>
    /// </summary>
    /// <returns>True if can be bred.</returns>
    private static bool IsBreedableForm(ushort species, byte form) => species switch
    {
        (int)Pikachu or (int)Eevee => false, // can't get these forms as egg
        (int)Pichu => false, // can't get Spiky Ear Pichu eggs
        (int)Floette when form == 5 => false, // can't get Eternal Flower from egg
        (int)Greninja when form == 1 => false, // can't get Battle Bond Greninja from egg
        (int)Sinistea or (int)Polteageist => false, // can't get Antique eggs
        _ => true,
    };

    /// <summary>
    /// Species that cannot hatch from an egg.
    /// </summary>
    private static readonly HashSet<ushort> NoHatchFromEgg = new()
    {
        // Gen1
        (int)Ditto,
        (int)Articuno, (int)Zapdos, (int)Moltres,
        (int)Mewtwo, (int)Mew,

        // Gen2
        (int)Unown,
        (int)Raikou, (int)Entei, (int)Suicune,
        (int)Lugia, (int)HoOh, (int)Celebi,

        // Gen3
        (int)Regirock, (int)Regice, (int)Registeel,
        (int)Latias, (int)Latios,
        (int)Kyogre, (int)Groudon, (int)Rayquaza,
        (int)Jirachi, (int)Deoxys,

        // Gen4
        (int)Uxie, (int)Mesprit, (int)Azelf,
        (int)Dialga, (int)Palkia, (int)Heatran,
        (int)Regigigas, (int)Giratina, (int)Cresselia,
        (int)Manaphy, (int)Darkrai, (int)Shaymin, (int)Arceus,

        // Gen5
        (int)Victini,
        (int)Cobalion, (int)Terrakion, (int)Virizion,
        (int)Tornadus, (int)Thundurus,
        (int)Reshiram, (int)Zekrom,
        (int)Landorus, (int)Kyurem,
        (int)Keldeo, (int)Meloetta, (int)Genesect,

        // Gen6
        (int)Xerneas, (int)Yveltal, (int)Zygarde,
        (int)Diancie, (int)Hoopa, (int)Volcanion,

        // Gen7
        (int)TypeNull, (int)Silvally,
        (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini,
        (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala,
        (int)Nihilego, (int)Buzzwole, (int)Pheromosa, (int)Xurkitree, (int)Celesteela, (int)Kartana, (int)Guzzlord, (int)Necrozma,
        (int)Magearna, (int)Marshadow,
        (int)Poipole, (int)Naganadel, (int)Stakataka, (int)Blacephalon, (int)Zeraora,

        (int)Meltan, (int)Melmetal,

        // Gen8
        (int)Dracozolt, (int)Arctozolt, (int)Dracovish, (int)Arctovish,
        (int)Zacian, (int)Zamazenta, (int)Eternatus,
        (int)Kubfu, (int)Urshifu, (int)Zarude,
        (int)Regieleki, (int)Regidrago,
        (int)Glastrier, (int)Spectrier, (int)Calyrex,
        (int)Enamorus,

        // Gen9
        (int)Gimmighoul, (int)Gholdengo,
        (int)GreatTusk, (int)BruteBonnet, (int)_980, (int)SandyShocks, (int)ScreamTail, (int)FlutterMane, (int)SlitherWing, (int)RoaringMoon,
        (int)IronTreads, (int)_987, (int)IronMoth, (int)IronHands, (int)IronJugulis, (int)IronThorns, (int)IronBundle, (int)IronValiant,
        (int)TingLu, (int)ChienPao, (int)WoChien, (int)ChiYu,
        (int)Koraidon, (int)Miraidon,
    };
}
