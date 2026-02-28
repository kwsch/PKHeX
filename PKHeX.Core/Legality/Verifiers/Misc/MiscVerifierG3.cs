using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

public sealed class MiscVerifierG3 : Verifier
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
        else if (Legal.IsForeignFRLG(pk.Species))
            data.AddLine(GetInvalid(TradeNotAvailable));

        if (ItemStorage3FRLG_VC.IsUnreleasedHeld(pk.HeldItem))
            data.AddLine(GetInvalid(ItemUnreleased));
    }
}
