using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="IRegionOrigin.ConsoleRegion"/> and <see cref="IRegionOrigin.Country"/> of origin values.
    /// </summary>
    public sealed class ConsoleRegionVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Geography;

        public override void Verify(LegalityAnalysis data)
        {
            if (data.pkm is not IRegionOrigin tr)
                return;
            var result = VerifyConsoleRegion(tr);
            data.AddLine(result);
        }

        private CheckResult VerifyConsoleRegion(IRegionOrigin pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return GetInvalid(LGeoHardwareRange);

            return Verify3DSDataPresent(pkm, consoleRegion);
        }

        private CheckResult Verify3DSDataPresent(IRegionOrigin pkm, int consoleRegion)
        {
            if (!Locale3DS.IsConsoleRegionCountryValid(consoleRegion, pkm.Country))
                return GetInvalid(LGeoHardwareInvalid);
            return GetValid(LGeoHardwareValid);
        }
    }
}
