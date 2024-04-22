using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Categorizes <see cref="Species"/> into groups, or specific edge-case qualities.
/// </summary>
public static class SpeciesCategory
{
    /// <summary>
    /// Checks if a Species is classified as a Mythical Distributions (disallowed species for competitive rulesets)
    /// </summary>
    public static bool IsMythical(ushort species) => species is
        (int)Mew or
        (int)Celebi or
        (int)Jirachi or (int)Deoxys or
        (int)Phione or (int)Manaphy or (int)Darkrai or (int)Shaymin or (int)Arceus or
        (int)Victini or (int)Keldeo or (int)Meloetta or (int)Genesect or
        (int)Diancie or (int)Hoopa or (int)Volcanion or
        (int)Magearna or (int)Marshadow or
        (int)Zeraora or (int)Meltan or (int)Melmetal or
        (int)Zarude or
        (int)Pecharunt
        ;

    /// <summary>
    /// Checks if a Species is classified as "Legend" by the game code.
    /// </summary>
    /// <remarks>Previous games may have included Mythicals in this species list, but that list should be considered separately.</remarks>
    public static bool IsLegendary(ushort species) => species is

        (int)Mewtwo or
        (int)Lugia or (int)HoOh or
        (int)Kyogre or (int)Groudon or (int)Rayquaza or
        (int)Dialga or (int)Palkia or (int)Giratina or
        (int)Reshiram or (int)Zekrom or (int)Kyurem or
        (int)Xerneas or (int)Yveltal or (int)Zygarde or
        (int)Cosmog or (int)Cosmoem or (int)Solgaleo or (int)Lunala or (int)Necrozma or
        (int)Zacian or (int)Zamazenta or (int)Eternatus or (int)Calyrex or
        (int)Koraidon or (int)Miraidon or
        (int)Terapagos
        ;

    /// <summary>
    /// Checks if a Species is classified as "SubLegend" by the game code.
    /// </summary>
    public static bool IsSubLegendary(ushort species) => species is

        (int)Articuno or (int)Zapdos or (int)Moltres or
        (int)Raikou or (int)Entei or (int)Suicune or
        (int)Regirock or (int)Regice or (int)Registeel or (int)Latias or (int)Latios or
        (int)Uxie or (int)Mesprit or (int)Azelf or (int)Heatran or (int)Regigigas or (int)Cresselia or
        (int)Cobalion or (int)Terrakion or (int)Virizion or (int)Tornadus or (int)Thundurus or (int)Landorus or
        (int)TypeNull or (int)Silvally or (int)TapuKoko or (int)TapuLele or (int)TapuBulu or (int)TapuFini or
        (int)Kubfu or (int)Urshifu or (int)Regieleki or (int)Regidrago or (int)Glastrier or (int)Spectrier or (int)Enamorus or
        (int)WoChien or (int)ChienPao or (int)TingLu or (int)ChiYu or
        (int)Okidogi or (int)Munkidori or (int)Fezandipiti or (int)Ogerpon
        ;

    /// <summary>
    /// Checks if the <see cref="species"/> is an Ultra Beast Pokémon.
    /// </summary>
    public static bool IsUltraBeast(ushort species) => species is (>= (int)Nihilego and <= (int)Guzzlord) or (>= (int)Poipole and <= (int)Blacephalon);

    /// <summary>
    /// Checks if the <see cref="species"/> is a Paradox Pokémon.
    /// </summary>
    public static bool IsParadox(ushort species) => species is (>= (int)GreatTusk and <= (int)IronThorns)
        or (int)RoaringMoon or (int)IronValiant
        or (int)WalkingWake or (int)IronLeaves
        or (int)GougingFire or (int)RagingBolt or (int)IronBoulder or (int)IronCrown;

    public static bool IsFixedGenderFromDual(ushort currentSpecies) => currentSpecies switch
    {
        (int)Shedinja => true, // Genderless

        (int)Wormadam => true, //(F)
        (int)Mothim => true, // (M)
        (int)Vespiquen => true, // (F)
        (int)Gallade => true, // (M)
        (int)Froslass => true, // (F)

        // Species introduced after Gender has been disassociated from PID
        (int)Meowstic => true, // (M/F) form specific
        (int)Salazzle => true, // (F)
        (int)Oinkologne => true, // (M/F) form specific

        _ => false,
    };
}
