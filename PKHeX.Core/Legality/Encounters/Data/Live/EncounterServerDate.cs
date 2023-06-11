using System;
using System.Collections.Generic;
using static PKHeX.Core.EncounterServerDateCheck;

namespace PKHeX.Core;

/// <summary>
/// Logic to check if a date obtained is within the date of availability.
/// </summary>
public static class EncounterServerDate
{
    private static bool IsValidDate(DateOnly obtained, DateOnly start) => obtained >= start && obtained <= DateOnly.FromDateTime(DateTime.UtcNow);

    private static bool IsValidDate(DateOnly obtained, DateOnly start, DateOnly end) => obtained >= start && obtained <= end;

    private static bool IsValidDate(DateOnly obtained, (DateOnly Start, DateOnly? End) value)
    {
        var (start, end) = value;
        if (end is not { } x)
            return IsValidDate(obtained, start);
        return IsValidDate(obtained, start, x);
    }

    private static EncounterServerDateCheck Result(bool result) => result ? Valid : Invalid;

    public static EncounterServerDateCheck IsValidDate(this IEncounterServerDate enc, DateOnly obtained) => enc switch
    {
        WC8 wc8 => Result(IsValidDateWC8(wc8, obtained)),
        WA8 wa8 => Result(IsValidDateWA8(wa8, obtained)),
        WB8 wb8 => Result(IsValidDateWB8(wb8, obtained)),
        WC9 wc9 => Result(IsValidDateWC9(wc9, obtained)),
        _ => throw new ArgumentOutOfRangeException(nameof(enc)),
    };

    public static bool IsValidDateWC8(WC8 card, DateOnly obtained) => (WC8Gifts.TryGetValue(card.CardID, out var time)
                                                                      || WC8GiftsChk.TryGetValue(card.Checksum, out time)) && IsValidDate(obtained, time);

    public static bool IsValidDateWA8(WA8 card, DateOnly obtained) => WA8Gifts.TryGetValue(card.CardID, out var time) && IsValidDate(obtained, time);

    public static bool IsValidDateWB8(WB8 card, DateOnly obtained) => WB8Gifts.TryGetValue(card.CardID, out var time) && IsValidDate(obtained, time);

    public static bool IsValidDateWC9(WC9 card, DateOnly obtained) => (WC9Gifts.TryGetValue(card.CardID, out var time)
                                                                      || WC9GiftsChk.TryGetValue(card.Checksum, out time)) && IsValidDate(obtained, time);

    private static readonly DateOnly? Never = null;

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC8GiftsChk = new()
    {
        // HOME 1.0.0 to 2.0.2 - PID, EC, Height, Weight = 0 (rev 1)
        {0xFBBE, (new(2020, 02, 12), new(2023, 05, 29))}, // Bulbasaur
        {0x48F5, (new(2020, 02, 12), new(2023, 05, 29))}, // Charmander
        {0x47DB, (new(2020, 02, 12), new(2023, 05, 29))}, // Squirtle
        {0x671A, (new(2020, 02, 12), new(2023, 05, 29))}, // Pikachu
        {0x81A2, (new(2020, 02, 15), new(2023, 05, 29))}, // Original Color Magearna
        {0x4CC7, (new(2020, 02, 12), new(2023, 05, 29))}, // Eevee
        {0x1A0B, (new(2020, 02, 12), new(2023, 05, 29))}, // Rotom
        {0x1C26, (new(2020, 02, 12), new(2023, 05, 29))}, // Pichu

        // HOME 3.0.0 onward - PID, EC, Height, Weight = random (rev 2)
        {0x7124, (new(2023, 05, 30), Never)}, // Bulbasaur
        {0xC26F, (new(2023, 05, 30), Never)}, // Charmander
        {0xCD41, (new(2023, 05, 30), Never)}, // Squirtle
        {0xED80, (new(2023, 05, 30), Never)}, // Pikachu
        {0x0B38, (new(2023, 05, 30), Never)}, // Original Color Magearna
        {0xC65D, (new(2023, 05, 30), Never)}, // Eevee
        {0x9091, (new(2023, 05, 30), Never)}, // Rotom
        {0x96BC, (new(2023, 05, 30), Never)}, // Pichu
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC8Gifts = new()
    {
        {9008, (new(2020, 06, 02), Never)}, // Hidden Ability Grookey
        {9009, (new(2020, 06, 02), Never)}, // Hidden Ability Scorbunny
        {9010, (new(2020, 06, 02), Never)}, // Hidden Ability Sobble
        {9011, (new(2020, 06, 30), Never)}, // Shiny Zeraora
        {9012, (new(2020, 11, 10), Never)}, // Gigantamax Melmetal
        {9013, (new(2021, 06, 17), Never)}, // Gigantamax Bulbasaur
        {9014, (new(2021, 06, 17), Never)}, // Gigantamax Squirtle
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WA8Gifts = new()
    {
        {0138, (new(2022, 01, 27), new(2023, 02, 01))}, // Poké Center Happiny
        {0301, (new(2022, 02, 04), new(2023, 03, 01))}, // プロポチャ Piplup
        {0801, (new(2022, 02, 25), new(2022, 06, 01))}, // Teresa Roca Hisuian Growlithe
        {1201, (new(2022, 05, 31), new(2022, 08, 01))}, // 전이마을 Regigigas
        {1202, (new(2022, 05, 31), new(2022, 08, 01))}, // 빛나's Piplup
        {1203, (new(2022, 08, 18), new(2022, 11, 01))}, // Arceus Chronicles Hisuian Growlithe
        {0151, (new(2022, 09, 03), new(2022, 10, 01))}, // Otsukimi Festival 2022 Clefairy

        {9018, (new(2022, 05, 18), Never)}, // Hidden Ability Rowlet
        {9019, (new(2022, 05, 18), Never)}, // Hidden Ability Cyndaquil
        {9020, (new(2022, 05, 18), Never)}, // Hidden Ability Oshawott
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WB8Gifts = new()
    {
        {9015, (new(2022, 05, 18), Never)}, // Hidden Ability Turtwig
        {9016, (new(2022, 05, 18), Never)}, // Hidden Ability Chimchar
        {9017, (new(2022, 05, 18), Never)}, // Hidden Ability Piplup
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC9GiftsChk = new()
    {
        {0xE5EB, (new(2022, 11, 17), new(2023, 02, 03))}, // Fly Pikachu - rev 1 (male 128 height/weight)
        {0x908B, (new(2023, 02, 02), new(2023, 03, 01))}, // Fly Pikachu - rev 2 (both 0 height/weight)
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateOnly Start, DateOnly? End)> WC9Gifts = new()
    {
        {0001, (new(2022, 11, 17), Never)}, // PokéCenter Flabébé
        {0006, (new(2022, 12, 16), new(2023, 02, 01))}, // Jump Festa Gyarados
        {0501, (new(2023, 02, 16), new(2023, 02, 21))}, // Jiseok's Garganacl
        {1513, (new(2023, 02, 27), new(2024, 03, 01))}, // Hisuian Zoroark DLC Purchase Gift
        {0502, (new(2023, 03, 31), new(2023, 07, 01))}, // TCG Flying Lechonk
        {0503, (new(2023, 04, 13), new(2023, 04, 18))}, // Gavin's Palafin (-1 start date tolerance for GMT-10 regions)
        {0025, (new(2023, 04, 21), new(2023, 07, 01))}, // Pokémon Center Pikachu (Mini & Jumbo)
        {1003, (new(2023, 05, 29), new(2023, 08, 01))}, // Arceus and the Jewel of Life Distribution - Pokémon Store Tie-In Bronzong
        {1002, (new(2023, 05, 31), new(2023, 08, 01))}, // Arceus and the Jewel of Life Distribution Pichu
        {0028, (new(2023, 06, 09), new(2023, 06, 12))}, // そらみつ's Bronzong

        {9021, (new(2023, 05, 30), Never)}, // Hidden Ability Sprigatito
        {9022, (new(2023, 05, 30), Never)}, // Hidden Ability Fuecoco
        {9023, (new(2023, 05, 30), Never)}, // Hidden Ability Quaxly
    };
}
