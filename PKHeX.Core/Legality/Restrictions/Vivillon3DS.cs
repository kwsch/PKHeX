using System;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Provides information for Vivillon origins with respect to the 3DS game data.
/// </summary>
public static class Vivillon3DS
{
    public const int MaxWildFormID = 17; // 0-17 valid form indexes

    private sealed class CountryTable
    {
        public readonly byte BaseForm;
        public readonly byte CountryID;
        public readonly FormSubregionTable[] SubRegionForms;

        public CountryTable(byte country, byte form, params FormSubregionTable[] subs)
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

    /// <summary>
    /// List of valid regions as bitflags indexed by Vivillon form.
    /// </summary>
    private static readonly Region3DSFlags[] VivillonRegionTable =
    {
        /* 0 Icy Snow    */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /* 1 Polar       */ Region3DSFlags.Americas | Region3DSFlags.Europe | Region3DSFlags.China,
        /* 2 Tundra      */ Region3DSFlags.Japan    | Region3DSFlags.Europe,
        /* 3 Continental */ Region3DSFlags.Americas | Region3DSFlags.Europe | Region3DSFlags.China | Region3DSFlags.Korea | Region3DSFlags.Taiwan,
        /* 4 Garden      */                           Region3DSFlags.Europe,
        /* 5 Elegant     */ Region3DSFlags.Japan,
        /* 6 Meadow      */                           Region3DSFlags.Europe,
        /* 7 Modern      */ Region3DSFlags.Americas,
        /* 8 Marine      */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /* 9 Archipelago */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /*10 High Plains */ Region3DSFlags.Americas | Region3DSFlags.Europe | Region3DSFlags.China,
        /*11 Sandstorm   */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /*12 River       */                           Region3DSFlags.Europe,
        /*13 Monsoon     */ Region3DSFlags.Japan    | Region3DSFlags.Europe | Region3DSFlags.China | Region3DSFlags.Taiwan,
        /*14 Savanna     */ Region3DSFlags.Americas,
        /*15 Sun         */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /*16 Ocean       */ Region3DSFlags.Americas | Region3DSFlags.Europe,
        /*17 Jungle      */ Region3DSFlags.Americas | Region3DSFlags.Europe,
    };

    private static Region3DSFlags GetConsoleRegionFlag(int consoleRegion) => (Region3DSFlags)(1 << consoleRegion);

    /// <summary>
    /// List of valid countries for each Vivillon form.
    /// </summary>
    private static readonly byte[][] VivillonCountryTable =
    {
        /* 0 Icy Snow    */ new byte[] {018,076,096,100,107},
        /* 1 Polar       */ new byte[] {010,018,020,049,076,096,100,107,160},
        /* 2 Tundra      */ new byte[] {001,074,081,096},
        /* 3 Continental */ new byte[] {010,067,073,074,075,077,078,084,087,094,096,097,100,107,128,136,144,160,169},
        /* 4 Garden      */ new byte[] {065,082,095,110,125},
        /* 5 Elegant     */ new byte[] {001},
        /* 6 Meadow      */ new byte[] {066,077,078,083,086,088,105,108,122,127},
        /* 7 Modern      */ new byte[] {018,049,186},
        /* 8 Marine      */ new byte[] {020,064,066,068,070,071,073,077,078,079,080,083,089,090,091,098,099,100,101,102,103,105,109,123,124,126,184,185},
        /* 9 Archipelago */ new byte[] {008,009,011,012,013,017,021,023,024,028,029,032,034,035,036,037,038,043,044,045,047,048,049,051,052,077,085,104},
        /*10 High Plains */ new byte[] {018,036,049,100,109,113,160},
        /*11 Sandstorm   */ new byte[] {072,109,118,119,120,121,168,174},
        /*12 River       */ new byte[] {065,069,085,093,104,105,114,115,116,117},
        /*13 Monsoon     */ new byte[] {001,128,160,169},
        /*14 Savanna     */ new byte[] {010,015,016,041,042,050},
        /*15 Sun         */ new byte[] {014,019,026,030,033,036,039,065,085,092,104,106,111,112},
        /*16 Ocean       */ new byte[] {049,077},
        /*17 Jungle      */ new byte[] {016,021,022,025,027,031,040,042,046,052,077,153,156,169},
    };

    /// <summary>
    /// List of valid subregions for countries that can have multiple Vivillon forms.
    /// </summary>
    /// <remarks>BaseForm is the form for no selected subregion.</remarks>
    private static readonly CountryTable[] RegionFormTable =
    {
        new(001, 05, // Japan: Elegant
            new FormSubregionTable(02, new byte[] {03,04}),
            new FormSubregionTable(13, new byte[] {48})),

        new(010, 14, // Argentina: Savanna
            new FormSubregionTable(01, new byte[] {21,24}),
            new FormSubregionTable(03, new byte[] {06,12,14,16,17,19,20})),

        new(016, 14, // Brazil: Savanna
            new FormSubregionTable(17, new byte[] {03,05,06,21,22})),

        new(018, 01, // Canada: Polar
            new FormSubregionTable(00, new byte[] {12,13,14}),
            new FormSubregionTable(07, new byte[] {05}),
            new FormSubregionTable(10, new byte[] {04})),

        new(020, 08, // Chile: Marine
            new FormSubregionTable(01, new byte[] {12})),

        new(021, 17, // Colombia: Jungle
            new FormSubregionTable(09, new byte[] {07,19,20})),

        new(036, 15, // Mexico: Sun
            new FormSubregionTable(10, new byte[] {03,04,05,08,09,11,12,15,19,20,23,25,26,27,29,33})),

        new(042, 14, // Peru: Savanna
            new FormSubregionTable(17, new byte[] {03,08,12,15,16,17,21,23,25,26})),

        new(049, 07, // USA: Modern
            new FormSubregionTable(01, new byte[] {03,09,21,23,24,32,33,36,40,41,48,50}),
            new FormSubregionTable(09, new byte[] {53}),
            new FormSubregionTable(10, new byte[] {06,07,08,15,28,34,35,39,46,49}),
            new FormSubregionTable(16, new byte[] {13})),

        new(052, 09, // Venezuela: Archipelago
            new FormSubregionTable(17, new byte[] {03,04,05,07,08,09,10,11,13,15,17,19,21})),

        new(065, 12, // Australia: River
            new FormSubregionTable(04, new byte[] {07}),
            new FormSubregionTable(15, new byte[] {04})),

        new(066, 08, // Austria: Marine
            new FormSubregionTable(06, new byte[] {10})),

        new(073, 03, // Czech Republic: Continental
            new FormSubregionTable(08, new byte[] {04,05,13,14,15})),

        new(074, 03, // Denmark: Continental
            new FormSubregionTable(02, new byte[] {18,24})),

        new(076, 00, // Finland: Icy Snow
            new FormSubregionTable(01, new byte[] {27})),

        new(077, 06, // France: Meadow
            new FormSubregionTable(03, new byte[] {18}),
            new FormSubregionTable(08, new byte[] {04,06,08,19}),
            new FormSubregionTable(09, new byte[] {24,25}),
            new FormSubregionTable(16, new byte[] {27}),
            new FormSubregionTable(17, new byte[] {26})),

        new(078, 03, // Germany: Continental
            new FormSubregionTable(06, new byte[] {04,12,13}),
            new FormSubregionTable(08, new byte[] {05})),

        new(083, 08, // Italy: Marine
            new FormSubregionTable(06, new byte[] {03,04,05,06})),

        new(085, 12, // Lesotho: River
            new FormSubregionTable(09, new byte[] {10}),
            new FormSubregionTable(15, new byte[] {06,07,08,09})),

        new(096, 03, // Norway: Continental
            new FormSubregionTable(00, new byte[] {11,26}),
            new FormSubregionTable(01, new byte[] {12,15,16,17,20,22}),
            new FormSubregionTable(02, new byte[] {13,14,19})),

        new(100, 03, // Russia: Continental
            new FormSubregionTable(00, new byte[] {14,22,34,38,40,52,53,66,88}),
            new FormSubregionTable(01, new byte[] {11,12,13,16,19,21,23,26,27,32,35,36,37,39,41,43,44,48,49,50,54,55,56,57,58,60,61,62,67,68,70,74,75,76,77,80,81,82,83,84,90,91}),
            new FormSubregionTable(08, new byte[] {42,64}),
            new FormSubregionTable(10, new byte[] {10,15,20,24,25,28,30,33,71,73,85})),

        new(104, 12, // South Africa: River
            new FormSubregionTable(15, new byte[] {06,09}),
            new FormSubregionTable(09, new byte[] {03,05})),

        new(105, 08, // Spain: Marine
            new FormSubregionTable(06, new byte[] {11}),
            new FormSubregionTable(12, new byte[] {07})),

        new(107, 03, // Sweden: Continental
            new FormSubregionTable(00, new byte[] {10,11}),
            new FormSubregionTable(01, new byte[] {09,13,17})),

        new(109, 11, // Turkey: Sandstorm
            new FormSubregionTable(08, new byte[] {03,04,05,17,20,23,24,26,27,28,30,33,34,36,41,42,44,47,48,50,52,55,57,60,62,63,65,66,69,71,73,75,80,83}),
            new FormSubregionTable(10, new byte[] {53,54,70,79})),

        new(128, 13, // Taiwan: Monsoon
            new FormSubregionTable(03, new byte[] {24,25,26})),

        new(160, 03, // China: Continental
            new FormSubregionTable(01, new byte[] {13,19,20}),
            new FormSubregionTable(10, new byte[] {30,32}),
            new FormSubregionTable(13, new byte[] {10,26,29,33})),

        new(169, 13, // India: Monsoon
            new FormSubregionTable(03, new byte[] {06,09,10,21,36}),
            new FormSubregionTable(17, new byte[] {12})),
    };

    /// <summary>
    /// Compares the Vivillon pattern against its console region to determine if the pattern is legal.
    /// </summary>
    public static bool IsPatternValid(byte form, int consoleRegion)
    {
        if ((uint)form > MaxWildFormID)
            return false;
        var permit = GetConsoleRegionFlag(consoleRegion);
        return VivillonRegionTable[form].HasFlag(permit);
    }

    /// <summary>
    /// Compares the Vivillon pattern against its country and subregion to determine if the pattern could have been natively obtained.
    /// </summary>
    /// <param name="form">Alternate Form Pattern</param>
    /// <param name="country">Country ID</param>
    /// <param name="region">Subregion ID</param>
    /// <returns>True if valid</returns>
    public static bool IsPatternNative(byte form, byte country, byte region)
    {
        if ((uint)form > MaxWildFormID)
            return false;
        if (!VivillonCountryTable[form].Contains(country))
            return false; // Country mismatch

        var ct = Array.Find(RegionFormTable, t => t.CountryID == country);
        if (ct == null) // empty = one form for country
            return true; // No subregion table, already checked if Country can have this form

        if (ct.BaseForm == form)
            return !ct.SubRegionForms.Any(e => e.Regions.Contains(region)); //true if Mainform not in other specific region

        return ct.SubRegionForms.Any(e => e.Form == form && e.Regions.Contains(region));
    }

    /// <summary>
    /// Gets a compatible Vivillon pattern based on its country and subregion.
    /// </summary>
    /// <param name="country">Country ID</param>
    /// <param name="region">Subregion ID</param>
    public static byte GetPattern(byte country, byte region)
    {
        var ct = Array.Find(RegionFormTable, t => t.CountryID == country);
        if (ct == null) // empty = no forms referenced
            return GetPattern(country);

        foreach (var sub in ct.SubRegionForms)
        {
            if (sub.Regions.Contains(region))
                return sub.Form;
        }
        return ct.BaseForm;
    }

    private static byte GetPattern(byte country)
    {
        var form = Array.FindIndex(VivillonCountryTable, z => z.Contains(country));
        if (form == -1)
            return 0;
        return (byte)form;
    }

    /// <summary>
    /// Console Region Flags for the 3DS.
    /// </summary>
    /// <remarks>Not to be confused with <see cref="Region3DSIndex"/>.</remarks>
    [Flags]
    private enum Region3DSFlags : ushort
    {
        None,
        Japan = 1,
        Americas = 1 << 1,
        Europe = 1 << 2,
        China = 1 << 4,
        Korea = 1 << 5,
        Taiwan = 1 << 6,
    }
}
