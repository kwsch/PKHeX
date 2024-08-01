using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic related to breeding.
/// </summary>
public static class Breeding
{
    /// <summary>
    /// Species that have special handling for breeding.
    /// </summary>
    private static bool IsMixedGenderBreed(ushort species) => species switch
    {
        (int)NidoranF => true,
        (int)NidoranM => true,

        (int)Volbeat => true,
        (int)Illumise => true,

        (int)Indeedee => true, // male/female

        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="species"/> can be born with inherited moves from the parents.
    /// </summary>
    /// <param name="species">Entity species ID</param>
    /// <returns>True if it can inherit moves, false if cannot.</returns>
    internal static bool GetCanInheritMoves(ushort species)
    {
        var pi = PKX.Personal[species];
        if (pi is { Genderless: false, OnlyMale: false })
            return true;
        if (IsMixedGenderBreed(species))
            return true;
        return false;
    }

    /// <summary>
    /// Species that can yield a different baby species when bred.
    /// </summary>
    public static bool IsSplitBreedNotBabySpecies(ushort species, byte generation)
    {
        if (generation == 3)
            return IsSplitBreedNotBabySpecies3(species);
        if (generation is 4 or 5 or 6 or 7 or 8)
            return IsSplitBreedNotBabySpecies4(species);
        // Gen9 does not have split-breed egg generation.
        return false;
    }

    /// <summary>
    /// Checks if the species can yield a different baby species when bred via incense in Generation 3.
    /// </summary>
    /// <remarks>
    /// This is a special case for Marill and Wobbuffet, which can be bred with incense to yield Azurill and Wynaut respectively.
    /// </remarks>
    public static bool IsSplitBreedNotBabySpecies3(ushort species) => species is (ushort)Marill or (ushort)Wobbuffet;

    /// <summary>
    /// Checks if the species can yield a different baby species when bred via incense in Generation 4-8.
    /// </summary>
    public static bool IsSplitBreedNotBabySpecies4(ushort species) => species switch
    {
        (int)Marill => true,
        (int)Wobbuffet => true,

        (int)Chansey => true,
        (int)MrMime => true,
        (int)Snorlax => true,
        (int)Sudowoodo => true,
        (int)Mantine => true,
        (int)Roselia => true,
        (int)Chimecho => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="species"/> can be obtained from a daycare egg.
    /// </summary>
    /// <remarks>Chained with the other 2 overloads for incremental checks with different parameters.</remarks>
    /// <param name="species">Current species</param>
    public static bool CanHatchAsEgg(ushort species) => IsAbleToHatchFromEgg(species);

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
    /// <returns>True if it can be bred.</returns>
    private static bool IsBreedableForm(ushort species, byte form) => species switch
    {
        (int)Pikachu or (int)Eevee => false, // can't get these forms as egg
        (int)Pichu => false, // can't get Spiky Ear Pichu eggs
        (int)Floette when form == 5 => false, // can't get Eternal Flower from egg
        (int)Greninja when form == 1 => false, // can't get Battle Bond Greninja from egg
        (int)Sinistea or (int)Polteageist => false, // can't get Antique eggs
        (int)Poltchageist or (int)Sinistcha => false, // can't get Masterpiece eggs
        _ => true,
    };

    /// <summary>
    /// Species that cannot hatch from an egg.
    /// </summary>
    private static bool IsAbleToHatchFromEgg(ushort species) => species switch
    {
        // Gen1
        (int)Ditto => false,
        (int)Articuno or (int)Zapdos or (int)Moltres => false,
        (int)Mewtwo or (int)Mew => false,

        // Gen2
        (int)Unown => false,
        (int)Raikou or (int)Entei or (int)Suicune => false,
        (int)Lugia or (int)HoOh or (int)Celebi => false,

        // Gen3
        (int)Regirock or (int)Regice or (int)Registeel => false,
        (int)Latias or (int)Latios => false,
        (int)Kyogre or (int)Groudon or (int)Rayquaza => false,
        (int)Jirachi or (int)Deoxys => false,

        // Gen4
        (int)Uxie or (int)Mesprit or (int)Azelf => false,
        (int)Dialga or (int)Palkia or (int)Heatran => false,
        (int)Regigigas or (int)Giratina or (int)Cresselia => false,
        (int)Manaphy or (int)Darkrai or (int)Shaymin or (int)Arceus => false,

        // Gen5
        (int)Victini => false,
        (int)Cobalion or (int)Terrakion or (int)Virizion => false,
        (int)Tornadus or (int)Thundurus => false,
        (int)Reshiram or (int)Zekrom => false,
        (int)Landorus or (int)Kyurem => false,
        (int)Keldeo or (int)Meloetta or (int)Genesect => false,

        // Gen6
        (int)Xerneas or (int)Yveltal or (int)Zygarde => false,
        (int)Diancie or (int)Hoopa or (int)Volcanion => false,

        // Gen7
        (int)TypeNull or (int)Silvally => false,
        (int)TapuKoko or (int)TapuLele or (int)TapuBulu or (int)TapuFini => false,
        (int)Cosmog or (int)Cosmoem or (int)Solgaleo or (int)Lunala => false,
        (int)Nihilego or (int)Buzzwole or (int)Pheromosa or (int)Xurkitree or (int)Celesteela or (int)Kartana or (int)Guzzlord or (int)Necrozma => false,
        (int)Magearna or (int)Marshadow => false,
        (int)Poipole or (int)Naganadel or (int)Stakataka or (int)Blacephalon or (int)Zeraora => false,

        (int)Meltan or (int)Melmetal => false,

        // Gen8
        (int)Dracozolt or (int)Arctozolt or (int)Dracovish or (int)Arctovish => false,
        (int)Zacian or (int)Zamazenta or (int)Eternatus => false,
        (int)Kubfu or (int)Urshifu or (int)Zarude => false,
        (int)Regieleki or (int)Regidrago => false,
        (int)Glastrier or (int)Spectrier or (int)Calyrex => false,
        (int)Enamorus => false,

        // Gen9
        (int)GreatTusk or (int)ScreamTail or (int)BruteBonnet or (int)FlutterMane or (int)SlitherWing or (int)SandyShocks => false,
        (int)IronTreads or (int)IronBundle or (int)IronHands or (int)IronJugulis or (int)IronMoth or (int)IronThorns => false,
        (int)Gimmighoul or (int)Gholdengo => false,
        (int)WoChien or (int)ChienPao or (int)TingLu or (int)ChiYu => false,
        (int)RoaringMoon or (int)IronValiant => false,
        (int)Koraidon or (int)Miraidon => false,
        (int)WalkingWake or (int)IronLeaves => false,
        (int)Okidogi or (int)Munkidori or (int)Fezandipiti or (int)Ogerpon => false,
        (int)GougingFire or (int)RagingBolt or (int)IronBoulder or (int)IronCrown or (int)Terapagos or (int)Pecharunt => false,

        _ => true,
    };
}
