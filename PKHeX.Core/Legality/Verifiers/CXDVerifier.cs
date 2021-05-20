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
            if (pidiv.Type is not (PIDType.CXD or PIDType.CXDAnti))
                return; // already flagged as invalid

            var pkm = data.pkm;
            bool valid = data.EncounterMatch.Species switch
            {
                (int)Species.Eevee => LockFinder.IsXDStarterValid(pidiv.OriginSeed, pkm.TID, pkm.SID),
                (int)Species.Espeon or (int)Species.Umbreon => pidiv.Type == PIDType.CXD_ColoStarter,
                _ => true
            };
            if (!valid)
                data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
        }
    }
}
