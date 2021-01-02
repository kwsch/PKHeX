using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.HeldItem"/>.
    /// </summary>
    public sealed class ItemVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.HeldItem;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (!ItemRestrictions.IsHeldItemAllowed(pkm))
                data.AddLine(GetInvalid(LItemUnreleased));

            if (pkm.Format == 3 && pkm.HeldItem == 175) // Enigma Berry
                VerifyEReaderBerry(data);

            if (pkm.IsEgg && pkm.HeldItem != 0)
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
            _ => null
        };
    }
}
