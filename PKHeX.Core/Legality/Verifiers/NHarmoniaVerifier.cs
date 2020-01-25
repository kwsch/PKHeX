using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PK5.NPokémon"/> data.
    /// </summary>
    public sealed class NHarmoniaVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;

            bool checksRequired = EncounterMatch is EncounterStatic5N;
            if (pkm is PK5 pk5)
            {
                bool has = pk5.NPokémon;
                if (checksRequired && !has)
                    data.AddLine(GetInvalid(LG5SparkleRequired, CheckIdentifier.Fateful));
                if (!checksRequired && has)
                    data.AddLine(GetInvalid(LG5SparkleInvalid, CheckIdentifier.Fateful));
            }

            if (!checksRequired)
                return;

            if (pkm.IVTotal != 30*6)
                data.AddLine(GetInvalid(LG5IVAll30, CheckIdentifier.IVs));
            if (!VerifyNsPKMOTValid(pkm))
                data.AddLine(GetInvalid(LG5ID_N, CheckIdentifier.Trainer));
            if (pkm.IsShiny)
                data.AddLine(GetInvalid(LG5PIDShinyN, CheckIdentifier.Shiny));
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
