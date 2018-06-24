using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class ConsoleRegionVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Geography;
        public override void Verify(LegalityAnalysis data)
        {
            var result = VerifyConsoleRegion(data.pkm);
            data.AddLine(result);
        }
        private static CheckResult VerifyConsoleRegion(PKM pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return new CheckResult(Severity.Invalid, V301, CheckIdentifier.Geography);
            return Legal.IsConsoleRegionCountryValid(consoleRegion, pkm.Country)
                ? new CheckResult(Severity.Valid, V303, CheckIdentifier.Geography)
                : new CheckResult(Severity.Invalid, V302, CheckIdentifier.Geography);
        }
    }
}
