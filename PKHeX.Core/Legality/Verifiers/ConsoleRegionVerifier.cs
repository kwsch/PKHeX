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
        private CheckResult VerifyConsoleRegion(PKM pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return GetInvalid(V301);

            if (!Legal.IsConsoleRegionCountryValid(consoleRegion, pkm.Country))
                return GetInvalid(V302);

            return GetValid(V303);
        }
    }
}
