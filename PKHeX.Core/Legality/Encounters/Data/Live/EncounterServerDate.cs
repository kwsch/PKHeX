using System;
using System.Collections.Generic;
using static PKHeX.Core.EncounterServerDateCheck;

namespace PKHeX.Core;

/// <summary>
/// Logic to check if a date obtained is within the date of availability.
/// </summary>
public static class EncounterServerDate
{
    private static EncounterServerDateCheck Result(bool result) => result ? Valid : Invalid;

    /// <summary>
    /// Checks if the date obtained is within the date of availability for the given <see cref="enc"/>.
    /// </summary>
    /// <param name="enc">Encounter to check.</param>
    /// <param name="obtained">Date obtained.</param>
    /// <returns>True if the date obtained is within the date of availability for the given <see cref="enc"/>.</returns>
    public static EncounterServerDateCheck IsWithinDistributionWindow(this IEncounterServerDate enc, DateOnly obtained) => enc switch
    {
        WB7 wb7 => Result(wb7.IsWithinDistributionWindow(obtained)),
        WC8 wc8 => Result(wc8.IsWithinDistributionWindow(obtained)),
        WA8 wa8 => Result(wa8.IsWithinDistributionWindow(obtained)),
        WB8 wb8 => Result(wb8.IsWithinDistributionWindow(obtained)),
        WC9 wc9 => Result(wc9.IsWithinDistributionWindow(obtained)),
        EncounterSlot7GO g7 => Result(g7.IsWithinDistributionWindow(obtained)),
        EncounterSlot8GO g8 => Result(g8.IsWithinDistributionWindow(obtained)),
        _ => throw new ArgumentOutOfRangeException(nameof(enc)),
    };

    /// <inheritdoc cref="IsWithinDistributionWindow(IEncounterServerDate,DateOnly)"/>
    public static bool IsWithinDistributionWindow(this WB7 card, DateOnly obtained) => card.GetDistributionWindow(out var window) && window.Contains(obtained);

    /// <inheritdoc cref="IsWithinDistributionWindow(IEncounterServerDate,DateOnly)"/>
    public static bool IsWithinDistributionWindow(this WC8 card, DateOnly obtained) => card.GetDistributionWindow(out var window) && window.Contains(obtained);

    /// <inheritdoc cref="IsWithinDistributionWindow(IEncounterServerDate,DateOnly)"/>
    public static bool IsWithinDistributionWindow(this WA8 card, DateOnly obtained) => card.GetDistributionWindow(out var window) && window.Contains(obtained);

    /// <inheritdoc cref="IsWithinDistributionWindow(IEncounterServerDate,DateOnly)"/>
    public static bool IsWithinDistributionWindow(this WB8 card, DateOnly obtained) => card.GetDistributionWindow(out var window) && window.Contains(obtained);

    /// <inheritdoc cref="IsWithinDistributionWindow(IEncounterServerDate,DateOnly)"/>
    public static bool IsWithinDistributionWindow(this WC9 card, DateOnly obtained) => card.GetDistributionWindow(out var window) && window.Contains(obtained);

    public static bool GetDistributionWindow(this WB7 card, out DistributionWindow window) => WB7Gifts.TryGetValue(card.CardID, out window);
    public static bool GetDistributionWindow(this WC8 card, out DistributionWindow window) => WC8Gifts.TryGetValue(card.CardID, out window) || WC8GiftsChk.TryGetValue(card.Checksum, out window);
    public static bool GetDistributionWindow(this WA8 card, out DistributionWindow window) => WA8Gifts.TryGetValue(card.CardID, out window);
    public static bool GetDistributionWindow(this WB8 card, out DistributionWindow window) => WB8Gifts.TryGetValue(card.CardID, out window);
    public static bool GetDistributionWindow(this WC9 card, out DistributionWindow window) => WC9Gifts.TryGetValue(card.CardID, out window) || WC9GiftsChk.TryGetValue(card.Checksum, out window);

    /// <summary>
    /// Initial introduction of HOME support for SW/SH; gift availability (generating) was revised in 3.0.0.
    /// </summary>
    private static readonly DistributionWindow HOME1 = new(2020, 02, 12, 2023, 05, 29);

    /// <summary>
    /// Revision of HOME support for SW/SH; gift availability (generating) was revised in 3.0.0.
    /// </summary>
    private static readonly DistributionWindow HOME3 = new(2023, 05, 30);

    /// <summary>
    /// Introduction of BD/SP and PLA support; gift availability time window for these games.
    /// </summary>
    private static readonly DistributionWindow HOME2_AB = new(2022, 05, 18);

    /// <summary>
    /// Introduction of S/V support; gift availability time window for these games.
    /// </summary>
    private static readonly DistributionWindow HOME3_ML = new(2023, 05, 30);

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WB7Gifts = new()
    {
        {9028, new(2025, 02, 11)}, // Shiny Meltan
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WC8GiftsChk = new()
    {
        // HOME 1.0.0 to 2.0.2 - PID, EC, Height, Weight = 0 (rev 1)
        {0xFBBE, HOME1}, // Bulbasaur
        {0x48F5, HOME1}, // Charmander
        {0x47DB, HOME1}, // Squirtle
        {0x671A, HOME1}, // Pikachu
        {0x81A2, HOME1}, // Original Color Magearna
        {0x4CC7, HOME1}, // Eevee
        {0x1A0B, HOME1}, // Rotom
        {0x1C26, HOME1}, // Pichu

        // HOME 3.0.0 onward - PID, EC, Height, Weight = random (rev 2)
        {0x7124, HOME3}, // Bulbasaur
        {0xC26F, HOME3}, // Charmander
        {0xCD41, HOME3}, // Squirtle
        {0xED80, HOME3}, // Pikachu
        {0x0B38, HOME3}, // Original Color Magearna
        {0xC65D, HOME3}, // Eevee
        {0x9091, HOME3}, // Rotom
        {0x96BC, HOME3}, // Pichu
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WC8Gifts = new()
    {
        {9008, new(2020, 06, 02)}, // Hidden Ability Grookey
        {9009, new(2020, 06, 02)}, // Hidden Ability Scorbunny
        {9010, new(2020, 06, 02)}, // Hidden Ability Sobble
        {9011, new(2020, 06, 30)}, // Shiny Zeraora
        {9012, new(2020, 11, 10)}, // Gigantamax Melmetal
        {9013, new(2021, 06, 17)}, // Gigantamax Bulbasaur
        {9014, new(2021, 06, 17)}, // Gigantamax Squirtle
        {9029, new(2025, 02, 11)}, // Shiny Keldeo
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WA8Gifts = new()
    {
        {0138, new(2022, 01, 27, 2023, 02, 01)}, // Poké Center Happiny
        {0301, new(2022, 02, 04, 2023, 03, 01)}, // プロポチャ Piplup
        {0801, new(2022, 02, 25, 2022, 06, 01)}, // Teresa Roca Hisuian Growlithe
        {1201, new(2022, 05, 31, 2022, 08, 01)}, // 전이마을 Regigigas
        {1202, new(2022, 05, 31, 2022, 08, 01)}, // 빛나's Piplup
        {1203, new(2022, 08, 18, 2022, 11, 01)}, // Arceus Chronicles Hisuian Growlithe
        {0151, new(2022, 09, 03, 2022, 10, 01)}, // Otsukimi Festival 2022 Clefairy

        {9018, HOME2_AB}, // Hidden Ability Rowlet
        {9019, HOME2_AB}, // Hidden Ability Cyndaquil
        {9020, HOME2_AB}, // Hidden Ability Oshawott
        {9027, new(2025, 01, 27)}, // Shiny Enamorus
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WB8Gifts = new()
    {
        {9015, HOME2_AB}, // Hidden Ability Turtwig
        {9016, HOME2_AB}, // Hidden Ability Chimchar
        {9017, HOME2_AB}, // Hidden Ability Piplup
        {9026, new(2025, 01, 27)}, // Shiny Manaphy
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WC9GiftsChk = new()
    {
        {0xE5EB, new(2022, 11, 17, 2023, 02, 03)}, // Fly Pikachu - rev 1 (male 128 height/weight)
        {0x908B, new(2023, 02, 02, 2023, 03, 01)}, // Fly Pikachu - rev 2 (both 0 height/weight)
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    private static readonly Dictionary<int, DistributionWindow> WC9Gifts = new()
    {
        {0001, new(2022, 11, 17)}, // PokéCenter Birthday Flabébé
        {0006, new(2022, 12, 16, 2023, 02, 01)}, // Jump Festa Gyarados
        {0501, new(2023, 02, 16, 2023, 02, 21)}, // Jiseok's Garganacl
        {1513, new(2023, 02, 27, 2024, 03, 01)}, // Hisuian Zoroark DLC Purchase Gift
        {0502, new(2023, 03, 31, 2023, 07, 01)}, // TCG Flying Lechonk
        {0503, new(2023, 04, 13, 2023, 04, 18)}, // Gavin's Palafin (-1 start date tolerance for GMT-10 regions)
        {0025, new(2023, 04, 21, 2023, 08, 01)}, // Pokémon Center Pikachu (Mini & Jumbo)
        {1003, new(2023, 05, 29, 2023, 08, 01)}, // Arceus and the Jewel of Life Distribution - Pokémon Store Tie-In Bronzong
        {1002, new(2023, 05, 31, 2023, 08, 01)}, // Arceus and the Jewel of Life Distribution Pichu
        {0028, new(2023, 06, 09, 2023, 06, 12)}, // そらみつ's Bronzong (-1 start date tolerance for GMT-10 regions)
        {1005, new(2023, 06, 16, 2023, 06, 20)}, // 정원석's Gastrodon (-1 start date tolerance for GMT-10 regions)
        {0504, new(2023, 06, 30, 2023, 07, 04)}, // Paul's Shiny Arcanine
        {1522, new(2023, 07, 21, 2023, 09, 01)}, // Dark Tera Type Charizard
        {0024, new(2023, 07, 26, 2023, 08, 19)}, // Nontaro's Shiny Grimmsnarl
        {0505, new(2023, 08, 07, 2023, 09, 01)}, // WCS 2023 Stretchy Form Tatsugiri
        {1521, new(2023, 08, 08, 2023, 09, 19)}, // My Very Own Mew
        {0506, new(2023, 08, 10, 2023, 08, 15)}, // Eduardo Gastrodon
        {1524, new(2023, 09, 06, 2024, 09, 01)}, // Glaseado Cetitan
        {0507, new(2023, 10, 13, 2024, 01, 01)}, // Trixie Mimikyu
        {0031, new(2023, 11, 01, 2025, 02, 01)}, // PokéCenter Birthday Charcadet and Pawmi
        {1006, new(2023, 11, 02, 2024, 01, 01)}, // Korea Bundle Fidough
        {0508, new(2023, 11, 17, 2023, 11, 21)}, // Alex's Dragapult
        {1526, new(2023, 11, 22, 2024, 11, 01)}, // Team Star Revavroom
        {1529, new(2023, 12, 07, 2023, 12, 22)}, // New Moon Darkrai
        {1530, new(2023, 12, 07, 2024, 01, 04)}, // Shiny Buddy Lucario
        {1527, new(2023, 12, 13, 2024, 12, 01)}, // Paldea Gimmighoul
        {0036, new(2023, 12, 14, 2024, 02, 14)}, // コロコロ Roaring Moon and Iron Valiant
        {1007, new(2023, 12, 29, 2024, 02, 11)}, // 윈터페스타 Baxcalibur
        {0038, new(2024, 01, 14, 2024, 03, 14)}, // コロコロ Scream Tail, Brute Bonnet, Flutter Mane, Iron Hands, Iron Jugulis, and Iron Thorns
        {0048, new(2024, 02, 22, 2024, 04, 01)}, // Project Snorlax Campaign Gift
        {1534, new(2024, 03, 12, 2025, 03, 01)}, // YOASOBI Pawmot
        {1535, new(2024, 03, 14, 2024, 10, 01)}, // Liko's Sprigatito
        {0509, new(2024, 04, 04, 2024, 04, 09)}, // Marco's Iron Hands
        {1008, new(2024, 05, 04, 2024, 05, 08)}, // 신여명's Flutter Mane
        {0052, new(2024, 05, 11, 2024, 07, 01)}, // Sophia's Gyarados
        {1536, new(2024, 05, 18, 2024, 12, 01)}, // Dot's Quaxly
        {0049, new(2024, 05, 31, 2024, 06, 03)}, // ナーク's Talonflame
        {0510, new(2024, 06, 07, 2024, 06, 11)}, // Nils's Porygon2
        {0050, new(2024, 07, 13, 2024, 10, 01)}, // Japan's Pokéss Summer Festival Eevee
        {1537, new(2024, 07, 24, 2025, 02, 01)}, // Roy's Fuecoco
        {0511, new(2024, 08, 15, 2024, 08, 31)}, // WCS 2024 Steenee
        {0512, new(2024, 08, 16, 2024, 08, 20)}, // Tomoya's Sylveon
        {0062, new(2024, 10, 31, 2026, 02, 01)}, // PokéCenter Birthday Tandemaus
        {0513, new(2024, 11, 15, 2024, 11, 23)}, // Patrick's Pelipper
        {0054, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's JPN Keldeo
        {0055, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's JPN Zarude
        {0056, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's JPN Deoxys
        {1011, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's KOR Keldeo
        {1012, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's KOR Zarude
        {1013, new(2024, 11, 21, 2025, 06, 01)}, // Operation Get Mythical's KOR Deoxys
        {1010, new(2025, 01, 21, 2025, 04, 01)}, // Pokémon Lucario & The Mystery of Mew Movie Gift KOR 아론's Lucario
        {0514, new(2025, 02, 05, 2025, 07, 01, +2)}, // Pokémon Day 2025 Flying Tera Type Eevee
        {0519, new(2025, 02, 20, 2025, 03, 01)}, // Marco's Jumpluff
        {0066, new(2025, 04, 18, 2025, 08, 01)}, // Wei Chyr's Rillaboom
        {1019, new(2025, 04, 24, 2025, 07, 01)}, // Pokémon Town - KOR Ditto Project
        {1020, new(2025, 06, 06, 2025, 06, 10)}, // PTC 2025 홍주영's Porygon2
        {0523, new(2025, 06, 13, 2025, 06, 21)}, // NAIC 2025 Wolfe's Incineroar
        {0067, new(2025, 06, 20, 2025, 06, 23)}, // PJCS 2025 Hyuma Hara's Flutter Mane
        {0068, new(2025, 06, 20, 2025, 10, 01)}, // PJCS 2025 Ray Yamanaka's Amoonguss
        {9996, new(2025, 08, 07, 2025, 10, 01)}, // Shiny Wo-Chien
        {9997, new(2025, 08, 21, 2025, 10, 01)}, // Shiny Chien-Pao
        {9998, new(2025, 09, 04, 2025, 10, 01)}, // Shiny Ting-Lu
        {9999, new(2025, 09, 18, 2025, 10, 01)}, // Shiny Chi-Yu

        {9021, HOME3_ML}, // Hidden Ability Sprigatito
        {9022, HOME3_ML}, // Hidden Ability Fuecoco
        {9023, HOME3_ML}, // Hidden Ability Quaxly
        {9024, new(2024, 10, 16)}, // Shiny Meloetta
        {9025, new(2024, 11, 01)}, // PokéCenter Birthday Tandemaus
    };
}
