using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class NHarmoniaVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;

            bool checksRequired = EncounterMatch is EncounterStaticPID s && s.NSparkle;
            if (pkm is PK5 pk5)
            {
                bool has = pk5.NPokémon;
                if (checksRequired && !has)
                    data.AddLine(GetInvalid(V326, CheckIdentifier.Fateful));
                if (!checksRequired && has)
                    data.AddLine(GetInvalid(V327, CheckIdentifier.Fateful));
            }

            if (!checksRequired)
                return;

            if (pkm.IVs.Any(iv => iv != 30))
                data.AddLine(GetInvalid(V218, CheckIdentifier.IVs));
            if (!VerifyNsPKMOTValid(pkm))
                data.AddLine(GetInvalid(V219, CheckIdentifier.Trainer));
            if (pkm.IsShiny)
                data.AddLine(GetInvalid(V220, CheckIdentifier.Shiny));
        }
        private static bool VerifyNsPKMOTValid(PKM pkm)
        {
            if (pkm.TID != 00002 || pkm.SID != 00000)
                return false;
            var ot = pkm.OT_Name;
            if (ot.Length != 1)
                return false;
            var c = Legal.GetG5OT_NSparkle(pkm.Language);
            return c == ot;
        }
    }
}
