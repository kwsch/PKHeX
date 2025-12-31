using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.Species;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Retrieves localized form names for indicating <see cref="ISpeciesForm.Form"/> values.
/// </summary>
public static class FormConverter
{
    /// <summary>
    /// Gets a list of forms that the species can have.
    /// </summary>
    /// <param name="species"><see cref="Species"/> of the Pokémon.</param>
    /// <param name="types">List of type names</param>
    /// <param name="forms">List of form names</param>
    /// <param name="genders">List of genders names</param>
    /// <param name="context">Current context for exclusive forms</param>
    /// <returns>A list of strings corresponding to the forms that a Pokémon can have.</returns>
    public static string[] GetFormList(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, EntityContext context)
    {
        // Mega List
        if (context.IsMegaContext && IsFormListSingleMega(species, context))
            return GetMegaSingle(types, forms);

        if (context is Gen7 && FormInfo.HasTotemForm(species))
            return GetFormsTotem(species, types, forms);

        return species switch
        {
            <= Legal.MaxSpeciesID_1 => GetFormsGen1(species, types, forms, context),
            <= Legal.MaxSpeciesID_2 => GetFormsGen2(species, types, forms, context),
            <= Legal.MaxSpeciesID_3 => GetFormsGen3(species, types, forms, context),
            <= Legal.MaxSpeciesID_4 => GetFormsGen4(species, types, forms, context),
            <= Legal.MaxSpeciesID_5 => GetFormsGen5(species, types, forms, context),
            <= Legal.MaxSpeciesID_6 => GetFormsGen6(species, types, forms, genders, context),
            <= Legal.MaxSpeciesID_7_USUM => GetFormsGen7(species, types, forms, context),
            <= Legal.MaxSpeciesID_8a => GetFormsGen8(species, context, types, forms, genders),
            _ => GetFormsGen9(species, context, types, forms, genders),
        };
    }

    /// <summary>
    /// Used to indicate that the form list is a single form, so no name is specified.
    /// </summary>
    private static readonly string[] EMPTY = [string.Empty];

    /// <summary>
    /// Lets Go, Pikachu! &amp; Eevee! Starter form name.
    /// </summary>
    /// <remarks>
    /// Different from the "Partner Cap" form.
    /// </remarks>
    private const string Starter = nameof(Starter);

    private static string[] GetFormsGen1(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Charizard or Mewtwo when context.IsMegaContext => GetMegaXY(types, forms),
            Eevee when context is Gen7b =>
            [
                types[0], // Normal
                Starter,
            ],
            Pikachu => GetFormsPikachu(context, types, forms),
            Slowbro when context.Generation >= 8 => GetFormsGalarSlowbro(types, forms),

            Weezing or Ponyta or Rapidash or Slowpoke or MrMime or Farfetchd or Articuno or Zapdos or Moltres when context.Generation >= 8 => GetFormsGalar(types, forms),
            Growlithe or Arcanine or Voltorb or Electrode when context.Generation >= 8 => GetFormsHisui(species, context, types, forms),
            Tauros when context.Generation >= 9 => GetFormsPaldea(species, types, forms),

            _ => GetFormsAlolan(context, types, forms, species),
        };
    }

    private static string[] GetFormsGen2(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Pichu when context is Gen4 => GetFormsPichu(types, forms),
            Slowking or Corsola when context.Generation >= 8 => GetFormsGalar(types, forms),
            Typhlosion or Qwilfish or Sneasel when context.Generation >= 8 => GetFormsHisui(species, context, types, forms),
            Wooper when context.Generation >= 9 => GetFormsPaldea(species, types, forms),
            Unown => GetFormsUnown(context),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen3(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Zigzagoon or Linoone when context.Generation >= 8 => GetFormsGalar(types, forms),
            Absol when context.IsMegaContext => GetMegaZ(types, forms), // Single mega would return earlier
            Castform => [
                types[0], // Normal
                forms[889], // Sunny
                forms[890], // Rainy
                forms[891], // Snowy
            ],
            Kyogre or Groudon when context.IsMegaContext => [
                types[0], // Normal
                forms[899], // Primal
            ],
            Deoxys => [
                types[0], // Normal
                forms[902], // Attack
                forms[903], // Defense
                forms[904], // Speed
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen4(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Burmy or Wormadam or Mothim => [
                forms[(int)Burmy], // Plant
                forms[905], // Sandy
                forms[906], // Trash
            ],
            Cherrim => [
                forms[(int)Cherrim], // Overcast
                forms[909], // Sunshine
            ],
            Shellos or Gastrodon => [
                forms[(int)Shellos], // West
                forms[911], // East
            ],
            Garchomp or Lucario when context.IsMegaContext => GetMegaZ(types, forms), // Single mega would return earlier
            Rotom => [
                types[0], // Normal
                forms[917], // Heat
                forms[918], // Wash
                forms[919], // Frost
                forms[920], // Fan
                forms[921], // Mow
            ],
            Dialga or Palkia when context.Generation >= 8 => [
                types[0], // Normal
                forms[922], // Origin
            ],
            Giratina => [
                forms[(int)Giratina], // Altered
                forms[922], // Origin
            ],
            Shaymin => [
                forms[(int)Shaymin], // Land
                forms[923], // Sky
            ],
            Arceus => GetFormsArceus(context, types, forms),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen5(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Samurott or Lilligant or Zorua or Zoroark or Braviary when context.Generation >= 8 => GetFormsHisui(species, context, types, forms),
            Basculin when context.Generation >= 8 => [
                forms[(int)Basculin], // Red
                forms[942], // Blue
                forms[989], // White
            ],
            Basculin => [
                forms[(int)Basculin], // Red
                forms[942], // Blue
            ],
            Darumaka or Stunfisk or Yamask when context.Generation >= 8 => GetFormsGalar(types, forms),
            Darmanitan when context.Generation >= 8 => [
                forms[(int)Darmanitan], // Standard
                forms[943], // Zen
                forms[Galarian], // Standard
                forms[Galarian] + " " + forms[943], // Zen
            ],
            Darmanitan => [
                forms[(int)Darmanitan], // Standard
                forms[943], // Zen
            ],
            Deerling or Sawsbuck => [
                forms[(int)Deerling], // Spring
                forms[947], // Summer
                forms[948], // Autumn
                forms[949], // Winter
            ],
            Tornadus or Thundurus or Landorus => [
                forms[(int)Tornadus], // Incarnate
                forms[952], // Therian
            ],
            Kyurem => [
                types[0], // Normal
                forms[953], // White
                forms[954], // Black
            ],
            Keldeo => [
                forms[(int)Keldeo], // Ordinary
                forms[955], // Resolute
            ],
            Meloetta => [
                forms[(int)Meloetta], // Aria
                forms[956], // Pirouette
            ],
            Genesect => [
                types[0], // Normal
                types[010], // Douse (Water)
                types[012], // Shock (Electric)
                types[009], // Burn (Fire)
                types[014], // Chill (Ice)
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen6(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, EntityContext context)
    {
        return (Species)species switch
        {
            Greninja => GetFormsGreninja(types, forms, new string[!context.IsMegaContext ? 2 : context.Generation >= 9 ? 4 : 3]),
            Scatterbug or Spewpa or Vivillon => [
                forms[(int)Vivillon], // Icy Snow
                forms[963], // Polar
                forms[964], // Tundra
                forms[965], // Continental
                forms[966], // Garden
                forms[967], // Elegant
                forms[968], // Meadow
                forms[969], // Modern
                forms[970], // Marine
                forms[971], // Archipelago
                forms[972], // High-Plains
                forms[973], // Sandstorm
                forms[974], // River
                forms[975], // Monsoon
                forms[976], // Savanna
                forms[977], // Sun
                forms[978], // Ocean
                forms[979], // Jungle
                forms[980], // Fancy
                forms[981], // Poké Ball
            ],
            Flabébé or Florges => [
                forms[(int)Flabébé], // Red
                forms[986], // Yellow
                forms[987], // Orange
                forms[988], // Blue
                forms[989], // White
            ],
            Floette => GetFormsFloette(forms, new string[!context.IsMegaContext ? 5 : context.Generation >= 9 ? 7 : 6]),
            Furfrou => [
                forms[(int)Furfrou], // Natural
                forms[995], // Heart
                forms[996], // Star
                forms[997], // Diamond
                forms[998], // Debutante
                forms[999], // Matron
                forms[1000], // Dandy
                forms[1001], // La Reine
                forms[1002], // Kabuki
                forms[1003], // Pharaoh
            ],
            Meowstic when context is { IsMegaContext: true, Generation: >= 9 } => [
                genders[000], // Male
                genders[001], // Female
                $"{genders[000]}-{forms[Mega]}", // Mega (Male)
                $"{genders[001]}-{forms[Mega]}", // Mega (Female)
            ],
            Meowstic => [
                genders[000], // Male
                genders[001], // Female
            ],
            Aegislash => [
                forms[(int)Aegislash], // Shield
                forms[1005], // Blade
            ],
            Sliggoo or Goodra or Avalugg when context.Generation >= 8 => GetFormsHisui(species, context, types, forms),
            Pumpkaboo or Gourgeist when context.Generation >= 9 => [
                forms[MediumVariety],
                forms[SmallVariety],
                forms[LargeVariety],
                forms[JumboVariety],
            ],
            Pumpkaboo or Gourgeist => [
                forms[(int)Pumpkaboo], // Average
                forms[1006], // Small
                forms[1007], // Large
                forms[1008], // Super
            ],
            Xerneas when context.Generation < 9 => [
                forms[(int)Xerneas], // Neutral
                forms[1012], // Active
            ],
            Zygarde => GetFormsZygarde(forms, new string[context is { IsMegaContext: true, Generation: >= 9 } ? 6 : 5]),
            Hoopa => [
                forms[(int)Hoopa], // Confined
                forms[1018], // Unbound
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen7(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Decidueye when context.Generation >= 8 => GetFormsHisui(species, context, types, forms),
            Oricorio => [
                forms[(int)Oricorio], // "RED" - Baile
                forms[1021], // "YLW" - Pom-Pom
                forms[1022], // "PNK" - Pa'u
                forms[1023], // "BLU" - Sensu
            ],
            Rockruff => [
                types[0], // Normal
                forms[1064], // Dusk
            ],
            Lycanroc => [
                forms[(int)Lycanroc], // Midday
                forms[1024], // Midnight
                forms[1064], // Dusk
            ],
            Wishiwashi => [
                forms[(int)Wishiwashi],
                forms[1025], // School
            ],
            Silvally => GetFormsArceus(Gen7, types, forms),
            Minior => [
                forms[(int)Minior], // "R-Meteor", // Meteor Red
                forms[1045], // "O-Meteor", // Meteor Orange
                forms[1046], // "Y-Meteor", // Meteor Yellow
                forms[1047], // "G-Meteor", // Meteor Green
                forms[1048], // "B-Meteor", // Meteor Blue
                forms[1049], // "I-Meteor", // Meteor Indigo
                forms[1050], // "V-Meteor", // Meteor Violet
                forms[1051], // "R-Core", // Core Red
                forms[1052], // "O-Core", // Core Orange
                forms[1053], // "Y-Core", // Core Yellow
                forms[1054], // "G-Core", // Core Green
                forms[1055], // "B-Core", // Core Blue
                forms[1056], // "I-Core", // Core Indigo
                forms[1057], // "V-Core", // Core Violet
            ],
            Mimikyu => [
                forms[(int)Mimikyu], // Disguised
                forms[1058], // Busted
            ],
            Necrozma when context is Gen7 => [
                types[0], // Normal
                forms[1065], // Dusk Mane
                forms[1066], // Dawn Wings
                forms[1067], // Ultra Necrozma
            ],
            Necrozma => [
                types[0], // Normal
                forms[1065], // Dusk Mane
                forms[1066], // Dawn Wings
            ],
            Magearna when context is { IsMegaContext: true, Generation: >= 9 } => [
                types[0],
                forms[1062], // Original Color
                $"{types[0]}-{forms[Mega]}", // Mega
                $"{forms[1062]}-{forms[Mega]}", // Mega (Original Color)
            ],
            Magearna => [
                types[0],
                forms[1062], // Original
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen8(ushort species, EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
    {
        return (Species)species switch
        {
            Cramorant => [
                types[0], // Normal
                forms[Gulping],
                forms[Gorging],
            ],
            Toxtricity => [
                forms[(int)Toxtricity], // Amped
                forms[LowKey],
            ],
            Sinistea or Polteageist => [
                forms[Phony],
                forms[Antique],
            ],
            Alcremie => [
                forms[(int)Alcremie], // Vanilla Cream
                forms[RubyCream],
                forms[MatchaCream],
                forms[MintCream],
                forms[LemonCream],
                forms[SaltedCream],
                forms[RubySwirl],
                forms[CaramelSwirl],
                forms[RainbowSwirl],
            ],
            Eiscue => [
                forms[IceFace],
                forms[NoiceFace],
            ],
            Indeedee or Basculegion => [
                genders[000], // Male
                genders[001], // Female
            ],
            Morpeko => [
                forms[FullBellyMode],
                forms[HangryMode],
            ],
            Zacian or Zamazenta => [
                forms[HeroOfManyBattles],
                forms[Crowned],
            ],
            Eternatus when context.Generation == 8 => [
                types[0], // Normal
                forms[Eternamax],
            ],
            Urshifu => [
                forms[SingleStrike],
                forms[RapidStrike],
            ],
            Zarude => [
                types[0], // Normal
                forms[Dada],
            ],
            Calyrex => [
                types[0], // Normal
                forms[IceRider],
                forms[ShadowRider],
            ],
            Kleavor when context.Generation == 8 => [
                types[0],
                forms[Lord],
            ],
            Ursaluna when context.Generation >= 9 => [
                types[0],
                forms[Bloodmoon],
            ],
            Enamorus => [
                forms[641], // Incarnate
                forms[952], // Therian
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen9(ushort species, EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
    {
        return (Species)species switch
        {
            Oinkologne => [
                genders[000], // Male
                genders[001], // Female
            ],
            Maushold => [
                forms[FamilyOfThree],
                forms[FamilyOfFour],
            ],
            Squawkabilly => [
                forms[Green],
                forms[988], // Blue
                forms[986], // Yellow
                forms[989], // White
            ],
            Palafin => [
                forms[Zero],
                forms[HeroPalafin],
            ],
            Tatsugiri when context.IsMegaContext => [
                forms[Curly],
                forms[Droopy],
                forms[Stretchy],
                $"{forms[Curly]}-{forms[Mega]}",
                $"{forms[Droopy]}-{forms[Mega]}",
                $"{forms[Stretchy]}-{forms[Mega]}",
            ],
            Tatsugiri => [
                forms[Curly],
                forms[Droopy],
                forms[Stretchy],
            ],
            Dudunsparce => [
                forms[TwoSegment],
                forms[ThreeSegment],
            ],
            Gimmighoul => [
                forms[Chest],
                forms[Roaming],
            ],
            Koraidon => [
                forms[Apex],
                forms[Limited],
                forms[Sprinting],
                forms[Swimming],
                forms[Gliding],
            ],
            Miraidon => [
                forms[Ultimate],
                forms[LowPower],
                forms[Drive],
                forms[Aquatic],
                forms[Glide],
            ],
            Poltchageist => [
                forms[Counterfeit],
                forms[Artisan],
            ],
            Sinistcha => [
                forms[Unremarkable],
                forms[Masterpiece],
            ],
            Ogerpon => [
                forms[MaskTeal],
                forms[MaskWellspring],
                forms[MaskHearthflame],
                forms[MaskCornerstone],
                $"*{forms[MaskTeal]}",
                $"*{forms[MaskWellspring]}",
                $"*{forms[MaskHearthflame]}",
                $"*{forms[MaskCornerstone]}",
            ],
            Terapagos => [
                types[0], // Normal
                forms[Terastal],
                forms[Stellar],
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsAlolan(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms, ushort species)
    {
        byte generation = context.Generation;
        if (generation < 7)
            return EMPTY;

        return (Species)species switch
        {
            Meowth when generation >= 8 => [
                types[0],
                forms[Alolan], // Alolan
                forms[Galarian], // Galarian
            ],

            Raichu when generation >= 9 && context.IsMegaContext => [
                types[0],
                forms[Alolan], // Alolan
                forms[MegaX], // Mega X
                forms[MegaY], // Mega Y
            ],

            // Only reached when Gen8+, as Totem logic picks up Gen7 earlier.
            Rattata or Raticate
                or Raichu
                or Sandshrew or Sandslash
                or Vulpix or Ninetales
                or Diglett or Dugtrio
                or Meowth or Persian
                or Geodude or Graveler or Golem
                or Grimer or Muk
                or Exeggutor
                or Marowak => [
                    types[0],
                    forms[Alolan], // Alolan
                ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsPikachu(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return context switch
        {
            < Gen6 => EMPTY,
            Gen6 => [
                types[0], // Normal
                forms[729], // Rockstar
                forms[730], // Belle
                forms[731], // Pop
                forms[732], // PhD
                forms[733], // Libre
                forms[734], // Cosplay
            ],
            Gen7b => [
                types[0], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
                Starter,
            ],
            Gen7 => [
                types[0], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
            ],
            _ => [
                types[0], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
                Starter,
                forms[1085], // World
            ],
        };
    }

    private static string[] GetFormsPichu(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[000], // Spiky
        ];
    }

    private static string[] GetFormsArceus(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return context switch
        {
            Gen4 => [
                types[00], // Normal
                types[01], // Fighting
                types[02], // Flying
                types[03], // Poison
                types[04], // etc
                types[05],
                types[06],
                types[07],
                types[08],
                "???", // ???-type arceus
                types[09],
                types[10],
                types[11],
                types[12],
                types[13],
                types[14],
                types[15],
                types[16], // No Fairy Type
            ],
            Gen5 => [
                types[00], // Normal
                types[01], // Fighting
                types[02], // Flying
                types[03], // Poison
                types[04], // etc
                types[05],
                types[06],
                types[07],
                types[08],
                types[09],
                types[10],
                types[11],
                types[12],
                types[13],
                types[14],
                types[15],
                types[16], // No Fairy type
            ],
            Gen8a =>
            [
                types[00], // Normal
                types[01], // Fighting
                types[02], // Flying
                types[03], // Poison
                types[04], // etc
                types[05],
                types[06],
                types[07],
                types[08],
                types[09],
                types[10],
                types[11],
                types[12],
                types[13],
                types[14],
                types[15],
                types[16],
                types[17],
                forms[Legend],
            ],
            _ => [
                types[00], // Normal
                types[01], // Fighting
                types[02], // Flying
                types[03], // Poison
                types[04], // etc
                types[05],
                types[06],
                types[07],
                types[08],
                types[09],
                types[10],
                types[11],
                types[12],
                types[13],
                types[14],
                types[15],
                types[16],
                types[17],
            ],
        };
    }

    private static string[] GetFormsTotem(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms) => species switch
    {
        (int)Mimikyu =>
        [
            forms[(int)Mimikyu], // Disguised
            forms[1058], // Busted
            forms[1007], // Large
            "*" + forms[1058], // Busted
        ],
        (int)Raticate or (int)Marowak =>
        [
            types[0], // Normal
            forms[Alolan], // Alolan
            forms[1007], // Large
        ],
        _ =>
        [
            types[0], // Normal
            forms[1007], // Large
        ],
    };

    private static string[] GetFormsUnown(EntityContext context) => context switch
    {
        Gen2 =>
        [
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T",
            "U", "V", "W", "X", "Y",
            "Z",
            // "!", "?", not in Gen2
        ],
        _ =>
        [
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T",
            "U", "V", "W", "X", "Y",
            "Z",
            "!", "?",
        ],
    };

    private static bool IsFormListSingleMega(ushort species, EntityContext context)
    {
        if (context.Generation < 9)
            return IsFormListSingleMega6(species);

        if (species is (int)Slowbro)
            return false; // Galar, Mega
        if (species is (int)Absol or (int)Garchomp or (int)Lucario)
            return false; // Mega, Z Mega
        if (IsFormListSingleMega9(species))
            return true;

        return IsFormListSingleMega6(species);
    }

    private static bool IsFormListSingleMega6(ushort species) => (Species)species is
        // XY
        Venusaur or Blastoise or Alakazam or Gengar or Kangaskhan or Pinsir or Gyarados or Aerodactyl or Ampharos or Scizor or
        Heracross or Houndoom or Tyranitar or Blaziken or Gardevoir or Mawile or Aggron or Medicham or Manectric or Banette or
        Absol or Latias or Latios or Garchomp or Lucario or Abomasnow or

        // AO
        Beedrill or Pidgeot or Slowbro or Steelix or Sceptile or Swampert or Sableye or Sharpedo or Camerupt or Altaria or
        Glalie or Salamence or Metagross or Rayquaza or Lopunny or Gallade or Audino or Diancie
    ;

    private static bool IsFormListSingleMega9(ushort species) => (Species)species is
        // XY
        Venusaur or Blastoise or Alakazam or Gengar or Kangaskhan or Pinsir or Gyarados or Aerodactyl or Ampharos or Scizor or
        Heracross or Houndoom or Tyranitar or Blaziken or Gardevoir or Mawile or Aggron or Medicham or Manectric or Banette or
        /* Absol or */ Latias or Latios /* or Garchomp or Lucario */ or Abomasnow or

        // AO
        Beedrill or Pidgeot or Slowbro or Steelix or Sceptile or Swampert or Sableye or Sharpedo or Camerupt or Altaria or
        Glalie or Salamence or Metagross or Rayquaza or Lopunny or Gallade or Audino or Diancie or

        // ZA
        Clefable or Victreebel or Starmie or Dragonite or
        Meganium or Feraligatr or Skarmory or
        Froslass or
        Emboar or Excadrill or Scolipede or Scrafty or Eelektross or Chandelure or
        Chesnaught or Delphox or Pyroar or Malamar or Barbaracle or Dragalge or Hawlucha or
        Drampa or
        Falinks

        // MD
        or Chimecho
        or Staraptor or Heatran or Darkrai
        or Golurk
        or Crabominable or Golisopod or Zeraora
        or Scovillain or Glimmora or Baxcalibur
    ;

    private static string[] GetMegaSingle(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[Mega], // Mega
        ];
    }

    private static string[] GetMegaXY(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[MegaX], // Mega X
            forms[MegaY], // Mega Y
        ];
    }

    private static string[] GetMegaZ(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[Mega], // Mega
            forms[MegaZ], // Mega Z
        ];
    }

    private static string[] GetFormsGalar(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[Galarian], // Galarian
        ];
    }

    private static string[] GetFormsHisui(ushort species, EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms) => context switch
    {
        Gen8a => (Species)species switch
        {
            Lilligant =>
            [
                types[0], // Normal
                forms[Hisuian],
                forms[Lady],
            ],
            Arcanine or Electrode or Avalugg =>
            [
                types[0], // Normal
                forms[Hisuian],
                forms[Lord],
            ],
            _ =>
            [
                types[0], // Normal
                forms[Hisuian],
            ],
        },
        _ =>
        [
            types[0], // Normal
            forms[Hisuian],
        ],
    };

    private static string[] GetFormsPaldea(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms) => (Species)species switch
    {
        Tauros => [
            types[0], // Normal
            $"{forms[Paldean]} {forms[PaldeanCombat]}",
            $"{forms[Paldean]} {forms[PaldeanBlaze]}",
            $"{forms[Paldean]} {forms[PaldeanAqua]}",
        ],
        _ =>
        [
            types[0], // Normal
            forms[Paldean],
        ],
    };

    private static string[] GetFormsGalarSlowbro(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[Mega], // Mega
            forms[Galarian], // Galarian
        ];
    }

    private static string[] GetFormsGreninja(IReadOnlyList<string> types, IReadOnlyList<string> forms, string[] result)
    {
        result[0] = types[0]; // Normal
        result[1] = forms[962]; // Battle Bond
        if (result.Length > 2)
            result[2] = forms[1012]; // Ash-Greninja
        if (result.Length > 3)
            result[3] = forms[Mega]; // Mega Greninja
        return result;
    }

    private static string[] GetFormsFloette(IReadOnlyList<string> forms, string[] result)
    {
        result[0] = forms[(int)Floette]; // Red
        result[1] = forms[986]; // Yellow
        result[2] = forms[987]; // Orange
        result[3] = forms[988]; // Blue
        result[4] = forms[989]; // White
        if (result.Length > 5)
            result[5] = forms[990]; // Eternal
        if (result.Length > 6)
            result[6] = forms[Mega];
        return result;
    }

    private static string[] GetFormsZygarde(IReadOnlyList<string> forms, string[] result)
    {
        result[0] = forms[(int)Zygarde]; // 50% Forme (Aura Break)
        result[1] = forms[1013]; // 10% Forme (Aura Break)
        result[2] = forms[1014] + "-C"; // 10% Forme (Power Construct)
        result[3] = forms[1015] + "-C"; // 50% Forme (Power Construct)
        result[4] = forms[1016]; // Complete Forme
        if (result.Length > 5)
            result[5] = forms[Mega];
        return result;
    }

    public static MegaFormNames GetMegaFormNames(ReadOnlySpan<string> forms, IReadOnlyList<string> gender, IReadOnlyList<string> types) => new()
    {
        Regular = forms[Mega],
        X = forms[MegaX],
        Y = forms[MegaY],
        Z = forms[MegaZ],

        Tatsu0 = $"{forms[Curly]}-{forms[Mega]}",
        Tatsu1 = $"{forms[Droopy]}-{forms[Mega]}",
        Tatsu2 = $"{forms[Stretchy]}-{forms[Mega]}",

        MeowsticM = $"{gender[000]}-{forms[Mega]}",
        MeowsticF = $"{gender[001]}-{forms[Mega]}",

        Magearna0 = $"{types[000]}-{forms[Mega]}",
        Magearna1 = $"{forms[1062]}-{forms[Mega]}",
    };

    private const int Mega = 804;
    private const int MegaX = 805;
    private const int MegaY = 806;
    private const int MegaZ = 1141;

    private const int Alolan = 810;

    private const int Galarian = 1068;
    private const int Gigantamax = 1069;
    private const int Gulping = 1070;
    private const int Gorging = 1071;
    private const int LowKey = 1072;

    private const int RubyCream = 1073;
    private const int MatchaCream = 1074;
    private const int MintCream = 1075;
    private const int LemonCream = 1076;
    private const int SaltedCream = 1077;
    private const int RubySwirl = 1078;
    private const int CaramelSwirl = 1079;
    private const int RainbowSwirl = 1080;

    private const int IceFace = 1091;
    private const int NoiceFace = 1081;
    private const int HangryMode = 1082;
    private const int FullBellyMode = 1092;
    private const int Phony = 1098;
    private const int Antique = 1099;

    private const int HeroOfManyBattles = 1093;
    private const int Crowned = 1083;
    private const int Eternamax = 1084;

    private const int SingleStrike = 1086;
    private const int RapidStrike = 1087;
    private const int Dada = 1088;
    private const int IceRider = 1089;
    private const int ShadowRider = 1090;

    private const int Hisuian = 1094;
    private const int Lord = 1095;
    private const int Lady = 1096;
    private const int Legend = 1097;

    private const int Paldean = 1100;
    private const int TwoSegment = 1101;
    private const int ThreeSegment = 1102;
    private const int Zero = 1103;
    private const int HeroPalafin = 1104;
    private const int FamilyOfThree = 1105;
    private const int FamilyOfFour = 1106;
    private const int Curly = 1107;
    private const int Droopy = 1108;
    private const int Stretchy = 1109;
    private const int Green = 1110;
    private const int Chest = 1111;
    private const int Roaming = 1112;
    private const int Apex = 1113;
    private const int Limited = 1114;
    private const int Sprinting = 1115;
    private const int Swimming = 1116;
    private const int Gliding = 1117;
    private const int Ultimate = 1118;
    private const int LowPower = 1119;
    private const int Drive = 1120;
    private const int Aquatic = 1121;
    private const int Glide = 1122;
    private const int PaldeanCombat = 1123;
    private const int PaldeanBlaze = 1124;
    private const int PaldeanAqua = 1125;
    internal const int MaskTeal = 1126;
    private const int Counterfeit = 1127;
    private const int Unremarkable = 1128;
    private const int Bloodmoon = 1129;
    internal const int MaskWellspring = 1130;
    internal const int MaskHearthflame = 1131;
    internal const int MaskCornerstone = 1132;
    private const int Artisan = 1133;
    private const int Masterpiece = 1134;
    private const int Terastal = 1135;
    private const int Stellar = 1136;
    private const int MediumVariety = 1137;
    private const int SmallVariety = 1138;
    private const int LargeVariety = 1139;
    private const int JumboVariety = 1140;

    public static string GetGigantamaxName(IReadOnlyList<string> forms) => forms[Gigantamax];

    private const byte AlcremieCountDecoration = 7;
    private const byte AlcremieCountForms = 9;
    private const byte AlcremieCountDifferent = AlcremieCountDecoration * AlcremieCountForms;

    /// <summary>
    /// Used to enumerate the possible combinations of Alcremie forms and decorations.
    /// </summary>
    /// <param name="forms">Form names</param>
    /// <remarks>
    /// Used for Pokédex display listings.
    /// </remarks>>
    public static string[] GetAlcremieFormList(IReadOnlyList<string> forms)
    {
        var result = new string[AlcremieCountDifferent]; // 63
        SetAlcremieFormList(forms, result);
        return result;
    }

    private static void SetAlcremieFormList(IReadOnlyList<string> forms, Span<string> result)
    {
        SetDecorations(result, 0, forms[(int)Alcremie]); // Vanilla Cream
        SetDecorations(result, 1, forms[RubyCream]);
        SetDecorations(result, 2, forms[MatchaCream]);
        SetDecorations(result, 3, forms[MintCream]);
        SetDecorations(result, 4, forms[LemonCream]);
        SetDecorations(result, 5, forms[SaltedCream]);
        SetDecorations(result, 6, forms[RubySwirl]);
        SetDecorations(result, 7, forms[CaramelSwirl]);
        SetDecorations(result, 8, forms[RainbowSwirl]);

        static void SetDecorations(Span<string> result, [ConstantExpected] byte f, ReadOnlySpan<char> baseName)
        {
            int start = f * AlcremieCountDecoration;
            var slice = result.Slice(start, AlcremieCountDecoration);
            for (int i = 0; i < slice.Length; i++)
                slice[i] = $"{baseName} ({(AlcremieDecoration)i})";
        }
    }

    public static bool GetFormArgumentIsNamedIndex(ushort species) => species == (int)Alcremie;

    public static string[] GetFormArgumentStrings(ushort species) => species switch
    {
        (int)Alcremie => Enum.GetNames<AlcremieDecoration>(),
        _ => EMPTY,
    };

    /// <summary>
    /// Compatibility check for past-generation form list for <see cref="Pikachu"/>.
    /// </summary>
    /// <param name="formName">Desired form name</param>
    /// <param name="formNames">List of all form names</param>
    /// <returns><see langword="true"/> if the form name is a cosplay Pikachu form.</returns>
    public static bool IsCosplayPikachu(ReadOnlySpan<char> formName, ReadOnlySpan<string> formNames)
    {
        for (int i = 729; i <= 734; i++)
        {
            if (formName.Equals(formNames[i], StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Converts a Form ID to string.
    /// </summary>
    /// <param name="form">Form to get the form name of</param>
    /// <param name="strings">Localized string source to fetch with</param>
    /// <param name="species">Species ID the form belongs to</param>
    /// <param name="genders">List of genders names</param>
    /// <param name="context">Format the form name should appear in</param>
    public static string GetStringFromForm(byte form, GameStrings strings, ushort species, IReadOnlyList<string> genders, EntityContext context)
    {
        var forms = GetFormList(species, strings.Types, strings.forms, genders, context);
        var result = form >= forms.Length ? string.Empty : forms[form];
        return result;
    }
}

public sealed record MegaFormNames
{
    public required string Regular { get; init; }
    public required string X { get; init; }
    public required string Y { get; init; }
    public required string Z { get; init; }

    public required string Tatsu0 { get; init; }
    public required string Tatsu1 { get; init; }
    public required string Tatsu2 { get; init; }
    public required string MeowsticM { get; init; }
    public required string MeowsticF { get; init; }
    public required string Magearna0 { get; init; }
    public required string Magearna1 { get; init; }
}
