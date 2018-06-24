using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class CXDVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Misc;
        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (data.EncounterMatch is EncounterStatic)
                VerifyCXDStarterCorrelation(data);
            else if (pkm.WasEgg) // can't obtain eggs in CXD
                data.AddLine(GetInvalid(V80, CheckIdentifier.Encounter)); // invalid encounter

            if (pkm.OT_Gender == 1)
                data.AddLine(GetInvalid(V407, CheckIdentifier.Trainer));
        }
        private static void VerifyCXDStarterCorrelation(LegalityAnalysis data)
        {
            var pidiv = data.Info.PIDIV;
            if (pidiv.Type != PIDType.CXD)
                return;

            bool valid;
            var EncounterMatch = data.EncounterMatch;
            var pkm = data.pkm;
            switch (EncounterMatch.Species)
            {
                case 133:
                    valid = LockFinder.IsXDStarterValid(pidiv.OriginSeed, pkm.TID, pkm.SID); break;
                case 196:
                case 197:
                    valid = pidiv.Type == PIDType.CXD_ColoStarter; break;
                default:
                    return;
            }
            if (!valid)
                data.AddLine(GetInvalid(V400, CheckIdentifier.PID));
        }
    }
}
