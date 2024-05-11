using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for interacting with Pokédex Form flags
/// </summary>
public static class DexFormUtil
{
    public static int GetDexFormIndexSM(ushort species, byte formCount)
        => GetDexFormBitIndex(species, formCount, DexSpeciesWithForm_SM, DexSpeciesCount_SM);
    public static int GetDexFormIndexUSUM(ushort species, byte formCount)
        => GetDexFormBitIndex(species, formCount, DexSpeciesWithForm_USUM, DexSpeciesCount_USUM);
    public static int GetDexFormIndexGG(ushort species, byte formCount)
        => GetDexFormBitIndex(species, formCount, DexSpeciesWithForm_GG, DexSpeciesCount_GG);

    public static int GetDexFormCountSM(ushort species)
    {
        var index = DexSpeciesWithForm_SM.BinarySearch(species);
        if (index < 0)
            return 0;
        return DexSpeciesCount_SM[index];
    }

    public static int GetDexFormCountUSUM(ushort species)
    {
        var index = DexSpeciesWithForm_USUM.BinarySearch(species);
        if (index < 0)
            return 0;
        return DexSpeciesCount_USUM[index];
    }

    public static int GetDexFormCountGG(ushort species)
    {
        var index = DexSpeciesWithForm_GG.BinarySearch(species);
        if (index < 0)
            return 0;
        return DexSpeciesCount_GG[index];
    }

    private static ReadOnlySpan<ushort> DexSpeciesWithForm_SM =>
    [
        003, 006, 009, 015, 018, 019, 020, 025, 026, 027, 028, 037, 038, 050, 051, 052,
        053, 065, 074, 075, 076, 080, 088, 089, 094, 103, 105, 115, 127, 130, 142, 150,
        181, 201, 208, 212, 214, 229, 248, 254, 257, 260, 282, 302, 303, 306, 308, 310,
        319, 323, 334, 351, 354, 359, 362, 373, 376, 380, 381, 382, 383, 384, 386, 412,
        413, 421, 422, 423, 428, 445, 448, 460, 475, 479, 487, 492, 493, 531, 550, 555,
        585, 586, 641, 642, 645, 646, 647, 648, 649, 658, 666, 669, 670, 671, 676, 678,
        681, 710, 711, 716, 718, 719, 720, 735, 738, 741, 745, 746, 754, 758, 773, 774,
        778, 784, 801,
    ];

    private static ReadOnlySpan<byte> DexSpeciesCount_SM =>
    [
        02,03,02,02,02,02,03,07,02,02,02,02,02,02,02,02,
        02,02,02,02,02,02,02,02,02,02,02,02,02,02,02,03,
        02,28,02,02,02,02,02,02,02,02,02,02,02,02,02,02,
        02,02,02,04,02,02,02,02,02,02,02,02,02,02,04,03,
        03,02,02,02,02,02,02,02,02,06,02,02,18,02,02,02,
        04,04,02,02,02,03,02,02,05,03,20,05,06,05,10,02,
        02,04,04,02,05,02,02,02,02,04,02,02,02,02,18,14,
        04,02,02,
    ];

    private static ReadOnlySpan<ushort> DexSpeciesWithForm_USUM =>
    [
        003, 006, 009, 015, 018, 019, 020, 025, 026, 027, 028, 037, 038, 050, 051, 052,
        053, 065, 074, 075, 076, 080, 088, 089, 094, 103, 105, 115, 127, 130, 142, 150,
        181, 201, 208, 212, 214, 229, 248, 254, 257, 260, 282, 302, 303, 306, 308, 310,
        319, 323, 334, 351, 354, 359, 362, 373, 376, 380, 381, 382, 383, 384, 386, 412,
        413, 414, 421, 422, 423, 428, 445, 448, 460, 475, 479, 487, 492, 493, 531, 550,
        555, 585, 586, 641, 642, 645, 646, 647, 648, 649, 658, 664, 665, 666, 669, 670,
        671, 676, 678, 681, 710, 711, 716, 718, 719, 720, 735, 738, 741, 743, 744, 745,
        746, 752, 754, 758, 773, 774, 777, 778, 784, 800, 801,
    ];

    private static ReadOnlySpan<byte> DexSpeciesCount_USUM =>
    [
        02,03,02,02,02,02,03,08,02,02,02,02,02,02,02,02,
        02,02,02,02,02,02,02,02,02,02,03,02,02,02,02,03,
        02,28,02,02,02,02,02,02,02,02,02,02,02,02,02,02,
        02,02,02,04,02,02,02,02,02,02,02,02,02,02,04,03,
        03,03,02,02,02,02,02,02,02,02,06,02,02,18,02,02,
        02,04,04,02,02,02,03,02,02,05,03,20,20,20,05,06,
        05,10,02,02,04,04,02,05,02,02,02,02,04,02,02,03,
        02,02,02,02,18,14,02,04,02,04,02,
    ];

    private static ReadOnlySpan<ushort> DexSpeciesWithForm_GG =>
    [
        003, 006, 009, 015, 018, 019, 020, 025, 026, 027, 028, 037, 038, 050, 051, 052,
        053, 065, 074, 075, 076, 080, 088, 089, 094, 103, 105, 115, 127, 130, 142, 150,
    ];

    private static ReadOnlySpan<byte> DexSpeciesCount_GG =>
    [
        02,03,02,02,02,02,03,09,02,02,02,02,02,02,02,02,
        02,02,02,02,02,02,02,02,02,02,03,02,02,02,02,03,
    ];

    private static int GetDexFormBitIndex(ushort species, byte formCount, ReadOnlySpan<ushort> withForm, ReadOnlySpan<byte> count)
    {
        var index = withForm.BinarySearch(species);
        if (index < 0)
            return -1;

        var c = count[index];
        if (c > formCount)
            return -1;

        // account for base form (0) occupying the arr[species] indexes...
        int prior = -index; // -1 * (count)
        foreach (var p in count[..index])
            prior += p;
        return prior;
    }

    public static int GetDexFormIndexBW(ushort species, byte formCount)
    {
        if (formCount < 1 || species > Legal.MaxSpeciesID_5)
            return -1; // invalid
        return species switch
        {
            201 => 000, // 28 Unown
            386 => 028, // 4 Deoxys
            492 => 032, // 2 Shaymin
            487 => 034, // 2 Giratina
            479 => 036, // 6 Rotom
            422 => 042, // 2 Shellos
            423 => 044, // 2 Gastrodon
            412 => 046, // 3 Burmy
            413 => 049, // 3 Wormadam
            351 => 052, // 4 Castform
            421 => 056, // 2 Cherrim
            585 => 058, // 4 Deerling
            586 => 062, // 4 Sawsbuck
            648 => 066, // 2 Meloetta
            555 => 068, // 2 Darmanitan
            550 => 070, // 2 Basculin
            _ => -1,
        };
    }

    public static int GetDexFormIndexB2W2(ushort species, byte formCount)
    {
        if (formCount < 1 || species > Legal.MaxSpeciesID_5)
            return -1; // invalid
        return species switch
        {
            646 => 072, // 3 Kyurem
            647 => 075, // 2 Keldeo
            642 => 077, // 2 Thundurus
            641 => 079, // 2 Tornadus
            645 => 081, // 2 Landorus
            _ => GetDexFormIndexBW(species, formCount),
        };
    }

    public static int GetDexFormIndexXY(ushort species, byte formCount)
    {
        if (formCount < 1 || species > Legal.MaxSpeciesID_6)
            return -1; // invalid
        return species switch
        {
            666 => 083, // 20 Vivillion
            669 => 103, // 5 Flabébé
            670 => 108, // 6 Floette
            671 => 114, // 5 Florges
            710 => 119, // 4 Pumpkaboo
            711 => 123, // 4 Gourgeist
            681 => 127, // 2 Aegislash
            716 => 129, // 2 Xerneas
            003 => 131, // 2 Venusaur
            006 => 133, // 3 Charizard
            009 => 136, // 2 Blastoise
            065 => 138, // 2 Alakazam
            094 => 140, // 2 Gengar
            115 => 142, // 2 Kangaskhan
            127 => 144, // 2 Pinsir
            130 => 146, // 2 Gyarados
            142 => 148, // 2 Aerodactyl
            150 => 150, // 3 Mewtwo
            181 => 153, // 2 Ampharos
            212 => 155, // 2 Scizor
            214 => 157, // 2 Heracros
            229 => 159, // 2 Houndoom
            248 => 161, // 2 Tyranitar
            257 => 163, // 2 Blaziken
            282 => 165, // 2 Gardevoir
            303 => 167, // 2 Mawile
            306 => 169, // 2 Aggron
            308 => 171, // 2 Medicham
            310 => 173, // 2 Manetric
            354 => 175, // 2 Banette
            359 => 177, // 2 Absol
            380 => 179, // 2 Latias
            381 => 181, // 2 Latios
            445 => 183, // 2 Garchomp
            448 => 185, // 2 Lucario
            460 => 187, // 2 Abomasnow
            _ => GetDexFormIndexB2W2(species, formCount),
        };
    }

    public static int GetDexFormIndexORAS(ushort species, byte formCount)
    {
        if (formCount < 1 || species > Legal.MaxSpeciesID_6)
            return -1; // invalid
        return species switch
        {
            025 => 189, // 7 Pikachu
            720 => 196, // 2 Hoopa
            015 => 198, // 2 Beedrill
            018 => 200, // 2 Pidgeot
            080 => 202, // 2 Slowbro
            208 => 204, // 2 Steelix
            254 => 206, // 2 Sceptile
            260 => 208, // 2 Swampert
            302 => 210, // 2 Sableye
            319 => 212, // 2 Sharpedo
            323 => 214, // 2 Camerupt
            334 => 216, // 2 Altaria
            362 => 218, // 2 Glalie
            373 => 220, // 2 Salamence
            376 => 222, // 2 Metagross
            384 => 224, // 2 Rayquaza
            428 => 226, // 2 Lopunny
            475 => 228, // 2 Gallade
            531 => 230, // 2 Audino
            719 => 232, // 2 Diancie
            382 => 234, // 2 Kyogre
            383 => 236, // 2 Groudon
            493 => 238, // 18 Arceus
            649 => 256, // 5 Genesect
            676 => 261, // 10 Furfrou
            _ => GetDexFormIndexXY(species, formCount),
        };
    }
}
