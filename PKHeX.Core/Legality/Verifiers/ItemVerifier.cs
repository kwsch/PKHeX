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
        {
            data.AddLine(GetInvalid(LItemUnreleased));
        }
        else if (pk.Format == 3 && item == 175) // Enigma Berry
        {
            // A Pokémon holding this Berry cannot be traded to Pokémon Colosseum or Pokémon XD: Gale of Darkness, nor can it be stored in Pokémon Box Ruby & Sapphire.
            if (pk is CK3 or XK3)
                data.AddLine(GetInvalid(LItemUnreleased));
            else
                VerifyEReaderBerry(data);
        }
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
