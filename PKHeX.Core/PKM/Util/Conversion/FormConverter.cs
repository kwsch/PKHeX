using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        byte generation = context.Generation();

        // Mega List
        if (context.IsMegaGeneration() && IsFormListSingleMega(species))
            return GetMegaSingle(types, forms);

        if (context is Gen7 && FormInfo.HasTotemForm(species))
            return GetFormsTotem(species, types, forms);

        return species switch
        {
            <= Legal.MaxSpeciesID_1 => GetFormsGen1(species, types, forms, context),
            <= Legal.MaxSpeciesID_2 => GetFormsGen2(species, types, forms, context),
            <= Legal.MaxSpeciesID_3 => GetFormsGen3(species, types, forms, generation),
            <= Legal.MaxSpeciesID_4 => GetFormsGen4(species, types, forms, generation),
            <= Legal.MaxSpeciesID_5 => GetFormsGen5(species, types, forms, generation),
            <= Legal.MaxSpeciesID_6 => GetFormsGen6(species, types, forms, genders, generation),
            <= Legal.MaxSpeciesID_7_USUM => GetFormsGen7(species, types, forms, generation),
            <= Legal.MaxSpeciesID_8a => GetFormsGen8(species, generation, types, forms, genders),
            _ => GetFormsGen9(species, generation, types, forms, genders),
        };
    }

    private static bool IsMegaGeneration(this EntityContext context) => context is Gen6 or Gen7 or Gen7b;

    private static readonly string[] EMPTY = [string.Empty];
    private const string Starter = nameof(Starter);

    private static string[] GetFormsGen1(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        byte generation = context.Generation();
        return (Species)species switch
        {
            Charizard or Mewtwo when context.IsMegaGeneration() => GetMegaXY(types, forms),
            Eevee when context is Gen7b =>
            [
                types[0], // Normal
                Starter,
            ],
            Pikachu => GetFormsPikachu(context, types, forms),
            Slowbro when generation >= 8 => GetFormsGalarSlowbro(types, forms),

            Weezing or Ponyta or Rapidash or Slowpoke or MrMime or Farfetchd or Articuno or Zapdos or Moltres when generation >= 8 => GetFormsGalar(types, forms),
            Growlithe or Arcanine or Voltorb or Electrode when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Tauros when generation >= 9 => GetFormsPaldea(species, types, forms),

            _ => GetFormsAlolan(context, types, forms, species),
        };
    }

    private static string[] GetFormsGen2(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, EntityContext context)
    {
        byte generation = context.Generation();
        return (Species)species switch
        {
            Pichu when context is Gen4 => GetFormsPichu(types, forms),
            Slowking or Corsola when generation >= 8 => GetFormsGalar(types, forms),
            Typhlosion or Qwilfish or Sneasel when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Wooper when generation >= 9 => GetFormsPaldea(species, types, forms),
            Unown => GetFormsUnown(generation),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen3(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, byte generation)
    {
        return (Species)species switch
        {
            Zigzagoon or Linoone when generation >= 8 => GetFormsGalar(types, forms),
            Castform => [
                types[0], // Normal
                forms[889], // Sunny
                forms[890], // Rainy
                forms[891], // Snowy
            ],
            Kyogre or Groudon when generation < 8 => [
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

    private static string[] GetFormsGen4(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, byte generation)
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
            Rotom => [
                types[0], // Normal
                forms[917], // Heat
                forms[918], // Wash
                forms[919], // Frost
                forms[920], // Fan
                forms[921], // Mow
            ],
            Dialga or Palkia when generation >= 8 => [
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
            Arceus => GetFormsArceus(species, generation, types, forms),
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen5(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, byte generation)
    {
        return (Species)species switch
        {
            Samurott or Lilligant or Zorua or Zoroark or Braviary when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Basculin when generation >= 8 => [
                forms[(int)Basculin], // Red
                forms[942], // Blue
                forms[989], // White
            ],
            Basculin => [
                forms[(int)Basculin], // Red
                forms[942], // Blue
            ],
            Darumaka or Stunfisk or Yamask when generation >= 8 => GetFormsGalar(types, forms),
            Darmanitan when generation >= 8 => [
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

    private static string[] GetFormsGen6(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, byte generation)
    {
        return (Species)species switch
        {
            Greninja when generation < 9 => [
                types[0], // Normal
                forms[962], // Ash
                forms[1012], // "Bonded" - Active
            ],
            Greninja => [
                types[0], // Normal
                forms[962], // Ash
            ],
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
            Floette when generation < 9 => [
                forms[(int)Floette], // Red
                forms[986], // Yellow
                forms[987], // Orange
                forms[988], // Blue
                forms[989], // White
                forms[990], // Eternal
            ],
            Flabébé or Floette or Florges => [
                forms[(int)Flabébé], // Red
                forms[986], // Yellow
                forms[987], // Orange
                forms[988], // Blue
                forms[989], // White
            ],
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
            Meowstic => [
                genders[000], // Male
                genders[001], // Female
            ],
            Aegislash => [
                forms[(int)Aegislash], // Shield
                forms[1005], // Blade
            ],
            Sliggoo or Goodra or Avalugg when generation >= 8 => GetFormsHisui(species, generation, types, forms),
            Pumpkaboo or Gourgeist => [
                forms[(int)Pumpkaboo], // Average
                forms[1006], // Small
                forms[1007], // Large
                forms[1008], // Super
            ],
            Xerneas => [
                forms[(int)Xerneas], // Neutral
                forms[1012], // Active
            ],
            Hoopa => [
                forms[(int)Hoopa], // Confined
                forms[1018], // Unbound
            ],
            Zygarde => [
                forms[(int)Zygarde], // 50% (Aura Break)
                forms[1013], // 10% (Aura Break)
                forms[1014] + "-C", // 10% Cell (Power Construct)
                forms[1015] + "-C", // 50% Cell (Power Construct)
                forms[1016], // 100% Cell (Power Construct)
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen7(ushort species, IReadOnlyList<string> types, IReadOnlyList<string> forms, byte generation)
    {
        return (Species)species switch
        {
            Decidueye when generation >= 8 => GetFormsHisui(species, generation, types, forms),
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
            Silvally => GetFormsArceus(species, 7, types, forms),
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
            Necrozma when generation == 7 => [
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
            Magearna => [
                types[0],
                forms[1062], // Original
            ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsGen8(ushort species, byte generation, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
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
            Indeedee or Basculegion => [
                genders[000], // Male
                genders[001], // Female
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
            Morpeko => [
                forms[FullBellyMode],
                forms[HangryMode],
            ],
            Eiscue => [
                forms[IceFace],
                forms[NoiceFace],
            ],
            Zacian or Zamazenta => [
                forms[HeroOfManyBattles],
                forms[Crowned],
            ],
            Eternatus when generation == 8 => [
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
                forms[CalyIce],
                forms[CalyGhost],
            ],
            Kleavor when generation == 8 => [
                types[0],
                forms[Lord],
            ],
            Ursaluna when generation >= 9 => [
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

    private static string[] GetFormsGen9(ushort species, byte generation, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
    {
        return (Species)species switch
        {
            Oinkologne => [
                genders[000], // Male
                genders[001], // Female
            ],
            Dudunsparce => [
                forms[TwoSegment],
                forms[ThreeSegment],
            ],
            Palafin => [
                forms[Zero],
                forms[HeroPalafin],
            ],
            Maushold => [
                forms[FamilyOfThree],
                forms[FamilyOfFour],
            ],
            Tatsugiri => [
                forms[Curly],
                forms[Droopy],
                forms[Stretchy],
            ],
            Squawkabilly => [
                forms[Green],
                forms[988], // Blue
                forms[986], // Yellow
                forms[989], // White
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
            Poltchageist => [
                forms[Counterfeit],
                forms[Artisan],
            ],
            Sinistcha => [
                forms[Unremarkable],
                forms[Masterpiece],
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
        byte generation = context.Generation();
        if (generation < 7)
            return EMPTY;

        return (Species)species switch
        {
            Meowth when generation >= 8 => [
                types[0],
                forms[810], // Alolan
                forms[Galarian], // Galarian
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
                    forms[810], // Alolan
                ],
            _ => EMPTY,
        };
    }

    private static string[] GetFormsPikachu(EntityContext context, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        byte generation = context.Generation();
        return generation switch
        {
            6 => [
                types[0], // Normal
                forms[729], // Rockstar
                forms[730], // Belle
                forms[731], // Pop
                forms[732], // PhD
                forms[733], // Libre
                forms[734], // Cosplay
            ],
            7 when context is Gen7b => [
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
            7 => [
                types[0], // Normal
                forms[813], // Original
                forms[814], // Hoenn
                forms[815], // Sinnoh
                forms[816], // Unova
                forms[817], // Kalos
                forms[818], // Alola
                forms[1063], // Partner
            ],
            8 or 9 => [
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
            _ => EMPTY,
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

    private static string[] GetFormsArceus(ushort species, byte generation, IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return generation switch
        {
            4 => [
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
            5 => [
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
            8 when (Species)species is Arceus =>
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
            forms[810], // Alolan
            forms[1007], // Large
        ],
        _ =>
        [
            types[0], // Normal
            forms[1007], // Large
        ],
    };

    private static string[] GetFormsUnown(byte generation) => generation switch
    {
        2 =>
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

    private static bool IsFormListSingleMega(ushort species) => species is
        // XY
        003 or 009 or 065 or 094 or 115 or 127 or 130 or 142 or 181 or 212 or
        214 or 229 or 248 or 257 or 282 or 303 or 306 or 308 or 310 or 354 or
        359 or 380 or 381 or 445 or 448 or 460 or

        // AO
        015 or 018 or 080 or 208 or 254 or 260 or 302 or 319 or 323 or 334 or
        362 or 373 or 376 or 384 or 428 or 475 or 531 or 719
    ;

    private static string[] GetMegaSingle(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[804], // Mega
        ];
    }

    private static string[] GetMegaXY(IReadOnlyList<string> types, IReadOnlyList<string> forms)
    {
        return
        [
            types[0], // Normal
            forms[805], // Mega X
            forms[806], // Mega Y
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

    private static string[] GetFormsHisui(ushort species, byte generation, IReadOnlyList<string> types, IReadOnlyList<string> forms) => generation switch
    {
        8 => (Species)species switch
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
            forms[804], // Mega
            forms[Galarian], // Galarian
        ];
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
    private const int Phony = 1098;
    private const int Antique = 1099;

    private const int HeroOfManyBattles = 1093;
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

        static void SetDecorations(Span<string> result, [ConstantExpected] byte f, string baseName)
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
}
