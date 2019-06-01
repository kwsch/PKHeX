using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the specific origin data of <see cref="GameVersion.CXD"/> encounters.
    /// </summary>
    public sealed class CXDVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Misc;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (data.EncounterMatch is EncounterStatic)
                VerifyCXDStarterCorrelation(data);
            else if (pkm.Egg_Location != 0) // can't obtain eggs in CXD
                data.AddLine(GetInvalid(LEncInvalid, CheckIdentifier.Encounter)); // invalid encounter

            if (pkm.OT_Gender == 1)
                data.AddLine(GetInvalid(LG3OTGender, CheckIdentifier.Trainer));
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
                case (int)Species.Eevee:
                    valid = LockFinder.IsXDStarterValid(pidiv.OriginSeed, pkm.TID, pkm.SID); break;
                case (int)Species.Espeon:
                case (int)Species.Umbreon:
                    valid = pidiv.Type == PIDType.CXD_ColoStarter; break;
                default:
                    return;
            }
            if (!valid)
                data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
        }
    }
}
