using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for various <see cref="PKM.AltForm"/> related requests.
    /// </summary>
    public static class FormConverter
    {
        /// <summary>
        /// Gets a list of formes that the species can have.
        /// </summary>
        /// <param name="species"><see cref="Species"/> of the Pokémon.</param>
        /// <param name="types">List of type names</param>
        /// <param name="forms">List of form names</param>
        /// <param name="genders">List of genders names</param>
        /// <param name="generation">Generation number for exclusive formes</param>
        /// <returns>A list of strings corresponding to the formes that a Pokémon can have.</returns>
        public static string[] GetFormList(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, int generation)
        {
            // Mega List
            if (generation < 8 && IsFormListSingleMega(species))
                return GetMegaSingle(types, forms);

            if (generation == 7 && Legal.Totem_USUM.Contains(species))
                return GetFormsTotem(species, types, forms);

            if (species <= Legal.MaxSpeciesID_1)
                return GetFormsGen1(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_2)
                return GetFormsGen2(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_3)
                return GetFormsGen3(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_4)
                return GetFormsGen4(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_5)
                return GetFormsGen5(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_6)
                return GetFormsGen6(species, types, forms, genders);
            if (species <= Legal.MaxSpeciesID_7_USUM)
                return GetFormsGen7(species, types, forms);
            return GetFormsGen8(species, types, forms, genders);
        }

        // this is a hack; depends on currently loaded SaveFile's Game ID
        private static bool IsGG() => GameVersion.GG.Contains(PKMConverter.Game);

        public static bool IsTotemForm(int species, int form, int generation)
        {
            if (generation != 7)
                return false;
            if (form == 0)
                return false;
            if (!Legal.Totem_USUM.Contains(species))
                return false;
            if (species == (int)Mimikyu)
                return form == 2 || form == 3;
            if (Legal.Totem_Alolan.Contains(species))
                return form == 2;
            return form == 1;
        }

        public static int GetTotemBaseForm(int species, int form)
        {
            if (species == (int)Mimikyu)
                return form - 2;
            return form - 1;
        }

        public static bool IsValidOutOfBoundsForme(int species, int form, int generation)
        {
            switch ((Species)species)
            {
                case Unown:
                    return form < (generation == 2 ? 26 : 28); // A-Z : A-Z?!
                case Mothim: // Burmy base form is kept
                    return form < 3;
                case Scatterbug:
                case Spewpa: // Vivillon Pre-evolutions
                    return form < 18;
                default:
                    return false;
            }
        }

        private static readonly string[] EMPTY = { string.Empty };
        private const string Starter = nameof(Starter);

        private static string[] GetFormsGen1(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch ((Species)species)
            {
                case Charizard when generation < 8:
                case Mewtwo when generation < 8:
                    return GetMegaXY(types, forms);

                case Eevee when IsGG():
                    return new[]
                    {
                        types[000], // Normal
                        Starter,
                    };

                case Pikachu:
                    return GetFormsPikachu(generation, types, forms);

                case Weezing when generation >= 8:
                case Ponyta when generation >= 8:
                case Rapidash when generation >= 8:
                case Slowpoke when generation >= 8:
                case MrMime when generation >= 8:
                case Farfetchd when generation >= 8:
                    return GetFormsGalar(types, forms);

                default:
                    return GetFormsAlolan(generation, types, forms, species);
            }
        }

        private static string[] GetFormsGen2(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            return species switch
            {
                (int)Pichu when generation == 4 => GetFormsPichu(types, forms),
                (int)Unown => GetFormsUnown(generation),
                (int)Corsola when generation >= 8 => GetFormsGalar(types, forms),
                _ => EMPTY
            };
        }

        private static string[] GetFormsGen3(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Zigzagoon when generation >= 8:
                case Linoone when generation >= 8:
                    return GetFormsGalar(types, forms);

                case Castform: // Casftorm
                    return new[]
                    {
                        types[000], // Normal
                        forms[889], // Sunny
                        forms[890], // Rainy
                        forms[891], // Snowy
                    };
                case Kyogre: // Kyogre
                case Groudon: // Groudon
                    return new[]
                    {
                        types[000], // Normal
                        forms[899], // Primal
                    };
                case Deoxys: // Deoxys
                    return new[]
                    {
                        types[000], // Normal
                        forms[902], // Attack
                        forms[903], // Defense
                        forms[904], // Speed
                    };
            }
        }

        private static string[] GetFormsGen4(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Burmy:
                case Wormadam:
                case Mothim:
                    return new[]
                    {
                        forms[412], // Plant
                        forms[905], // Sandy
                        forms[906], // Trash
                    };

                case Cherrim:
                    return new[]
                    {
                        forms[421], // Overcast
                        forms[909], // Sunshine
                    };

                case Shellos:
                case Gastrodon:
                    return new[]
                    {
                        forms[422], // West
                        forms[911], // East
                    };

                case Rotom:
                    return new[]
                    {
                        types[000], // Normal
                        forms[917], // Heat
                        forms[918], // Wash
                        forms[919], // Frost
                        forms[920], // Fan
                        forms[921], // Mow
                    };

                case Giratina:
                    return new[]
                    {
                        forms[487], // Altered
                        forms[922], // Origin
                    };

                case Shaymin:
                    return new[]
                    {
                        forms[492], // Land
                        forms[923], // Sky
                    };

                case Arceus:
                case Silvally:
                    return GetFormsArceus(generation, types);
            }
        }

        private static string[] GetFormsGen5(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Basculin:
                    return new[]
                    {
                        forms[550], // Red
                        forms[942], // Blue
                    };

                case Darumaka when generation >= 8:
                    return GetFormsGalar(types, forms);

                case Darmanitan:
                {
                    if (generation <= 7)
                    {
                        return new[]
                        {
                            forms[555], // Standard
                            forms[943], // Zen
                        };
                    }
                    return new[]
                    {
                        forms[555], // Standard
                        forms[943], // Zen
                        forms[Galarian], // Standard
                        forms[Galarian] + " " + forms[943], // Zen
                    };
                }

                case Yamask when generation >= 8:
                    return GetFormsGalar(types, forms);

                case Deerling:
                case Sawsbuck:
                    return new[]
                    {
                        forms[585], // Spring
                        forms[947], // Summer
                        forms[948], // Autumn
                        forms[949], // Winter
                    };

                case Stunfisk when generation >= 8:
                    return GetFormsGalar(types, forms);

                case Tornadus:
                case Thundurus:
                case Landorus:
                    return new[]
                    {
                        forms[641], // Incarnate
                        forms[952], // Therian
                    };

                case Kyurem:
                    return new[]
                    {
                        types[000], // Normal
                        forms[953], // White
                        forms[954], // Black
                    };

                case Keldeo:
                    return new[]
                    {
                        forms[647], // Ordinary
                        forms[955], // Resolute
                    };

                case Meloetta:
                    return new[]
                    {
                        forms[648], // Aria
                        forms[956], // Pirouette
                    };

                case Genesect:
                    return new[]
                    {
                        types[000], // Normal
                        types[010], // Douse (Water)
                        types[012], // Shock (Electric)
                        types[009], // Burn (Fire)
                        types[014], // Chill (Ice)
                    };
            }
        }

        private static string[] GetFormsGen6(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Greninja:
                    return new[]
                    {
                        types[000], // Normal
                        forms[962], // "Ash",
                        forms[1012], // "Bonded" - Active
                    };

                case Scatterbug:
                case Spewpa:
                case Vivillon:
                    return new[]
                    {
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
                        forms[976], // Savannah
                        forms[977], // Sun
                        forms[978], // Ocean
                        forms[979], // Jungle
                        forms[980], // Fancy
                        forms[981], // Poké Ball
                    };

                case Flabébé:
                case Florges:
                    return new[]
                    {
                        forms[669], // Red
                        forms[986], // Yellow
                        forms[987], // Orange
                        forms[988], // Blue
                        forms[989], // White
                    };

                case Floette:
                    return new[]
                    {
                        forms[669], // Red
                        forms[986], // Yellow
                        forms[987], // Orange
                        forms[988], // Blue
                        forms[989], // White
                        forms[990], // Eternal
                    };

                case Furfrou:
                    return new[]
                    {
                        forms[676], // Natural
                        forms[995], // Heart
                        forms[996], // Star
                        forms[997], // Diamond
                        forms[998], // Deputante
                        forms[999], // Matron
                        forms[1000], // Dandy
                        forms[1001], // La Reine
                        forms[1002], // Kabuki
                        forms[1003], // Pharaoh
                    };

                case Meowstic:
                    return new[]
                    {
                        genders[000], // Male
                        genders[001], // Female
                    };

                case Aegislash:
                    return new[]
                    {
                        forms[681], // Shield
                        forms[1005], // Blade
                    };

                case Pumpkaboo:
                case Gourgeist:
                    return new[]
                    {
                        forms[710], // Average
                        forms[1006], // Small
                        forms[1007], // Large
                        forms[1008], // Super
                    };

                case Xerneas:
                    return new[]
                    {
                        forms[716], // Neutral
                        forms[1012], // Active
                    };

                case Hoopa:
                    return new[]
                    {
                        forms[720], // Confined
                        forms[1018], // Unbound
                    };

                case Zygarde:
                    return new[]
                    {
                        forms[718], // 50% (Aura Break)
                        forms[1013], // 10% (Aura Break)
                        forms[1014] + "-C", // 10% Cell (Power Construct)
                        forms[1015] + "-C", // 50% Cell (Power Construct)
                        forms[1016], // 100% Cell (Power Construct)
                    };
            }
        }

        private static string[] GetFormsGen7(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Oricorio:
                    return new[]
                    {
                        forms[741], // "RED" - Baile
                        forms[1021], // "YLW" - Pom-Pom
                        forms[1022], // "PNK" - Pa'u
                        forms[1023], // "BLU" - Sensu
                    };

                case Rockruff:
                    return new[]
                    {
                        types[0], // Normal
                        forms[1064], // Dusk
                    };
                case Lycanroc:
                    return new[]
                    {
                        forms[745], // Midday
                        forms[1024], // Midnight
                        forms[1064], // Dusk
                    };

                case Wishiwashi:
                    return new[]
                    {
                        forms[746],
                        forms[1025], // School
                    };

                case Silvally:
                    return GetFormsArceus(7, types);

                case Minior:
                    return new[]
                    {
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
                    };

                case Mimikyu:
                    return new[]
                    {
                        forms[778], // Disguised
                        forms[1058], // Busted
                    };

                case Necrozma:
                    return new[]
                    {
                        types[000], // Normal
                        forms[1065], // Dusk Mane
                        forms[1066], // Dawn Wings
                        forms[1067], // Ultra Necrozma
                    };

                case Magearna:
                    return new[]
                    {
                        types[000],
                        forms[1062], // Original
                    };
            }
        }

        private static string[] GetFormsGen8(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders)
        {
            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Cramorant:
                    return new[]
                    {
                        types[0], // Normal
                        forms[Gulping],
                        forms[Gorging],
                    };

                case Toxtricity:
                    return new[]
                    {
                        forms[(int)Toxtricity], // Amped
                        forms[LowKey],
                    };

                case Indeedee:
                    return new[]
                    {
                        genders[000], // Male
                        genders[001], // Female
                    };

                case Sinistea:
                case Polteageist:
                    return new[]
                    {
                        "Phony",
                        "Antique",
                    };

                case Alcremie:
                    return new[]
                    {
                        forms[(int)Alcremie], // Vanilla Cream
                        forms[RubyCream],
                        forms[MatchaCream],
                        forms[MintCream],
                        forms[LemonCream],
                        forms[SaltedCream],
                        forms[RubySwirl],
                        forms[CaramelSwirl],
                        forms[RainbowSwirl],
                    };

                case Morpeko:
                    return new[]
                    {
                        types[0], // Normal
                        forms[HangryMode],
                    };

                case Eiscue:
                    return new[]
                    {
                        types[0], // Normal
                        forms[NoiceFace],
                    };

                case Zacian:
                case Zamazenta:
                    return new[]
                    {
                        types[0], // Normal
                        forms[Crowned],
                    };

                case Eternatus:
                    return new[]
                    {
                        types[0], // Normal
                        forms[Eternamax],
                    };
            }
        }

        private static string[] GetFormsAlolan (int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms, int species)
        {
            if (generation < 7)
                return EMPTY;

            switch ((Species)species)
            {
                default:
                    return EMPTY;

                case Meowth when generation >= 8:
                    return new[]
                    {
                        types[000],
                        forms[810], // Alolan
                        forms[Galarian], // Alolan
                    };

                case Rattata:
                case Raichu:
                case Sandshrew:
                case Sandslash:
                case Vulpix:
                case Ninetales:
                case Diglett:
                case Dugtrio:
                case Meowth:
                case Persian:
                case Geodude:
                case Graveler:
                case Golem:
                case Grimer:
                case Muk:
                case Exeggutor:
                    return new[]
                    {
                        types[000],
                        forms[810] // Alolan
                    };
            }
        }

        private static string[] GetFormsPikachu(int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            switch (generation)
            {
                default:
                    return EMPTY;

                case 6:
                    return new[]
                    {
                        types[000], // Normal
                        forms[729], // Rockstar
                        forms[730], // Belle
                        forms[731], // Pop
                        forms[732], // PhD
                        forms[733], // Libre
                        forms[734], // Cosplay
                    };

                case 7 when IsGG():
                    return new[]
                    {
                        types[000], // Normal
                        forms[813], // Original
                        forms[814], // Hoenn
                        forms[815], // Sinnoh
                        forms[816], // Unova
                        forms[817], // Kalos
                        forms[818], // Alola
                        forms[1063], // Partner
                        Starter,
                    };

                case 7:
                case 8:
                    return new[]
                    {
                        types[000], // Normal
                        forms[813], // Original
                        forms[814], // Hoenn
                        forms[815], // Sinnoh
                        forms[816], // Unova
                        forms[817], // Kalos
                        forms[818], // Alola
                        forms[1063], // Partner
                    };
            }
        }

        private static string[] GetFormsPichu  (IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            return new[]
            {
                types[000], // Normal
                forms[000], // Spiky
            };
        }

        private static string[] GetFormsArceus (int generation, IReadOnlyList<string> types)
        {
            switch (generation)
            {
                case 4:
                    return new[]
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
                        "???", // ???-type arceus
                        types[09],
                        types[10],
                        types[11],
                        types[12],
                        types[13],
                        types[14],
                        types[15],
                        types[16] // No Fairy Type
                    };
                case 5:
                    return new[]
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
                        types[16] // No Fairy type
                    };
                default:
                    return new[]
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
                    };
            }
        }

        private static string[] GetFormsTotem  (int species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
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

        private static string[] GetFormsUnown(int generation)
        {
            return generation switch
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
                }
            };
        }

        private static bool IsFormListSingleMega(int species) => Mega_6_Single.Contains(species);

        private static readonly HashSet<int> Mega_6_Single = new HashSet<int>
        {
            // XY
            003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359,
            380, 381, 445, 448, 460,

            // AO
            015, 018, 080, 208, 254, 260, 302, 319, 323, 334, 362, 373, 376, 384, 428, 475, 531, 719,
        };

        /// <summary>
        /// Checks if the <see cref="PKM"/> data should have a drop-down selection visible for the <see cref="PKM.AltForm"/> value.
        /// </summary>
        /// <param name="pi">Game specific personal info</param>
        /// <param name="species"><see cref="Species"/> ID</param>
        /// <param name="format"><see cref="PKM.AltForm"/> ID</param>
        /// <returns>True if has formes that can be provided by <see cref="GetFormList"/>, otherwise false for none.</returns>
        public static bool HasFormSelection(PersonalInfo pi, int species, int format)
        {
            if (format <= 3 && species != (int)Unown)
                return false;

            if (HasFormeValuesNotIndicatedByPersonal.Contains(species))
                return true;

            int count = pi.FormeCount;
            return count > 1;
        }

        private static readonly HashSet<int> HasFormeValuesNotIndicatedByPersonal = new HashSet<int>
        {
            (int)Unown,
            (int)Mothim, // (Burmy forme carried over, not cleared)
            (int)Scatterbug, (int)Spewpa, // Vivillon pre-evos
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

        private const int Galarian = 1068;
        // private const int Gigantamax = 1069;
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

        private const int NoiceFace = 1081;
        private const int HangryMode = 1082;

        private const int Crowned = 1083;
        private const int Eternamax = 1084;

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
                    result[start + i] = $"{result[start]} ({((AlcremieDecoration)i).ToString()})";
                }
            }

            return result;
        }

        public static string[] GetFormArgumentStrings(int species, int form, int generation)
        {
            if (generation <= 5)
                return EMPTY;

            static string[] GetBlank(int count)
            {
                var result = new string[count];
                for (int i = 0; i < result.Length; i++)
                    result[i] = i.ToString();
                return result;
            }

            return species switch
            {
                (int) Furfrou when form != 0 => new[] {"0", "1", "2", "3", "4", "5"},
                (int) Hoopa when form == 1 => new[] {"0", "1", "2", "3"},
                (int) Yamask when form == 1 => GetBlank(320),
                (int) Runerigus when form == 0 => GetBlank(320), // max Runerigus HP
                (int) Alcremie => Enum.GetNames(typeof(AlcremieDecoration)),
                _ => EMPTY
            };
        }
    }
}
