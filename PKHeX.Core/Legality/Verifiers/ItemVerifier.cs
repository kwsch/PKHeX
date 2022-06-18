using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.HeldItem"/>.
/// </summary>
public sealed class ItemVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.HeldItem;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (!ItemRestrictions.IsHeldItemAllowed(pk))
            data.AddLine(GetInvalid(LItemUnreleased));

        if (pk.Format == 3 && pk.HeldItem == 175) // Enigma Berry
            VerifyEReaderBerry(data);

        if (pk.IsEgg && pk.HeldItem != 0)
            data.AddLine(GetInvalid(LItemEgg));
    }

    private void VerifyEReaderBerry(LegalityAnalysis data)
    {
        var status = EReaderBerrySettings.GetStatus();
        var chk = GetEReaderCheckResult(status);
        if (chk != null)
            data.AddLine(chk);
    }

    private CheckResult? GetEReaderCheckResult(EReaderBerryMatch status) => status switch
    {
        EReaderBerryMatch.NoMatch => GetInvalid(LEReaderInvalid),
        EReaderBerryMatch.NoData => GetInvalid(LItemUnreleased),
        EReaderBerryMatch.InvalidUSA => GetInvalid(LEReaderAmerica),
        EReaderBerryMatch.InvalidJPN => GetInvalid(LEReaderJapan),
        _ => null,
    };
}
