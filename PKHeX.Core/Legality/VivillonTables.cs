using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        private struct CountryTable
        {
            public byte countryID;
            public byte mainform;
            public FormSubregionTable[] otherforms;
        }

        private struct FormSubregionTable
        {
            public byte form;
            public int[] region;
        }

        private static readonly int[][] VivillonCountryTable =
        {
               //missing ID 051,068,102,127,160,186
               /* 0 Icy Snow    */ new[] { 018, 076, 096, 100, 107 },
               /* 1 Polar       */ new[] { 010, 018, 020, 049, 076, 096, 100, 107 },
               /* 2 Tundra      */ new[] { 001, 081, 096, },
               /* 3 Continental */ new[] { 010, 067, 073, 074, 075, 077, 078, 084, 087, 094, 096, 097, 100, 107, 136},
               /* 4 Garden      */ new[] { 065, 082, 095, 097, 101, 110, 125},
               /* 5 Elegant     */ new[] { 001 },
               /* 6 Meadow      */ new[] { 066, 077, 078, 083, 086, 088, 105, 108, 122},
               /* 7 Modern      */ new[] { 018, 049},
               /* 8 Marine      */ new[] { 020, 064, 066, 070, 071, 073, 077, 078, 079, 080, 083, 089, 090, 091, 098, 099, 103, 105, 123, 124, 126, 184, 185},
               /* 9 Archipelago */ new[] { 008, 009, 011, 012, 013, 017, 021, 023, 024, 028, 029, 032, 034, 035, 036, 037, 038, 043, 044, 045, 047, 048, 049, 052, 085, 104,},
               /*10 High Plains */ new[] { 018, 036, 049, 100, 113},
               /*11 Sandstorm   */ new[] { 072, 109, 118, 119, 120, 121, 168, 174},
               /*12 River       */ new[] { 065, 069, 085, 093, 104, 105, 114, 115, 116, 117},
               /*13 Monsoon     */ new[] { 001, 128, 144, 169},
               /*14-Savanna     */ new[] { 010, 015, 016, 041, 042, 050},
               /*15 Sun         */ new[] { 036, 014, 019, 026, 030, 033, 036, 039, 065, 092, 106, 111, 112},
               /*16 Ocean       */ new[] { 049, 077},
               /*17 Jungle      */ new[] { 016, 021, 022, 025, 027, 031, 040, 046, 052, 169, 153, 156},
        };

        private static readonly CountryTable[] RegionFormTable =
        {
            new CountryTable{
                countryID = 001, // Japan
                mainform = 05, // Elegant
                otherforms = new[]
                {
                    new FormSubregionTable { form = 02, region = new[] {03,04} },
                    new FormSubregionTable { form = 13, region = new[] {48} },
                }
            },
            new CountryTable{
                countryID = 049, // USA
                mainform = 07, // Modern
                otherforms = new[]
                {
                    new FormSubregionTable { form = 01, region = new[] {03,09,21,23,24,32,33,36,40,41,48,50} },
                    new FormSubregionTable { form = 09, region = new[] {53} },
                    new FormSubregionTable { form = 10, region = new[] {06,07,08,15,28,34,35,39,46,49} },
                }
            },
            new CountryTable{
                countryID = 018, // Canada
                mainform = 01, // Polar
                otherforms = new[]
                {
                    new FormSubregionTable { form = 00, region = new[] {12,13,14} },
                    new FormSubregionTable { form = 07, region = new[] {05} },
                    new FormSubregionTable { form = 10, region = new[] {04} },
                }
            },
            new CountryTable{
                countryID = 016, // Brazil
                mainform = 14, // Savanna
                otherforms = new[]
                {
                    new FormSubregionTable { form = 17, region = new[] {03,06} },
                }
            },
            new CountryTable{
                countryID = 010, // Argentina
                mainform = 14, // Savanna
                otherforms = new[]
                {
                    new FormSubregionTable { form = 01, region = new[] {21,24} },
                    new FormSubregionTable { form = 03, region = new[] {16} },
                }
            },
            new CountryTable{
                countryID = 020, // Chile
                mainform = 08, // Marine
                otherforms = new[]
                {
                    new FormSubregionTable { form = 01, region = new[] {12} },
                }
            },
            new CountryTable{
                countryID = 036, // Mexico
                mainform = 15, // Sun
                otherforms = new[]
                {
                    new FormSubregionTable { form = 09, region = new[] {32} },
                    new FormSubregionTable { form = 10, region = new[] {04,08,09,12,15,19,20,23,26,27,29} },
                }
            },
            new CountryTable{
                countryID = 052, // Venezuela
                mainform = 09, // Archipelago
                otherforms = new[]
                {
                    new FormSubregionTable { form = 17, region = new[] {17} },
                }
            },
            new CountryTable{
                countryID = 065, // Australia
                mainform = 09, // River
                otherforms = new[]
                {
                    new FormSubregionTable { form = 04, region = new[] {07} },
                    new FormSubregionTable { form = 15, region = new[] {04} },
                }
            },
            new CountryTable{
                countryID = 066, // Austria
                mainform = 08, // Marine
                otherforms = new[]
                {
                    new FormSubregionTable { form = 06, region = new[] {10} },
                }
            },
            new CountryTable{
                countryID = 073, // Czecg Republic
                mainform = 08, // Marine
                otherforms = new[]
                {
                    new FormSubregionTable { form = 03, region = new[] {03} },
                }
            },
            new CountryTable{
                countryID = 076, // Finland
                mainform = 00, // Icy Snow
                otherforms = new[]
                {
                    new FormSubregionTable { form = 01, region = new[] {27} },
                }
            },
            new CountryTable{
                countryID = 077, // France
                mainform = 06, // Meadow
                otherforms = new[]
                {
                    new FormSubregionTable { form = 03, region = new[] {18} },
                    new FormSubregionTable { form = 08, region = new[] {04,06,08,19} },
                    new FormSubregionTable { form = 16, region = new[] {27} },
                }
            },
            new CountryTable{
                countryID = 078, // Germany
                mainform = 03, // Continental
                otherforms = new[]
                {
                    new FormSubregionTable { form = 06, region = new[] {04,13} },
                    new FormSubregionTable { form = 08, region = new[] {05} },
                }
            },
            new CountryTable{
                countryID = 078, // Italy
                mainform = 08, // Marine
                otherforms = new[]
                {
                    new FormSubregionTable { form = 06, region = new[] {04,06} },
                }
            },
            new CountryTable{
                countryID = 085, // Lesotho
                mainform = 09, // Archipelago ??
                otherforms = new[]
                {
                    new FormSubregionTable { form = 12, region = new[] {04} },
                }
            },
            new CountryTable{
                countryID = 096, // Norway
                mainform = 03, // Continental ??
                otherforms = new[]
                {
                    new FormSubregionTable { form = 00, region = new[] {11} },
                    new FormSubregionTable { form = 01, region = new[] {12,15,16,17,20,22} },
                    new FormSubregionTable { form = 02, region = new[] {13,14} },
                }
            },
            new CountryTable{
                countryID = 097, // Poland
                mainform = 03, // Continental
                otherforms = new[]
                {
                    new FormSubregionTable { form = 04, region = new[] {11} },
                }
            },
            new CountryTable{
                countryID = 100, // Russia
                mainform = 01, // Polar
                otherforms = new[]
                {
                    new FormSubregionTable { form = 00, region = new[] {14,22,34,38,40,52,66,88} },
                    new FormSubregionTable { form = 03, region = new[] {29,46,51,69} },
                    new FormSubregionTable { form = 10, region = new[] {20,24,25,28,33,71,73} },
                }
            },
            new CountryTable{
                countryID = 104, //South Africa
                mainform = 12, // River ??
                otherforms = new[]
                {
                    new FormSubregionTable { form = 03, region = new[] {03,05} },
                }
            },
            new CountryTable{
                countryID = 105, // Spain
                mainform = 08, // Marine
                otherforms = new[]
                {
                    new FormSubregionTable { form = 06, region = new[] {11} },
                    new FormSubregionTable { form = 12, region = new[] {07} },
                }
            },
            new CountryTable{
                countryID = 107, // Sweden
                mainform = 03, // Continental
                otherforms = new[]
                {
                    new FormSubregionTable { form = 00, region = new[] {11,21} },
                    new FormSubregionTable { form = 01, region = new[] {09,13} },
                }
            },
            new CountryTable{
                countryID = 169, // India
                mainform = 13, // Monsoon ??
                otherforms = new[]
                {
                    new FormSubregionTable { form = 17, region = new[] {12} },
                }
            },
        };

        /// <summary>
        /// Compares the Vivillon pattern against its country and region to determine if the pattern is able to be obtained legally.
        /// </summary>
        /// <param name="form">Alternate Forme Pattern</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Console Region ID</param>
        /// <returns></returns>
        public static bool CheckVivillonPattern(int form, int country, int region)
        {
            if (!VivillonCountryTable[form].Contains(country))
                return false; // Country mismatch

            var ct = Array.Find(RegionFormTable, t => t.countryID == country);
            if (ct.otherforms == null) // empty struct = no forms referenced
                return true; // No subregion table

            if (ct.mainform == form)
                return !ct.otherforms.Any(e => e.region.Contains(region)); //true if Mainform not in other specific region

            return ct.otherforms.Any(e => e.form == form && e.region.Contains(region));
        }

        /// <summary>
        /// Compares the Vivillon pattern against its country and region to determine if the pattern is able to be obtained legally.
        /// </summary>
        /// <param name="country">Country ID</param>
        /// <param name="region">Console Region ID</param>
        public static int GetVivillonPattern(int country, int region)
        {
            var ct = Array.Find(RegionFormTable, t => t.countryID == country);
            if (ct.otherforms == null) // empty struct = no forms referenced
                return ct.mainform; // No subregion table

            foreach (var sub in ct.otherforms)
            {
                if (sub.region.Contains(region))
                    return sub.form;
            }

            return ct.mainform;
        }

        /// <summary>
        /// Compares the <see cref="PKM.ConsoleRegion"/> and <see cref="PKM.Country"/> to determine if the country is available within that region.
        /// </summary>
        /// <param name="consoleRegion">Console region.</param>
        /// <param name="country">Country of nationality</param>
        /// <returns>Country is within Console Region</returns>
        public static bool IsConsoleRegionCountryValid(int consoleRegion, int country)
        {
            switch (consoleRegion)
            {
                case 0: // Japan
                    return country == 1;
                case 1: // Americas
                    return (8 <= country && country <= 52) || ExtendedAmericas.Contains(country);
                case 2: // Europe
                    return (64 <= country && country <= 127) || ExtendedEurope.Contains(country);
                case 4: // China
                    return country == 144 || country == 160;
                case 5: // Korea
                    return country == 136;
                case 6: // Taiwan
                    return country == 144 || country == 128;
                default:
                    return false;
            }
        }

        private static readonly HashSet<int> ExtendedAmericas = new HashSet<int> {153, 156, 168, 174, 186};
        private static readonly HashSet<int> ExtendedEurope = new HashSet<int> {169, 184, 185};
    }
}