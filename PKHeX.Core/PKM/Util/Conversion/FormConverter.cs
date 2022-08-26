using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Retrieves localized form names for indicating <see cref="PKM.Form"/> values.
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
        if (context.IsMegaGeneration() && IsFormListSingleMega(species))
            return GetMegaSingle(types, forms);

        if (context is Gen7 && Legal.Totem_USUM.Contains(species))
            return GetFormsTotem(species, types, forms);

        return species switch
        {
            <= Legal.MaxSpeciesID_1 => GetFormsGen1(species, types, forms, context),
            <= Legal.MaxSpeciesID_2 => GetFormsGen2(species, types, forms, context),
            <= Legal.MaxSpeciesID_3 => GetFormsGen3(species, types, forms, context.Generation()),
            <= Legal.MaxSpeciesID_4 => GetFormsGen4(species, types, forms, context.Generation()),
            <= Legal.MaxSpeciesID_5 => GetFormsGen5(species, types, forms, context.Generation()),
            <= Legal.MaxSpeciesID_6 => GetFormsGen6(species, types, forms, genders, context.Generation()),
            <= Legal.MaxSpeciesID_7_USUM => GetFormsGen7(species, types, forms, context.Generation()),
            _ => GetFormsGen8(species, context.Generation(), types, forms, genders),
        };
    }

    private static bool IsMegaGeneration(this EntityContext context) => context is Gen6 or Gen7 or Gen7b;

    private static readonly string[] EMPTY = { string.Empty };
    private const string Starter = nameof(Starter);

    private static string[] GetFormsGen1(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        return (Species)species switch
        {
            Charizard or Mewtwo when context.IsMegaGeneration() => GetMegaXY(types, forms),
            Eevee when context is Gen7b => new[]
            {
                types[000], // Normal
                Starter,
            },
            Pikachu => GetFormsPikachu(context, types, forms),
            Slowbro when context.Generation() >= 8 => GetFormsGalarSlowbro(types, forms),

            Weezing or Ponyta or Rapidash or Slowpoke or MrMime or Farfetchd
                or Articuno or Zapdos or Moltres when context.Generation() >= 8 => GetFormsGalar(types, forms),

            Growlithe or Arcanine or Voltorb or Electrode when context.Generation() >= 8 => GetFormsHisui(species, context.Generation(), types, forms),

            _ => GetFormsAlolan(context, types, forms, species),
        };
    }

    private static string[] GetFormsGen2(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        int generation = context.Generation();
        return (Species)species switch
        {
            Pichu when context is Gen4 => GetFormsPichu(types, forms),
            Slowking or Corsola when generation >= 8 => GetFormsGalar(types, forms),
            Typhlosion or Qwilfish or Sneasel when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Unown => GetFormsUnown(generation),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen3(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
    {
        return (Species)species switch
        {
            Zigzagoon or Linoone when generation >= 8 => GetFormsGalar(types, forms),
            Castform => new[] {
                types[000], // Normal
                forms[889], // Sunny
                forms[890], // Rainy
                forms[891], // Snowy
            },
            Kyogre when generation < 8 => new[] {
                types[000], // Normal
                forms[899], // Primal
            },
            Groudon when generation < 8 => new[] {
                types[000], // Normal
                forms[899], // Primal
            },
            Deoxys => new[] {
                types[000], // Normal
                forms[902], // Attack
                forms[903], // Defense
                forms[904], // Speed
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen4(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
    {
        return (Species)species switch
        {
            Burmy or Wormadam or Mothim => new[] {
                forms[412], // Plant
                forms[905], // Sandy
                forms[906], // Trash
            },
            Cherrim => new[] {
                forms[421], // Overcast
                forms[909], // Sunshine
            },
            Shellos or Gastrodon => new[] {
                forms[422], // West
                forms[911], // East
            },
            Rotom => new[] {
                types[000], // Normal
                forms[917], // Heat
                forms[918], // Wash
                forms[919], // Frost
                forms[920], // Fan
                forms[921], // Mow
            },
            Dialga or Palkia when generation >= 8 => new[] {
                types[000], // Normal
                forms[922], // Origin
            },
            Giratina => new[] {
                forms[487], // Altered
                forms[922], // Origin
            },
            Shaymin => new[] {
                forms[492], // Land
                forms[923], // Sky
            },
            Arceus => GetFormsArceus(species, generation, types, forms),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen5(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
    {
        return (Species)species switch
        {
            Samurott or Lilligant or Zorua or Zoroark or Braviary when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Basculin when generation >= 8 => new[] {
                forms[550], // Red
                forms[942], // Blue
                forms[989], // White
            },
            Basculin => new[] {
                forms[550], // Red
                forms[942], // Blue
            },
            Darumaka or Stunfisk or Yamask when generation >= 8 => GetFormsGalar(types, forms),
            Darmanitan when generation >= 8 => new[] {
                forms[555], // Standard
                forms[943], // Zen
                forms[Galarian], // Standard
                forms[Galarian] + " " + forms[943], // Zen
            },
            Darmanitan => new[] {
                forms[555], // Standard
                forms[943], // Zen
            },
            Deerling or Sawsbuck => new[] {
                forms[585], // Spring
                forms[947], // Summer
                forms[948], // Autumn
                forms[949], // Winter
            },
            Tornadus or Thundurus or Landorus => new[] {
                forms[641], // Incarnate
                forms[952], // Therian
            },
            Kyurem => new[] {
                types[000], // Normal
                forms[953], // White
                forms[954], // Black
            },
            Keldeo => new[] {
                forms[647], // Ordinary
                forms[955], // Resolute
            },
            Meloetta => new[] {
                forms[648], // Aria
                forms[956], // Pirouette
            },
            Genesect => new[] {
                types[000], // Normal
                types[010], // Douse (Water)
                types[012], // Shock (Electric)
                types[009], // Burn (Fire)
                types[014], // Chill (Ice)
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen6(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, int generation)
    {
        return (Species)species switch
        {
            Greninja => new[] {
                types[000], // Normal
                forms[962], // "Ash",
                forms[1012], // "Bonded" - Active
            },
            Scatterbug or Spewpa or Vivillon => new[] {
                forms[666], // Icy Snow
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
            },
            Flabébé or Florges => new[] {
                forms[669], // Red
                forms[986], // Yellow
                forms[987], // Orange
                forms[988], // Blue
                forms[989], // White
            },
            Floette => new[] {
                forms[669], // Red
                forms[986], // Yellow
                forms[987], // Orange
                forms[988], // Blue
                forms[989], // White
                forms[990], // Eternal
            },
            Furfrou => new[] {
                forms[676], // Natural
                forms[995], // Heart
                forms[996], // Star
                forms[997], // Diamond
                forms[998], // Debutante
                forms[999], // Matron
                forms[1000], // Dandy
                forms[1001], // La Reine
                forms[1002], // Kabuki
                forms[1003], // Pharaoh
            },
            Meowstic => new[] {
                genders[000], // Male
                genders[001], // Female
            },
            Aegislash => new[] {
                forms[681], // Shield
                forms[1005], // Blade
            },
            Sliggoo or Goodra or Avalugg when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Pumpkaboo or Gourgeist => new[] {
                forms[710], // Average
                forms[1006], // Small
                forms[1007], // Large
                forms[1008], // Super
            },
            Xerneas => new[] {
                forms[716], // Neutral
                forms[1012], // Active
            },
            Hoopa => new[] {
                forms[720], // Confined
                forms[1018], // Unbound
            },
            Zygarde => new[] {
                forms[718], // 50% (Aura Break)
                forms[1013], // 10% (Aura Break)
                forms[1014] + "-C", // 10% Cell (Power Construct)
                forms[1015] + "-C", // 50% Cell (Power Construct)
                forms[1016], // 100% Cell (Power Construct)
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen7(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
    {
        return (Species)species switch
        {
            Decidueye when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Oricorio => new[] {
                forms[741], // "RED" - Baile
                forms[1021], // "YLW" - Pom-Pom
                forms[1022], // "PNK" - Pa'u
                forms[1023], // "BLU" - Sensu
            },
            Rockruff => new[] {
                types[0], // Normal
                forms[1064], // Dusk
            },
            Lycanroc => new[] {
                forms[745], // Midday
                forms[1024], // Midnight
                forms[1064], // Dusk
            },
            Wishiwashi => new[] {
                forms[746],
                forms[1025], // School
            },
            Silvally => GetFormsArceus(species, 7, types, forms),
            Minior => new[] {
                forms[774], // "R-Meteor", // Meteor Red
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
            },
            Mimikyu => new[] {
                forms[778], // Disguised
                forms[1058], // Busted
            },
            Necrozma => new[] {
                types[000], // Normal
                forms[1065], // Dusk Mane
                forms[1066], // Dawn Wings
                forms[1067], // Ultra Necrozma
            },
            Magearna => new[] {
                types[000],
                forms[1062], // Original
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen8(ushort species, int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
    {
        return (Species)species switch
        {
            Cramorant => new[] {
                types[0], // Normal
                forms[Gulping],
                forms[Gorging],
            },
            Toxtricity => new[] {
                forms[(int)Toxtricity], // Amped
                forms[LowKey],
            },
            Indeedee or Basculegion => new[] {
                genders[000], // Male
                genders[001], // Female
            },
            Sinistea or Polteageist => new[] {
                "Phony",
                "Antique",
            },
            Alcremie => new[] {
                forms[(int)Alcremie], // Vanilla Cream
                forms[RubyCream],
                forms[MatchaCream],
                forms[MintCream],
                forms[LemonCream],
                forms[SaltedCream],
                forms[RubySwirl],
                forms[CaramelSwirl],
                forms[RainbowSwirl],
            },
            Morpeko => new[] {
                forms[FullBellyMode],
                forms[HangryMode],
            },
            Eiscue => new[] {
                forms[IceFace],
                forms[NoiceFace],
            },
            Zacian or Zamazenta => new[] {
                forms[Hero],
                forms[Crowned],
            },
            Eternatus => new[] {
                types[0], // Normal
                forms[Eternamax],
            },
            Urshifu => new[] {
                forms[SingleStrike],
                forms[RapidStrike],
            },
            Zarude => new[] {
                types[0], // Normal
                forms[Dada],
            },
            Calyrex => new[] {
                types[0], // Normal
                forms[CalyIce],
                forms[CalyGhost],
            },
            Kleavor when generation == 8 => new[] {
                types[000],
                forms[Lord],
            },
            Enamorus => new[] {
                forms[641], // Incarnate
                forms[952], // Therian
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsAlolan(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms, ushort species)
    {
        int generation = context.Generation();
        if (generation < 7)
            return EMPTY;

        return (Species)species switch
        {
            Meowth when generation >= 8 => new[] {
                types[000],
                forms[810], // Alolan
                forms[Galarian], // Alolan
            },

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
                or Marowak => new[] {
                    types[000],
                    forms[810], // Alolan
                },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsPikachu(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        int generation = context.Generation();
        return generation switch
        {
            6 => new[] {
                types[000], // Normal
                forms[729], // Rockstar
                forms[730], // Belle
                forms[731], // Pop
                forms[732], // PhD
                forms[733], // Libre
                forms[734], // Cosplay
            },
            7 when context is Gen7b => new[] {
                types[000], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
                Starter,
            },
            7 => new[] {
                types[000], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
            },
            8 => new[] {
                types[000], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
                Starter,
                forms[1085], // World
            },
            _ => EMPTY,
        };
    }

    private static string[] GetFormsPichu(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return new[]
        {
            types[000], // Normal
            forms[000], // Spiky
        };
    }

    private static string[] GetFormsArceus(ushort species, int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return generation switch
        {
            4 => new[] {
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
            },
            5 => new[] {
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
            },
            8 when (Species)species is Arceus => new[]
            {
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
            },
            _ => new[] {
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
            },
        };
    }

    private static string[] GetFormsTotem(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        if ((Species)species == Mimikyu) // Mimikyu
        {
            return new[]
            {
                forms[778], // Disguised
                forms[1058], // Busted
                forms[1007], // Large
                "*" + forms[1058], // Busted
            };
        }

        if (Legal.Totem_Alolan.Contains(species))
        {
            return new[]
            {
                types[0], // Normal
                forms[810], // Alolan
                forms[1007], // Large
            };
        }

        return new[]
        {
            types[0], // Normal
            forms[1007], // Large
        };
    }

    private static string[] GetFormsUnown(int generation) => generation switch
    {
        2 => new[]
        {
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T",
            "U", "V", "W", "X", "Y",
            "Z",
            // "!", "?", not in Gen II
        },
        _ => new[]
        {
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T",
            "U", "V", "W", "X", "Y",
            "Z",
            "!", "?",
        },
    };

    private static bool IsFormListSingleMega(ushort species) => Mega_6_Single.Contains(species);

    private static readonly HashSet<ushort> Mega_6_Single = new()
    {
        // XY
        003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359,
        380, 381, 445, 448, 460,

        // AO
        015, 018, 080, 208, 254, 260, 302, 319, 323, 334, 362, 373, 376, 384, 428, 475, 531, 719,
    };

    private static string[] GetMegaSingle(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return new[]
        {
            types[000], // Normal
            forms[804], // Mega
        };
    }

    private static string[] GetMegaXY(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return new[]
        {
            types[000], // Normal
            forms[805], // Mega X
            forms[806], // Mega Y
        };
    }

    private static string[] GetFormsGalar(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return new[]
        {
            types[000], // Normal
            forms[Galarian], // Galarian
        };
    }

    private static string[] GetFormsHisui(ushort species, int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms) => generation switch
    {
        8 => (Species)species switch
        {
            Lilligant => new[]
            {
                types[000], // Normal
                forms[Hisuian],
                forms[Lady],
            },
            Arcanine or Electrode or Avalugg => new[]
            {
                types[000], // Normal
                forms[Hisuian],
                forms[Lord],
            },
            _ => new[]
            {
                types[000], // Normal
                forms[Hisuian],
            },
        },
        _ => new[]
        {
            types[000], // Normal
            forms[Hisuian],
        },
    };

    private static string[] GetFormsGalarSlowbro(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return new[]
        {
            types[000], // Normal
            forms[804], // Mega
            forms[Galarian], // Galarian
        };
    }

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

    private const int Hero = 1093;
    private const int Crowned = 1083;
    private const int Eternamax = 1084;

    private const int SingleStrike = 1086;
    private const int RapidStrike = 1087;
    private const int Dada = 1088;
    private const int CalyIce = 1089; // Ice
    private const int CalyGhost = 1090; // Shadow

    private const int Hisuian = 1094;
    private const int Lord = 1095;
    private const int Lady = 1096;
    private const int Legend = 1097;

    public static string GetGigantamaxName(IReadOnlyList<string> forms) => forms[Gigantamax];

    public static string[] GetAlcremieFormList(IReadOnlyList<string> forms)
    {
        var result = new string[63];
        // seed form0 with the pattern
        result[0 * 7] = forms[(int) Alcremie]; // Vanilla Cream
        result[1 * 7] = forms[RubyCream];
        result[2 * 7] = forms[MatchaCream];
        result[3 * 7] = forms[MintCream];
        result[4 * 7] = forms[LemonCream];
        result[5 * 7] = forms[SaltedCream];
        result[6 * 7] = forms[RubySwirl];
        result[7 * 7] = forms[CaramelSwirl];
        result[8 * 7] = forms[RainbowSwirl];

        const int deco = 7;
        const int fc = 9;
        for (int f = 0; f < fc; f++)
        {
            int start = f * deco;
            // iterate downwards using form0 as pattern ref, replacing on final loop
            for (int i = deco - 1; i >= 0; i--)
            {
                result[start + i] = $"{result[start]} ({(AlcremieDecoration)i})";
            }
        }

        return result;
    }

    public static bool GetFormArgumentIsNamedIndex(ushort species) => species == (int)Alcremie;

    public static string[] GetFormArgumentStrings(ushort species) => species switch
    {
        (int)Alcremie => Enum.GetNames(typeof(AlcremieDecoration)),
        _ => EMPTY,
    };
}
