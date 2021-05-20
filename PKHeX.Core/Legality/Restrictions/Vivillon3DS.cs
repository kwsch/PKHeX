using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides information for Vivillon origins with respect to the 3DS game data.
    /// </summary>
    public static class Vivillon3DS
    {
        private sealed class CountryTable
        {
            public readonly byte BaseForm;
            public readonly byte CountryID;
            public readonly FormSubregionTable[] SubRegionForms;

            internal CountryTable(byte form, byte country, params FormSubregionTable[] subs)
            {
                BaseForm = form;
                CountryID = country;
                SubRegionForms = subs;
            }
        }

        private sealed class FormSubregionTable
        {
            public readonly byte Form;
            public readonly byte[] Regions;

            internal FormSubregionTable(byte form, byte[] regions)
            {
                Form = form;
                Regions = regions;
            }
        }

        private static readonly byte[][] VivillonCountryTable =
        {
               //missing ID 051,068,102,127,160,186
               /* 0 Icy Snow    */ new byte[] { 018, 076, 096, 100, 107 },
               /* 1 Polar       */ new byte[] { 010, 018, 020, 049, 076, 096, 100, 107 },
               /* 2 Tundra      */ new byte[] { 001, 081, 096, },
               /* 3 Continental */ new byte[] { 010, 067, 073, 074, 075, 077, 078, 084, 087, 094, 096, 097, 100, 107, 136},
               /* 4 Garden      */ new byte[] { 065, 082, 095, 097, 101, 110, 125},
               /* 5 Elegant     */ new byte[] { 001 },
               /* 6 Meadow      */ new byte[] { 066, 077, 078, 083, 086, 088, 105, 108, 122},
               /* 7 Modern      */ new byte[] { 018, 049},
               /* 8 Marine      */ new byte[] { 020, 064, 066, 070, 071, 073, 077, 078, 079, 080, 083, 089, 090, 091, 098, 099, 103, 105, 123, 124, 126, 184, 185},
               /* 9 Archipelago */ new byte[] { 008, 009, 011, 012, 013, 017, 021, 023, 024, 028, 029, 032, 034, 035, 036, 037, 038, 043, 044, 045, 047, 048, 049, 052, 085, 104,},
               /*10 High Plains */ new byte[] { 018, 036, 049, 100, 113},
               /*11 Sandstorm   */ new byte[] { 072, 109, 118, 119, 120, 121, 168, 174},
               /*12 River       */ new byte[] { 065, 069, 085, 093, 104, 105, 114, 115, 116, 117},
               /*13 Monsoon     */ new byte[] { 001, 128, 144, 169},
               /*14-Savanna     */ new byte[] { 010, 015, 016, 041, 042, 050},
               /*15 Sun         */ new byte[] { 036, 014, 019, 026, 030, 033, 036, 039, 065, 092, 106, 111, 112},
               /*16 Ocean       */ new byte[] { 049, 077},
               /*17 Jungle      */ new byte[] { 016, 021, 022, 025, 027, 031, 040, 046, 052, 169, 153, 156},
        };

        private static readonly CountryTable[] RegionFormTable =
        {
            new(05, 1, // Japan: Elegant
                new FormSubregionTable(02, new byte[] {03,04}),
                new FormSubregionTable(13, new byte[] {48})),

            new(07, 49, // USA: Modern
                new FormSubregionTable(01, new byte[] {03,09,21,23,24,32,33,36,40,41,48,50}),
                new FormSubregionTable(09, new byte[] {53}),
                new FormSubregionTable(10, new byte[] {06,07,08,15,28,34,35,39,46,49})),

            new(01, 18, // Canada: Polar
                new FormSubregionTable(00, new byte[] {12,13,14}),
                new FormSubregionTable(07, new byte[] {05}),
                new FormSubregionTable(10, new byte[] {04})),

            new(14, 16, // Brazil: Savanna
                new FormSubregionTable(17, new byte[] {03,06})),

            new(14, 10, // Argentina: Savanna
                new FormSubregionTable(01, new byte[] {21,24}),
                new FormSubregionTable(03, new byte[] {16})),

            new(08, 20, // Chile: Marine
                new FormSubregionTable(01, new byte[] {12})),

            new(15, 36, // Mexico: Sun
                new FormSubregionTable(09, new byte[] {32}),
                new FormSubregionTable(10, new byte[] {04,08,09,12,15,19,20,23,26,27,29})),

            new(09, 52, // Venezuela: Archipelago
                new FormSubregionTable(17, new byte[] {17})),

            new(09, 65, // Australia: River
                new FormSubregionTable(04, new byte[] {07}),
                new FormSubregionTable(15, new byte[] {04})),

            new(08, 66, // Austria: Marine
                new FormSubregionTable(06, new byte[] {10})),

            new(08, 73, // Czech Republic: Marine
                new FormSubregionTable(03, new byte[] {03})),

            new(00, 76, // Finland: Icy Snow
                new FormSubregionTable(01, new byte[] {27})),

            new(06, 77, // France: Meadow
                new FormSubregionTable(03, new byte[] {18}),
                new FormSubregionTable(08, new byte[] {04,06,08,19}),
                new FormSubregionTable(16, new byte[] {27})),

            new(03, 078, // Germany: Continental
                new FormSubregionTable(06, new byte[] {04,13}),
                new FormSubregionTable(08, new byte[] {05})),

            new(08, 83, // Italy: Marine
                new FormSubregionTable(06, new byte[] {04,06})),

            new(09, 85, // Lesotho: Archipelago ??
                new FormSubregionTable(12, new byte[] {04})),

            new(03, 96, // Norway: Continental ??
                new FormSubregionTable(00, new byte[] {11}),
                new FormSubregionTable(01, new byte[] {12,15,16,17,20,22}),
                new FormSubregionTable(02, new byte[] {13,14})),

            new(03, 97, // Poland: Continental
                new FormSubregionTable(04, new byte[] {11})),

            new(01, 100, // Russia: Polar
                new FormSubregionTable(00, new byte[] {14,22,34,38,40,52,66,88}),
                new FormSubregionTable(03, new byte[] {29,46,51,69}),
                new FormSubregionTable(10, new byte[] {20,24,25,28,33,71,73})),

            new(12, 104, // South Affrica: River ??
                    new FormSubregionTable(03, new byte[] {03,05})),

            new(08, 105, // Spain: Marine
                new FormSubregionTable(06, new byte[] {11}),
                new FormSubregionTable(12, new byte[] {07})),

            new(03, 107, // Sweden: Continental
                new FormSubregionTable(00, new byte[] {11,21}),
                new FormSubregionTable(01, new byte[] {09,13})),

            new(13, 169, // India: Monsoon ??
                new FormSubregionTable(17, new byte[] {12})),
        };

        /// <summary>
        /// Compares the Vivillon pattern against its country and region to determine if the pattern is able to be obtained legally.
        /// </summary>
        /// <param name="form">Alternate Forme Pattern</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Console Region ID</param>
        /// <returns></returns>
        public static bool IsPatternValid(int form, byte country, byte region)
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
        public static int GetPattern(byte country, byte region)
        {
            var ct = Array.Find(RegionFormTable, t => t.CountryID == country);
            if (ct == default(CountryTable)) // empty = no forms referenced
                return GetPattern(country);

            foreach (var sub in ct.SubRegionForms)
            {
                if (sub.Regions.Contains(region))
                    return sub.Form;
            }

            return ct.BaseForm;
        }

        private static int GetPattern(byte country)
        {
            var form = Array.FindIndex(VivillonCountryTable, z => z.Contains(country));
            return Math.Max(0, form);
        }
    }
}
