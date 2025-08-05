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
}
