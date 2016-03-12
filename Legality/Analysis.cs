using System;
using System.Linq;

namespace PKHeX
{
    public class LegalityAnalysis
    {
        private readonly PK6 pk6;
        public LegalityAnalysis(PK6 pk)
        {
            pk6 = pk;
            updateRelearnLegality();
            updateMoveLegality();
        }
        public void updateRelearnLegality()
        {
            try { vRelearn = LegalityCheck.verifyRelearn(pk6); }
            catch { for (int i = 0; i < 4; i++) vRelearn[i] = new LegalityCheck(Severity.Invalid, "Internal error."); }
            
        }
        public void updateMoveLegality()
        {
            try { vMoves = LegalityCheck.verifyMoves(pk6); }
            catch { for (int i = 0; i < 4; i++) vMoves[i] = new LegalityCheck(Severity.Invalid, "Internal error."); }
        }
        
        public LegalityCheck EC, Nickname, PID, IDs, IVs, EVs, Encounter;
        public LegalityCheck[] vMoves = new LegalityCheck[4];
        public LegalityCheck[] vRelearn = new LegalityCheck[4];
        public string Report => getLegalityReport();
        private string getLegalityReport()
        {
            if (!pk6.Gen6)
                return "Analysis only available for Pokémon that originate from X/Y & OR/AS.";

            EC = LegalityCheck.verifyECPID(pk6);
            Nickname = LegalityCheck.verifyNickname(pk6);
            PID = LegalityCheck.verifyECPID(pk6);
            IVs = LegalityCheck.verifyIVs(pk6);
            EVs = LegalityCheck.verifyEVs(pk6);
            IDs = LegalityCheck.verifyID(pk6);
            Encounter = LegalityCheck.verifyEncounter(pk6);

            var chks = new[] {EC, Nickname, PID, IVs, EVs, IDs, Encounter};

            string r = "";
            for (int i = 0; i < 4; i++)
                if (!vMoves[i].Valid)
                    r += $"{vMoves[i].Judgement} Move {i + 1}: {vMoves[i].Comment}" + Environment.NewLine;
            for (int i = 0; i < 4; i++)
                if (!vRelearn[i].Valid)
                    r += $"{vRelearn[i].Judgement} Relearn Move {i + 1}: {vRelearn[i].Comment}" + Environment.NewLine;

            if (r.Length == 0 && chks.All(chk => chk.Valid))
                return "Legal!";

            // Build result string...
            r += chks.Where(chk => !chk.Valid).Aggregate("", (current, chk) => current + $"{chk.Judgement}: {chk.Comment}{Environment.NewLine}");

            return r;
        }
    }
}
