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
            if (!Legal.IsHeldItemAllowed(pkm))
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
                data.AddLine(GetInvalid(LEReaderInvalid));
            else if (matchJP && !Legal.SavegameJapanese && Legal.ActiveTrainer.Language >= 0) // E-Reader is region locked
                data.AddLine(GetInvalid(LEReaderJapan));
            else if (matchUSA && Legal.SavegameJapanese && Legal.ActiveTrainer.Language >= 0) // E-Reader is region locked
                data.AddLine(GetInvalid(LEReaderAmerica));
        }
    }
}
