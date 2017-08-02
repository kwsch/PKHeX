using System.Collections.Generic;

namespace PKHeX.Core
{
    internal static class FormConverter
    {
        /// <summary>
        /// Gets a list of formes that the species can have.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon.</param>
        /// <param name="types">List of type names</param>
        /// <param name="forms">List of form names</param>
        /// <param name="genders">List of genders names</param>
        /// <param name="generation">Generation number for exclusive formes</param>
        /// <returns>A list of strings corresponding to the formes that a Pokémon can have.</returns>
        internal static string[] GetFormList(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, IReadOnlyList<string> genders, int generation)
        {
            // Mega List            
            if (IsFormListSingleMega(species))
                return new[]
                {
                    types[000], // Normal
                    forms[804], // Mega
                };

            if (species <= Legal.MaxSpeciesID_1)
                return GetFormsGen1(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_2)
                return GetFormsGen2(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_3)
                return GetFormsGen3(species, types, forms);
            if (species <= Legal.MaxSpeciesID_4)
                return GetFormsGen4(species, types, forms, generation);
            if (species <= Legal.MaxSpeciesID_5)
                return GetFormsGen5(species, types, forms);
            if (species <= Legal.MaxSpeciesID_6)
                return GetFormsGen6(species, types, forms, genders);
            //if (species <= Legal.MaxSpeciesID_7)
                return GetFormsGen7(species, types, forms);
        }

        private static string[] GetFormsGen1(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch (species)
            {
                case 6:
                case 150:
                    return new[]
                    {
                        types[000], // Normal
                        forms[805], // Mega X
                        forms[806], // Mega Y
                    };
                case 025:
                    return GetFormsPikachu(generation, types, forms);

                default:
                    return GetFormsAlolan(generation, types, forms, species);
            }
        }
        private static string[] GetFormsGen2(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms, int generation)
        {
            switch (species)
            {
                default:
                    return new[] { "" };

                case 172:
                    return GetFormsPichu(generation, types, forms);
                case 201:
                    return GetFormsUnown(generation);
            }
        }
        private static string[] GetFormsGen3(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            switch (species)
            {
                default:
                    return new[] { "" };

                case 351: // Casftorm
                    return new[]
                    {
                        types[000], // Normal
                        forms[889], // Sunny
                        forms[890], // Rainy
                        forms[891], // Snowy
                    };
                case 382: // Kyogre
                case 383: // Groudon
                    return new[]
                    {
                        types[000], // Normal
                        forms[899], // Primal
                    };
                case 386: // Deoxys
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
            switch (species)
            {
                default:
                    return new[] { "" };

                case 412:
                case 413:
                case 414:
                    return new[]
                    {
                        forms[412], // Plant
                        forms[905], // Sandy
                        forms[906], // Trash
                    };

                case 421:
                    return new[]
                    {
                        forms[421], // Overcast
                        forms[909], // Sunshine
                    };

                case 422:
                case 423:
                    return new[]
                    {
                        forms[422], // West
                        forms[911], // East
                    };

                case 479:
                    return new[]
                    {
                        types[000], // Normal
                        forms[917], // Heat
                        forms[918], // Wash
                        forms[919], // Frost
                        forms[920], // Fan
                        forms[921], // Mow
                    };

                case 487:
                    return new[]
                    {
                        forms[487], // Altered
                        forms[922], // Origin
                    };

                case 492:
                    return new[]
                    {
                        forms[492], // Land
                        forms[923], // Sky
                    };

                case 493: // Arceus
                case 773: // Silvally
                    return GetFormsArceus(generation, types);
            }
        }
        private static string[] GetFormsGen5(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            switch (species)
            {
                default:
                    return new[] { "" };

                case 550:
                    return new[]
                    {
                        forms[550], // Red
                        forms[942], // Blue
                    };

                case 555:
                    return new[]
                    {
                        forms[555], // Standard
                        forms[943], // Zen
                    };

                case 585:
                case 586:
                    return new[]
                    {
                        forms[585], // Spring
                        forms[947], // Summer
                        forms[948], // Autumn
                        forms[949], // Winter
                    };

                case 641:
                case 642:
                case 645:
                    return new[]
                    {
                        forms[641], // Incarnate
                        forms[952], // Therian
                    };

                case 646:
                    return new[]
                    {
                        types[000], // Normal
                        forms[953], // White
                        forms[954], // Black
                    };

                case 647:
                    return new[]
                    {
                        forms[647], // Ordinary
                        forms[955], // Resolute
                    };

                case 648:
                    return new[]
                    {
                        forms[648], // Aria
                        forms[956], // Pirouette
                    };

                case 649:
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
            switch (species)
            {
                default:
                    return new[] { "" };

                case 658:
                    return new[]
                    {
                        types[000], // Normal
                        forms[962], // "Ash",
                        forms[1012], // "Bonded" - Active
                    };

                case 664:
                case 665:
                case 666:
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

                case 669:
                case 671:
                    return new[]
                    {
                        forms[669], // Red
                        forms[986], // Yellow
                        forms[987], // Orange
                        forms[988], // Blue
                        forms[989], // White
                    };

                case 670:
                    return new[]
                    {
                        forms[669], // Red
                        forms[986], // Yellow
                        forms[987], // Orange
                        forms[988], // Blue
                        forms[989], // White
                        forms[990], // Eternal
                    };

                case 676:
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

                case 678:
                    return new[]
                    {
                        genders[000], // Male
                        genders[001], // Female
                    };

                case 681:
                    return new[]
                    {
                        forms[681], // Shield
                        forms[1005], // Blade
                    };

                case 710:
                case 711:
                    return new[]
                    {
                        forms[710], // Average
                        forms[1006], // Small
                        forms[1007], // Large
                        forms[1008], // Super
                    };

                case 716:
                    return new[]
                    {
                        forms[716], // Neutral
                        forms[1012], // Active
                    };

                case 720:
                    return new[]
                    {
                        forms[720], // Confined
                        forms[1018], // Unbound
                    };

                case 718: // Zygarde
                    return new[]
                    {
                        forms[718], // 50% (Aura Break)
                        "10%", // (Aura Break)
                        "10%-C", // Cell (Power Construct)
                        "50%-C", // Cell (Power Construct)
                        "100%-C" // Cell (Power Construct)
                    };
            }
        }
        private static string[] GetFormsGen7(int species, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            switch (species)
            {
                default:
                    return new[] { "" };
                
                case 741: // Oricorio
                    return new[]
                    {
                        forms[741], // "RED" - Baile
                        forms[1021], // "YLW" - Pom-Pom
                        forms[1022], // "PNK" - Pa'u
                        forms[1023], // "BLU" - Sensu
                    };

                case 745: // Lycanroc
                    return new[]
                    {
                        forms[745], // Midday
                        forms[1024], // Midnight
                    };

                case 746: // Wishiwashi
                    return new[]
                    {
                        forms[746],
                        forms[1025], // School
                    };

                case 773: // Silvally
                    return GetFormsArceus(7, types);

                case 774: // Minior
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

                case 778: // Mimikyu
                    return new[]
                    {
                        forms[778], // Disguised
                        forms[1058], // Busted
                    };

                case 801: // Magearna
                    return new[]
                    {
                        types[000],
                        forms[1062], // Original
                    };
            }
        }

        private static string[] GetFormsAlolan (int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms, int species)
        {
            if (generation < 7)
                return new[] { "" };

            switch (species)
            {
                default:
                    return new[] { "" };

                case 19: // Rattata
                case 20: // Raticate
                case 26: // Raichu
                case 27: // Sandshrew
                case 28: // Sandslash
                case 37: // Vulpix
                case 38: // Ninetails
                case 50: // Diglett
                case 51: // Dugtrio
                case 52: // Meowth
                case 53: // Persian
                case 74: // Geodude
                case 75: // Graveler
                case 76: // Golem
                case 88: // Grimer
                case 89: // Muk
                case 105: // Marowak
                case 103: // Exeggutor
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
                    return new[] { "" };

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
                case 7:
                    return new[]
                    {
                        types[000], // Normal
                        forms[813], // Original
                        forms[814], // Hoenn
                        forms[815], // Sinnoh
                        forms[816], // Unova
                        forms[817], // Kalos
                        forms[818], // Alola
                    };
            }
        }
        private static string[] GetFormsPichu  (int generation, IReadOnlyList<string> types, IReadOnlyList<string> forms)
        {
            if (generation == 4)
                return new[]
                {
                    types[000], // Normal
                    forms[000], // Spiky
                };
            return new[] { "" };
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
        private static string[] GetFormsUnown(int generation)
        {
            switch (generation)
            {
                case 2:
                    return new[]
                    {
                        "A", "B", "C", "D", "E",
                        "F", "G", "H", "I", "J",
                        "K", "L", "M", "N", "O",
                        "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y",
                        "Z",
                        // "!", "?", not in Gen II
                    };
                default:
                    return new[]
                    {
                        "A", "B", "C", "D", "E",
                        "F", "G", "H", "I", "J",
                        "K", "L", "M", "N", "O",
                        "P", "Q", "R", "S", "T",
                        "U", "V", "W", "X", "Y",
                        "Z",
                        "!", "?",
                    };
            }
        }

        private static bool IsFormListSingleMega(int species) => Mega_6_Single.Contains(species);
        private static readonly HashSet<int> Mega_6_Single = new HashSet<int>
        {
            // XY
            003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359,
            380, 381, 445, 448, 460,

            // AO
            003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359,
            380, 381, 445, 448, 460,
        };
    }
}
