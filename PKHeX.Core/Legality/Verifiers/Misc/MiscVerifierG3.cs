using System;
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
    }


    /// <summary>
    /// For a species with a potentially valid FR/LG origin encounter, flag if not permitted.
    /// </summary>
    public static bool IsForeignFRLG(ushort species) => IsForeign(ForeignFRLG, species);

    public static bool IsForeign(ReadOnlySpan<byte> bitSet, int species)
    {
        species -= ShiftFRLG;
        var offset = species >> 3;
        if (offset >= bitSet.Length)
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
}
