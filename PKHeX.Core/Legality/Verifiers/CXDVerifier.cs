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
            if (data.EncounterMatch is EncounterStatic3 s3)
                VerifyCXDStarterCorrelation(data, s3);
            else if (pkm.Egg_Location != 0 && pkm is not PB8 {Egg_Location: Locations.Default8bNone}) // can't obtain eggs in CXD
                data.AddLine(GetInvalid(LEncInvalid, CheckIdentifier.Encounter)); // invalid encounter

            if (pkm.OT_Gender == 1)
                data.AddLine(GetInvalid(LG3OTGender, CheckIdentifier.Trainer));
        }

        private static void VerifyCXDStarterCorrelation(LegalityAnalysis data, EncounterStatic3 enc)
        {
            var (type, seed) = data.Info.PIDIV;
            if (type is not (PIDType.CXD or PIDType.CXDAnti or PIDType.CXD_ColoStarter))
                return; // already flagged as invalid

            var pkm = data.pkm;
            bool valid = enc.Species switch
            {
                (int)Species.Eevee => LockFinder.IsXDStarterValid(seed, pkm.TID, pkm.SID),
                (int)Species.Espeon or (int)Species.Umbreon => type == PIDType.CXD_ColoStarter,
                _ => true,
            };
            if (!valid)
                data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
        }
    }
}
