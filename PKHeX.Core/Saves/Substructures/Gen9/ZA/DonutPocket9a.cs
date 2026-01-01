using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class DonutPocket9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public const int MaxCount = 999;

    public Donut9a GetDonut(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>((uint)index, MaxCount);
        var slice = Raw.Slice(index * Donut9a.Size, Donut9a.Size);
        return new Donut9a(slice);
    }

    public void Compress()
    {
        // Remove slots where donut is empty, shift entry up.
        int writePos = 0;
        for (int readPos = 0; readPos < MaxCount; readPos++)
        {
            var read = GetDonut(readPos);
            if (read.IsEmpty)
                continue;
            if (writePos != readPos)
            {
                var write = GetDonut(writePos);
                read.CopyTo(write);
                read.Clear(); // Clear old slot
            }
            writePos++;
        }
    }

    private static ReadOnlySpan<byte> Template =>
    [
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x54, 0x96, 0x00, 0x10, 0x0E, 0x74, 0x0A,
        0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A, 0x74, 0x0A,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x15, 0xD4, 0xEC, 0xA6, 0x6A, 0x53, 0x37, 0x0D,
        0x2E, 0xA0, 0x7F, 0xD6, 0xA0, 0x1E, 0xCF, 0xAD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    ];

    public void SetAllAsShinyTemplate()
    {
        var rand = Util.Rand;
        var flavorDict = DonutInfo.Flavors.ToDictionary(z => z.Name, z => z.Hash);

        for (int i = 0; i < MaxCount; i++)
        {
            // Apply template data
            var entry = GetDonut(i);
            Template.CopyTo(entry.Data);

            // Update donut with data to match what we want.
            ApplyShinySizeCatch(entry, rand, flavorDict);

            // Update creation time
            ApplyTimestampNow(entry, i);
        }
    }

    public void SetAllRandomLv3()
    {
        var rand = Util.Rand;
        var flavorDict = DonutInfo.Flavors.Where(z => z.Name.EndsWith("lv3")).ToArray();

        for (int i = 0; i < MaxCount; i++)
        {
            // Apply template data
            var entry = GetDonut(i);
            Template.CopyTo(entry.Data);

            // Update donut with data to match what we want.
            rand.Shuffle(flavorDict);
            entry.Flavor0 = flavorDict[0].Hash;
            entry.Flavor1 = flavorDict[1].Hash;
            entry.Flavor2 = flavorDict[2].Hash;

            // Update creation time
            ApplyTimestampNow(entry, i);
        }
    }

    private static void ApplyTimestampNow(Donut9a entry, int bias = 0)
    {
        var now = DateTime.Now;
        if (bias != 0) // some fudge factor to differentiate donuts made on the same millisecond
            now = now.AddMilliseconds(bias);
        var ticks = (ulong)(now - Donut9a.Epoch).TotalMilliseconds;
        entry.MillisecondsSince1900 = ticks;
        entry.DateTime1900.Timestamp = now;
    }

    private static void ApplyShinySizeCatch(Donut9a entry, Random rand, Dictionary<string, ulong> flavorDict)
    {
        // Shiny power is sweet 3-21. Allow "all" rather than just be single type.
        var type = rand.Next(17 + 1 + 1);
        var flavor0 = $"sweet_{type + 3:00}_lv3"; // Sparkling (sweet_03-21)

        var roll2 = rand.Next(3);
        var flavor1 = roll2 switch
        {
            0 or 1 => $"fresh_{1 + roll2:00}_lv3", // Humungo (fresh_01) or Teensy (fresh_02)
            _ => "sweet_01_lv3", // Alpha
        };
        var flavor2 = $"fresh_{type + 4:00}_lv3"; // Catching Power (fresh_04-22)

        // Set flavors to donut
        entry.Flavor0 = flavorDict[flavor0];
        entry.Flavor1 = flavorDict[flavor1];
        entry.Flavor2 = flavorDict[flavor2];
    }

    public void CloneAllFromIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>((uint)index, MaxCount - 1);
        var source = GetDonut(index);
        for (int i = 0; i < MaxCount; i++)
        {
            if (i != index)
                source.CopyTo(GetDonut(i));
        }
    }
}

public static class DonutInfo
{
    public static IReadOnlyList<DonutBerryDetail> Berries { get; } =
    [
        new(0149, 0  ,10, 0 , 0 , 0 , 0 , 1 , 60),
        new(0150, 1  ,0 , 10, 0 , 0 , 0 , 1 , 60),
        new(0151, 2  ,0 , 0 , 10, 0 , 0 , 1 , 60),
        new(0152, 3  ,0 , 0 , 0 , 10, 0 , 1 , 60),
        new(0153, 4  ,0 , 0 , 0 , 0 , 10, 1 , 60),
        new(0155, 5  ,5 , 5 , 0 , 5 , 5 , 1 , 60),
        new(0156, 6  ,5 , 5 , 5 , 0 , 5 , 1 , 60),
        new(0157, 7  ,5 , 5 , 5 , 5 , 0 , 2 , 65),
        new(0158, 8  ,0 , 5 , 5 , 5 , 5 , 2 , 65),
        new(0169, 9  ,10, 0 , 10, 10, 0 , 2 , 65),
        new(0170, 10 ,0 , 10, 0 , 10, 10, 2 , 65),
        new(0171, 11 ,10, 0 , 10, 0 , 10, 2 , 65),
        new(0172, 12 ,10, 10, 0 , 10, 0 , 2 , 65),
        new(0173, 13 ,0 , 10, 10, 0 , 10, 2 , 65),
        new(0174, 14 ,15, 10, 0 , 0 , 0 , 2 , 65),
        new(0184, 15 ,15, 0 , 10, 0 , 0 , 3 , 70),
        new(0185, 16 ,0 , 15, 0 , 10, 0 , 3 , 70),
        new(0186, 17 ,0 , 0 , 15, 0 , 10, 3 , 70),
        new(0187, 18 ,10, 0 , 0 , 15, 0 , 3 , 70),
        new(0188, 19 ,0 , 10, 0 , 0 , 15, 3 , 70),
        new(0189, 20 ,15, 0 , 0 , 10, 0 , 3 , 70),
        new(0190, 21 ,0 , 15, 0 , 0 , 10, 3 , 70),
        new(0191, 22 ,10, 0 , 15, 0 , 0 , 3 , 70),
        new(0192, 23 ,0 , 10, 0 , 15, 0 , 3 , 70),
        new(0193, 24 ,0 , 0 , 10, 0 , 15, 3 , 70),
        new(0194, 25 ,20, 0 , 0 , 0 , 10, 3 , 70),
        new(0195, 26 ,10, 20, 0 , 0 , 0 , 3 , 70),
        new(0196, 27 ,0 , 10, 20, 0 , 0 , 3 , 70),
        new(0197, 28 ,0 , 0 , 10, 20, 0 , 3 , 70),
        new(0198, 29 ,0 , 0 , 0 , 10, 20, 3 , 70),
        new(0199, 30 ,25, 10, 0 , 0 , 0 , 3 , 70),
        new(0200, 31 ,0 , 25, 10, 0 , 0 , 3 , 70),
        new(0686, 32 ,0 , 0 , 25, 10, 0 , 3 , 70),
        new(2651, 0 , 40, 0 , 0 , 0 , 0 , 5 , 80),
        new(2652, 1 , 0 , 40, 0 , 0 , 0 , 3 , 100),
        new(2653, 2 , 0 , 0 , 40, 0 , 0 , 2 , 100),
        new(2654, 3 , 0 , 0 , 0 , 40, 0 , 3 , 110),
        new(2655, 4 , 0 , 0 , 0 , 0 , 40, 4 , 90),
        new(2656, 5 , 20, 0 , 10, 15, 15, 6 , 90),
        new(2657, 6 , 15, 20, 0 , 10, 15, 4 , 110),
        new(2658, 7 , 15, 15, 20, 0 , 10, 3 , 110),
        new(2659, 8 , 10, 15, 15, 20, 0 , 4 , 120),
        new(2660, 9 , 35, 5 , 30, 0 , 0 , 7 , 140),
        new(2661, 10, 0 , 35, 5 , 30, 0 , 5 , 160),
        new(2662, 11, 0 , 0 , 35, 5 , 30, 4 , 160),
        new(2663, 12, 5 , 30, 0 , 0 , 35, 6 , 150),
        new(2664, 13, 60, 5 , 0 , 0 , 25, 8 , 140),
        new(2665, 14, 25, 60, 5 , 0 , 0 , 6 , 180),
        new(2666, 15, 0 , 25, 60, 5 , 0 , 5 , 180),
        new(2667, 16, 0 , 0 , 25, 60, 5 , 6 , 200),
        new(2668, 17, 5 , 0 , 0 , 25, 60, 7 , 160),
        new(2669, 18, 55, 25, 15, 5 , 0 , 9 , 210),
        new(2670, 19, 0 , 55, 25, 15, 5 , 7 , 250),
        new(2671, 20, 5 , 0 , 55, 25, 15, 6 , 250),
        new(2672, 21, 15, 5 , 0 , 55, 25, 7 , 270),
        new(2673, 22, 25, 15, 5 , 0 , 55, 8 , 230),
        new(2674, 23, 95, 5 , 10, 10, 0 , 10, 240),
        new(2675, 24, 0 , 95, 5 , 10, 10, 8 , 300),
        new(2676, 25, 10, 0 , 95, 5 , 10, 7 , 300),
        new(2677, 26, 10, 10, 0 , 95, 5 , 8 , 330),
        new(2678, 27, 5 , 10, 10, 0 , 95, 9 , 270),
        new(2679, 28, 0 , 65, 85, 0 , 0 , 8 , 370),
        new(2680, 29, 0 , 85, 0 , 0 , 65, 9 , 370),
        new(2681, 30, 0 , 0 , 0 , 85, 65, 9 , 400),
        new(2682, 31, 85, 0 , 0 , 65, 0 , 9 , 370),
        new(2683, 32, 65, 0 , 0 , 0 , 85, 10, 340),
    ];

    /// <summary>
    /// Recalculate the donut stats based on its berries.
    /// </summary>
    /// <param name="donut">The donut to recalculate.</param>
    public static void RecalculateDonutStats(this Donut9a donut)
    {
        // sum up the stats for each berry
        var boost = 1;
        var calories = 0;
        var berries = donut.GetBerries();
        var flavorScore = 0;
        foreach (var berry in berries)
        {
            if (!TryGetBerry(berry, out var detail))
                continue;
            calories += detail.Calories;
            boost += detail.Boost;
            flavorScore += detail.FlavorScore;
        }

        donut.Calories = (ushort)Math.Min(calories, 9999);
        donut.LevelBoost = (byte)boost;
        donut.Stars = GetDonutStarCount(flavorScore);
    }

    public static byte GetDonutStarCount(int flavorScore) => flavorScore switch
    {
        >= 960 => 5,
        >= 700 => 4,
        >= 360 => 3,
        >= 240 => 2,
        >= 120 => 1,
        _ => 0
    };

    public static void RecalculateDonutFlavors(this Donut9a donut, Span<int> flavors)
    {
        var berries = donut.GetBerries();
        foreach (var berry in berries)
        {
            if (!TryGetBerry(berry, out var detail))
                continue;
            flavors[0] += detail.Spicy;
            flavors[1] += detail.Fresh;
            flavors[2] += detail.Sweet;
            flavors[3] += detail.Bitter;
            flavors[4] += detail.Sour;
        }
    }

    public static bool TryGetBerry(ushort berry, out DonutBerryDetail detail)
    {
        foreach (var b in Berries)
        {
            if (b.Item != berry)
                continue;
            detail = b;
            return true;
        }
        detail = default;
        return false;
    }

    public static bool TryGetFlavorName(ulong hash, [NotNullWhen(true)] out string? name)
    {
        foreach (var (h, n) in Flavors)
        {
            if (h != hash)
                continue;
            name = n;
            return true;
        }
        name = null;
        return false;
    }

    // Could compute these during runtime, but I guess it helps discoverability for search.
    // Hash is FnvHash.HashFnv1a_64 of the internal flavor Name.
    public static IReadOnlyList<(ulong Hash, string Name)> Flavors { get; } =
    [
        (0xCCFCBB9681D321F1, "sweet_01_lv1"),
        (0xCCFCB89681D31CD8, "sweet_01_lv2"),
        (0xCCFCB99681D31E8B, "sweet_01_lv3"),
        (0xA92EF5B2B4003DDF, "sweet_03_lv1"),
        (0xA92EF6B2B4003F92, "sweet_03_lv2"),
        (0xA92EF7B2B4004145, "sweet_03_lv3"),
        (0x6F78E974FC99251E, "sweet_04_lv1"),
        (0x6F78E874FC99236B, "sweet_04_lv2"),
        (0x6F78E774FC9921B8, "sweet_04_lv3"),
        (0xC5C2B39FF7DDDEF5, "sweet_05_lv1"),
        (0xC5C2B09FF7DDD9DC, "sweet_05_lv2"),
        (0xC5C2B19FF7DDDB8F, "sweet_05_lv3"),
        (0xF9BF21792E9AF2DC, "sweet_06_lv1"),
        (0xF9BF24792E9AF7F5, "sweet_06_lv2"),
        (0xF9BF23792E9AF642, "sweet_06_lv3"),
        (0xB9EB85668F2373C3, "sweet_07_lv1"),
        (0xB9EB86668F237576, "sweet_07_lv2"),
        (0xB9EB87668F237729, "sweet_07_lv3"),
        (0xBA2A639672CE56D2, "sweet_08_lv1"),
        (0xBA2A629672CE551F, "sweet_08_lv2"),
        (0xBA2A619672CE536C, "sweet_08_lv3"),
        (0x541E83577FBC1009, "sweet_09_lv1"),
        (0x541E80577FBC0AF0, "sweet_09_lv2"),
        (0x541E81577FBC0CA3, "sweet_09_lv3"),
        (0x1F44DF68B728B1EF, "sweet_10_lv1"),
        (0x1F44E068B728B3A2, "sweet_10_lv2"),
        (0x1F44E168B728B555, "sweet_10_lv3"),
        (0x742F1750D66962F8, "sweet_11_lv1"),
        (0x742F1A50D6696811, "sweet_11_lv2"),
        (0x742F1950D669665E, "sweet_11_lv3"),
        (0xC138697B4F4B4CC1, "sweet_12_lv1"),
        (0xC138667B4F4B47A8, "sweet_12_lv2"),
        (0xC138677B4F4B495B, "sweet_12_lv3"),
        (0x69FD2589A16AC1CA, "sweet_13_lv1"),
        (0x69FD2489A16AC017, "sweet_13_lv2"),
        (0x69FD2389A16ABE64, "sweet_13_lv3"),
        (0x05EC3138449C8AD3, "sweet_14_lv1"),
        (0x05EC3238449C8C86, "sweet_14_lv2"),
        (0x05EC3338449C8E39, "sweet_14_lv3"),
        (0x16A60F5F34A05A6C, "sweet_15_lv1"),
        (0x16A6125F34A05F85, "sweet_15_lv2"),
        (0x16A6115F34A05DD2, "sweet_15_lv3"),
        (0xBBEBD92E8CB57645, "sweet_16_lv1"),
        (0xBBEBD62E8CB5712C, "sweet_16_lv2"),
        (0xBBEBD72E8CB572DF, "sweet_16_lv3"),
        (0x0E989541C730682E, "sweet_17_lv1"),
        (0x0E989441C730667B, "sweet_17_lv2"),
        (0x0E989341C73064C8, "sweet_17_lv3"),
        (0xFB824941B7F1E4A7, "sweet_18_lv1"),
        (0xFB824A41B7F1E65A, "sweet_18_lv2"),
        (0xFB824B41B7F1E80D, "sweet_18_lv3"),
        (0xBB417F175A6600D0, "sweet_19_lv1"),
        (0xBB4182175A6605E9, "sweet_19_lv2"),
        (0xBB4181175A660436, "sweet_19_lv3"),
        (0x0D37506AA6ECCEFC, "sweet_20_lv1"),
        (0x0D37536AA6ECD415, "sweet_20_lv2"),
        (0x0D37526AA6ECD262, "sweet_20_lv3"),
        (0xD373B02CEF7A3063, "sweet_21_lv1"),
        (0xD373B12CEF7A3216, "sweet_21_lv2"),
        (0xD373B22CEF7A33C9, "sweet_21_lv3"),
        (0x64DCD76EF1844453, "spicy_01_lv1"),
        (0x64DCD86EF1844606, "spicy_01_lv2"),
        (0x64DCD96EF18447B9, "spicy_01_lv3"),
        (0x6D893B78741821AE, "spicy_02_lv1"),
        (0x6D893A7874181FFB, "spicy_02_lv2"),
        (0x6D89397874181E48, "spicy_02_lv3"),
        (0x1ADC7F65399D2FC5, "spicy_03_lv1"),
        (0x1ADC7C65399D2AAC, "spicy_03_lv2"),
        (0x1ADC7D65399D2C5F, "spicy_03_lv3"),
        (0xD31FBD8783511C78, "spicy_04_lv1"),
        (0xD31FC08783512191, "spicy_04_lv2"),
        (0xD31FBF8783511FDE, "spicy_04_lv3"),
        (0x7E35859F64106B6F, "spicy_05_lv1"),
        (0x7E35869F64106D22, "spicy_05_lv2"),
        (0x7E35879F64106ED5, "spicy_05_lv3"),
        (0xC8EDCBC04E527B4A, "spicy_06_lv1"),
        (0xC8EDCAC04E527997, "spicy_06_lv2"),
        (0xC8EDC9C04E5277E4, "spicy_06_lv3"),
        (0x20290FB1FC330641, "spicy_07_lv1"),
        (0x20290CB1FC330128, "spicy_07_lv2"),
        (0x20290DB1FC3302DB, "spicy_07_lv3"),
        (0x51849B43C4EBCD84, "spicy_08_lv1"),
        (0x51849E43C4EBD29D, "spicy_08_lv2"),
        (0x51849D43C4EBD0EA, "spicy_08_lv3"),
        (0xFF0DDF308A9E6B0B, "spicy_09_lv1"),
        (0xFF0DE0308A9E6CBE, "spicy_09_lv2"),
        (0xFF0DE1308A9E6E71, "spicy_09_lv3"),
        (0xC9EDD87FA7D9A829, "spicy_10_lv1"),
        (0xC9EDD57FA7D9A310, "spicy_10_lv2"),
        (0xC9EDD67FA7D9A4C3, "spicy_10_lv3"),
        (0x2F3D34937D29A3F2, "spicy_11_lv1"),
        (0x2F3D33937D29A23F, "spicy_11_lv2"),
        (0x2F3D32937D29A08C, "spicy_11_lv3"),
        (0x2690F089FA95FCF7, "spicy_12_lv1"),
        (0x2690F189FA95FEAA, "spicy_12_lv2"),
        (0x2690F289FA96005D, "spicy_12_lv3"),
        (0x10C36659799A2EE0, "spicy_13_lv1"),
        (0x10C36959799A33F9, "spicy_13_lv2"),
        (0x10C36859799A3246, "spicy_13_lv3"),
        (0xBD946E7AE879D92D, "spicy_14_lv1"),
        (0xBD946B7AE879D414, "spicy_14_lv2"),
        (0xBD946C7AE879D5C7, "spicy_14_lv3"),
        (0xD6D2244BA5767AF6, "spicy_15_lv1"),
        (0xD6D2234BA5767943, "spicy_15_lv2"),
        (0xD6D2224BA5767790, "spicy_15_lv3"),
        (0x21B1603D385CF85B, "spicy_16_lv1"),
        (0x21B1613D385CFA0E, "spicy_16_lv2"),
        (0x21B1623D385CFBC1, "spicy_16_lv3"),
        (0xC5DD1E6872A66694, "spicy_17_lv1"),
        (0xC5DD216872A66BAD, "spicy_17_lv2"),
        (0xC5DD206872A669FA, "spicy_17_lv3"),
        (0x42CC10BEA9F0BA11, "spicy_18_lv1"),
        (0x42CC0DBEA9F0B4F8, "spicy_18_lv2"),
        (0x42CC0EBEA9F0B6AB, "spicy_18_lv3"),
        (0x429AC68EC6515CDA, "spicy_19_lv1"),
        (0x429AC58EC6515B27, "spicy_19_lv2"),
        (0x429AC48EC6515974, "spicy_19_lv3"),
        (0xB2CBA0F9F0DEE7AA, "spicy_20_lv1"),
        (0xB2CB9FF9F0DEE5F7, "spicy_20_lv2"),
        (0xB2CB9EF9F0DEE444, "spicy_20_lv3"),
        (0x633560BB9BE1A5A1, "spicy_21_lv1"),
        (0x63355DBB9BE1A088, "spicy_21_lv2"),
        (0x63355EBB9BE1A23B, "spicy_21_lv3"),
        (0x99AA421EDED5D2C0, "sour_01_lv1"),
        (0x99AA451EDED5D7D9, "sour_01_lv2"),
        (0x99AA441EDED5D626, "sour_01_lv3"),
        (0x54C2ABEED4759209, "sour_02_lv1"),
        (0x54C2A8EED4758CF0, "sour_02_lv2"),
        (0x54C2A9EED4758EA3, "sour_02_lv3"),
        (0xBACE8C2DC787D8D2, "sour_03_lv1"),
        (0xBACE8B2DC787D71F, "sour_03_lv2"),
        (0xBACE8A2DC787D56C, "sour_03_lv3"),
        (0xAA983C029D989C3B, "sour_04_lv1"),
        (0xAA983D029D989DEE, "sour_04_lv2"),
        (0xAA983E029D989FA1, "sour_04_lv3"),
        (0xAA9D71D2BA27A7F4, "sour_05_lv1"),
        (0xAA9D74D2BA27AD0D, "sour_05_lv2"),
        (0xAA9D73D2BA27AB5A, "sour_05_lv3"),
        (0x9FAAC6104AD9630D, "sour_06_lv1"),
        (0x9FAAC3104AD95DF4, "sour_06_lv2"),
        (0x9FAAC4104AD95FA7, "sour_06_lv3"),
        (0x5EFCFBE5ECF0AD56, "sour_07_lv1"),
        (0x5EFCFAE5ECF0ABA3, "sour_07_lv2"),
        (0x5EFCF9E5ECF0A9F0, "sour_07_lv3"),
        (0x982B8B2119CA2502, "sour_10_lv1"),
        (0x982B8A2119CA234F, "sour_10_lv2"),
        (0x982B892119CA219C, "sour_10_lv3"),
        (0x09D1ECF67D0A80B9, "sour_11_lv1"),
        (0x09D1E9F67D0A7BA0, "sour_11_lv2"),
        (0x09D1EAF67D0A7D53, "sour_11_lv3"),
        (0x5738D765D08ED3C5, "bitter_01_lv1"),
        (0x5738D465D08ECEAC, "bitter_01_lv2"),
        (0x5738D565D08ED05F, "bitter_01_lv3"),
        (0xB1F30D967879B7EC, "bitter_02_lv1"),
        (0xB1F310967879BD05, "bitter_02_lv2"),
        (0xB1F30F967879BB52, "bitter_02_lv3"),
        (0xA1392F6F8875E853, "bitter_03_lv1"),
        (0xA139306F8875EA06, "bitter_03_lv2"),
        (0xA139316F8875EBB9, "bitter_03_lv3"),
        (0x054A23C0E5441F4A, "bitter_04_lv1"),
        (0x054A22C0E5441D97, "bitter_04_lv2"),
        (0x054A21C0E5441BE4, "bitter_04_lv3"),
        (0x5C8567B29324AA41, "bitter_05_lv1"),
        (0x5C8564B29324A528, "bitter_05_lv2"),
        (0x5C8565B29324A6DB, "bitter_05_lv3"),
        (0x0F7C15881A42C078, "bitter_06_lv1"),
        (0x0F7C18881A42C591, "bitter_06_lv2"),
        (0x0F7C17881A42C3DE, "bitter_06_lv3"),
        (0xBA91DD9FFB020F6F, "bitter_07_lv1"),
        (0xBA91DE9FFB021122, "bitter_07_lv2"),
        (0xBA91DF9FFB0212D5, "bitter_07_lv3"),
        (0x98C57D52A6A7CBE6, "bitter_08_lv1"),
        (0x98C57C52A6A7CA33, "bitter_08_lv2"),
        (0x98C57B52A6A7C880, "bitter_08_lv3"),
        (0x59E4856B25205D9D, "bitter_09_lv1"),
        (0x59E4826B25205884, "bitter_09_lv2"),
        (0x59E4836B25205A37, "bitter_09_lv3"),
        (0x6EC0AE6433538DE3, "bitter_10_lv1"),
        (0x6EC0AF6433538F96, "bitter_10_lv2"),
        (0x6EC0B06433539149, "bitter_10_lv3"),
        (0xA8844EA1EAC62C7C, "bitter_11_lv1"),
        (0xA88451A1EAC63195, "bitter_11_lv2"),
        (0xA88450A1EAC62FE2, "bitter_11_lv3"),
        (0x2356D877664B3295, "bitter_12_lv1"),
        (0x2356D577664B2D7C, "bitter_12_lv2"),
        (0x2356D677664B2F2F, "bitter_12_lv3"),
        (0x1E3E169DB8C45EBE, "bitter_13_lv1"),
        (0x1E3E159DB8C45D0B, "bitter_13_lv2"),
        (0x1E3E149DB8C45B58, "bitter_13_lv3"),
        (0x5E041EB0583057FF, "bitter_14_lv1"),
        (0x5E041FB0583059B2, "bitter_14_lv2"),
        (0x5E0420B058305B65, "bitter_14_lv3"),
        (0x5DD2D4807490FAC8, "bitter_15_lv1"),
        (0x5DD2D7807490FFE1, "bitter_15_lv2"),
        (0x5DD2D6807490FE2E, "bitter_15_lv3"),
        (0x7F2868BF40E25E11, "bitter_16_lv1"),
        (0x7F2865BF40E258F8, "bitter_16_lv2"),
        (0x7F2866BF40E25AAB, "bitter_16_lv3"),
        (0x7EF71E8F5D4300DA, "bitter_17_lv1"),
        (0x7EF71D8F5D42FF27, "bitter_17_lv2"),
        (0x7EF71C8F5D42FD74, "bitter_17_lv3"),
        (0x5E0DB83DCF4E9C5B, "bitter_18_lv1"),
        (0x5E0DB93DCF4E9E0E, "bitter_18_lv2"),
        (0x5E0DBA3DCF4E9FC1, "bitter_18_lv3"),
        (0x0239766909980A94, "bitter_19_lv1"),
        (0x0239796909980FAD, "bitter_19_lv2"),
        (0x0239786909980DFA, "bitter_19_lv3"),
        (0xC4223D53A3682370, "bitter_20_lv1"),
        (0xC4224053A3682889, "bitter_20_lv2"),
        (0xC4223F53A36826D6, "bitter_20_lv3"),
        (0xCF24B0DFA2D10011, "fresh_01_lv1"),
        (0xCF24ADDFA2D0FAF8, "fresh_01_lv2"),
        (0xCF24AEDFA2D0FCAB, "fresh_01_lv3"),
        (0xADCF1CA0D67F9CC8, "fresh_02_lv1"),
        (0xADCF1FA0D67FA1E1, "fresh_02_lv2"),
        (0xADCF1EA0D67FA02E, "fresh_02_lv3"),
        (0xAE0066D0BA1EF9FF, "fresh_03_lv1"),
        (0xAE0067D0BA1EFBB2, "fresh_03_lv2"),
        (0xAE0068D0BA1EFD65, "fresh_03_lv3"),
        (0x6E3A5EBE1AB300BE, "fresh_04_lv1"),
        (0x6E3A5DBE1AB2FF0B, "fresh_04_lv2"),
        (0x6E3A5CBE1AB2FD58, "fresh_04_lv3"),
        (0x73532097C839D495, "fresh_05_lv1"),
        (0x73531D97C839CF7C, "fresh_05_lv2"),
        (0x73531E97C839D12F, "fresh_05_lv3"),
        (0xF88096C24CB4CE7C, "fresh_06_lv1"),
        (0xF88099C24CB4D395, "fresh_06_lv2"),
        (0xF88098C24CB4D1E2, "fresh_06_lv3"),
        (0xBEBCF68495422FE3, "fresh_07_lv1"),
        (0xBEBCF78495423196, "fresh_07_lv2"),
        (0xBEBCF88495423349, "fresh_07_lv3"),
        (0xBB95D4B47609E9F2, "fresh_08_lv1"),
        (0xBB95D3B47609E83F, "fresh_08_lv2"),
        (0xBB95D2B47609E68C, "fresh_08_lv3"),
        (0x564678A0A0B9EE29, "fresh_09_lv1"),
        (0x564675A0A0B9E910, "fresh_09_lv2"),
        (0x564676A0A0B9EAC3, "fresh_09_lv3"),
        (0x27AD619F321DDB8F, "fresh_10_lv1"),
        (0x27AD629F321DDD42, "fresh_10_lv2"),
        (0x27AD639F321DDEF5, "fresh_10_lv3"),
        (0xD0721DAD843D5098, "fresh_11_lv1"),
        (0xD07220AD843D55B1, "fresh_11_lv2"),
        (0xD0721FAD843D53FE, "fresh_11_lv3"),
        (0x1A156FD7FA3C1161, "fresh_12_lv1"),
        (0x1A156CD7FA3C0C48, "fresh_12_lv2"),
        (0x1A156DD7FA3C0DFB, "fresh_12_lv3"),
        (0x6EFFA7C0197CC26A, "fresh_13_lv1"),
        (0x6EFFA6C0197CC0B7, "fresh_13_lv2"),
        (0x6EFFA5C0197CBF04, "fresh_13_lv3"),
        (0x5EC93794EF8D4F73, "fresh_14_lv1"),
        (0x5EC93894EF8D5126, "fresh_14_lv2"),
        (0x5EC93994EF8D52D9, "fresh_14_lv3"),
        (0x6F8315BBDF911F0C, "fresh_15_lv1"),
        (0x6F8318BBDF912425, "fresh_15_lv2"),
        (0x6F8317BBDF912272, "fresh_15_lv3"),
        (0xC1A9DF9022880EE5, "fresh_16_lv1"),
        (0xC1A9DC90228809CC, "fresh_16_lv2"),
        (0xC1A9DD9022880B7F, "fresh_16_lv3"),
        (0x64CC1FC98D004ECE, "fresh_17_lv1"),
        (0x64CC1EC98D004D1B, "fresh_17_lv2"),
        (0x64CC1DC98D004B68, "fresh_17_lv3"),
        (0xFDDA4FA34AE15447, "fresh_18_lv1"),
        (0xFDDA50A34AE155FA, "fresh_18_lv2"),
        (0xFDDA51A34AE157AD, "fresh_18_lv3"),
        (0x141E85740556C570, "fresh_19_lv1"),
        (0x141E88740556CA89, "fresh_19_lv2"),
        (0x141E87740556C8D6, "fresh_19_lv3"),
        (0x0F8FD6CC39DD181C, "fresh_20_lv1"),
        (0x0F8FD9CC39DD1D35, "fresh_20_lv2"),
        (0x0F8FD8CC39DD1B82, "fresh_20_lv3"),
        (0xD3223AB99D48C203, "fresh_21_lv1"),
        (0xD3223BB99D48C3B6, "fresh_21_lv2"),
        (0xD3223CB99D48C569, "fresh_21_lv3"),
        (0xDF349EC322BFC85E, "fresh_22_lv1"),
        (0xDF349DC322BFC6AB, "fresh_22_lv2"),
        (0xDF349CC322BFC4F8, "fresh_22_lv3"),
        (0x2E27B49C885F70F0, "sp_01"),
        (0x2E27B79C885F7609, "sp_02"),
        (0x2E27B69C885F7456, "sp_03"),
        (0x2E27B89C885F77BC, "sp_05"),
        (0x2E27B99C885F796F, "sp_04"),
    ];

    public static ulong GetFlavorHash(string text) => FnvHash.HashFnv1a_64(text);
}

public readonly record struct DonutBerryDetail(ushort Item, byte Donut, byte Spicy, byte Fresh, byte Sweet, byte Bitter, byte Sour, byte Boost, ushort Calories)
{
    public int FlavorScore => Spicy + Fresh + Sweet + Bitter + Sour;
}

public readonly record struct Donut9a(Memory<byte> Raw)
{
    public const int Size = 0x48;
    public Span<byte> Data => Raw.Span;

    public static DateTime Epoch => new(1900, 1, 1, 0, 0, 0, DateTimeKind.Local);

    /*
    0x00    u64 MillisecondsSince1900
       
    0x08    u8 Stars
    0x09    u8 LevelBoost
    0x0A    u16 Donut
    0x0C    u16 Calories
    0x0E    u16 BerryName
    0x10    u16 Berry1
    0x12    u16 Berry2
    0x14    u16 Berry3
    0x16    u16 Berry4
    0x18    u16 Berry5
    0x1A    u16 Berry6
    0x1C    u16 Berry7
    0x1E    u16 Berry8
    
    0x20    u64-Epoch1900DateTimeValue
    
    0x28    u64 Flavor0
    0x30    u64 Flavor1
    0x38    u64 Flavor2
    0x40    u64 Reserved
    */

    /// <summary>
    /// Timestamp the donut was created, in milliseconds since January 1, 1900.
    /// </summary>
    public ulong MillisecondsSince1900 { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }

    public byte Stars { get => Data[0x08]; set => Data[0x08] = value; }
    public byte LevelBoost { get => Data[0x09]; set => Data[0x09] = value; }

    /// <summary> Indicates which donut sprite to load. </summary>
    public ushort Donut { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }
    public ushort Calories { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], value); }

    /// <summary>
    /// The berry that is used in the name of the donut.
    /// </summary>
    /// <remarks>
    /// Must be one of the 8 berries.
    /// Only the 8 berries (not this value) are considered when calculating the flavor profile of the donut.
    /// This is only used for display purposes related to <see cref="Donut"/>.
    /// </remarks>
    public ushort BerryName { get => ReadUInt16LittleEndian(Data[0x0E..]); set => WriteUInt16LittleEndian(Data[0x0E..], value); }

    public ushort Berry1 { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ushort Berry2 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    public ushort Berry3 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], value); }
    public ushort Berry4 { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], value); }
    public ushort Berry5 { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], value); }
    public ushort Berry6 { get => ReadUInt16LittleEndian(Data[0x1A..]); set => WriteUInt16LittleEndian(Data[0x1A..], value); }
    public ushort Berry7 { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], value); }
    public ushort Berry8 { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }

    /// <summary>
    /// Alternate date storage format for the donut creation time.
    /// </summary>
    /// <remarks>
    /// <see cref="MillisecondsSince1900"/> is the source of truth; this value should be kept in sync via <see cref="UpdateDateTime"/>.
    /// </remarks>
    public Epoch1900DateTimeValue DateTime1900 => new(Raw[0x20..0x28]);

    public ulong Flavor0 { get => ReadUInt64LittleEndian(Data[0x28..]); set => WriteUInt64LittleEndian(Data[0x28..], value); }
    public ulong Flavor1 { get => ReadUInt64LittleEndian(Data[0x30..]); set => WriteUInt64LittleEndian(Data[0x30..], value); }
    public ulong Flavor2 { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }

    public ulong Reserved { get => ReadUInt64LittleEndian(Data[0x40..]); set => WriteUInt64LittleEndian(Data[0x40..], value); }

    /// <summary>
    /// Updates the <see cref="DateTime1900"/> value based on the stored <see cref="MillisecondsSince1900"/>.
    /// </summary>
    /// <remarks>
    /// The values should always mirror each other, but their storage formats differ. Treat <see cref="MillisecondsSince1900"/> as the main source of truth.
    /// </remarks>
    public void UpdateDateTime() => DateTime1900.Timestamp = Epoch.AddMilliseconds(MillisecondsSince1900);

    /// <summary>
    /// A non-zero <see cref="MillisecondsSince1900"/> indicates that this donut slot is occupied. A zero-value with non-zero flavors/berries is invalid (and causes display glitches in-game).
    /// </summary>
    public bool IsEmpty => MillisecondsSince1900 != 0;

    /// <summary>
    /// Count of flavor perks granted by this donut.
    /// </summary>
    public int FlavorCount => Flavor0 == 0 ? 0 : Flavor1 == 0 ? 1 : Flavor2 == 0 ? 2 : 3;

    /// <summary> Berries used in this donut. </summary>
    public ReadOnlySpan<ushort> GetBerries() => new[] { Berry1, Berry2, Berry3, Berry4, Berry5, Berry6, Berry7, Berry8 };

    /// <summary> Flavor perks granted by this donut. </summary>
    public ReadOnlySpan<ulong> GetFlavors() => new[] { Flavor0, Flavor1, Flavor2 };

    /// <summary>
    /// Copies the data from the current instance to the specified <see cref="Donut9a"/> instance.
    /// </summary>
    public void CopyTo(Donut9a other) => Data.CopyTo(other.Data);

    /// <summary> Wipes all data in this donut slot, marking it as empty. </summary>
    public void Clear() => Data.Clear();

    /// <summary>
    /// Wipes only the stored <see cref="DateTime1900"/> value.
    /// </summary>
    /// <remarks>
    /// Used along with <see cref="UpdateDateTime"/> to reset the date from milliseconds.
    /// </remarks>
    public void ClearDateTime() => Data[0x20..0x28].Clear();

    /// <summary>
    /// Indicates whether a <see cref="DateTime1900"/> is stored. Does not guarantee that the date is possible/valid.
    /// </summary>
    public bool HasDateTime() => ReadUInt64LittleEndian(Data[0x20..0x28]) != 0;
}
