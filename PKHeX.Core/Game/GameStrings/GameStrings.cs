using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Repository of localized game strings for a given <see cref="LanguageID"/>.
/// </summary>
public sealed class GameStrings : IBasicStrings
{
    // PKM Info
    public readonly string[] specieslist, movelist, itemlist, abilitylist, types, natures, forms,
        memories, genloc, feeling6, feeling8, intensity,
        trainingbags, trainingstage, characteristics,
        groundtiletypes, balllist, gamelist, pokeblocks, ribbons;

    private readonly string[] g4items, g3coloitems, g3xditems, g3items, g2items, g1items;

    // Met Locations
    public readonly LocationSet0 Gen2, Gen3, CXD;
    public readonly LocationSet4 Gen4;
    public readonly LocationSet6 Gen5, Gen6, Gen7, Gen7b, Gen8, Gen8a, Gen8b, Gen9;

    // Misc
    public readonly string[] wallpapernames, puffs, walkercourses;
    public readonly string[] uggoods, ugspheres, ugtraps, ugtreasures;
    public readonly string[] seals, accessories, backdrops, poketchapps;
    private readonly string lang;
    private readonly int LanguageIndex;

    public string EggName { get; }
    public IReadOnlyList<string> Species => specieslist;
    public IReadOnlyList<string> Item => itemlist;
    public IReadOnlyList<string> Move => movelist;
    public IReadOnlyList<string> Ability => abilitylist;
    public IReadOnlyList<string> Types => types;
    public IReadOnlyList<string> Natures => natures;

    private string[] Get(string ident) => GameLanguage.GetStrings(ident, lang);
    private const string NPC = "NPC";
    private const string EmptyIndex = "---";

    /// <summary>
    /// Item IDs that correspond to the <see cref="Ball"/> value.
    /// </summary>
    private static ReadOnlySpan<ushort> Items_Ball =>
    [
        0000, 0001, 0002, 0003, 0004, 0005, 0006, 0007, 0008, 0009,
        0010, 0011, 0012, 0013, 0014, 0015, 0016, 0492, 0493, 0494,
        0495, 0496, 0497, 0498, 0499, 0576, 0851,
        1785, 1710, 1711,
        1712, 1713, 1746, 1747, 1748, 1749, 1750, 1771,
    ];

    internal GameStrings(string l)
    {
        lang = l;
        LanguageIndex = GameLanguage.GetLanguageIndex(l);
        ribbons = Get("ribbons");

        // Past Generation strings
        g3items = Get("ItemsG3");
        g3coloitems = GetG3CXD(g3items, "ItemsG3Colosseum");
        g3xditems = GetG3CXD(g3items, "ItemsG3XD");

        g2items = Get("ItemsG2");
        g1items = Get("ItemsG1");
        Gen2 = new(Get("gsc_00000"));
        Gen3 = new(Get("rsefrlg_00000"));
        CXD = new(Get("cxd_00000"));

        // Less than 10% of location values are unique.
        // Just mark them with the ID if they aren't empty.
        AppendLocationIndex(CXD.Met0.AsSpan(0, 227));

        // Current Generation strings
        natures = Util.GetNaturesList(l);
        types = Get("types");
        abilitylist = Get("abilities");

        movelist = Get("moves");
        // Differentiate Physical/Special Z-Moves
        for (int i = 622; i < 658; i++)
        {
            const string p = " (P)";
            const string s = " (S)";
            bool isPhysicalZMove = (i & 1) == 0;
            movelist[i] += isPhysicalZMove ? p : s;
        }

        itemlist = Get("items");
        characteristics = Get("character");
        specieslist = Get("species");
        wallpapernames = Get("wallpaper");
        groundtiletypes = Get("groundtile");
        gamelist = Get("games");

        var balls = Items_Ball;
        balllist = new string[balls.Length];
        for (int i = 0; i < balllist.Length; i++)
            balllist[i] = itemlist[balls[i]];

        pokeblocks = Get("pokeblock");
        forms = Get("forms");
        memories = Get("memories");
        feeling6 = Get("feeling6");
        feeling8 = Get("feeling");
        intensity = Get("intensity");
        genloc = Get("genloc");
        trainingbags = Get("trainingbag");
        trainingstage = Get("supertraining");
        puffs = Get("puff");

        walkercourses = Get("walkercourses");

        uggoods = Get("uggoods");
        ugspheres = Get("ugspheres");
        ugtraps = Get("ugtraps");
        ugtreasures = Get("ugtreasures");

        seals = Get("seals");
        accessories = Get("accessories");
        backdrops = Get("backdrops");
        poketchapps = Get("poketchapps");

        EggName = specieslist[0];
        Gen4 = Get4("hgss");
        Gen5 = Get6("bw2");
        Gen6 = Get6("xy");
        Gen7 = Get6("sm");

        Gen7b = Get6("gg", Gen7.Met3, Gen7.Met6);
        Gen8 = Get6("swsh");
        Gen8a = Get6("la");
        Gen8b = Get6("bdsp");
        Gen9 = Get6("sv");

        Sanitize();

        g4items = [..itemlist];
        Get("mail4").CopyTo(g4items, 137);
    }

    private LocationSet4 Get4([ConstantExpected] string ident)
    {
        var met0 = Get($"{ident}_00000");
        var met2 = Get($"{ident}_02000");
        var met3 = Get($"{ident}_03000");
        return new LocationSet4(met0, met2, met3);
    }

    private LocationSet6 Get6([ConstantExpected] string ident)
    {
        var met0 = Get($"{ident}_00000");
        var met3 = Get($"{ident}_30000");
        var met4 = Get($"{ident}_40000");
        var met6 = Get($"{ident}_60000");
        return new LocationSet6(met0, met3, met4, met6);
    }

    private LocationSet6 Get6([ConstantExpected] string ident, string[] met3, string[] met6)
    {
        var met0 = Get($"{ident}_00000");
        var met4 = Get($"{ident}_40000");
        return new LocationSet6(met0, met3, met4, met6);
    }

    private string[] GetG3CXD(ReadOnlySpan<string> arr, [ConstantExpected] string fileName)
    {
        // Concatenate the Gen3 Item list with the CXD item array; CXD items starting at index 500.
        var item500 = Get(fileName);
        var result = new string[500 + item500.Length];
        for (int i = arr.Length; i < result.Length; i++)
            result[i] = string.Empty;
        arr.CopyTo(result);
        item500.CopyTo(result, 500);
        return result;
    }

    private static void AppendLocationIndex(Span<string> names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            ref var str = ref names[i];
            if (str.Length != 0)
                str += $" [{i:000}]";
        }
    }

    private void Sanitize()
    {
        SanitizeItemNames();
        SanitizeMetLocations();

        // De-duplicate the Calyrex ability names
        abilitylist[(int)Core.Ability.AsOneI] += $" ({specieslist[(int)Core.Species.Glastrier]})";
        abilitylist[(int)Core.Ability.AsOneG] += $" ({specieslist[(int)Core.Species.Spectrier]})";
        // De-duplicate the Ogerpon ability names
        abilitylist[(int)Core.Ability.EmbodyAspect0] += $" ({forms[FormConverter.MaskTeal]})";
        abilitylist[(int)Core.Ability.EmbodyAspect1] += $" ({forms[FormConverter.MaskHearthflame]})";
        abilitylist[(int)Core.Ability.EmbodyAspect2] += $" ({forms[FormConverter.MaskWellspring]})";
        abilitylist[(int)Core.Ability.EmbodyAspect3] += $" ({forms[FormConverter.MaskCornerstone]})";

        // Replace the Egg Name with ---; egg name already stored to eggname
        specieslist[0] = EmptyIndex;
        // Fix (None) tags
        var none = $"({itemlist[0]})";
        abilitylist[0] = itemlist[0] = movelist[0] = Gen6.Met0[0] = Gen5.Met0[0] = Gen4.Met0[0] = CXD.Met0[0] = puffs[0] = none;
    }

    private void SanitizeItemNames()
    {
        // Fix Item Names (Duplicate entries)
        var HM06 = itemlist[425];
        var HM0 = HM06[..^1]; // language ambiguous!
        itemlist[426] = $"{HM0}7 (G4)";
        itemlist[427] = $"{HM0}8 (G4)";
        itemlist[456] += " (HG/SS)"; // S.S. Ticket
        itemlist[736] += " (OR/AS)"; // S.S. Ticket
        itemlist[463] += " (DPPt)"; // Storage Key
        itemlist[734] += " (OR/AS)"; // Storage Key
        itemlist[476] += " (HG/SS)"; // Basement Key
        itemlist[723] += " (OR/AS)"; // Basement Key
        itemlist[621] += " (M)"; // Xtransceiver
        itemlist[626] += " (F)"; // Xtransceiver
        itemlist[629] += " (2)"; // DNA Splicers
        itemlist[637] += " (2)"; // Dropped Item
        itemlist[707] += " (2)"; // Travel Trunk
        itemlist[713] += " (2)"; // Alt Bike
        itemlist[714] += " (2)"; // Holo Caster
        itemlist[729] += " (1)"; // Meteorite
        itemlist[740] += " (2)"; // Contest Costume
        itemlist[751] += " (2)"; // Meteorite
        itemlist[771] += " (3)"; // Meteorite
        itemlist[772] += " (4)"; // Meteorite
        itemlist[842] += " (SM)"; // Fishing Rod
        itemlist[945] += " (2)"; // Used Solarizer
        itemlist[946] += " (2)"; // Used Lunarizer

        itemlist[873] += " (GP/GE)"; // S.S. Ticket
        itemlist[459] += " (HG/SS)"; // Parcel
        itemlist[467] += " (Pt)"; // Secret Key
        itemlist[475] += " (HG/SS)"; // Card Key
        itemlist[894] += " (GP)"; // Leaf Letter
        itemlist[895] += " (GE)"; // Leaf Letter

        // some languages have same names for other items!
        itemlist[878] += " (GP/GE)"; // Lift Key (Elevator Key=700)
        itemlist[479] += " (HG/SS)"; // Lost Item (Dropped Item=636)

        // Append Z-Crystal Key Item differentiator
        foreach (var i in ItemStorage7USUM.Pouch_ZCrystal_USUM)
            itemlist[i] += " [Z]";

        itemlist[0121] += " (1)"; // Pokémon Box Link
        itemlist[1075] += " (2)"; // Pokémon Box Link

        itemlist[1080] += " (SW/SH)"; // Fishing Rod

        itemlist[1081] += " (1)"; // Rotom Bike
        itemlist[1266] += " (2)"; // Rotom Bike
        itemlist[1585] += " (3)"; // Rotom Bike
        itemlist[1586] += " (4)"; // Rotom Bike

        itemlist[1590] += " (1)"; // Reins of Unity
        itemlist[1591] += " (2)"; // Reins of Unity
        itemlist[1607] += " (3)"; // Reins of Unity

        for (int i = 12; i <= 29; i++) // Differentiate DNA Samples
            g3coloitems[500 + i] += $" ({i - 11:00})";
        // differentiate G3 Card Key from Colo
        g3coloitems[500 + 10] += " (COLO)";

        SanitizeItemsLA(itemlist);
        SanitizeItemsSV(itemlist);

        if (lang is "fr")
        {
            itemlist[1681] += " (LA)"; // Galet Noir       dup with 617 (Dark Stone | Black Tumblestone)
            itemlist[1262] += " (G8)"; // Nouilles         dup with 1934 (Instant Noodles | Rice)
            itemlist[1263] += " (G8)"; // Steak Haché      dup with 1925 (Precooked Burger | Herbed Sausage)
        }
        else if (lang is "ja")
        {
            itemlist[1693] += " (LA)"; // むしよけスプレー   dup with 79 (Repel)
            itemlist[1716] += " (LA)"; // ビビリだま        dup with 847 (Adrenaline Orb | Scatter Bang)
            itemlist[1717] += " (LA)"; // けむりだま        dup with 228 (Smoke Ball | Smoke Bomb)
        }

        itemlist[464] += " (G4)"; // Secret Medicine
        itemlist[1763] += " (LA)"; // Secret Medicine
    }

    private static void SanitizeItemsSV(Span<string> items)
    {
        items[2313] += " (1)"; // Academy Bottle
        items[2314] += " (2)"; // Academy Bottle
        items[2318] += " (1)"; // Academy Cup
        items[2319] += " (2)"; // Academy Cup
        items[2323] += " (1)"; // Academy Tablecloth
        items[2324] += " (2)"; // Academy Tablecloth
        items[2329] += " (1)"; // Academy Ball
        items[2330] += " (2)"; // Academy Ball
        items[0694] += " (G6-8)"; // TM100, not held.

        items[2418] += " (SL)"; // Academy Chairs
        items[2419] += " (VL)"; // Academy Chairs

        items[1834] += " (1)"; // Scarlet Book
        items[1835] += " (1)"; // Violet Book
        items[2555] += " (2)"; // Scarlet Book
        items[2556] += " (2)"; // Violet Book
    }

    private static void SanitizeItemsLA(Span<string> items)
    {
        // Recipes
        items[1784] += " (~)"; // Gigaton Ball
        items[1783] += " (~)"; // Leaden Ball
        items[1753] += " (~)"; // Heavy Ball
        items[1752] += " (~)"; // Jet Ball
        items[1751] += " (~)"; // Wing Ball
        items[1731] += " (~)"; // Twice-Spiced Radish
        items[1730] += " (~)"; // Choice Dumpling
        items[1729] += " (~)"; // Swap Snack
        items[1677] += " (~)"; // Aux Powerguard
        items[1676] += " (~)"; // Aux Evasion
        items[1675] += " (~)"; // Dire Hit
        items[1674] += " (~)"; // Aux Guard
        items[1673] += " (~)"; // Aux Power
        items[1671] += " (~)"; // Stealth Spray
        items[1670] += " (~)"; // Max Elixir
        items[1669] += " (~)"; // Max Ether
        items[1668] += " (~)"; // Max Revive
        items[1667] += " (~)"; // Revive
        items[1666] += " (~)"; // Full Heal
        items[1665] += " (~)"; // Jubilife Muffin
        items[1664] += " (~)"; // Old Gateau
        items[1663] += " (~)"; // Superb Remedy
        items[1662] += " (~)"; // Fine Remedy
        items[1661] += " (~)"; // Remedy
        items[1660] += " (~)"; // Full Restore
        items[1659] += " (~)"; // Max Potion
        items[1658] += " (~)"; // Hyper Potion
        items[1657] += " (~)"; // Super Potion
        items[1656] += " (~)"; // Potion
        items[1655] += " (~)"; // Salt Cake
        items[1654] += " (~)"; // Bean Cake
        items[1653] += " (~)"; // Grain Cake
        items[1652] += " (~)"; // Honey Cake
        items[1650] += " (~)"; // Mushroom Cake
        items[1649] += " (~)"; // Star Piece
        items[1648] += " (~)"; // Sticky Glob
        items[1647] += " (~)"; // Scatter Bang
        items[1646] += " (~)"; // Smoke Bomb
        items[1644] += " (~)"; // Pokéshi Doll
        items[1643] += " (~)"; // Feather Ball
        items[1642] += " (~)"; // Ultra Ball
        items[1641] += " (~)"; // Great Ball
        items[1640] += " (~)"; // Poké Ball

        // Items
        items[1616] += " (LA)"; // Dire Hit
        items[1689] += " (LA)"; // Snowball
        items[1710] += " (LA)"; // Poké Ball
        items[1711] += " (LA)"; // Great Ball
        items[1712] += " (LA)"; // Ultra Ball
        items[1748] += " (LA)"; // Heavy Ball

        // Key Items
        items[1622] += " (-)"; // Poké Ball
        items[1765] += " (1)"; // Lost Satchel
        items[1766] += " (2)"; // Lost Satchel
        items[1767] += " (3)"; // Lost Satchel
        items[1768] += " (4)"; // Lost Satchel
        items[1769] += " (5)"; // Lost Satchel
    }

    private void SanitizeMetLocations()
    {
        // Fix up some of the Location strings to make them more descriptive
        SanitizeMetGen4(Gen4);
        SanitizeMetGen5(Gen5);
        SanitizeMetGen6(Gen6);
        SanitizeMetGen7(Gen7);
        SanitizeMetGen7b(Gen7b);
        SanitizeMetGen8(Gen8);
        SanitizeMetGen8a(Gen8a);
        SanitizeMetGen8b(Gen8b);
        SanitizeMetGen9(Gen9);

        if (lang is "es" or "it")
        {
            // Campeonato Mundial duplicates
            for (int i = 28; i < 35; i++)
                Gen6.Met4[i] += " (-)";

            // Evento de Videojuegos -- first as duplicate
            Gen6.Met4[35] += " (-)";
            Gen7.Met4[38] += " (-)";
            Gen7b.Met4[27] += " (-)";
        }

        if (lang == "ko")
        {
            // Pokémon Ranger duplicate (should be Ranger Union)
            Gen5.Met4[71] += " (-)";
        }
    }

    private void SanitizeMetGen4(LocationSet4 set)
    {
        set.Met0[054] += " (D/P/Pt)"; // Victory Road
        set.Met0[221] += " (HG/SS)"; // Victory Road

        // German language duplicate; handle for all since it can be confused.
        set.Met0[104] += " (D/P/Pt)"; // Vista Lighthouse
        set.Met0[212] += " (HG/SS)"; // Lighthouse

        set.Met2[1] += $" ({NPC})";     // Anything from an NPC
        set.Met2[2] += $" ({EggName})"; // Egg From Link Trade
    }

    private void SanitizeMetGen5(LocationSet6 set)
    {
        set.Met0[36] = $"{set.Met0[84]}/{set.Met0[36]}"; // Cold Storage in B/W = PWT in B2/W2
        set.Met0[40] += " (B/W)"; // Victory Road in B/W
        set.Met0[134] += " (B2/W2)"; // Victory Road in B2/W2
        // B2/W2 Entries from 76 to 105 are for Entralink in B/W
        for (int i = 76; i < 106; i++)
            set.Met0[i] += "●";

        // Collision between 40002 (legal) and 00002 (illegal) "Faraway place"
        if (set.Met0[2] == set.Met4[2])
            set.Met0[2] += " (2)";

        for (int i = 97; i < 109; i++)
            set.Met4[i] += $" ({i - 97})";

        // Localize the Poketransfer to the language (30001)
        set.Met3[1] = GameLanguage.GetTransporterName(LanguageIndex);
        set.Met3[2] += $" ({NPC})";             // Anything from an NPC
        set.Met3[3] += $" ({EggName})";         // Link Trade (Egg)

        // Zorua/Zoroark events
        set.Met3[10] = $"{specieslist[251]} ({specieslist[570]} 1)"; // Celebi's Zorua Event
        set.Met3[11] = $"{specieslist[251]} ({specieslist[570]} 2)"; // Celebi's Zorua Event
        set.Met3[12] = $"{specieslist[571]} (1)"; // Zoroark
        set.Met3[13] = $"{specieslist[571]} (2)"; // Zoroark

        set.Met6[3] += $" ({EggName})";  // Egg Treasure Hunter/Breeder, whatever...
    }

    private void SanitizeMetGen6(LocationSet6 set)
    {
        // Add in the sub-location if available.
        for (int i = 8; i <= 136; i += 2)
        {
            var nextLoc = set.Met0[i + 1];
            if (nextLoc.Length == 0)
                continue;
            set.Met0[i + 1] = string.Empty;
            set.Met0[i] += $" ({nextLoc})";
        }

        set.Met0[104] += " (X/Y)";      // Victory Road
        set.Met0[106] += " (X/Y)";      // Pokémon League
        set.Met0[202] += " (OR/AS)";    // Pokémon League
        set.Met0[298] += " (OR/AS)";    // Victory Road
        set.Met3[1] += $" ({NPC})";     // Anything from an NPC
        set.Met3[2] += $" ({EggName})"; // Egg From Link Trade

        for (int i = 63; i <= 69; i++)
            set.Met4[i] += $" ({i - 62})";
    }

    private void SanitizeMetGen7(LocationSet6 set)
    {
        // Sun/Moon duplicates -- elaborate!
        for (int i = 6; i < set.Met0.Length; i += 2)
        {
            if (i is >= 194 and < 198)
                continue; // Skip Island Names (unused)
            var nextLoc = set.Met0[i + 1];
            if (nextLoc.Length == 0)
                continue;
            set.Met0[i + 1] = string.Empty;
            set.Met0[i] += $" ({nextLoc})";
        }
        set.Met0[32] += " (2)";
        set.Met0[102] += " (2)";

        set.Met3[1] += $" ({NPC})";      // Anything from an NPC
        set.Met3[2] += $" ({EggName})";  // Egg From Link Trade
        for (int i = 3; i <= 6; i++) // distinguish first set of regions (unused) from second (used)
            set.Met3[i] += " (-)";

        for (int i = 59; i < 66; i++) // distinguish Event year duplicates
            set.Met4[i] += " (-)";
    }

    private static void SanitizeMetGen7b(LocationSet6 set)
    {
        for (int i = 48; i < 55; i++) // distinguish Event year duplicates
            set.Met4[i] += " (-)";
    }

    private void SanitizeMetGen8(LocationSet6 set)
    {
        // SW/SH duplicates -- elaborate!
        for (int i = 88; i < set.Met0.Length; i += 2)
        {
            var nextLoc = set.Met0[i + 1];
            if (nextLoc.Length == 0)
                continue;
            set.Met0[i + 1] = string.Empty;
            set.Met0[i] += $" ({nextLoc})";
        }

        set.Met3[1] += $" ({NPC})";      // Anything from an NPC
        set.Met3[2] += $" ({EggName})";  // Egg From Link Trade
        for (int i = 3; i <= 6; i++) // distinguish first set of regions (unused) from second (used)
            set.Met3[i] += " (-)";
        set.Met3[19] += " (?)"; // Kanto for the third time

        for (int i = 55; i < 61; i++) // distinguish Event year duplicates
            set.Met4[i] += " (-)";
        set.Met4[30] += " (-)"; // a Video game Event (in spanish etc.) -- duplicate with line 39
        set.Met4[53] += " (-)"; // a Pokémon event -- duplicate with line 37

        set.Met4[81] += " (-)"; // Pokémon GO -- duplicate with 30000's entry
        set.Met4[86] += " (-)"; // Pokémon HOME -- duplicate with 30000's entry
     // set.Met3[12] += " (-)"; // Pokémon GO -- duplicate with 40000's entry
     // set.Met3[18] += " (-)"; // Pokémon HOME -- duplicate with 40000's entry
    }

    private void SanitizeMetGen8b(LocationSet6 set)
    {
        set.Met3[1] += $" ({NPC})";      // Anything from an NPC
        set.Met3[2] += $" ({EggName})";  // Egg From Link Trade

        Deduplicate(set.Met0, 00000);
        Deduplicate(set.Met3, 30000);
        Deduplicate(set.Met4, 40000);
        Deduplicate(set.Met6, 60000);
    }

    private void SanitizeMetGen8a(LocationSet6 set)
    {
        set.Met0[31] += " (2)"; // in Floaro Gardens
        set.Met3[1] += $" ({NPC})";      // Anything from an NPC
        set.Met3[2] += $" ({EggName})";  // Egg From Link Trade
        for (int i = 3; i <= 6; i++) // distinguish first set of regions (unused) from second (used)
            set.Met3[i] += " (-)";
        set.Met3[19] += " (?)"; // Kanto for the third time

        set.Met4[30] += " (-)"; // a Video game Event (in spanish etc.) -- duplicate with line 39
        set.Met4[53] += " (-)"; // a Pokémon event -- duplicate with line 37

        set.Met4[81] += " (-)"; // Pokémon GO -- duplicate with 30000's entry
        set.Met4[86] += " (-)"; // Pokémon HOME -- duplicate with 30000's entry
     // set.Met3[12] += " (-)"; // Pokémon GO -- duplicate with 40000's entry
     // set.Met3[18] += " (-)"; // Pokémon HOME -- duplicate with 40000's entry

        for (int i = 55; i <= 60; i++) // distinguish second set of YYYY Event from the first
            set.Met4[i] += " (-)";

        if (lang is "en" or "es" or "de" or "it" or "fr")
        {
            // Final four locations are not nouns, rather the same location reference (at the...) as prior entries.
            set.Met0[152] += " (152)"; // Galaxy Hall
            set.Met0[153] += " (153)"; // Front Gate
            set.Met0[154] += " (154)"; // Farm
            set.Met0[155] += " (155)"; // Training Grounds
        }
    }

    private void SanitizeMetGen9(LocationSet6 set)
    {
        var m = set.Met0;
        m[110] += " (1)"; // Area Zero
        m[112] += " (2)"; // Area Zero
        m[114] += " (3)"; // Area Zero
        m[116] += " (4)"; // Area Zero
        m[124] += " (5)"; // Area Zero
        m[126] += " (6)"; // Area Zero
        m[128] += " (7)"; // Area Zero

        m[040] += " (1)"; // Casseroya Lake
        m[108] += " (2)"; // Casseroya Lake
        m[034] += " (1)"; // East Province (Area One)
        m[104] += " (2)"; // East Province (Area One)
        m[038] += " (1)"; // Glaseado Mountain
        m[042] += " (2)"; // Glaseado Mountain
        m[068] += " (3)"; // Glaseado Mountain
        m[008] += " (1)"; // Mesagoza
        m[072] += " (2)"; // Mesagoza
        m[074] += " (3)"; // Mesagoza
        m[044] += " (1)"; // North Province (Area Three)
        m[102] += " (2)"; // North Province (Area Three)
        m[047] += " (1)"; // North Province (Area Two)
        m[098] += " (2)"; // North Province (Area Two)
        m[016] += " (1)"; // South Province (Area Six)
        m[066] += " (2)"; // South Province (Area Six)
        m[030] += " (1)"; // Tagtree Thicket
        m[106] += " (2)"; // Tagtree Thicket
        m[022] += " (1)"; // West Province (Area One)
        m[100] += " (2)"; // West Province (Area One)
        m[052] += " (1)"; // Zero Gate
        m[054] += " (2)"; // Zero Gate
        m[118] += " (1)"; // Zero Lab
        m[120] += " (2)"; // Zero Lab
        m[122] += " (3)"; // Zero Lab

        m[144] += " (1)"; // Oni Mountain
        m[147] += " (2)"; // Oni Mountain
        m[149] += " (3)"; // Oni Mountain
        m[150] += " (4)"; // Oni Mountain
        m[169] += " (5)"; // Oni Mountain

        m[152] += " (1)"; // Crystal Pool
        m[154] += " (2)"; // Crystal Pool
        m[153] += " (1)"; // Oni Mountain Summit
        m[155] += " (2)"; // Oni Mountain Summit
        m[164] += " (1)"; // Kitakami Wilds
        m[167] += " (2)"; // Kitakami Wilds

        m[196] += " (1)"; // Area Zero Underdepths
        m[198] += " (2)"; // Area Zero Underdepths

        set.Met3[1] += $" ({NPC})";      // Anything from an NPC
        set.Met3[2] += $" ({EggName})";  // Egg From Link Trade
        for (int i = 3; i <= 6; i++) // distinguish first set of regions (unused) from second (used)
            set.Met3[i] += " (-)";
        set.Met3[19] += " (?)"; // Kanto for the third time

        for (int i = 49; i <= 54; i++) // distinguish Event year duplicates
            set.Met4[i] += " (-)";

        set.Met4[27] += " (-)"; // a Video game Event (in spanish etc.) -- duplicate with line 36
        set.Met4[48] += " (-)"; // a Pokémon event -- duplicate with line 34

        set.Met4[73] += " (-)"; // Pokémon GO -- duplicate with 30000's entry
        set.Met4[78] += " (-)"; // Pokémon HOME -- duplicate with 30000's entry
     // set.Met3[12] += " (-)"; // Pokémon GO -- duplicate with 40000's entry
     // set.Met3[18] += " (-)"; // Pokémon HOME -- duplicate with 40000's entry
    }

    private static void Deduplicate(Span<string> arr, int group)
    {
        var counts = new Dictionary<string, int>();

        foreach (var s in arr)
        {
            counts.TryGetValue(s, out var value);
            counts[s] = value + 1;
        }

#if !DEBUG
        var maxCounts = new Dictionary<string, int>(counts);
#endif
        for (var i = arr.Length - 1; i >= 0; i--)
        {
#if DEBUG
            arr[i] += $" ({group + i:00000})";
#else
            var s = arr[i];
            var count = counts[s]--;
            if (count == 1)
                continue;
            var format = maxCounts[s] switch
            {
                >= 100 => " ({0:000})",
                >=  10 => " ({0:00})",
                     _ => " ({0})",
            };
            arr[i] += string.Format(format, count);
#endif
        }
    }

    public string[] GetItemStrings(EntityContext context, GameVersion game = GameVersion.Any) => context switch
    {
        EntityContext.Gen1 => g1items,
        EntityContext.Gen2 => g2items,
        EntityContext.Gen3 => GetItemStrings3(game),
        EntityContext.Gen4 => g4items, // mail names changed 4->5
        EntityContext.Gen8b => GetItemStrings8b(),
        EntityContext.Gen9 => GetItemStrings9(),
        _ => itemlist,
    };

    private string[] GetItemStrings8b()
    {
        // Item Indexes 
        var clone = (string[])itemlist.Clone();
        var tm = clone[419][..2];
        for (int i = 420; i <= 427; i++)
            clone[i] = $"{tm}{i - 420 + 93}";

        clone[618] += "(-)"; // TM93
        clone[619] += "(-)"; // TM94
        clone[620] += "(-)"; // TM95
        clone[690] += "(-)"; // TM96
        clone[691] += "(-)"; // TM97
        clone[692] += "(-)"; // TM98
        clone[693] += "(-)"; // TM99
        clone[694] += "(-)"; // TM100
        return clone;
    }

    private string[] GetItemStrings9()
    {
        // in Generation 9, TM #'s are padded to 3 digits; format them appropriately here
        var clone = (string[])itemlist.Clone();
        var span = clone.AsSpan();
        var zero = lang is "ja" or "zh" or "zh2" ? "０" : "0";
        InsertZero(span[328..420], zero); // 01-92
        InsertZero(span[618..621], zero); // 93-95
        InsertZero(span[690..694], zero); // 96-99
        return clone;

        static void InsertZero(Span<string> arr, string insert)
        {
            foreach (ref var str in arr)
                str = str.Insert(str.Length - 2, insert);
        }
    }

    private string[] GetItemStrings3(GameVersion game)
    {
        switch (game)
        {
            case GameVersion.COLO:
                return g3coloitems;
            case GameVersion.XD:
                return g3xditems;
            default:
                if (EReaderBerrySettings.IsEnigma)
                    return g3items;

                var g3ItemsWithEBerry = (string[])g3items.Clone();
                g3ItemsWithEBerry[175] = EReaderBerrySettings.DisplayName;
                return g3ItemsWithEBerry;
        }
    }

    /// <summary>
    /// Gets the location name for the specified parameters.
    /// </summary>
    /// <param name="isEggLocation">Location is from the <see cref="PKM.EggLocation"/></param>
    /// <param name="location">Location value</param>
    /// <param name="format">Current <see cref="PKM.Format"/></param>
    /// <param name="generation"><see cref="PKM.Generation"/> of origin</param>
    /// <param name="version">Current GameVersion (only applicable for <see cref="GameVersion.Gen7b"/> differentiation)</param>
    /// <returns>Location name. Potentially an empty string if no location name is known for that location value.</returns>
    public string GetLocationName(bool isEggLocation, ushort location, byte format, byte generation, GameVersion version)
    {
        if (format == 1)
        {
            // Legality binaries have Location IDs that were manually remapped to Gen3 location IDs.
            if (location == 0)
                return string.Empty;
            return Gen3.GetLocationName(location);
        }

        generation = GetGeneration(generation, isEggLocation, format);
        var set = GetLocations(generation, version);
        if (set is null)
            return string.Empty;
        return set.GetLocationName(location);
    }

    private static byte GetGeneration(byte generation, bool isEggLocation, byte format)
    {
        if (format == 2)
            return 2;
        if (format == 3)
            return 3;
        if (generation == 4 && (isEggLocation || format == 4))
            return 4;
        if (generation >= 5)
            return generation;
        if (format >= 5)
            return format;
        return 0; // Nonsensical inputs.
    }

    /// <summary>
    /// Gets the location names array for a specified generation.
    /// </summary>
    /// <param name="generation">Generation to get location names for.</param>
    /// <param name="version">Version of origin</param>
    /// <returns>List of location names.</returns>
    public ILocationSet? GetLocations(byte generation, GameVersion version) => generation switch
    {
        2 => Gen2,
        3 => version is (GameVersion.COLO or GameVersion.XD or GameVersion.CXD) ? CXD : Gen3,
        4 => Gen4,
        5 => Gen5,
        6 => Gen6,
        7 => GameVersion.Gen7b.Contains(version) ? Gen7b : Gen7,

        8 when version is GameVersion.PLA => Gen8a,
        8 when GameVersion.BDSP.Contains(version) => Gen8b,
        8 => Gen8,
        9 => Gen9,

        _ => null,
    };

    /// <summary>
    /// Gets the location names array for a specified generation.
    /// </summary>
    /// <param name="generation">Generation to get location names for.</param>
    /// <param name="version">Version of origin</param>
    /// <param name="bankID">BankID used to choose the text bank.</param>
    /// <returns>List of location names.</returns>
    public ReadOnlySpan<string> GetLocationNames(byte generation, GameVersion version, int bankID = 0)
    {
        var set = GetLocations(generation, version);
        if (set is null)
            return [];
        return set.GetLocationNames(bankID);
    }
}
