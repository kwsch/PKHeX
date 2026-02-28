using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierG3 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is G3PKM pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, G3PKM pk)
    {
        if (ParseSettings.AllowGBACrossTransferRSE(pk))
            return;

        // Only FR/LG are released. Only can originate from FR/LG.
        if (pk.Version is not (GameVersion.FR or GameVersion.LG))
            data.AddLine(GetInvalid(TradeNotAvailable));
        else if (IsForeignFRLG(pk.Species))
            data.AddLine(GetInvalid(TradeNotAvailable));

        if (UnavailableHeld.BinarySearch((ushort)pk.HeldItem) >= 0)
            data.AddLine(GetInvalid(ItemUnreleased));
    }


    /// <summary>
    /// For a species with a potentially valid FR/LG origin encounter, flag if not permitted.
    /// </summary>
    public static bool IsForeignFRLG(ushort species) => IsForeign(ForeignFRLG, species, ShiftFRLG);

    public static bool IsForeign(ReadOnlySpan<byte> bitSet, int species, [ConstantExpected] int shift)
    {
        species -= shift;
        var offset = species >> 3;
        if ((uint)offset >= bitSet.Length)
            return false;

        var bit = species & 7;
        if ((bitSet[offset] & (1 << bit)) != 0)
            return true;
        return false;
    }

    private const ushort ShiftFRLG = 152; // First unavailable Species (ignoring Mew)

    /// <summary>
    /// Bitset representing species that are considered unobtainable in FR/LG.
    /// Includes foreign transfers and time-of-day evolutions.
    /// First species is Chikorita (152), last is Deoxys (386). Mew (151) is not included since it is not obtainable with a FR/LG version ID (eager check).
    /// </summary>
    /// <remarks>
    /// Source: https://www.serebii.net/fireredleafgreen/unobtainable.shtml
    /// </remarks>
    private static ReadOnlySpan<byte> ForeignFRLG =>
    [
        0xFF, 0x19, 0x0C, 0x30, 0x82, 0x31, 0xA8, 0x06,
        0x42, 0x20, 0x00, 0x02, 0xF8, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFB, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        0xFF, 0xFF, 0xFE, 0xFF, 0xFF, 0x03,
    ];

    private static ReadOnlySpan<ushort> UnavailableHeld =>
    [
        039,      041, 042, 043, // Flutes (Yellow is obtainable via Coins)
        046, 047, // Shoal Salt, Shoal Shell
        048, 049, 050, 051, // Shards
        081, // Fluffy Tail

        121, 122, 123, 124, 125, 126, 127, 128, 129, // Mail

        168, // Liechi Berry (Mirage Island)
        169, // Ganlon Berry (Event)
        170, // Salac Berry (Event)
        171, // Petaya Berry (Event)
        172, // Apicot Berry (Event)
        173, // Lansat Berry (Event)
        174, // Starf Berry (Event)
        175, // Enigma Berry (Event)

        179, // BrightPowder
        180, // White Herb
        185, // Mental Herb
        186, // Choice Band
        191, // Soul Dew
        192, // DeepSeaTooth
        193, // DeepSeaScale

        198, // Scope Lens

        202, // Light Ball

        219, // Shell Bell

        254, 255, 256, 257, 258, 259, // Scarves
    ];
}
