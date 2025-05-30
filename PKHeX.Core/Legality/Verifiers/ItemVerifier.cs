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
        var item = pk.HeldItem;
        if (pk.IsEgg && item != 0)
            data.AddLine(GetInvalid(LItemEgg));

        if (!ItemRestrictions.IsHeldItemAllowed(item, context: pk.Context))
            data.AddLine(GetInvalid(LItemUnreleased));
        else if (item == 175 && pk is G3PKM g3) // Enigma Berry
            VerifyEnigmaGen3(data, g3);
    }

    private void VerifyEnigmaGen3(LegalityAnalysis data, G3PKM g3)
    {
        // A Pokémon holding this Berry cannot be traded to Pokémon Colosseum or Pokémon XD: Gale of Darkness,
        // nor can it be stored in Pokémon Box Ruby & Sapphire.
        if (g3 is CK3 or XK3 || ParseSettings.ActiveTrainer is SAV3RSBox)
            data.AddLine(GetInvalid(LItemUnreleased));
        else
            VerifyEReaderBerry(data);
    }

    private void VerifyEReaderBerry(LegalityAnalysis data)
    {
        var status = EReaderBerrySettings.GetStatus();
        var chk = GetEReaderCheckResult(status);
        if (chk != default)
            data.AddLine(chk);
    }

    private CheckResult GetEReaderCheckResult(EReaderBerryMatch status) => status switch
    {
        EReaderBerryMatch.NoMatch => GetInvalid(LEReaderInvalid),
        EReaderBerryMatch.NoData => GetInvalid(LItemUnreleased),
        EReaderBerryMatch.InvalidUSA => GetInvalid(LEReaderAmerica),
        EReaderBerryMatch.InvalidJPN => GetInvalid(LEReaderJapan),
        _ => default,
    };
}
