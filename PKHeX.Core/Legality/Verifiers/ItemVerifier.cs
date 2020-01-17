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
            if (Legal.EReaderBerryIsEnigma) // no E-Reader berry data provided, can't hold berry.
            {
                data.AddLine(GetInvalid(LItemUnreleased));
                return;
            }

            var matchUSA = Legal.EReaderBerriesNames_USA.Contains(Legal.EReaderBerryName);
            var matchJP = Legal.EReaderBerriesNames_JP.Contains(Legal.EReaderBerryName);
            if (!matchJP && !matchUSA) // Does not match any released E-Reader berry
            {
                data.AddLine(GetInvalid(LEReaderInvalid));
                return;
            }
            if (ParseSettings.ActiveTrainer.Language <= 0)
                return;

            bool jp = ParseSettings.ActiveTrainer.Language == 1;
            if (matchJP == jp)
                return; // matches

            // E-Reader is region locked
            var msg = matchUSA ? LEReaderAmerica : LEReaderJapan;
            data.AddLine(GetInvalid(msg));
        }
    }
}
