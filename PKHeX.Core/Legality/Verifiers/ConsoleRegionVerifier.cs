using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="IGeoTrack.ConsoleRegion"/> and <see cref="IGeoTrack.Country"/> of origin values.
    /// </summary>
    public sealed class ConsoleRegionVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Geography;

        public override void Verify(LegalityAnalysis data)
        {
            if (!(data.pkm is IGeoTrack tr))
                return;
            var result = VerifyConsoleRegion(tr);
            data.AddLine(result);
        }

        private CheckResult VerifyConsoleRegion(IGeoTrack pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return GetInvalid(LGeoHardwareRange);

            return Verify3DSDataPresent(pkm, consoleRegion);
        }

        private CheckResult Verify3DSDataPresent(IGeoTrack pkm, int consoleRegion)
        {
            if (!Legal.IsConsoleRegionCountryValid(consoleRegion, pkm.Country))
                return GetInvalid(LGeoHardwareInvalid);
            return GetValid(LGeoHardwareValid);
        }
    }
}
