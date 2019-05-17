using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        private class CountryTable
        {
            public byte CountryID;
            public byte BaseForm;
            public FormSubregionTable[] SubRegionForms;
        }

        private class FormSubregionTable
        {
            public byte Form;
            public int[] Regions;
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
                CountryID = 001, // Japan
                BaseForm = 05, // Elegant
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 02, Regions = new[] {03,04} },
                    new FormSubregionTable { Form = 13, Regions = new[] {48} },
                }
            },
            new CountryTable{
                CountryID = 049, // USA
                BaseForm = 07, // Modern
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 01, Regions = new[] {03,09,21,23,24,32,33,36,40,41,48,50} },
                    new FormSubregionTable { Form = 09, Regions = new[] {53} },
                    new FormSubregionTable { Form = 10, Regions = new[] {06,07,08,15,28,34,35,39,46,49} },
                }
            },
            new CountryTable{
                CountryID = 018, // Canada
                BaseForm = 01, // Polar
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 00, Regions = new[] {12,13,14} },
                    new FormSubregionTable { Form = 07, Regions = new[] {05} },
                    new FormSubregionTable { Form = 10, Regions = new[] {04} },
                }
            },
            new CountryTable{
                CountryID = 016, // Brazil
                BaseForm = 14, // Savanna
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 17, Regions = new[] {03,06} },
                }
            },
            new CountryTable{
                CountryID = 010, // Argentina
                BaseForm = 14, // Savanna
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 01, Regions = new[] {21,24} },
                    new FormSubregionTable { Form = 03, Regions = new[] {16} },
                }
            },
            new CountryTable{
                CountryID = 020, // Chile
                BaseForm = 08, // Marine
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 01, Regions = new[] {12} },
                }
            },
            new CountryTable{
                CountryID = 036, // Mexico
                BaseForm = 15, // Sun
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 09, Regions = new[] {32} },
                    new FormSubregionTable { Form = 10, Regions = new[] {04,08,09,12,15,19,20,23,26,27,29} },
                }
            },
            new CountryTable{
                CountryID = 052, // Venezuela
                BaseForm = 09, // Archipelago
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 17, Regions = new[] {17} },
                }
            },
            new CountryTable{
                CountryID = 065, // Australia
                BaseForm = 09, // River
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 04, Regions = new[] {07} },
                    new FormSubregionTable { Form = 15, Regions = new[] {04} },
                }
            },
            new CountryTable{
                CountryID = 066, // Austria
                BaseForm = 08, // Marine
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 06, Regions = new[] {10} },
                }
            },
            new CountryTable{
                CountryID = 073, // Czecg Republic
                BaseForm = 08, // Marine
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 03, Regions = new[] {03} },
                }
            },
            new CountryTable{
                CountryID = 076, // Finland
                BaseForm = 00, // Icy Snow
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 01, Regions = new[] {27} },
                }
            },
            new CountryTable{
                CountryID = 077, // France
                BaseForm = 06, // Meadow
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 03, Regions = new[] {18} },
                    new FormSubregionTable { Form = 08, Regions = new[] {04,06,08,19} },
                    new FormSubregionTable { Form = 16, Regions = new[] {27} },
                }
            },
            new CountryTable{
                CountryID = 078, // Germany
                BaseForm = 03, // Continental
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 06, Regions = new[] {04,13} },
                    new FormSubregionTable { Form = 08, Regions = new[] {05} },
                }
            },
            new CountryTable{
                CountryID = 078, // Italy
                BaseForm = 08, // Marine
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 06, Regions = new[] {04,06} },
                }
            },
            new CountryTable{
                CountryID = 085, // Lesotho
                BaseForm = 09, // Archipelago ??
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 12, Regions = new[] {04} },
                }
            },
            new CountryTable{
                CountryID = 096, // Norway
                BaseForm = 03, // Continental ??
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 00, Regions = new[] {11} },
                    new FormSubregionTable { Form = 01, Regions = new[] {12,15,16,17,20,22} },
                    new FormSubregionTable { Form = 02, Regions = new[] {13,14} },
                }
            },
            new CountryTable{
                CountryID = 097, // Poland
                BaseForm = 03, // Continental
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 04, Regions = new[] {11} },
                }
            },
            new CountryTable{
                CountryID = 100, // Russia
                BaseForm = 01, // Polar
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 00, Regions = new[] {14,22,34,38,40,52,66,88} },
                    new FormSubregionTable { Form = 03, Regions = new[] {29,46,51,69} },
                    new FormSubregionTable { Form = 10, Regions = new[] {20,24,25,28,33,71,73} },
                }
            },
            new CountryTable{
                CountryID = 104, //South Africa
                BaseForm = 12, // River ??
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 03, Regions = new[] {03,05} },
                }
            },
            new CountryTable{
                CountryID = 105, // Spain
                BaseForm = 08, // Marine
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 06, Regions = new[] {11} },
                    new FormSubregionTable { Form = 12, Regions = new[] {07} },
                }
            },
            new CountryTable{
                CountryID = 107, // Sweden
                BaseForm = 03, // Continental
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 00, Regions = new[] {11,21} },
                    new FormSubregionTable { Form = 01, Regions = new[] {09,13} },
                }
            },
            new CountryTable{
                CountryID = 169, // India
                BaseForm = 13, // Monsoon ??
                SubRegionForms = new[]
                {
                    new FormSubregionTable { Form = 17, Regions = new[] {12} },
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

            var ct = Array.Find(RegionFormTable, t => t.CountryID == country);
            if (ct == default(CountryTable)) // empty = one form for country
                return true; // No subregion table, already checked if Country can have this form

            if (ct.BaseForm == form)
                return !ct.SubRegionForms.Any(e => e.Regions.Contains(region)); //true if Mainform not in other specific region

            return ct.SubRegionForms.Any(e => e.Form == form && e.Regions.Contains(region));
        }

        /// <summary>
        /// Compares the Vivillon pattern against its country and region to determine if the pattern is able to be obtained legally.
        /// </summary>
        /// <param name="country">Country ID</param>
        /// <param name="region">Console Region ID</param>
        public static int GetVivillonPattern(int country, int region)
        {
            var ct = Array.Find(RegionFormTable, t => t.CountryID == country);
            if (ct == default(CountryTable)) // empty = no forms referenced
                return GetVivillonPattern(country);

            foreach (var sub in ct.SubRegionForms)
            {
                if (sub.Regions.Contains(region))
                    return sub.Form;
            }

            return ct.BaseForm;
        }

        private static int GetVivillonPattern(int country)
        {
            var form = Array.FindIndex(VivillonCountryTable, z => z.Contains(country));
            return Math.Max(0, form);
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