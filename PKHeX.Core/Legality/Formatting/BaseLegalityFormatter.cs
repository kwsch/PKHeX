using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class BaseLegalityFormatter : ILegalityFormatter
    {
        public string GetReport(LegalityAnalysis l)
        {
            if (l.Valid)
                return L_ALegal;
            if (!l.Parsed)
                return L_AnalysisUnavailable;

            var lines = GetLegalityReportLines(l);
            return string.Join(Environment.NewLine, lines);
        }

        public string GetReportVerbose(LegalityAnalysis l)
        {
            if (!l.Parsed)
                return L_AnalysisUnavailable;

            var lines = GetVerboseLegalityReportLines(l);
            return string.Join(Environment.NewLine, lines);
        }

        private static List<string> GetLegalityReportLines(LegalityAnalysis l)
        {
            var lines = new List<string>();
            var info = l.Info;
            var vMoves = info.Moves;
            var pkm = l.pkm;
            for (int i = 0; i < 4; i++)
            {
                if (!vMoves[i].Valid)
                    lines.Add(vMoves[i].Format(L_F0_M_1_2, i + 1));
            }

            if (pkm.Format >= 6)
            {
                var vRelearn = info.Relearn;
                for (int i = 0; i < 4; i++)
                {
                    if (!vRelearn[i].Valid)
                        lines.Add(vRelearn[i].Format(L_F0_RM_1_2, i + 1));
                }
            }

            // Build result string...
            var outputLines = l.Results.Where(chk => !chk.Valid);
            lines.AddRange(outputLines.Select(chk => chk.Format(L_F0_1)));
            return lines;
        }

        private IReadOnlyList<string> GetVerboseLegalityReportLines(LegalityAnalysis l)
        {
            var lines = l.Valid ? new List<string> {L_ALegal} : GetLegalityReportLines(l);
            var info = l.Info;
            var pkm = l.pkm;
            const string separator = "===";
            lines.Add(separator);
            lines.Add(string.Empty);
            int initialCount = lines.Count;

            var format = pkm.Format;
            LegalityFormatting.AddValidMoves(info, lines, format);

            if (format >= 6)
                LegalityFormatting.AddValidMovesRelearn(info, lines);

            if (lines.Count != initialCount) // move info added, break for next section
                lines.Add(string.Empty);

            LegalityFormatting.AddValidSecondaryChecks(l.Results, lines);

            lines.Add(separator);
            lines.Add(string.Empty);
            LegalityFormatting.AddEncounterInfo(l, lines);

            LegalityFormatting.AddInvalidMatchesIfAny(l, info, lines);

            return lines;
        }
    }
}
