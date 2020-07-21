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
            var result = VerifyConsoleRegion(data.pkm, data.Info.Generation);
            data.AddLine(result);
        }

        private CheckResult VerifyConsoleRegion(PKM pkm, int gen)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return GetInvalid(LGeoHardwareRange);

            if (gen >= 8 || pkm.Format >= 8 || pkm.GG)
                return VerifyNoDataPresent(pkm, consoleRegion);
            return Verify3DSDataPresent(pkm, consoleRegion);
        }

        private CheckResult Verify3DSDataPresent(PKM pkm, int consoleRegion)
        {
            if (!Legal.IsConsoleRegionCountryValid(consoleRegion, pkm.Country))
                return GetInvalid(LGeoHardwareInvalid);
            return GetValid(LGeoHardwareValid);
        }

        private CheckResult VerifyNoDataPresent(PKM pkm, int consoleRegion)
        {
            if (consoleRegion != 0 || pkm.Country != 0 || pkm.Region != 0)
                return GetInvalid(LGeoHardwareInvalid);
            return GetValid(LGeoHardwareValid);
        }
    }
}
