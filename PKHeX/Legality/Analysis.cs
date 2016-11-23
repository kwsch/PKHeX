using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public partial class LegalityAnalysis
    {
        private PKM pkm;
        private readonly List<CheckResult> Parse = new List<CheckResult>();

        private enum Encounters
        {
            Unknown = -1,
            Generic = 0
        }

        private object EncounterMatch;
        private Type EncounterType;
        private bool EncounterIsMysteryGift => EncounterType.IsSubclassOf(typeof (MysteryGift));
        private List<MysteryGift> EventGiftMatch;
        private CheckResult Encounter, History;
        private int[] RelearnBase;
        // private bool SecondaryChecked;

        public readonly bool Parsed;
        public readonly bool Valid;
        public CheckResult[] vMoves = new CheckResult[4];
        public CheckResult[] vRelearn = new CheckResult[4];
        public string Report => getLegalityReport();
        public string VerboseReport => getVerboseLegalityReport();
        public readonly int[] AllSuggestedMoves;
        public readonly int[] AllSuggestedRelearnMoves;
        public readonly int[] AllSuggestedMovesAndRelearn;

        public LegalityAnalysis(PKM pk)
        {
            for (int i = 0; i < 4; i++) 
            {
                vMoves[i] = new CheckResult(CheckIdentifier.Move);
                vRelearn[i] = new CheckResult(CheckIdentifier.RelearnMove);
            }

            try
            {
                switch (pk.GenNumber)
                {
                    case 6: parsePK6(pk); break;
                    case 7: parsePK7(pk); break;
                    default: return;
                }

                Valid = Parsed = Parse.Any();
                if (Parsed)
                {
                    if (Parse.Any(chk => !chk.Valid))
                        Valid = false;
                    if (vMoves.Any(m => m.Valid != true))
                        Valid = false;
                    else if (vRelearn.Any(m => m.Valid != true))
                        Valid = false;

                    if (pkm.FatefulEncounter && vRelearn.Any(chk => !chk.Valid) && EncounterMatch == null)
                        AddLine(Severity.Indeterminate, "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?", CheckIdentifier.Fateful);
                }
            }
            catch { Valid = false; }
            getLegalityReport();
            AllSuggestedMoves = !isOriginValid(pkm) ? new int[4] : getSuggestedMoves(true, true);
            AllSuggestedRelearnMoves = !isOriginValid(pkm) ? new int[4] : Legal.getValidRelearn(pkm, -1).ToArray();
            AllSuggestedMovesAndRelearn = AllSuggestedMoves.Concat(AllSuggestedRelearnMoves).ToArray();
        }

        private void AddLine(Severity s, string c, CheckIdentifier i)
        {
            AddLine(new CheckResult(s, c, i));
        }
        private void AddLine(CheckResult chk)
        {
            Parse.Add(chk);
        }
        private void parsePK6(PKM pk)
        {
            pkm = pk;
            if (!isOriginValid(pkm))
            { AddLine(Severity.Invalid, "Species does not exist in origin game.", CheckIdentifier.None); return; }

            updateRelearnLegality();
            updateMoveLegality();
            updateChecks();
        }
        private void parsePK7(PKM pk)
        {
            pkm = pk;
            if (!isOriginValid(pkm))
            { AddLine(Severity.Invalid, "Species does not exist in origin game.", CheckIdentifier.None); return; }

            updateRelearnLegality();
            updateMoveLegality();
            updateChecks();
        }
        private bool isOriginValid(PKM pk)
        {
            switch (pkm.GenNumber)
            {
                case 1: return pkm.Species <= 151;
                case 2: return pkm.Species <= 251;
                case 3: return pkm.Species <= 386;
                case 4: return pkm.Species <= 493;
                case 5: return pkm.Species <= 649;
                case 6: return pkm.Species <= 721;
                case 7: return pkm.Species <= 802;
                default: return false;
            }
        }

        private void updateRelearnLegality()
        {
            try { vRelearn = verifyRelearn(); }
            catch { for (int i = 0; i < 4; i++) vRelearn[i] = new CheckResult(Severity.Invalid, "Internal error.", CheckIdentifier.RelearnMove); }
            // SecondaryChecked = false;
        }
        private void updateMoveLegality()
        {
            try { vMoves = verifyMoves(); }
            catch { for (int i = 0; i < 4; i++) vMoves[i] = new CheckResult(Severity.Invalid, "Internal error.", CheckIdentifier.Move); }
            // SecondaryChecked = false;
        }

        private void updateChecks()
        {
            Encounter = verifyEncounter();

            // If EncounterMatch is null, nullrefexception will prevent a lot of analysis from happening at all.
            EncounterMatch = EncounterMatch ?? Encounters.Unknown;

            EncounterType = EncounterMatch?.GetType();
            if (EncounterType == typeof (MysteryGift))
                EncounterType = EncounterType.BaseType;
            History = verifyHistory();

            AddLine(Encounter);
            AddLine(History);

            verifyECPID();
            verifyNickname();
            verifyID();
            verifyIVs();
            verifyHyperTraining();
            verifyEVs();
            verifyLevel();
            verifyMedals();
            verifyRibbons();
            verifyAbility();
            verifyBall();
            verifyOTMemory();
            verifyHTMemory();
            verifyRegion();
            verifyForm();
            verifyMisc();
            verifyGender();
            // SecondaryChecked = true;
        }
        private string getLegalityReport()
        {
            if (!Parsed)
                return "Analysis not available for this Pokémon.";
            
            string r = "";
            for (int i = 0; i < 4; i++)
                if (!vMoves[i].Valid)
                    r += $"{vMoves[i].Judgement} Move {i + 1}: {vMoves[i].Comment}" + Environment.NewLine;
            for (int i = 0; i < 4; i++)
                if (!vRelearn[i].Valid)
                    r += $"{vRelearn[i].Judgement} Relearn Move {i + 1}: {vRelearn[i].Comment}" + Environment.NewLine;

            if (r.Length == 0 && Parse.All(chk => chk.Valid) && Valid)
                return "Legal!";
            
            // Build result string...
            r += Parse.Where(chk => !chk.Valid).Aggregate("", (current, chk) => current + $"{chk.Judgement}: {chk.Comment}{Environment.NewLine}");

            if (r.Length == 0)
                r = "Internal Error.";

            return r.TrimEnd();
        }
        private string getVerboseLegalityReport()
        {
            string r = getLegalityReport() + Environment.NewLine;
            if (pkm == null)
                return r;
            r += "===" + Environment.NewLine + Environment.NewLine;
            int rl = r.Length;

            for (int i = 0; i < 4; i++)
                if (vMoves[i].Valid)
                    r += $"{vMoves[i].Judgement} Move {i + 1}: {vMoves[i].Comment}" + Environment.NewLine;
            for (int i = 0; i < 4; i++)
                if (vRelearn[i].Valid)
                    r += $"{vRelearn[i].Judgement} Relearn Move {i + 1}: {vRelearn[i].Comment}" + Environment.NewLine;

            if (rl != r.Length) // move info added, break for next section
                r += Environment.NewLine;
            
            r += Parse.Where(chk => chk != null && chk.Valid && chk.Comment != "Valid").OrderBy(chk => chk.Judgement) // Fishy sorted to top
                .Aggregate("", (current, chk) => current + $"{chk.Judgement}: {chk.Comment}{Environment.NewLine}");
            return r.TrimEnd();
        }

        public int[] getSuggestedRelearn()
        {
            if (RelearnBase == null || pkm.GenNumber < 6 || !isOriginValid(pkm))
                return new int[4];

            if (!pkm.WasEgg)
                return RelearnBase;

            List<int> window = new List<int>(RelearnBase);

            for (int i = 0; i < 4; i++)
                if (!vMoves[i].Valid || vMoves[i].Flag)
                    window.Add(pkm.Moves[i]);

            if (window.Count < 4)
                window.AddRange(new int[4 - window.Count]);
            return window.Skip(window.Count - 4).Take(4).ToArray();
        }
        public int[] getSuggestedMoves(bool tm, bool tutor)
        {
            if (pkm == null || pkm.GenNumber < 6 || !isOriginValid(pkm))
                return null;
            return Legal.getValidMoves(pkm, Tutor: tutor, Machine: tm).Skip(1).ToArray(); // skip move 0
        }
    }
}
