using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.ConsoleRegion"/> and <see cref="PKM.Country"/> of origin values.
    /// </summary>
    public sealed class ConsoleRegionVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Geography;

        public override void Verify(LegalityAnalysis data)
        {
            var result = VerifyConsoleRegion(data.pkm);
            data.AddLine(result);
        }

        private CheckResult VerifyConsoleRegion(PKM pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return GetInvalid(LGeoHardwareRange);

            if (!Legal.IsConsoleRegionCountryValid(consoleRegion, pkm.Country))
                return GetInvalid(LGeoHardwareInvalid);

            return GetValid(LGeoHardwareValid);
        }
    }
}
