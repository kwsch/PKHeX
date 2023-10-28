using System.Collections.Generic;
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
    public static bool IsMythical(ushort species) => Mythicals.Contains(species);

    /// <summary>
    /// Checks if a Species is classified as "Legend" by the game code.
    /// </summary>
    /// <remarks>Previous games may have included Mythicals in this species list, but that list should be considered separately.</remarks>
    public static bool IsLegendary(ushort species) => Legends.Contains(species);

    /// <summary>
    /// Checks if a Species is classified as "SubLegend" by the game code.
    /// </summary>
    public static bool IsSubLegendary(ushort species) => SubLegends.Contains(species);

    private static readonly HashSet<ushort> Mythicals = new()
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

    private static readonly HashSet<ushort> Legends = new()
    {
        (int)Mewtwo,
        (int)Lugia, (int)HoOh,
        (int)Kyogre, (int)Groudon, (int)Rayquaza,
        (int)Dialga, (int)Palkia, (int)Giratina,
        (int)Reshiram, (int)Zekrom, (int)Kyurem,
        (int)Xerneas, (int)Yveltal, (int)Zygarde,
        (int)Cosmog, (int)Cosmoem, (int)Solgaleo, (int)Lunala, (int)Necrozma,
        (int)Zacian, (int)Zamazenta, (int)Eternatus, (int)Calyrex,
        (int)Koraidon, (int)Miraidon,
    };

    private static readonly HashSet<ushort> SubLegends = new()
    {
        (int)Articuno, (int)Zapdos, (int)Moltres,
        (int)Raikou, (int)Entei, (int)Suicune,
        (int)Regirock, (int)Regice, (int)Registeel, (int)Latias, (int)Latios,
        (int)Uxie, (int)Mesprit, (int)Azelf, (int)Heatran, (int)Regigigas, (int)Cresselia,
        (int)Cobalion, (int)Terrakion, (int)Virizion, (int)Tornadus, (int)Thundurus, (int)Landorus,
        (int)TypeNull, (int)Silvally, (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini,
        (int)Kubfu, (int)Urshifu, (int)Regieleki, (int)Regidrago, (int)Glastrier, (int)Spectrier, (int)Enamorus,
        (int)WoChien, (int)ChienPao, (int)TingLu, (int)ChiYu,
        (int)Okidogi, (int)Munkidori, (int)Fezandipiti, (int)Ogerpon,
    };

    /// <summary>
    /// Checks if the <see cref="species"/> is an Ultra Beast Pokémon.
    /// </summary>
    public static bool IsUltraBeast(ushort species) => species is (>= (int)Nihilego and <= (int)Guzzlord) or (>= (int)Poipole and <= (int)Blacephalon);

    /// <summary>
    /// Checks if the <see cref="species"/> is a Paradox Pokémon.
    /// </summary>
    public static bool IsParadox(ushort species) => species is (>= (int)GreatTusk and <= (int)IronThorns) or (int)RoaringMoon or (int)IronValiant;

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
